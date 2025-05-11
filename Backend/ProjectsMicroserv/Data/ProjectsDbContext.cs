using DataModels.Projects;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ProjectsDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Project_log> Project_Logs { get; set; }
        public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
