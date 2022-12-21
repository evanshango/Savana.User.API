namespace Savana.User.API.Entities;

public class UserGroup {
    public string? UserId { get; set; }
    public virtual UserEntity? User { get; set; }
    public string? GroupId { get; set; }
    public virtual GroupEntity? Group { get; set; }
}