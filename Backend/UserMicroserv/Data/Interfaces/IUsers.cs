
using DataModels.Users;

namespace Data.Interfaces
{
    public interface IUsers
    {
        public Task<List<User>> GetAllUsersAsync();
        public Task<User> GetUserAsync(Guid id);
        public Task CreateUserAsync(User user);
        public Task UpdateUserAsync(User user);
    }
}
