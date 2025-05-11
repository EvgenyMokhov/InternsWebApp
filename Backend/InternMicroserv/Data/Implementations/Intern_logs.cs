using Data.Interfaces;
using DataModels.Interns;

namespace Data.Implementations
{
    public class Intern_logs : IIntern_logs
    {
        private readonly InternsDbContext context;
        public Intern_logs(InternsDbContext context) => this.context = context;
        public async Task LogAsync(Intern_log log)
        {
            await context.Intern_Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
