using Savana.User.API.Dtos;
using Savana.User.API.Entities;
using Savana.User.API.Requests;
using Savana.User.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.User.API.Interfaces; 

public interface IRoleService {
    Task<PagedList<RoleEntity>> GetRoles(RoleParams roleParams);
    Task<RoleDto?> GetRoleById(string roleId);
    Task<RoleDto?> AddRole(RoleReq roleReq);
    Task<RoleDto?> UpdateRole(string roleId, RoleReq roleReq);
}