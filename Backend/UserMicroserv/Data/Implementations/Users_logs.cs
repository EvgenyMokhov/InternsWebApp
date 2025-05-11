using Data.Interfaces;
using DataModels.Users;

namespace Data.Implementations
{
    public class Users_logs : IUser_logs
    {
        private readonly UsersDbContext context;
        public Users_logs(UsersDbContext context) => this.context = context;

        public async Task LogAsync(User_log log)
        {
            await context.User_Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
