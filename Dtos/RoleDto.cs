namespace Savana.User.API.Dtos; 

public class RoleDto {
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? UniqueName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}