using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public class DataManager
    {
        public IInternshipDirections Directions { get; set; }
        public IInternshipDirection_logs DirectionLogs { get; set; }
        public DataManager(IServiceProvider provider)
        {
            Directions = provider.GetRequiredService<IInternshipDirections>();
            DirectionLogs = provider.GetRequiredService<IInternshipDirection_logs>();
        }
    }
}
