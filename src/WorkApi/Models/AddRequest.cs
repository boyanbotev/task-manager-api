using System.ComponentModel.DataAnnotations;

namespace WorkApi.Models;
public class AddRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }

    public AddRequest() { }
}
