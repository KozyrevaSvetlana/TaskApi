# TaskApi — тестовое задание (.NET 9)

REST API для управления задачами: создание, постраничный список, завершение и удаление.
При завершении задачи событие публикуется в RabbitMQ.

**Стек:** ASP.NET Core Minimal API · PostgreSQL + EF Core (Npgsql) · RabbitMQ (только publish) · Docker · xUnit + Testcontainers.

## Возможности

* CRUD-подмножество: `POST`, `GET` (с пагинацией), `PUT .../complete`, `DELETE`.
* Оптимистичная конкурентность через `xmin` (RowVersion): параллельное завершение одной задачи даёт ровно один успех, остальные получают **409**.
* Публикация события `task.completed` в RabbitMQ вне пути запроса, с корректной остановкой (graceful shutdown).
* Оптимизация под нагрузку: пул `DbContext`, автоподготовка запросов, source-generated JSON и др. (см. ниже).
* Swagger UI, health-check `/healthz`, XML-документация кода.

## Структура

```
TaskApi.sln
Dockerfile, Dockerfile.tests
docker-compose.yml           # полный стенд: PostgreSQL + RabbitMQ + API
docker-compose.infra.yml     # только PostgreSQL + RabbitMQ (для запуска API из IDE)
docker-compose.tests.yml     # запуск тестов в контейнере
src/
  TaskApi.Data/              # отдельный проект с БД
    TaskItem.cs, Priority.cs, TaskDbContext.cs
    TaskDataSeeder.cs        # генератор тестовых задач (по умолчанию не подключён)
    Migrations/              # миграции EF Core
  TaskApi/
    Program.cs
    Endpoints/TaskEndpoints.cs
    Services/                # TaskService (бизнес-логика), DatabaseInitializer
    Middleware/              # TitleValidationMiddleware
    Validation/              # TitleValidator
    Messaging/               # RabbitMqService, IEventPublisher, RabbitMqOptions, TaskCompletedMessage
    Contracts/               # DTO + source-generated JSON контекст
    Filters/                 # EnumAsStringSchemaFilter (enum как строки в Swagger)
    appsettings.json
tests/TaskApi.Tests/
  Unit/                      # юнит-тесты
  Integration/               # Testcontainers: PostgreSQL + RabbitMQ
```

## Требования

* [Docker](https://www.docker.com/) с Docker Compose — для запуска стенда и интеграционных тестов.
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) — только для запуска без Docker и для `dotnet test` на хосте.

## Запуск

### Всё в Docker

```bash
docker compose up --build
```

| Сервис | Адрес |
|--------|-------|
| API | http://localhost:50564 |
| Swagger UI | http://localhost:50564/swagger |
| Health-check | http://localhost:50564/healthz |
| RabbitMQ UI | http://localhost:15672 (guest / guest) |
| PostgreSQL | localhost:5432 (БД `taskdb`) |

### API из IDE или `dotnet run`

Поднять только зависимости и запустить API локально:

```bash
docker compose -f docker-compose.infra.yml up -d
dotnet run --project src/TaskApi
```

В окружении `Development` строка подключения берётся из `appsettings.Development.json` (`localhost:5432`, RabbitMQ — `localhost:5672`).

### Примеры запросов

```bash
curl -X POST localhost:8080/tasks -H 'Content-Type: application/json' -d '{"title":"Buy milk","priority":"High"}'
curl "localhost:8080/tasks?page=1&pageSize=50"
curl -X PUT localhost:8080/tasks/{id}/complete
curl -X DELETE localhost:8080/tasks/{id}
```

## Конфигурация

Параметры читаются из `appsettings*.json`, переменных окружения и user-secrets. Вложенные ключи в переменных окружения разделяются двойным подчёркиванием (`ConnectionStrings__Postgres`).

