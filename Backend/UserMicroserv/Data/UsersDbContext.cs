using DataModels.Users;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<User_log> User_Logs { get; set; }
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }
    }
}
