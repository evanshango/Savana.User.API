namespace Savana.User.API.Requests; 

public class RoleReq {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? Active { get; set; }
}