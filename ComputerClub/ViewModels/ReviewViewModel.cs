using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComputerClub.ViewModels;

public class ReviewViewModel
{
    public int ReviewId { get; set; }

    public string ReviewText { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int ComputerId { get; set; }
    public int Rating { get; set; }
    public string ModelName { get; set; }
    public string Email { get; set; }   
    public List<SelectListItem> Computers { get; set; }
    public List<SelectListItem> Users { get; set; }
}
