# TaskApi — тестовое задание (.NET 9)

ASP.NET Core Minimal API + PostgreSQL (EF Core) + RabbitMQ (только publish) + Docker.

## Структура

```
TaskApi.sln
Dockerfile, docker-compose.yml
src/
  TaskApi.Data/              # отдельный проект с БД: TaskItem, Priority, TaskDbContext
  TaskApi/
    Program.cs
    Endpoints/TaskEndpoints.cs
    Services/                # TaskService (бизнес-логика), DatabaseInitializer
    Middleware/              # TitleValidationMiddleware
    Validation/              # TitleValidator
    Messaging/               # RabbitMqService, RabbitMqOptions, TaskCompletedMessage
    Contracts/               # DTO + source-generated JSON контекст
    appsettings.json
tests/TaskApi.Tests/
  Unit/                      # юнит-тесты
  Integration/CompleteTaskIntegrationTest.cs   # Testcontainers: PostgreSQL + RabbitMQ
```

## Запуск

```bash
docker compose up --build        # API: http://localhost:8080, RabbitMQ UI: http://localhost:15672 (guest/guest)
```

```bash
curl -X POST localhost:8080/tasks -H 'Content-Type: application/json' -d '{"title":"Buy milk","priority":"High"}'
curl "localhost:8080/tasks?page=1&pageSize=50"
curl -X PUT localhost:8080/tasks/{id}/complete
curl -X DELETE localhost:8080/tasks/{id}
```

Тесты (нужен запущенный Docker — контейнеры поднимает Testcontainers):

```bash
dotnet test
```

Миграции (необязательно; без них схема создаётся через `EnsureCreated`, при наличии миграций применяются они):

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate -p src/TaskApi.Data -s src/TaskApi
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
