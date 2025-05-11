using Data;
using DataModels.Internships;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Other.Enums;
using Rabbit.Direction;

namespace BusinessLogic.Services
{
    public class DirectionService
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<DirectionService> logger;
        public DirectionService(IServiceProvider provider)
        {
            this.provider = provider;
            logger = provider.GetRequiredService<ILogger<DirectionService>>();
        }

        public async Task CreateDirectionAsync(DirectionDto direction)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            InternshipDirection dbDirection = new()
            {
                Id = direction.Id,
                Name = direction.Name,
                Description = "This is description!", 
                IsActive = true
            };
            InternshipDirection_log log = new()
            {
                Id = Guid.NewGuid(),
                LogType = (int)OperationType.Create,
                LogTime = DateTime.UtcNow,
                InternshipDirectionId = direction.Id,
                Name = direction.Name,
                Description = direction.Description,
                IsActive = direction.IsActive
            };
            await dataManager.Directions.CreateDirectionAsync(dbDirection);
            await dataManager.DirectionLogs.LogAsync(log);
        }

        public async Task UpdateDirectionAsync(DirectionDto direction)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            InternshipDirection dbDirection = await dataManager.Directions.GetDirectionAsync(direction.Id);
            dbDirection.Id = direction.Id;
            dbDirection.Name = direction.Name;
            dbDirection.Description = direction.Description;
            dbDirection.IsActive = direction.IsActive;
            await dataManager.Directions.UpdateDirectionAsync(dbDirection);
            await dataManager.DirectionLogs.LogAsync(new()
            {
                Id = Guid.NewGuid(),
                InternshipDirectionId = dbDirection.Id,
                LogType = (int)OperationType.Update,
                LogTime = DateTime.UtcNow,
                Name = dbDirection.Name,
                Description = dbDirection.Description,
                IsActive=dbDirection.IsActive
            });
        }

        public async Task<List<DirectionDto>> GetAllDirectionAsync()
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return (await dataManager.Directions.GetAllDirectionAsync()).Select(DbDirectionToDto).ToList();
        }

        public async Task<DirectionDto> GetDirectionAsync(Guid id)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            InternshipDirection direction = await dataManager.Directions.GetDirectionAsync(id);
            if (direction == null)
                throw new ArgumentException("Direction not found");
            return DbDirectionToDto(direction);
        }

        public async Task DeleteDirectionAsync(Guid id)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            InternshipDirection dbDirection = await dataManager.Directions.GetDirectionAsync(id);
            InternshipDirection_log log = new()
            {
                Id = Guid.NewGuid(),
                LogTime = DateTime.UtcNow,
                LogType = (int)OperationType.Delete,
                InternshipDirectionId = id,
                Description = dbDirection.Description,
                Name = dbDirection.Name
            };
            await dataManager.Directions.DeleteDirectionAsync(dbDirection);
            await dataManager.DirectionLogs.LogAsync(log);
        }

        private DirectionDto DbDirectionToDto(InternshipDirection direction)
        {
            return new()
            {
                Id = direction.Id,
                Name = direction.Name,
                Description = direction.Description,
                IsActive = direction.IsActive,
            };
        }
    }
}
