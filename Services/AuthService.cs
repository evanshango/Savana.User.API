using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Savana.User.API.Entities;
using Savana.User.API.Interfaces;
using Savana.User.API.Requests;
using Savana.User.API.Responses;

namespace Savana.User.API.Services;

public class AuthService : IAuthService {
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IGroupService _groupService;

    public AuthService(
        UserManager<UserEntity> uMgr, SignInManager<UserEntity> sMgr, ITokenService tkService, IGroupService grService
    ) {
        _userManager = uMgr;
        _signInManager = sMgr;
        _tokenService = tkService;
        _groupService = grService;
    }

    public async Task<SignupRes> Signup(SignupReq signupReq) {
        var group = await _groupService.FindGroupByName("User");

        var user = new UserEntity {
            FirstName = signupReq.FirstName, LastName = signupReq.LastName, Email = signupReq.Email,
            Gender = signupReq.Gender, PhoneNumber = signupReq.PhoneNumber, Enabled = true
        };
        var result = await _userManager.CreateAsync(user, signupReq.Password);

        if (!result.Succeeded)
            return new SignupRes {
                Message = result.Errors.ToList()[0].Description, TimeStamp = DateTime.UtcNow,
                StatusCode = (int)HttpStatusCode.BadRequest
            };

        var userGroup = new UserGroup { Group = group, GroupId = group?.Id, User = user, UserId = user.Id };

        user.UserGroups = new List<UserGroup> { userGroup };
        await _userManager.UpdateAsync(user);

        return new SignupRes {
            Message = "Account created successfully. Proceed to Signin.", TimeStamp = user.CreatedAt,
            StatusCode = (int)HttpStatusCode.Created
        };
    }

    public async Task<SigninRes> Signin(SigninReq signinReq) {
        var existing = await _userManager.Users
            .Include(u => u.UserGroups)
            .ThenInclude(ug => ug.Group)
            .ThenInclude(gr => gr!.RoleGroups)
            .ThenInclude(role => role.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(signinReq.Email!.ToLower()));

        if (existing == null)
            return new SigninRes {
                Message = "Invalid Credentials provided", StatusCode = (int)HttpStatusCode.Unauthorized,
                TimeStamp = DateTime.UtcNow
            };

        if (!existing.Enabled)
            return new SigninRes {
                Message = "Your account seems to have a problem. Contact customer care for assistance",
                StatusCode = (int)HttpStatusCode.Unauthorized, TimeStamp = DateTime.UtcNow
            };

        var result = await _signInManager.CheckPasswordSignInAsync(existing, signinReq.Password, true);
        if (!result.Succeeded)
            return new SigninRes {
                Message = "Invalid Credentials provided", StatusCode = (int)HttpStatusCode.Unauthorized,
                TimeStamp = DateTime.UtcNow
            };
        var token = _tokenService.GenerateToken(existing);
        return new SigninRes {
            Name = $"{existing.FirstName} {existing.LastName}", Email = existing.Email, Token = token,
            StatusCode = (int)HttpStatusCode.OK, TimeStamp = DateTime.UtcNow
        };
    }
}