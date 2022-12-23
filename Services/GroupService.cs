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

public class GroupService : IGroupService {
    private readonly IUnitOfWork _unitOfWork;

    public GroupService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<GroupDto?> AddGroup(GroupReq groupReq, string createdBy) {
        var groupByNameSpec = new GroupSpecification(groupName: groupReq.Name!);
        var existing = await _unitOfWork.Repository<GroupEntity>().GetEntityWithSpec(groupByNameSpec);

        if (existing != null) return existing.MapGroupToDto("single");

        var newGroup = new GroupEntity(Guid.NewGuid().ToString(), groupReq.Name!, groupReq.Description!, createdBy);
        newGroup.Slug = newGroup.GetSlug();

        var res = _unitOfWork.Repository<GroupEntity>().AddAsync(newGroup);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapGroupToDto("single");
    }

    public async Task<GroupDto?> GetGroupBySlug(string slug) {
        var groupSpec = new GroupSpecification(slug: slug, active: null);
        var existing = await _unitOfWork.Repository<GroupEntity>().GetEntityWithSpec(groupSpec);
        return existing?.MapGroupToDto("single");
    }

    public async Task<PagedList<GroupEntity>> GetGroups(GroupParams gParams) {
        var groupSpec = new GroupSpecification(gParams);
        return await _unitOfWork.Repository<GroupEntity>().GetPagedAsync(groupSpec, gParams.Page, gParams.PageSize);
    }

    public async Task<GroupDto?> UpdateGroup(string slug, GroupReq groupReq, string updatedBy) {
        var group = await FindGroup(slug);

        if (group == null) return null;
        var existingName = await FindGroupByName(groupReq.Name);
        if (existingName != null) return existingName.MapGroupToDto("single");

        if (groupReq.RoleIds?.Count > 0) {
            var gRoles = new List<GroupRole>();
            foreach (var roleId in groupReq.RoleIds) {
                var role = await _unitOfWork.Repository<RoleEntity>().GetByIdAsync(roleId);
                if (role != null) {
                    gRoles.Add(new GroupRole { Group = group, GroupId = group.Id, Role = role, RoleId = role.Id });
                }
            }

            group.RoleGroups.Clear();
            group.RoleGroups = gRoles;
        }
        else group.RoleGroups.Clear();

        group.Name = groupReq.Name ?? group.Name;
        group.Description = groupReq.Description ?? group.Description;
        group.Slug = group.GetSlug();
        group.UpdatedAt = DateTime.UtcNow;
        group.UpdatedBy = updatedBy;

        var res = _unitOfWork.Repository<GroupEntity>().UpdateAsync(group);
        var result = await _unitOfWork.Complete();
        return result < 1 ? null : res.MapGroupToDto("single");
    }

    public async Task<GroupEntity?> FindGroupByName(string? groupName) {
        var groupNameSpec = new GroupSpecification(groupName: groupName);
        return await _unitOfWork.Repository<GroupEntity>().GetEntityWithSpec(groupNameSpec);
    }

    private async Task<GroupEntity?> FindGroup(string slug) {
        var groupSpec = new GroupSpecification(slug: slug, active: null);
        return await _unitOfWork.Repository<GroupEntity>().GetEntityWithSpec(groupSpec);
    }
}