using Microsoft.AspNetCore.Mvc;
using Savana.User.API.Interfaces;
using Savana.User.API.Requests;
using Savana.User.API.Responses;
using Treasures.Common.Helpers;

namespace Savana.User.API.Controllers;

[ApiController, Route("auth"), Produces("application/json"), Tags("Authentication")]
public class AuthController : ControllerBase {
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("signup")]
    public async Task<ActionResult<SignupRes>> Signup([FromBody] SignupReq signupReq) {
        var response = await _authService.Signup(signupReq);
        return response.StatusCode != 201
            ? BadRequest(new ApiException(400, $"{response.Message}"))
            : new JsonResult(response) { StatusCode = StatusCodes.Status201Created };
    }

    [HttpPost("signin")]
    public async Task<ActionResult<SigninRes>> Signin([FromBody] SigninReq signinReq) {
        var res = await _authService.Signin(signinReq);
        return res.StatusCode == 200 ? Ok(res) : Unauthorized(new ApiException(401, $"{res.Message}"));
    }
}