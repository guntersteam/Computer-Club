using DL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    DbSet<AppUser> User {get; set; }
    DbSet<Computer> Computer { get; set; }
    DbSet<Order> Order { get; set; }

}
