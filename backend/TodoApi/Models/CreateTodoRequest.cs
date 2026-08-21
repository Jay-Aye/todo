using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public sealed class CreateTodoRequest : IValidatableObject
{
    [Required]
    [MaxLength(200)]
    public required string Title { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult("Title must not be empty.", [nameof(Title)]);
        }
    }
}
