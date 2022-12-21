namespace Savana.User.API.Requests; 

public class UserReq {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNo { get; set; }
    public string? CurrentPass { get; set; }
    public string? NewPass { get; set; }
}