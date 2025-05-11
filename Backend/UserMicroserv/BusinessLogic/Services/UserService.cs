using Data;
using DataModels.Users;
using Microsoft.Extensions.DependencyInjection;
using Other.Enums;
using Rabbit.Users;

namespace BusinessLogic.Services
{
    public class UserService
    {
        private readonly IServiceProvider provider;
        public UserService(IServiceProvider provider) => this.provider = provider;

        public async Task CreateUserAsync(UserDto user)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            User dbUser = new()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Sex = user.Sex,
                BirthDate = new DateTime(user.BirthDate, new TimeOnly(0)),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            User_log log = new()
            {
                Id = Guid.NewGuid(),
                LogType = (int)OperationType.Create,
                LogTime = DateTime.UtcNow,
                Name = dbUser.Name,
                Surname = dbUser.Surname,
                Sex = dbUser.Sex,
                UserId = dbUser.Id,
                BirthDate = dbUser.BirthDate,
                Email = dbUser.Email,
                PhoneNumber = dbUser.PhoneNumber
            };
            await dataManager.Users.CreateUserAsync(dbUser);
            await dataManager.UserLogs.LogAsync(log);
        }

        public async Task UpdateUserAsync(UserDto user)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            User dbUser = await dataManager.Users.GetUserAsync(user.Id);
            dbUser.Name = user.Name;
            dbUser.Surname = user.Surname;
            dbUser.Sex = user.Sex;
            dbUser.BirthDate = new(user.BirthDate, new());
            dbUser.Email = user.Email;
            dbUser.PhoneNumber = user.PhoneNumber;
            User_log log = new()
            {
                Id = Guid.NewGuid(),
                LogType = (int)OperationType.Update,
                LogTime = DateTime.UtcNow,
                Name = dbUser.Name,
                Surname = dbUser.Surname,
                Sex = dbUser.Sex,
                UserId = dbUser.Id,
                BirthDate = dbUser.BirthDate,
                Email = dbUser.Email,
                PhoneNumber = dbUser.PhoneNumber
            };
            await dataManager.Users.UpdateUserAsync(dbUser);
            await dataManager.UserLogs.LogAsync(log);
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return (await dataManager.Users.GetAllUsersAsync()).Select(DbUserToDto).ToList();
        }

        public async Task<UserDto> GetUserAsync(Guid id)
        {
            using IServiceScope scope = provider.CreateScope();
            DataManager dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
            return DbUserToDto(await dataManager.Users.GetUserAsync(id));
        }

        private UserDto DbUserToDto(User user)
        {
            return new()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Sex = user.Sex,
                BirthDate = DateOnly.FromDateTime(user.BirthDate),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
