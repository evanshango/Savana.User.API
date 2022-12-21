namespace Savana.User.API.Responses; 

public class SigninRes {
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
    public int? StatusCode { get; set; }
    public string? Token { get; set; }
    public DateTime TimeStamp { get; set; }
}