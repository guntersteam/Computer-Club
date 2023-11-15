using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ComputerClub.ViewModels;

public class OrderViewModel
{ 
    public int OrderId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartTime { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndTime { get; set; }
    public bool State { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; } 
    public int ComputerId { get; set; }
    public string ModelName { get; set;}
    public List<SelectListItem> Computers { get; set; }
    public List<SelectListItem> Users { get; set; }
}
