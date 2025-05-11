using DataModels.Projects;

namespace Data.Interfaces
{
    public interface IProjects
    {
        public Task<List<Project>> GetAllProjectsAsync();
        public Task<Project> GetProjectAsync(Guid id);
        public Task CreateProjectAsync(Project project);
        public Task UpdateProjectAsync(Project project);
        public Task DeleteProjectAsync(Project project);
    }
}
