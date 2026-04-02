using Microsoft.EntityFrameworkCore;
using OnitLab1.Models;

namespace OnitLab1.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
}