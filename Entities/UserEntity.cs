using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Savana.User.API.Entities; 

public class UserEntity : IdentityUser {
    [Required] public string? FirstName { get; set; }
    [Required] public string? LastName { get; set; }
    [Required] public string? Gender { get; set; }
    [Required] public bool Enabled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime PassExpiresIn { get; set; } = DateTime.UtcNow;
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}