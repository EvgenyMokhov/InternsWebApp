using Data;
using DataModels.Projects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Other.Enums;
using Rabbit.Projects;

namespace BusinessLogic.Services
{
    public class ProjectService
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<ProjectService> logger;
        public ProjectService(IServiceProvider provider)
        {
            this.provider = provider;
            logger = provider.GetRequiredService<ILogger<ProjectService>>();
        }

        public async Task CreateProjectAsync(ProjectDto project)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            Project dbProject = new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = "This is description!",
                IsActive = true
            };
            Project_log log = new()
            {
                Id = Guid.NewGuid(),
                LogType = (int)OperationType.Create,
                LogTime = DateTime.UtcNow,
                ProjectId = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsActive = project.IsActive
            };
            await dataManager.Projects.CreateProjectAsync(dbProject);
            await dataManager.ProjectLogs.LogAsync(log);
        }

        public async Task UpdateProjectAsync(ProjectDto project)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            Project dbProject = await dataManager.Projects.GetProjectAsync(project.Id);
            dbProject.Id = project.Id;
            dbProject.Name = project.Name;
            dbProject.Description = project.Description;
            dbProject.IsActive = project.IsActive;
            await dataManager.Projects.UpdateProjectAsync(dbProject);
            await dataManager.ProjectLogs.LogAsync(new()
            {
                Id = Guid.NewGuid(),
                ProjectId = dbProject.Id,
                LogType = (int)OperationType.Update,
                LogTime = DateTime.UtcNow,
                Name = dbProject.Name,
                Description = dbProject.Description,
                IsActive = dbProject.IsActive
            });
        }

        public async Task<List<ProjectDto>> GetAllProjectAsync()
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return (await dataManager.Projects.GetAllProjectsAsync()).Select(DbProjectToDto).ToList();
        }

        public async Task<ProjectDto> GetProjectAsync(Guid id)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            Project project = await dataManager.Projects.GetProjectAsync(id);
            if (project == null)
                throw new ArgumentException("Project not found");
            return DbProjectToDto(project);
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            Project project = await dataManager.Projects.GetProjectAsync(id);
            Project_log log = new()
            {
                Id = Guid.NewGuid(),
                LogTime = DateTime.UtcNow,
                LogType = (int)OperationType.Delete,
                IsActive = project.IsActive,
                Name = project.Name,
                Description = project.Description,
                ProjectId = project.Id
            };
            await dataManager.Projects.DeleteProjectAsync(project);
            await dataManager.ProjectLogs.LogAsync(log);
        }

        private ProjectDto DbProjectToDto(Project project)
        {
            return new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsActive = project.IsActive,
            };
        }
    }
}
