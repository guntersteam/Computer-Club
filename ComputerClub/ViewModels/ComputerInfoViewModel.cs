using DL.Entities;

namespace ComputerClub.ViewModels;

public class ComputerInfoViewModel
{
    Computer Computer {  get; set; }
    List<Order> orders { get; set; }

}
