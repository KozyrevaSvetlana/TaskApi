using TaskApi.Data;

namespace TaskApi.Messaging;

/// <summary>
/// Serialized as: { "taskId": "...", "title": "...", "completedAt": "2026-05-07T12:00:00Z", "priority": "High" }.
/// CompletedAt is a UTC DateTime so that it is written with the "Z" suffix.
/// </summary>
public sealed record TaskCompletedMessage(Guid TaskId, string Title, DateTime CompletedAt, Priority Priority);
