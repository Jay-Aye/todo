using System.Collections.Concurrent;
using TodoApi.Models;

namespace TodoApi.Repositories;

public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, Todo> _todos = new();

    public IReadOnlyList<Todo> GetAll() =>
        _todos.Values
            .OrderBy(t => t.CreatedAt)
            .ToList();

    public Todo? GetById(Guid id) =>
        _todos.TryGetValue(id, out var todo) ? todo : null;

    public Todo Add(Todo todo)
    {
        if (!_todos.TryAdd(todo.Id, todo))
        {
            throw new InvalidOperationException($"A todo with id '{todo.Id}' already exists.");
        }

        return todo;
    }

    public Todo? Update(Todo todo)
    {
        if (!_todos.ContainsKey(todo.Id))
        {
            return null;
        }

        _todos[todo.Id] = todo;
        return todo;
    }

    public bool Remove(Guid id) => _todos.TryRemove(id, out _);
}
