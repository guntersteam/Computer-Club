using DL.Enums;
using System.ComponentModel.DataAnnotations;

namespace ComputerClub.ViewModels;

public class PaymentViewModel
{ 
    public int PaymentId { get; set; }

    [Required]
    public int Amount { get; set; }
    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    public int OrderId { get; set; }
}
