#nullable disable
using Microsoft.EntityFrameworkCore;
using Fandom_Project.Models;

namespace Fandom_Project.Data
{
    public class FandomContext : DbContext
    {
        public FandomContext(DbContextOptions<FandomContext> options) : base(options) { }
        
        public DbSet<User> User { get; set; }        
        
        public DbSet<Category> Category { get; set; }
    }
}
