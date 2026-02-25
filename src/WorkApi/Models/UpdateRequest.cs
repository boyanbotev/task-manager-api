using System.ComponentModel.DataAnnotations;
public class UpdateRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string UserId { get; set; }
}