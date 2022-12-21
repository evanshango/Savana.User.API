using Savana.User.API.Dtos;
using Savana.User.API.Entities;

namespace Savana.User.API.Extensions;

public static class UserExtensions {
    public static UserDto MapUserToDto(this UserEntity user, string tag) {
        var groups = tag.Equals("single")
            ? user.UserGroups.Select(ug => ug.Group.MapGroupToDto("single")).ToList()
            : null;
        return new UserDto {
            Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, CreatedAt = user.CreatedAt,
            Email = user.Email, PhoneNo = user.PhoneNumber, Gender = user.Gender, Groups = groups
        };
    }
}