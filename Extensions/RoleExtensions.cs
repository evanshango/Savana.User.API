using Savana.User.API.Dtos;
using Savana.User.API.Entities;

namespace Savana.User.API.Extensions;

public static class RoleExtension {
    public static RoleDto MapRoleToDto(this RoleEntity role) {
        return new RoleDto {
            Id = role.Id, Name = role.Name, Description = role.Description, UniqueName = role.NormalizedName,
            CreatedAt = role.CreatedAt
        };
    }
}