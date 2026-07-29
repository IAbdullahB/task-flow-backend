using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Presentation.ViewModels.TaskItemVMs;

public class ReassignTaskVM
{
    [Required(ErrorMessage = "Requester user ID is required.")]
    public Guid RequesterUserId { get; set; }

    [Required(ErrorMessage = "Task ID is required.")]
    public Guid TaskId { get; set; }

    [Required(ErrorMessage = "New assigned user ID is required.")]
    public Guid NewAssignedUserId { get; set; }
}