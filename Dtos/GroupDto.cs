namespace Savana.User.API.Dtos; 

public class GroupDto {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public int? Members { get; set; }
    public IReadOnlyList<RoleDto>? Roles { get; set; }
    public DateTime CreatedAt { get; set; }
}