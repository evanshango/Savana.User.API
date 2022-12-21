using Savana.User.API.Dtos;
using Savana.User.API.Entities;
using Savana.User.API.Requests;
using Savana.User.API.Requests.Params;
using Treasures.Common.Helpers;

namespace Savana.User.API.Interfaces; 

public interface IGroupService {
    Task<GroupDto?> AddGroup(GroupReq groupReq, string createdBy);
    Task<GroupDto?> GetGroupById(string groupId);
    Task<PagedList<GroupEntity>> GetGroups(GroupParams groupParams);
    Task<GroupDto?> UpdateGroup(string groupId, GroupReq groupReq, string updatedBy);
    Task<GroupEntity?> FindGroupByName(string groupName);
}