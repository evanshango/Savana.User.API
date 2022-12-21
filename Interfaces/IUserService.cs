using Savana.User.API.Dtos;
using Savana.User.API.Entities;
using Savana.User.API.Requests;
using Savana.User.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.User.API.Interfaces; 

public interface IUserService {
    Task<PagedList<UserEntity>> GetUsers(UserParams userParams);
    Task<UserDto?> GetUserById(string userId);
    Task<UserDto?> UpdateUserGroups(string userId, UserGroupReq userGroupReq);
    Task<UserEntity?> DeleteUser(string userId);
    Task<UserDto?> UpdateUser(string userId, UserReq userReq);
}