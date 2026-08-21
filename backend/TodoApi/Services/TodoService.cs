using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services;

public sealed class TodoService(ITodoRepository repository) : ITodoService
{
    public IReadOnlyList<Todo> GetAll() => repository.GetAll();

    public Todo? GetById(Guid id) => repository.GetById(id);

    public Todo Create(string title)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        return repository.Add(todo);
    }

    public Todo? Update(Guid id, string title)
    {
        var existing = repository.GetById(id);
        if (existing is null)
        {
            return null;
        }

        var updated = new Todo
        {
            Id = existing.Id,
            Title = title.Trim(),
            CreatedAt = existing.CreatedAt
        };

        return repository.Update(updated);
    }

    public bool Delete(Guid id) => repository.Remove(id);
}
