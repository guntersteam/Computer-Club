using DL.Entities;

namespace ComputerClub.ViewModels;

public class UserOrdersViewModel
{
    public string UserId { get; set; }
    public List<Order> UserOrders { get; set; }
}
