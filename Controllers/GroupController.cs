using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savana.User.API.Dtos;
using Savana.User.API.Extensions;
using Savana.User.API.Interfaces;
using Savana.User.API.Requests;
using Savana.User.API.Requests.Params;
using Treasures.Common.Extensions;
using Treasures.Common.Helpers;
using Treasures.Common.Responses;

namespace Savana.User.API.Controllers;

[ApiController, Route("groups"), Produces("application/json"), Tags("Groups"), Authorize(Roles = "Admin")]
public class GroupController : ControllerBase {
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService) => _groupService = groupService;

    [HttpGet("")]
    public async Task<ActionResult<PagedList<GroupDto>>> GetGroups([FromQuery] GroupParams groupParams) {
        var groups = await _groupService.GetGroups(groupParams);
        Response.AddPaginationHeader(groups.MetaData);
        return Ok(groups.Select(g => g.MapGroupToDto("multi")).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> AddGroup([FromBody] GroupReq groupReq) {
        var createdBy = User.RetrieveEmailFromPrincipal();
        var response = await _groupService.AddGroup(groupReq, createdBy);
        return response != null
            ? CreatedAtRoute("GetGroup", new { slug = response.Slug }, response)
            : BadRequest(new ErrorResponse(400, "Unable to Create Group"));
    }

    [HttpGet("{slug}", Name = "GetGroup")]
    public async Task<ActionResult<GroupDto>> GetGroup([FromRoute] string slug) {
        var res = await _groupService.GetGroupBySlug(slug);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Group with slug '{slug}' not found"));
    }

    [HttpPut("{slug}")]
    public async Task<ActionResult<GroupDto>> UpdateGroup([FromRoute] string slug, [FromBody] GroupReq groupReq) {
        var updatedBy = User.RetrieveEmailFromPrincipal();
        var res = await _groupService.UpdateGroup(slug, groupReq, updatedBy);
        return res != null ? Ok(res) : NotFound(new ApiException(404, $"Group with slug '{slug}' not found"));
    }
}