using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests;

public class InMemoryTodoRepositoryTests
{
    private readonly InMemoryTodoRepository _repository = new();

    [Fact]
    public void GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var todos = _repository.GetAll();

        Assert.Empty(todos);
    }

    [Fact]
    public void Add_ThenGetById_ReturnsStoredTodo()
    {
        var todo = CreateTodo("Buy milk");

        var stored = _repository.Add(todo);
        var fetched = _repository.GetById(todo.Id);

        Assert.Same(todo, stored);
        Assert.NotNull(fetched);
        Assert.Equal(todo.Id, fetched.Id);
        Assert.Equal("Buy milk", fetched.Title);
    }

    [Fact]
    public void GetById_WhenMissing_ReturnsNull()
    {
        var fetched = _repository.GetById(Guid.NewGuid());

        Assert.Null(fetched);
    }

    [Fact]
    public void GetAll_ReturnsTodosOrderedByCreatedAt()
    {
        var older = CreateTodo("First", DateTimeOffset.UtcNow.AddMinutes(-5));
        var newer = CreateTodo("Second", DateTimeOffset.UtcNow);

        _repository.Add(newer);
        _repository.Add(older);

        var todos = _repository.GetAll();

        Assert.Equal([older.Id, newer.Id], todos.Select(t => t.Id));
    }

    [Fact]
    public void Update_WhenExists_ReplacesTodo()
    {
        var original = CreateTodo("Old title");
        _repository.Add(original);

        var updated = new Todo
        {
            Id = original.Id,
            Title = "New title",
            CreatedAt = original.CreatedAt
        };

        var result = _repository.Update(updated);
        var fetched = _repository.GetById(original.Id);

        Assert.NotNull(result);
        Assert.Equal("New title", result.Title);
        Assert.NotNull(fetched);
        Assert.Equal("New title", fetched.Title);
        Assert.Equal(original.CreatedAt, fetched.CreatedAt);
    }

    [Fact]
    public void Update_WhenMissing_ReturnsNull()
    {
        var result = _repository.Update(CreateTodo("Missing"));

        Assert.Null(result);
    }

    [Fact]
    public void Remove_WhenExists_ReturnsTrueAndDeletesTodo()
    {
        var todo = CreateTodo("Temporary");
        _repository.Add(todo);

        var removed = _repository.Remove(todo.Id);

        Assert.True(removed);
        Assert.Null(_repository.GetById(todo.Id));
    }

    [Fact]
    public void Remove_WhenMissing_ReturnsFalse()
    {
        var removed = _repository.Remove(Guid.NewGuid());

        Assert.False(removed);
    }

    [Fact]
    public void Add_WithDuplicateId_Throws()
    {
        var todo = CreateTodo("Once");
        _repository.Add(todo);

        Assert.Throws<InvalidOperationException>(() => _repository.Add(todo));
    }

    private static Todo CreateTodo(string title, DateTimeOffset? createdAt = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow
        };
}
