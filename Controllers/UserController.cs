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

[ApiController, Route("users"), Produces("application/json"), Tags("Users")]
public class UserController : ControllerBase {
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpGet(""), Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedList<UserDto>>> GetUsers([FromQuery] UserParams userParams) {
        var users = await _userService.GetUsers(userParams);
        Response.AddPaginationHeader(users.MetaData);
        return Ok(users.Select(g => g.MapUserToDto("multi")).ToList());
    }

    [HttpGet("{userId}", Name = "GetUser"), Authorize]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] string userId) {
        var res = await _userService.GetUserById(userId);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"User with id '{userId}' not found"));
    }

    [HttpPut("{userId}"), Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] string userId, [FromBody] UserReq userReq) {
        var current = User.RetrieveUserIdFromPrincipal();
        var roles = User.RetrieveRolesFromPrincipal();

        if (!(current.Equals(userId) || roles.Contains("Admin")))
            return Unauthorized(new ApiException(401, "Unauthorized Request"));

        var res = await _userService.UpdateUser(userId, userReq);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"User with id '{userId}' not found"));
    }

    [HttpDelete("{userId}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string userId) {
        var res = await _userService.DeleteUser(userId);
        return res != null
            ? Ok(new { Message = "User deleted successfully", Status = 200, Timestamp = DateTime.UtcNow })
            : BadRequest(new ApiException(400, "An error occurred while deleting user"));
    }

    [HttpPut("{userId}/groups"), Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateUserGroups(string userId, [FromBody] UserGroupReq userGroupReq) {
        var res = await _userService.UpdateUserGroups(userId, userGroupReq);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"User with id '{userId}' not found"));
    }
}