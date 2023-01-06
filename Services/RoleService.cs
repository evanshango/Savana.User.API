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

public class RoleService : IRoleService {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoleService> _logger;

    public RoleService(IUnitOfWork unitOfWork, ILogger<RoleService> logger) {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedList<RoleEntity>> GetRoles(RoleParams rParams) {
        var roleSpec = new RoleSpecification(rParams);
        return await _unitOfWork.Repository<RoleEntity>().GetPagedAsync(roleSpec, rParams.Page, rParams.PageSize);
    }

    public async Task<RoleDto?> GetRoleById(string roleId) {
        var existing = await _unitOfWork.Repository<RoleEntity>().GetByIdAsync(roleId);
        return existing?.MapRoleToDto();
    }

    public async Task<RoleDto?> AddRole(RoleReq roleReq) {
        var roleSpec = new RoleSpecification(roleReq.Name);
        var existing = await _unitOfWork.Repository<RoleEntity>().GetEntityWithSpec(roleSpec);
        if (existing != null) {
            _logger.LogWarning("Role with name {Name} already exists", existing.Name);
            return existing.MapRoleToDto();
        }

        var newRole = new RoleEntity(Guid.NewGuid().ToString(), roleReq.Name!, roleReq.Description!);

        var res = _unitOfWork.Repository<RoleEntity>().AddAsync(newRole);
        var result = await _unitOfWork.Complete();
        if (result >= 1) return res.MapRoleToDto();
        _logger.LogError("Error while creating Role with name {Name}", roleReq.Name);
        return null;
    }

    public async Task<RoleDto?> UpdateRole(string roleId, RoleReq roleReq) {
        var existing = await _unitOfWork.Repository<RoleEntity>().GetByIdAsync(roleId);
        if (existing == null) {
            _logger.LogWarning("Role with id {Id} not found", roleId);
            return null;
        }

        existing.Name = roleReq.Name ?? existing.Name;
        existing.Description = roleReq.Description ?? existing.Description;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.Active = roleReq.Active ?? existing.Active;

        var res = _unitOfWork.Repository<RoleEntity>().UpdateAsync(existing);
        var result = await _unitOfWork.Complete();
        if (result >= 1) return res.MapRoleToDto();
        _logger.LogError("Error while updating Role with name {Name}", roleReq.Name);
        return null;
    }
}