using Microsoft.EntityFrameworkCore;
using Data.Interfaces;
using DataModels.Users;

namespace Data.Implementations
{
    public class Users : IUsers
    {
        private readonly UsersDbContext context;
        public Users(UsersDbContext context) => this.context = context;
        public async Task CreateUserAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateUserAsync(User user)
        {
            context.Entry(user).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
