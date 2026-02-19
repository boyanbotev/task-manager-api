using System.ComponentModel.DataAnnotations;

namespace WorkApi.Models;

public class DeleteRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string UserId { get; set; }

    public DeleteRequest() { }
}
