using TaskApi.Data;

namespace TaskApi.Validation;

public static class TitleValidator
{
    public static bool TryNormalize(string? title, out string normalized, out string error)
    {
        normalized = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            error = "Title must not be empty.";
            return false;
        }

        var trimmed = title.Trim();
        if (trimmed.Length > TaskItem.TitleMaxLength)
        {
            error = $"Title must not exceed {TaskItem.TitleMaxLength} characters.";
            return false;
        }

        normalized = trimmed;
        return true;
    }
}
