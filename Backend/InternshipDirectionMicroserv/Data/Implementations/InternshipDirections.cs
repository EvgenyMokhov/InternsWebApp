using Data;
using Data.Interfaces;
using DataModels.Internships;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations
{
    public class InternshipDirections : IInternshipDirections
    {
        private readonly InternshipDirectionDbContext context;
        public InternshipDirections(InternshipDirectionDbContext context) => this.context = context;
        public async Task CreateDirectionAsync(InternshipDirection direction)
        {
            await context.AddAsync(direction);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDirectionAsync(InternshipDirection direction)
        {
            context.Directions.Remove(direction);
            await context.SaveChangesAsync();
        }

        public async Task<List<InternshipDirection>> GetAllDirectionAsync()
        {
            return await context.Directions.ToListAsync();
        }

        public async Task<InternshipDirection> GetDirectionAsync(Guid id)
        {
            return await context.Directions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateDirectionAsync(InternshipDirection direction)
        {
            context.Entry(direction).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
