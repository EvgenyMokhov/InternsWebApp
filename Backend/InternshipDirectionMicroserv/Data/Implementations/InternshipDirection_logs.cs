using Data;
using Data.Interfaces;
using DataModels.Internships;

namespace Data.Implementations
{
    public class InternshipDirection_logs : IInternshipDirection_logs
    {
        private readonly InternshipDirectionDbContext context;
        public InternshipDirection_logs(InternshipDirectionDbContext context) => this.context = context;

        public async Task LogAsync(InternshipDirection_log log)
        {
            await context.Direction_Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }
    }
}
