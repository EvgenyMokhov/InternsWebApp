using DataModels.Internships;

namespace Data.Interfaces
{
    public interface IInternshipDirections
    {
        public Task<List<InternshipDirection>> GetAllDirectionAsync();
        public Task<InternshipDirection> GetDirectionAsync(Guid id);
        public Task CreateDirectionAsync(InternshipDirection direction);
        public Task UpdateDirectionAsync(InternshipDirection direction);
        public Task DeleteDirectionAsync(InternshipDirection direction);
    }
}
