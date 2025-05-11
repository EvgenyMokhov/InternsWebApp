using DataModels.Interns;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class InternsDbContext : DbContext
    {
        public DbSet<Intern> Interns { get; set; }
        public DbSet<Intern_log> Intern_Logs { get; set; }
        public InternsDbContext(DbContextOptions<InternsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
