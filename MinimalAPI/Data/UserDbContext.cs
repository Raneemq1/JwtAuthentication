using Microsoft.EntityFrameworkCore;
using MinimalAPI.Models;

namespace MinimalAPI.Data
{
    public class UserDbContext:DbContext
    {
        public DbSet<User> Users {get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite("Data Source=users.db"); 
        }
    }
}
