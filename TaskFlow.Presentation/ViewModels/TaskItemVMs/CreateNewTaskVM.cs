using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.TaskItemVMs;

public class CreateNewTaskVM
{
    [Required(ErrorMessage = "Assigned User ID is required.")]
    public Guid AssignedUserId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Due date is required.")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    
}