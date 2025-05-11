using HttpDtos;
using Rabbit.Interns;
using Rabbit.Users;
using RabbitMQ;
using RabbitMQ.Services;

namespace BusinessLogic.Services
{
    public class Interns
    {
        private readonly RabbitManager Rabbit;
        private List<Action> RollbackList = new();
        public Interns(RabbitManager rabbit) => Rabbit = rabbit;

        public async Task<List<InternHttpDto>> GetInternsAsync(Guid transactionId)
        {
            List<InternDto> interns = await Rabbit.Interns.GetAllInternsAsync(transactionId);
            return (await Task.WhenAll(interns.Select(async intern => await GetInternHttpDto(intern, transactionId)))).ToList();
        }

        public async Task<List<InternLowDetailHttpDto>> GetLowDetailInternsAsync(Guid transactionId)
        {
            List<InternDto> interns = await Rabbit.Interns.GetAllInternsAsync(transactionId);
            return (await Task.WhenAll(interns.Select(async intern => await GetLowDetailInternHttpDto(intern, transactionId)))).ToList();
        }

        public async Task<InternHttpDto> GetInternAsync(Guid id, Guid transactionId)
        {
            InternDto intern = await Rabbit.Interns.GetInternAsync(id, transactionId);
            return await GetInternHttpDto(intern, transactionId);
        }

        public async Task CreateInternAsync(InternLowDetailHttpDto intern, Guid transactionId)
        {
            intern.User.Id = Guid.NewGuid();
            await Rabbit.Users.CreateUserAsync(new() 
            {
                Id = intern.User.Id, 
                BirthDate = intern.User.BirthDate,
                Email = intern.User.Email, 
                Name = intern.User.Name, 
                PhoneNumber = intern.User.PhoneNumber,
                Sex = intern.User.Sex,
                Surname = intern.User.Surname
            }, transactionId);
            await Rabbit.Directions.GetDirectionAsync(intern.DirectionId, transactionId);
            await Rabbit.Projects.GetProjectAsync(intern.ProjectId, transactionId);
            await Rabbit.Interns.CreateInternAsync(GetInternDto(intern), transactionId);
        }

        public async Task UpdateInternAsync(InternLowDetailHttpDto intern, Guid transactionId)
        {
            UserDto user = await Rabbit.Users.GetUserAsync((await Rabbit.Interns.GetInternAsync(intern.Id, transactionId)).UserId, transactionId);
            await Rabbit.Directions.GetDirectionAsync(intern.DirectionId, transactionId);
            await Rabbit.Projects.GetProjectAsync(intern.ProjectId, transactionId);
            UserDto rollbackData = new()
            {
                Id = user.Id,
                BirthDate = user.BirthDate,
                Sex = user.Sex,
                Surname = user.Surname,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
            };
            user.Name = intern.User.Name;
            user.Email = intern.User.Email;
            user.Surname = intern.User.Surname;
            user.Sex = intern.User.Sex;
            user.PhoneNumber = intern.User.PhoneNumber;
            user.BirthDate = intern.User.BirthDate;
            try
            {
                await Rabbit.Users.UpdateUserAsync(user, transactionId);
                RollbackList.Add(async () => await Rabbit.Users.UpdateUserAsync(rollbackData, transactionId));
                await Rabbit.Interns.UpdateInternAsync(GetInternDto(intern), transactionId);
            }
            catch (Exception ex) 
            {
                foreach (Action rollbackFunction in RollbackList)
                    rollbackFunction();
                throw new(ex.Message);
            }
        }

        public async Task<List<InternHttpDto>> GetFilteredInternsAsync(Guid projectId, Guid directionId, Guid transactionId)
        {
            if (projectId == Guid.Empty && directionId == Guid.Empty)
                return (await Task.WhenAll(
                    (await Rabbit.Interns.GetAllInternsAsync(transactionId))
                    .Select(async intern => await GetInternHttpDto(intern, transactionId))))
                    .ToList();
            if (projectId == Guid.Empty)
            {
                return (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByDirection(directionId, transactionId))
                    .Select(async i => await GetInternHttpDto(i, transactionId))))
                    .ToList();
            }
            if (directionId == Guid.Empty)
            {
                return (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByProject(projectId, transactionId))
                    .Select(async i => await GetInternHttpDto(i, transactionId))))
                    .ToList();
            }
            List<InternHttpDto> projFiltered = new();
            List<InternHttpDto> dirFiltered = new();
            dirFiltered = (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByDirection(directionId, transactionId))
                    .Select(async i => await GetInternHttpDto(i, transactionId))))
                    .ToList();
            projFiltered = (await Task.WhenAll(
                    (await Rabbit.Interns.GetInternsByProject(projectId, transactionId))
                    .Select(async i => await GetInternHttpDto(i, transactionId))))
                    .ToList();
            return dirFiltered.Intersect(projFiltered).ToList();
        }

        public async Task<InternLowDetailHttpDto> CreateInternLowDetailHttpDto(InternDto intern, Guid transactionId)
        {
            UserDto user = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            InternLowDetailHttpDto result = new();
            result.Id = intern.Id;
            result.User = new() 
            { 
                Id = user.Id, 
                BirthDate = user.BirthDate,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber, 
                Sex = user.Sex,
                Surname = user.Surname 
            };
            result.DirectionId = intern.InternshipDirectionId;
            result.ProjectId = intern.ProjectId;
            return result;
        }

        public async Task<InternHttpDto> GetInternHttpDto(InternDto intern, Guid transactionId)
        {
            Directions directionService = new(Rabbit);
            Projects projectService = new(Rabbit);
            UserDto user = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            InternHttpDto result = new InternHttpDto();
            result.Id = intern.Id;
            result.User = new()
            {
                Id = user.Id,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Sex = user.Sex,
                Surname = user.Surname
            };
            result.Direction = directionService.CreateDirectionLowDetailHttpDto(await Rabbit.Directions.GetDirectionAsync(intern.InternshipDirectionId, transactionId));
            result.Project = projectService.CreateLowDetailProjectHttpDto(await Rabbit.Projects.GetProjectAsync(intern.ProjectId, transactionId));
            return result;
        }

        public async Task<InternLowDetailHttpDto> GetLowDetailInternHttpDto(InternDto intern, Guid transactionId)
        {
            UserDto user = await Rabbit.Users.GetUserAsync(intern.UserId, transactionId);
            return new()
            {
                Id = intern.Id,
                DirectionId = intern.InternshipDirectionId,
                ProjectId = intern.ProjectId,
                User = new()
                {
                    Id = user.Id,
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Sex = user.Sex,
                    Surname = user.Surname
                }
            };
        }

        public InternDto GetInternDto(InternHttpDto intern)
        {
            return new()
            {
                Id = intern.Id,
                ProjectId = intern.Project.Id,
                UserId = intern.User.Id,
                InternshipDirectionId = intern.Direction.Id
            };
        }

        public InternDto GetInternDto(InternLowDetailHttpDto intern)
        {
            return new()
            {
                Id = intern.Id,
                ProjectId = intern.ProjectId,
                InternshipDirectionId = intern.DirectionId,
                UserId = intern.User.Id
            };
        }
    }
}
