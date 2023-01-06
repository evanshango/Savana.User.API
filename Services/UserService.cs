using Microsoft.AspNetCore.Identity;
using Savana.User.API.Dtos;
using Savana.User.API.Entities;
using Savana.User.API.Extensions;
using Savana.User.API.Interfaces;
using Savana.User.API.Requests;
using Savana.User.API.Requests.Params;
using Savana.User.API.Specifications;
using Treasures.Common.Helpers;
using Treasures.Common.Interfaces;

namespace Savana.User.API.Services;

public class UserService : IUserService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<UserEntity> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, UserManager<UserEntity> userManager, ILogger<UserService> logger) {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<PagedList<UserEntity>> GetUsers(UserParams userParams) {
        var userSpec = new UserSpecification(userParams);
        return await _unitOfWork.Repository<UserEntity>().GetPagedAsync(userSpec, userParams.Page, userParams.PageSize);
    }

    public async Task<UserDto?> GetUserById(string userId) {
        var user = await FindUser(userId);
        return user?.MapUserToDto("single");
    }

    public async Task<UserDto?> UpdateUserGroups(string userId, UserGroupReq userGroupReq) {
        var user = await FindUser(userId);

        if (user == null) {
            _logger.LogWarning("User with id {Id} not found", userId);
            return null;
        }

        if (userGroupReq.GroupIds?.Count > 0) {
            var uGroups = new List<UserGroup>();
            foreach (var groupId in userGroupReq.GroupIds) {
                var group = await _unitOfWork.Repository<GroupEntity>().GetByIdAsync(groupId);
                if (group != null) {
                    uGroups.Add(new UserGroup { Group = group, GroupId = group.Id, User = user, UserId = user.Id });
                }
            }

            user.UserGroups.Clear();
            user.UserGroups = uGroups;
        }
        else user.UserGroups.Clear();

        user.UpdatedAt = DateTime.UtcNow;

        var res = _unitOfWork.Repository<UserEntity>().UpdateAsync(user);
        var result = await _unitOfWork.Complete();
        if (result >= 1) return res.MapUserToDto("single");
        _logger.LogError("Error while updating User with email {Email}'s Group", user.Email);
        return null;
    }

    public async Task<UserEntity?> DeleteUser(string userId) {
        var existing = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(userId);

        if (existing == null) {
            _logger.LogWarning("User with id {Id} not found", userId);
            return null;
        }

        existing.UserGroups.Clear();
        existing.Enabled = false;
        existing.UpdatedAt = DateTime.UtcNow;

        var res = _unitOfWork.Repository<UserEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();
        if (result >= 1) return res;
        _logger.LogError("Error while deleting User with id {Id}", existing.Id);
        return null;
    }

    public async Task<UserDto?> UpdateUser(string userId, UserReq userReq) {
        var existing = await FindUser(userId);

        if (existing == null) {
            _logger.LogWarning("User with id {Id} not found", userId);
            return null;
        }

        if (userReq.CurrentPass == null && userReq.NewPass == null)
            return await FinalizeUpdate(existing, userReq);

        var res = await _userManager.ChangePasswordAsync(existing, userReq.CurrentPass, userReq.NewPass);

        return res.Succeeded ? await FinalizeUpdate(existing, userReq) : null;
    }

    private async Task<UserDto?> FinalizeUpdate(UserEntity existing, UserReq userReq) {
        existing.FirstName = userReq.FirstName ?? existing.FirstName;   
        existing.LastName = userReq.LastName ?? existing.LastName;
        existing.Gender = userReq.Gender ?? existing.Gender;
        existing.PhoneNumber = userReq.PhoneNo ?? existing.PhoneNumber;
        existing.UpdatedAt = DateTime.UtcNow;

        var res = _unitOfWork.Repository<UserEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();
        if (result >= 1) return res.MapUserToDto("single");
        _logger.LogError("Error while updating User with email {Email}", existing.Email);
        return null;
    }

    private async Task<UserEntity?> FindUser(string userId) => await _unitOfWork
        .Repository<UserEntity>().GetEntityWithSpec(new UserSpecification(userId));
}