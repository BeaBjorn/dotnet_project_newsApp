using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewsApp.Models;

//Namespace
namespace NewsApp.Data;

//Class for ApplicationDbContext
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //DbSet for the Category table
    public DbSet<Category>? categories { get; set; }

    //DbSet for the Article table
    public DbSet<Article>? Articles { get; set; }
}
