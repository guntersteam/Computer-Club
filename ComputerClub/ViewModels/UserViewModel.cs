using DL.Entities;
using System.ComponentModel.DataAnnotations;

namespace ComputerClub.ViewModels;

public class UserViewModel
{
    public int UserId { get; set; }
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Login { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
    //[DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    public List<Order> Orders { get; set; }
}
