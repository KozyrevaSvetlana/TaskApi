using TaskApi.Data;

namespace TaskApi.Messaging;

/// <summary>
/// Сериализуется в формате: { "taskId": "...", "title": "...", "completedAt": "2026-05-07T12:00:00Z", "priority": "High" }.
/// Свойство CompletedAt содержит дату и время в формате UTC, поэтому при сериализации используется суффикс "Z".
/// </summary>
public sealed record TaskCompletedMessage(Guid TaskId, string Title, DateTime CompletedAt, Priority Priority);
