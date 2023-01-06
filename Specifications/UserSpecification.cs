using Savana.User.API.Entities;
using Savana.User.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.User.API.Specifications;

public class UserSpecification : SpecificationService<UserEntity> {
    public UserSpecification(UserParams userParams) : base(x =>
        (string.IsNullOrEmpty(userParams.Name) || x.FirstName!.ToLower().Equals(userParams.Name.Trim().ToLower()) || 
         x.LastName!.ToLower().Equals(userParams.Name.Trim().ToLower())) && x.Enabled == userParams.Enabled
    ) {
        if (string.IsNullOrEmpty(userParams.OrderBy)) return;
        switch (userParams.OrderBy) {
            case "createdAt":
                AddOrderByAsc(u => u.CreatedAt);
                break;
            case "createdAtDesc":
                AddOrderByDesc(u => u.CreatedAt);
                break;
            default:
                AddOrderByDesc(u => u.CreatedAt);
                break;
        }
    }

    public UserSpecification(string userId) : base(x => x.Id.Equals(userId)) {
        AddInclude(u => u.UserGroups);
        AddInclude(
            $"{nameof(UserEntity.UserGroups)}.{nameof(UserGroup.Group)}" +
            $".{nameof(GroupEntity.RoleGroups)}.{nameof(GroupRole.Role)}"
        );
    }
}