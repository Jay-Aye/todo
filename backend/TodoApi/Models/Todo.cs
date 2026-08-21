namespace TodoApi.Models;

public sealed class Todo
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
