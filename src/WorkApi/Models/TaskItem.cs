using System.ComponentModel.DataAnnotations.Schema;

namespace WorkApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))] 
    public User User { get; set; }

    public TaskItem() { }
}
