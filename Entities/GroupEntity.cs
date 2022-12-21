using System.ComponentModel.DataAnnotations;
using Treasures.Common.Helpers;

namespace Savana.User.API.Entities;

public class GroupEntity : BaseEntity {
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string? Slug { get; set; }
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public virtual ICollection<GroupRole> RoleGroups { get; set; } = new List<GroupRole>();

    public GroupEntity(string id, string name, string description, string? createdBy) {
        Id = id;
        Name = name;
        Description = description;
        Slug = GetSlug();
        CreatedBy = createdBy;
    }

    public string GetSlug() {
        return Name.Replace(" ", "-").ToLower();
    }
}