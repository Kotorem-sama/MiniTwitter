using Microsoft.EntityFrameworkCore;
using MiniTwitter.Models;
using MiniTwitter.Models.Classes;

namespace MiniTwitter.Data;

public class ApplicationDbContext : DbContext
{
    public IConfiguration _configuration { get; set; }
    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DatabaseConnection"));
    }
    public DbSet<Tweet> Tweets { get; set; }
    public DbSet<User> Users { get; set; }
}