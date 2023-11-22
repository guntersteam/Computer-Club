using DL.Entities;
namespace ComputerClub.ViewModels;

public class ComputerOrdersViewModel
{
    public Computer Computer { get; set; }
    public Order?  Order { get; set; }
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
    public List<Order>? Orders { get; set; }
    public List<Review>? Reviews { get; set; }
    
    public SortParams? SortParams { get; set; }
}

public class SortParams
{
    public string? SortBy { get; set; }
    public int Id { get; set; }
}