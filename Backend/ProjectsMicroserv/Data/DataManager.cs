using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public class DataManager
    {
        public IProjects Projects { get; set; }
        public IProject_logs ProjectLogs { get; set; }
        public DataManager(IServiceProvider provider)
        {
            Projects = provider.GetRequiredService<IProjects>();
            ProjectLogs = provider.GetRequiredService<IProject_logs>();
        }
    }
}
