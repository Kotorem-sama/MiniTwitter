using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniTwitter.Models;
using MiniTwitter.Models.Classes;

namespace MiniTwitter.Data;

public class ApplicationDbContext : IdentityDbContext<User>
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
}