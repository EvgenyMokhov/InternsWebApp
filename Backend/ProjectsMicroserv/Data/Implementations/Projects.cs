using Microsoft.EntityFrameworkCore;
using Data;
using Data.Interfaces;
using DataModels.Projects;

namespace Data.Implementations
{
    public class Projects : IProjects
    {
        private readonly ProjectsDbContext context;
        public Projects(ProjectsDbContext context) => this.context = context;
        public async Task CreateProjectAsync(Project project)
        {
            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(Project project)
        {
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await context.Projects.ToListAsync();
        }

        public async Task<Project> GetProjectAsync(Guid id)
        {
            return await context.Projects.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateProjectAsync(Project project)
        {
            context.Entry(project).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