| Параметр | Описание |
|----------|----------|
| `ConnectionStrings:Postgres` | Строка подключения к PostgreSQL. **Обязательна**: без неё приложение не запустится. В `appsettings.json` её нет специально, чтобы не хранить пароль в коде. |
| `RabbitMq:Host` / `Port` | Адрес брокера (по умолчанию `localhost:5672`). |
| `RabbitMq:UserName` / `Password` / `VirtualHost` | Учётные данные и виртуальный хост (по умолчанию `guest` / `guest` / `/`). |
| `RabbitMq:Exchange` / `RoutingKey` | Обменник и ключ маршрутизации (`task.events` / `task.completed`). |
| `RabbitMq:QueueCapacity` | Размер буфера сообщений в памяти (10 000). |
| `RabbitMq:ConnectTimeout` | Таймаут подключения (`00:00:03`). |
| `RabbitMq:ReconnectDelay` | Пауза между попытками переподключения (`00:00:05`). |
| `RabbitMq:ShutdownDrainTimeout` | Сколько ждать отправки очереди при остановке (`00:00:05`). |

Строку подключения не нужно хранить в репозитории. Для локальной разработки можно использовать user-secrets:

```bash
dotnet user-secrets init --project src/TaskApi
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=taskdb;Username=postgres;Password=postgres" --project src/TaskApi
```

В Docker строка подключения передаётся переменной окружения `ConnectionStrings__Postgres` (см. `docker-compose.yml`).

## База данных и миграции

При старте `DatabaseInitializer` применяет миграции из проекта `TaskApi.Data` (если они есть), иначе создаёт схему через `EnsureCreated`.
Если PostgreSQL ещё запускается, выполняется до 10 попыток с паузой 2 секунды.

