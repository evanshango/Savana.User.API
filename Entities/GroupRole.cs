namespace Savana.User.API.Entities; 

public class GroupRole {
    public string? GroupId { get; set; }
    public virtual GroupEntity? Group { get; set; }
    public string? RoleId { get; set; }
    public virtual RoleEntity? Role { get; set; }
}