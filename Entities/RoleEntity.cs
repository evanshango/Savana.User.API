using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Savana.User.API.Entities;

public sealed class RoleEntity : IdentityRole {
    public ICollection<GroupRole>? RoleGroups { get; set; }
    public string Description { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    [Required] public bool Active { get; set; } = true;

    public RoleEntity(string id, string name, string description) {
        Id = id;
        Name = name;
        Description = description;
        NormalizedName = name.ToUpper();
    }
}