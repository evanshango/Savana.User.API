namespace Savana.User.API.Requests; 

public class UserGroupReq {
    public IList<string>? GroupIds { get; set; } = new List<string>();
}