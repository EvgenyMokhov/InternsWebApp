using DataModels.Internships;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class InternshipDirectionDbContext : DbContext
    {
        public DbSet<InternshipDirection> Directions { get; set; }
        public DbSet<InternshipDirection_log> Direction_Logs { get; set; }
        public InternshipDirectionDbContext(DbContextOptions<InternshipDirectionDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
