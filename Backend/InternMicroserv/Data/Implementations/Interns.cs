using Data.Interfaces;
using DataModels.Interns;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations
{
    public class Interns : IInterns
    {
        private readonly InternsDbContext context;
        public Interns(InternsDbContext context) => this.context = context;
        public async Task CreateInternAsync(Intern intern)
        {
            await context.Interns.AddAsync(intern);
            await context.SaveChangesAsync();
        }

        public async Task<List<Intern>> GetAllInternsAsync()
        {
            return await context.Interns.ToListAsync();
        }

        public async Task<Intern> GetInternAsync(Guid Id)
        {
            return await context.Interns.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<Intern>> GetInternsByDirectionIdAsync(Guid directionId)
        {
            return await context.Interns.Where(intern => intern.InternshipDirectionId == directionId).ToListAsync();
        }

        public async Task<List<Intern>> GetInternsByProjectIdAsync(Guid projectId)
        {
            return await context.Interns.Where(intern => intern.ProjectId == projectId).ToListAsync();
        }

        public async Task UpdateInternAsync(Intern intern)
        {
            context.Entry(intern).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
