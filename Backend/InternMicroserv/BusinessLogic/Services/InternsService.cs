using Data;
using DataModels.Interns;
using Microsoft.Extensions.DependencyInjection;
using Other.Enums;
using Rabbit.Interns;

namespace BusinessLogic.Services
{
    public class InternsService
    {
        private readonly IServiceProvider provider;
        public InternsService(IServiceProvider provider) => this.provider = provider;
        public async Task CreateInternAsync(InternDto intern)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            Intern dbIntern = new()
            {
                Id = Guid.NewGuid(),
                InternshipDirectionId = intern.InternshipDirectionId,
                ProjectId = intern.ProjectId,
                UserId = intern.UserId
            };
            Intern_log log = new()
            {
                Id = Guid.NewGuid(),
                LogType = (int)OperationType.Create,
                LogTime = DateTime.UtcNow,
                InternId = dbIntern.Id,
                InternshipDirectionId = dbIntern.InternshipDirectionId,
                ProjectId = dbIntern.ProjectId,
                UserId = dbIntern.UserId
            };
            await dataManager.Interns.CreateInternAsync(dbIntern);
            await dataManager.InternLogs.LogAsync(log);
        }

        public async Task UpdateInternAsync(InternDto intern)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            Intern dbIntern = await dataManager.Interns.GetInternAsync(intern.Id);
            dbIntern.Id = intern.Id;
            dbIntern.ProjectId = intern.ProjectId;
            dbIntern.UserId = intern.UserId;
            dbIntern.InternshipDirectionId = intern.InternshipDirectionId;
            await dataManager.Interns.UpdateInternAsync(dbIntern);
            await dataManager.InternLogs.LogAsync(new()
            {
                Id = Guid.NewGuid(),
                InternId = dbIntern.Id,
                InternshipDirectionId = dbIntern.InternshipDirectionId,
                LogType = (int)OperationType.Update,
                LogTime = DateTime.UtcNow,
                ProjectId = dbIntern.ProjectId,
                UserId = dbIntern.UserId
            });
        }

        public async Task<InternDto> GetInternAsync(Guid id)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return DbInternToDto(await dataManager.Interns.GetInternAsync(id));
        }

        public async Task<List<InternDto>> GetAllInternsAsync()
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return (await dataManager.Interns.GetAllInternsAsync()).Select(DbInternToDto).ToList();
        }

        public async Task<List<InternDto>> GetInternsByDirectionAsync(Guid directionId)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return (await dataManager.Interns.GetInternsByDirectionIdAsync(directionId)).Select(DbInternToDto).ToList();
        }

        public async Task<List<InternDto>> GetInternsByProjectAsync(Guid projectId)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return (await dataManager.Interns.GetInternsByProjectIdAsync(projectId)).Select(DbInternToDto).ToList();
        }

        private InternDto DbInternToDto(Intern intern)
        {
            return new()
            {
                Id = intern.Id,
                InternshipDirectionId = intern.InternshipDirectionId,
                ProjectId = intern.ProjectId,
                UserId = intern.UserId
            };
        }
    }
}
