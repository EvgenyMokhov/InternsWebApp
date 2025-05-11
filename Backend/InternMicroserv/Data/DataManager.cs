using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public class DataManager
    {
        public IInterns Interns { get; set; }
        public IIntern_logs InternLogs { get; set; }
        public DataManager(IServiceProvider provider)
        {
            Interns = provider.GetRequiredService<IInterns>();
            InternLogs = provider.GetRequiredService<IIntern_logs>();
        }
    }
}
