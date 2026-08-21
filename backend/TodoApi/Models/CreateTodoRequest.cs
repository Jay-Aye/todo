using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public sealed class CreateTodoRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public required string Title { get; init; }
}
