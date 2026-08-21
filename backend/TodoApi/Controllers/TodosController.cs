using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/todos")]
public sealed class TodosController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<Todo>> GetTodos() => Ok(todoService.GetAll());

    [HttpGet("{id:guid}")]
    public ActionResult<Todo> GetTodo(Guid id)
    {
        var todo = todoService.GetById(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public ActionResult<Todo> CreateTodo([FromBody] CreateTodoRequest request)
    {
        var todo = todoService.Create(request.Title);
        return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<Todo> UpdateTodo(Guid id, [FromBody] UpdateTodoRequest request)
    {
        var todo = todoService.Update(id, request.Title);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteTodo(Guid id)
    {
        if (!todoService.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}
