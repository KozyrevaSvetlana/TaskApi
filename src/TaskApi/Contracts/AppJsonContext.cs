using System.Text.Json.Serialization;
using TaskApi.Messaging;

namespace TaskApi.Contracts;

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
