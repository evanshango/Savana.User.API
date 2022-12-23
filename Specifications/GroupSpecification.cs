using Savana.User.API.Entities;
using Savana.User.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.User.API.Specifications;

public class GroupSpecification : SpecificationService<GroupEntity> {
    public GroupSpecification(string? slug, bool? active) : base(g =>
        (string.IsNullOrEmpty(slug) || g.Slug!.Equals(slug)) &&
        (active != null ? g.Active == active : g.Active == true)
    ) {
        AddInclude(g => g.UserGroups);
        AddInclude(g => g.RoleGroups);
        AddInclude($"{nameof(GroupEntity.RoleGroups)}.{nameof(GroupRole.Role)}");
    }

    public GroupSpecification(string? groupName) : base(g =>
        string.IsNullOrEmpty(groupName) || g.Name.ToLower().Equals(groupName.ToLower())
    ) => AddInclude(g => g.UserGroups);

    public GroupSpecification(GroupParams groupParams) : base(g =>
        string.IsNullOrEmpty(groupParams.SearchTerm) || g.Name.ToLower().Equals(groupParams.SearchTerm)
    ) {
        AddInclude(g => g.UserGroups);
        if (string.IsNullOrEmpty(groupParams.OrderBy)) return;
        switch (groupParams.OrderBy) {
            case "createdAt":
                AddOrderByAsc(g => g.CreatedAt);
                break;
            case "createdAtDesc":
                AddOrderByDesc(g => g.CreatedAt);
                break;
            default:
                AddOrderByDesc(g => g.CreatedAt);
                break;
        }
    }
}