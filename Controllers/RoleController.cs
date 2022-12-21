using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.User.API.Dtos;
using Savana.User.API.Extensions;
using Savana.User.API.Interfaces;
using Savana.User.API.Requests;
using Savana.User.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;

namespace Savana.User.API.Controllers;

[ApiController, Route("roles"), Produces("application/json"), Tags("Roles"), Authorize(Roles = "Admin")]
public class RoleController : ControllerBase {
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService) => _roleService = roleService;

        [HttpGet("")]
    public async Task<ActionResult<PagedList<RoleDto>>> GetRoles([FromQuery] RoleParams roleParams) {
        var roles = await _roleService.GetRoles(roleParams);
        Response.AddPaginationHeader(roles.MetaData);
        return Ok(roles.Select(g => g.MapRoleToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> AddGroup([FromBody] RoleReq roleReq) {
        var response = await _roleService.AddRole(roleReq);
        return response != null
            ? CreatedAtRoute("GetRole", new { roleId = response.Id }, response)
            : BadRequest(new ApiException(400, "Unable to add a new Role"));
    }

    [HttpGet("{roleId}", Name = "GetRole")]
    public async Task<ActionResult<RoleDto>> GetRole(string roleId) {
        var res = await _roleService.GetRoleById(roleId);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Role with id '{roleId}' not found"));
    }

    [HttpPut("{roleId}")]
    public async Task<ActionResult<RoleDto>> UpdateRole([FromRoute] string roleId, [FromBody] RoleReq roleReq) {
        var res = await _roleService.UpdateRole(roleId, roleReq);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Role with id '{roleId}' not found"));
    }
}