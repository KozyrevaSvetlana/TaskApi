using TaskApi.Data;

namespace TaskApi.Validation;

/// <summary>
/// Проверяет и нормализует заголовки задач.
/// </summary>
public static class TitleValidator
{
    /// <summary>
    /// Проверяет заголовок и удаляет пробелы в начале и конце.
    /// </summary>
    /// <param name="title">Заголовок для проверки.</param>
    /// <param name="normalized">Нормализованный заголовок, если проверка прошла успешно.</param>
    /// <param name="error">Описание ошибки, если заголовок не прошёл проверку.</param>
    /// <returns>
    /// <see langword="true"/>, если заголовок допустим; иначе <see langword="false"/>.
    /// </returns>
    public static bool TryNormalize(
        string? title,
        out string normalized,
        out string error)
    {
        normalized = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            error = "Заголовок не должен быть пустым.";
            return false;
        }

        var trimmed = title.Trim();
        if (trimmed.Length > TaskItem.TitleMaxLength)
        {
            error = $"Длина заголовка не должна превышать {TaskItem.TitleMaxLength} символов.";
            return false;
        }

        normalized = trimmed;
        return true;
    }
}