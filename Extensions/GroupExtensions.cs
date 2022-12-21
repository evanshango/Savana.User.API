using Savana.User.API.Dtos;
using Savana.User.API.Entities;

namespace Savana.User.API.Extensions;

public static class GroupExtension {
    public static GroupDto MapGroupToDto(this GroupEntity? group, string tag) {
        var roles = tag.Equals("single")
            ? group?.RoleGroups.Select(rg => new RoleDto {
                    Id = rg.Role?.Id, Name = rg.Role?.Name, Description = rg.Role?.Description,
                    UniqueName = rg.Role?.NormalizedName, CreatedAt = rg.Role!.CreatedAt
                }
            ).ToList()
            : null;
        return new GroupDto {
            Id = group?.Id, Name = group?.Name, Description = group?.Description, Slug = group?.Slug,
            CreatedAt = group!.CreatedAt, Members = group.UserGroups.Count, Roles = roles
        };
    }
}