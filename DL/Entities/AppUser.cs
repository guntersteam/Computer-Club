using DL.Enums;
using Microsoft.AspNetCore.Identity;

namespace DL.Entities;

public class AppUser :IdentityUser
{
    public UserRole Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string  Email { get; set; }
    public string PhoneNumber { get; set; }
    public ICollection<Order> Orders { get; set; }
}
