using DataModels.Users;

namespace Data.Interfaces
{
    public interface IUser_logs
    {
        public Task LogAsync(User_log log);
    }
}
