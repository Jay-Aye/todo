using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    IReadOnlyList<Todo> GetAll();
    Todo? GetById(Guid id);
    Todo Create(string title);
    Todo? Update(Guid id, string title);
    bool Delete(Guid id);
}
