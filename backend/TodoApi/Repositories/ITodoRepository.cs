using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITodoRepository
{
    IReadOnlyList<Todo> GetAll();
    Todo? GetById(Guid id);
    Todo Add(Todo todo);
    Todo? Update(Todo todo);
    bool Remove(Guid id);
}
