using System.ComponentModel.DataAnnotations;

namespace ComputerClub.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email address")]
    public string Email { get; set; }

    [Required(ErrorMessage ="Login is required")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Phone number is Required")]
    [Display(Name = "Phone number")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }


    [Required(ErrorMessage = "Password must be confirmed")]
    [Display(Name = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password don't match")]
    public string ConfirmPassword { get; set; }
}
