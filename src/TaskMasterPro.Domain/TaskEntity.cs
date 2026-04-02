using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerPro.TaskMasterPro.Domain;

public class TaskEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int UserId { get; set; }
    public UserEntity UserEntity { get; set; }
}