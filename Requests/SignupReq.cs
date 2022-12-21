using System.ComponentModel.DataAnnotations;

namespace Savana.User.API.Requests;

public class SignupReq {
    [Required(ErrorMessage = "FirstName is required")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email Address is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*?&])([a-zA-Z0-9@$!%*?&]{6,})$",
        ErrorMessage = "Password must have at least an uppercase, a lowercase, a number, a special character and at" +
                       " least 6 characters"
    )]
    public string? Password { get; set; }
}