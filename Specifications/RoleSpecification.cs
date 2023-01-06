using Savana.User.API.Entities;
using Savana.User.API.Requests.Params;
using Treasures.Common.Services;

namespace Savana.User.API.Specifications;

public class RoleSpecification : SpecificationService<RoleEntity> {
    public RoleSpecification(RoleParams roleParams) : base(r =>
        string.IsNullOrEmpty(roleParams.Name) || r.Name.ToLower().Equals(roleParams.Name.Trim().ToLower())
    ) { }

    public RoleSpecification(string? roleName) : base(r =>
        string.IsNullOrEmpty(roleName) || r.Name.ToLower().Equals(roleName.Trim())
    ) { }
}