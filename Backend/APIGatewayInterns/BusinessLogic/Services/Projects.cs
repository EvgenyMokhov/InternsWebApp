using System.Text.RegularExpressions;
using HttpDtos;
using Other.Enums;
using Rabbit.Interns;
using Rabbit.Projects;
using RabbitMQ;

namespace BusinessLogic.Services
{
    public class Projects
    {
        private readonly RabbitManager Rabbit;
        private readonly List<Action> RollbackList = new();

        public Projects(RabbitManager rabbit)
        {
            Rabbit = rabbit;
        }

        public async Task<IEnumerable<ProjectHttpDto>> GetProjectsAsync(Guid transactionId)
        {
            Interns internsService = new(Rabbit);
            Dictionary<Guid, ProjectHttpDto> projects = (await Rabbit.Projects.GetAllProjectsAsync(transactionId))
                .Select(CreateEmptyProjectHttpDto)
                .ToDictionary(proj => proj.Id);
            List<InternLowDetailHttpDto> interns = await internsService.GetLowDetailInternsAsync(transactionId);
            foreach (InternLowDetailHttpDto intern in interns)
                if (projects.ContainsKey(intern.ProjectId))
                    projects[intern.ProjectId].Interns.Add(intern);
            return projects.Values.Where(dir => dir.IsActive);
        }

        public async Task<GetPagedProjectsHttpResponseDto> GetPagedProjectsAsync(GetPagedProjectsHttpRequestDto requestData, Guid transactionId)
        {
            List<ProjectHttpDto> projects = (await GetProjectsAsync(transactionId))
                .Where(proj => Regex.IsMatch(proj.Name, requestData.ProjectsFilter))
                .ToList();
            if (requestData.SortingParameter == SortingParameter.InternCount)
                projects = projects.OrderByDescending(proj => proj.Interns.Count).ToList();
            else if (requestData.SortingParameter == SortingParameter.DirectionName)
                projects = projects.OrderByDescending(proj => proj.Name).ToList();
            GetPagedProjectsHttpResponseDto response = new() { Projects = new(), TotalCount = projects.Count };
            for (int i = ((requestData.PageNumber - 1) * requestData.ProjectCountOnPage); (i < requestData.PageNumber * requestData.ProjectCountOnPage) && i < projects.Count; i++)
                response.Projects.Add(projects[i]);
            return response;
        }

        public async Task<List<ProjectLowDetailHttpDto>> GetLowDetailProjectsAsync(Guid transactionId)
        {
            List<ProjectDto> projects = await Rabbit.Projects.GetAllProjectsAsync(transactionId);
            return projects.Select(CreateLowDetailProjectHttpDto).Where(project => project.IsActive).ToList();
        }

        public async Task<ProjectHttpDto> GetProjectAsync(Guid projectId, Guid transactionId)
        {
            ProjectDto project = await Rabbit.Projects.GetProjectAsync(projectId, transactionId);
            return await CreateProjectHttpDto(project, transactionId);
        }

        public async Task<Guid> CreateProjectAsync(ProjectLowDetailHttpDto project, Guid transactionId)
        {
            project.Id = Guid.NewGuid();
            await Rabbit.Projects.CreateProjectAsync(CreateProjectDto(project), transactionId);
            return project.Id;
        }

        public async Task UpdateProjectAsync(ProjectHttpDto project, Guid transactionId)
        {
            HashSet<Guid> oldInternsIdsSet = (await Rabbit.Interns.GetInternsByProject(project.Id, transactionId))
                .Select(intern => intern.Id)
                .ToHashSet();
            try
            {
                foreach (InternLowDetailHttpDto newIntern in project.Interns)
                    if (!oldInternsIdsSet.Contains(newIntern.Id))
                    {
                        InternDto internDto = await Rabbit.Interns.GetInternAsync(newIntern.Id, transactionId);
                        RollbackList.Add(async () => await Rabbit.Interns.UpdateInternAsync(new()
                        {
                            Id = internDto.Id,
                            InternshipDirectionId = internDto.InternshipDirectionId,
                            ProjectId = internDto.ProjectId,
                            UserId = internDto.UserId
                        }, transactionId));
                        internDto.ProjectId = project.Id;
                        await Rabbit.Interns.UpdateInternAsync(internDto, transactionId);
                    }
                await Rabbit.Projects.UpdateProjectAsync(CreateProjectDto(project), transactionId);
            }
            catch (Exception ex)
            {
                foreach (Action rollbackFunction in RollbackList)
                    rollbackFunction();
                throw new(ex.Message);
            }
        }

        private async Task<ProjectHttpDto> CreateProjectHttpDto(ProjectDto project, Guid transactionId)
        {
            Interns internsService = new(Rabbit);
            return new()
            {
                Id = project.Id,
                Description = project.Description,
                Interns = (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByProject(project.Id, transactionId))
                    .Select(async intern => await internsService.CreateInternLowDetailHttpDto(intern, transactionId))))
                    .ToList(),
                Name = project.Name,
                IsActive = project.IsActive,
            };
        }

        private ProjectHttpDto CreateEmptyProjectHttpDto(ProjectDto project)
        {
            return new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Interns = new(),
                IsActive = project.IsActive
            };
        }

        public ProjectLowDetailHttpDto CreateLowDetailProjectHttpDto(ProjectDto project)
        {
            return new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsActive = project.IsActive
            };
        }

        public ProjectDto CreateProjectDto(ProjectLowDetailHttpDto project)
        {
            return new()
            {
                Id = project.Id,
                Description = project.Description,
                Name = project.Name,
                IsActive = project.IsActive
            };
        }

        public ProjectDto CreateProjectDto(ProjectHttpDto project)
        {
            return new()
            {
                Id = project.Id,
                Description = project.Description,
                Name = project.Name,
                IsActive = project.IsActive
            };
        }
    }
}
