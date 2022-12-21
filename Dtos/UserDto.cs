namespace Savana.User.API.Dtos; 

public class UserDto {
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNo { get; set; }
    public string? Gender { get; set; }
    public IReadOnlyList<GroupDto?>? Groups { get; set; }   
    public DateTime CreatedAt { get; set; }
}