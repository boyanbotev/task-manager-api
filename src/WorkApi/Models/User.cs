
using Microsoft.AspNetCore.Identity;

namespace WorkApi.Models;
public class User: IdentityUser
{    
    public List<TaskItem> Tasks { get; set; }
}