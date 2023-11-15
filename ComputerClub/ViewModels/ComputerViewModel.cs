using System.ComponentModel.DataAnnotations;

namespace ComputerClub.ViewModels;

public class ComputerViewModel
{
    public int ComputerId { get; set; }
    [Required]
    public string ModelName { get; set;}

    [Required]
    public int PriceForHour { get; set; }

    [Required]
    public bool IsReserved { get; set; }
}
