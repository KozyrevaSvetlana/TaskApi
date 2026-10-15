using System.Text.Json.Serialization;
using TaskApi.Messaging;

namespace TaskApi.Contracts;

/// <summary>Source-generated JSON metadata: no reflection and fewer allocations on the hot path.</summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(CreateTaskRequest))]
[JsonSerializable(typeof(TaskResponse))]
[JsonSerializable(typeof(TaskPage))]
[JsonSerializable(typeof(TaskCompletedMessage))]
public partial class AppJsonContext : JsonSerializerContext
{
}
