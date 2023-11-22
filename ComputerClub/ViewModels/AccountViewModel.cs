using DL.Enums;

namespace ComputerClub.ViewModels;

public class AccountViewModel
{
    public string UserId { get; set; }
    public UserRole Role { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
}
