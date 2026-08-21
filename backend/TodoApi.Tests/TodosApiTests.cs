using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TodoApi.Tests;

public class TodosApiTests
{
    private static WebApplicationFactory<Program> CreateFactory() => new();

    [Fact]
    public async Task GetTodos_WhenEmpty_ReturnsEmptyArray()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/todos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todos = await response.Content.ReadFromJsonAsync<TodoResponse[]>();
        Assert.NotNull(todos);
        Assert.Empty(todos);
    }

    [Fact]
    public async Task CreateTodo_WithValidTitle_ReturnsCreatedTodo()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Buy milk"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(todo);
        Assert.NotEqual(Guid.Empty, todo.Id);
        Assert.Equal("Buy milk", todo.Title);
        Assert.True(todo.CreatedAt <= DateTimeOffset.UtcNow.AddSeconds(5));
        Assert.True(todo.CreatedAt >= DateTimeOffset.UtcNow.AddMinutes(-1));
        Assert.Equal($"/api/todos/{todo.Id}", response.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task CreateTodo_WithEmptyTitle_ReturnsBadRequest()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_ThenGetTodos_IncludesCreatedItem()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Walk the dog"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var listResponse = await client.GetAsync("/api/todos");
        var todos = await listResponse.Content.ReadFromJsonAsync<TodoResponse[]>();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.NotNull(created);
        Assert.NotNull(todos);
        Assert.Contains(todos, t => t.Id == created.Id && t.Title == "Walk the dog");
    }

    [Fact]
    public async Task GetTodo_WhenExists_ReturnsTodo()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Read a book"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);

        var response = await client.GetAsync($"/api/todos/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var todo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(todo);
        Assert.Equal(created.Id, todo.Id);
        Assert.Equal("Read a book", todo.Title);
        Assert.Equal(created.CreatedAt, todo.CreatedAt);
    }

    [Fact]
    public async Task GetTodo_WhenMissing_ReturnsNotFound()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var listResponse = await client.GetAsync("/api/todos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var response = await client.GetAsync($"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WhenExists_ReturnsUpdatedTodo()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Old title"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);

        var response = await client.PutAsJsonAsync(
            $"/api/todos/{created.Id}",
            new UpdateTodoRequest("New title"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("New title", updated.Title);
        Assert.Equal(created.CreatedAt, updated.CreatedAt);
    }

    [Fact]
    public async Task UpdateTodo_WhenMissing_ReturnsNotFound()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var listResponse = await client.GetAsync("/api/todos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var response = await client.PutAsJsonAsync(
            $"/api/todos/{Guid.NewGuid()}",
            new UpdateTodoRequest("Does not matter"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WithEmptyTitle_ReturnsBadRequest()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Keep me"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);

        var response = await client.PutAsJsonAsync(
            $"/api/todos/{created.Id}",
            new UpdateTodoRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WhenExists_ReturnsNoContentAndRemovesItem()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Temporary"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);

        var deleteResponse = await client.DeleteAsync($"/api/todos/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var listResponse = await client.GetAsync("/api/todos");
        var todos = await listResponse.Content.ReadFromJsonAsync<TodoResponse[]>();
        Assert.NotNull(todos);
        Assert.DoesNotContain(todos, t => t.Id == created.Id);
    }

    [Fact]
    public async Task DeleteTodo_WhenMissing_ReturnsNotFound()
    {
        await using var factory = CreateFactory();
        var client = factory.CreateClient();

        var listResponse = await client.GetAsync("/api/todos");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var response = await client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record CreateTodoRequest(string Title);

    private sealed record UpdateTodoRequest(string Title);

    private sealed record TodoResponse(Guid Id, string Title, DateTimeOffset CreatedAt);
}
