using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public class DataManager
    {
        public IUsers Users { get; set; }
        public IUser_logs UserLogs { get; set; }
        public DataManager(IServiceProvider provider)
        {
            Users = provider.GetRequiredService<IUsers>();
            UserLogs = provider.GetRequiredService<IUser_logs>();
        }
    }
}
