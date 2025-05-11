using Data.Interfaces;
using DataModels.Projects;

namespace Data.Implementations
{
    public class Project_logs : IProject_logs
    {
        private readonly ProjectsDbContext context;
        public Project_logs(ProjectsDbContext context) => this.context = context;

        public async Task LogAsync(Project_log log)
        {
            await context.Project_Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