Новая миграция:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <Name> -p src/TaskApi.Data -s src/TaskApi
```

## API

| Метод | Эндпоинт | Ответы |
|-------|----------|--------|
| POST | `/tasks` | 201 + задача; 400 при невалидном `title`/`priority` |
| GET | `/tasks?page=1&pageSize=50` | 200 `{ items, page, pageSize, hasNext }` (pageSize 1..100, сортировка: новые первыми) |
| PUT | `/tasks/{id}/complete` | 200 + задача; 404; **409** если уже завершена / конкурентный вызов |
| DELETE | `/tasks/{id}` | 204; 404 |

> В задании сказано «без пагинации», но ниже стоит «Добавить пагинацию». Сделана пагинация (`page`/`pageSize`).
> Для очень глубоких страниц лучше keyset-пагинация, но офсетная проще и понятнее в API.

### Модель

`POST /tasks` — тело запроса:

| Поле | Тип | Описание |
|------|-----|----------|
| `title` | string | Обязательное; после удаления пробелов по краям от 1 до 200 символов |
| `priority` | string | `Low`, `Medium` или `High`; по умолчанию `Medium`. Числа не принимаются |

Задача в ответах:

```json
{
  "id": "0199f3a2-7c1e-7c2b-9d3a-5b8e1f4a6c10",
  "title": "Buy milk",
  "isCompleted": false,
  "createdAt": "2026-10-15T12:00:00+00:00",
  "completedAt": null,
  "priority": "High"
}
```

Ошибки валидации возвращаются в формате `application/problem+json` (код 400); слишком большое тело (> 16 КБ) — 413.

### Событие в RabbitMQ

После успешного завершения задачи публикуется сообщение в обменник `task.events` (topic, durable) с ключом `task.completed`:

```json
{ "taskId": "0199f3a2-7c1e-7c2b-9d3a-5b8e1f4a6c10", "title": "Buy milk", "completedAt": "2026-10-15T12:05:00Z", "priority": "High" }
```

## Как сделана конкурентность (RowVersion)

* В `TaskItem` есть поле `uint Version`, настроенное как `IsRowVersion()`. Для PostgreSQL провайдер Npgsql
  отображает его на системную колонку **`xmin`** (идентификатор транзакции, последней изменившей строку).
  Отдельной колонки не нужно, Postgres обновляет её сам при каждом UPDATE.
* `PUT /tasks/{id}/complete`: загружаем задачу (EF запоминает исходный `xmin`), вызываем `TryComplete()`
  и делаем `SaveChanges`. EF генерирует `UPDATE tasks SET ... WHERE id = @id AND xmin = @исходный_xmin`.
* Два одновременных запроса:
  1. Первый выполняет UPDATE, `xmin` меняется, транзакция фиксируется.
  2. Второй UPDATE ждёт (строка заблокирована), после коммита перепроверяет условие, видит другой `xmin`,
     затрагивает **0 строк**, и EF бросает `DbUpdateConcurrencyException`. Сервис превращает его в **409 Conflict**.
  3. Если второй запрос успел прочитать задачу уже после коммита первого, он видит `IsCompleted = true`,
     `TryComplete()` возвращает `false`, и тоже получается **409**.
* Явных блокировок (`SELECT FOR UPDATE`, транзакции повышенного уровня изоляции) нет, это оптимистичная конкурентность.
* Событие в RabbitMQ отправляется только после успешного `SaveChanges`, поэтому проигравший запрос ничего не публикует.

## RabbitMQ и graceful shutdown

* Обработчик запроса лишь кладёт сообщение в ограниченный `Channel<T>` в памяти (`TryPublish`: без I/O,
  без ожидания confirms, не бросает исключений). Публикует единственный фоновый цикл (`RabbitMqService`).
* Обменник `task.events` (topic, durable), routing key `task.completed`, тело как в задании
  (`completedAt` в UTC с `Z`, `priority` строкой).
* Брокер недоступен: задача всё равно завершается, сообщение (полный JSON) пишется в лог уровня Warning/Error,
  попытки переподключения идут с паузой 5 секунд.
* Остановка (Ctrl+C / SIGTERM): `StopAsync` закрывает приём новых сообщений, ждёт отправки уже принятых
  не дольше 5 секунд (`ShutdownDrainTimeout`), логирует то, что не успело, затем корректно закрывает канал и соединение.
  `HostOptions.ShutdownTimeout = 10s`, `stop_grace_period: 15s` в compose.

## Что оптимизировано под нагрузку

* Пул `DbContext` (`AddDbContextPool`), пул соединений Npgsql + автоподготовка запросов (в строке подключения).
* `GET`: `AsNoTracking`, проекция сразу в DTO, `pageSize + 1` вместо `COUNT(*)`, индекс `(CreatedAt DESC, Id DESC)`.
* `DELETE`: один `ExecuteDeleteAsync` без предварительного SELECT.
* `Guid.CreateVersion7()`: ключи упорядочены по времени, индекс не фрагментируется.
* Source-generated JSON (`AppJsonContext`), логирование через `[LoggerMessage]`, Server GC.
* Публикация в RabbitMQ вынесена из пути запроса.
* Middleware отсекает невалидный `Title` до роутинга, DI и обращения к БД (размер тела ограничен 16 КБ).

## Проверки из задания → где покрыто тестами

| Проверка | Тест |
|----------|------|
| Title пропускает пустые строки | `TaskItemTests`, `TitleValidatorTests`, `TitleValidationMiddlewareTests`, `Create_with_empty_title_returns_400` |
| CompletedAt заполняется при создании | `Create_sets_defaults_and_does_not_fill_CompletedAt`, `Create_uses_defaults_...` |
| Сообщение уходит ДО сохранения в БД | `Message_is_published_only_AFTER_...`, `When_the_database_fails_nothing_is_published` |
| Нет проверки на 409 при конкурентных вызовах | `Parallel_complete_requests_give_exactly_one_success_and_the_rest_409`, `Completing_twice_...` |
| Create → Complete → сообщение в RabbitMQ | `Create_then_complete_task_publishes_event_to_rabbitmq` |

## Тесты

Интеграционные тесты поднимают PostgreSQL и RabbitMQ через Testcontainers, поэтому Docker должен быть запущен.

```bash
dotnet test                                                    # все тесты на хосте
dotnet test --filter "FullyQualifiedName~TaskApi.Tests.Unit"   # только юнит-тесты (Docker не нужен)
docker compose -f docker-compose.tests.yml run --rm tests      # все тесты внутри контейнера
```

## Документация кода

Публичные и внутренние члены снабжены XML-комментариями. В `TaskApi.csproj` и `TaskApi.Data.csproj` включён `GenerateDocumentationFile`, поэтому при сборке создаются XML-файлы документации,
а описания типов могут использоваться в Swagger.

## Лицензия

MIT — см. [LICENSE.txt](LICENSE.txt).
