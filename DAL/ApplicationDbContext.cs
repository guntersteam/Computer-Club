using DL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    DbSet<AppUser> User {get; set; }
    DbSet<Computer> Computer { get; set; }
    DbSet<Order> Order { get; set; }
    DbSet<Review> Review { get; set; }
    DbSet<Payment> Payment { get; set; }
}
