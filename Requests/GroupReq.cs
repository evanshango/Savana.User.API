namespace Savana.User.API.Requests; 

public class GroupReq {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IList<string>? RoleIds { get; set; } = new List<string>();
}