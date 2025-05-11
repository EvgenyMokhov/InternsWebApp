using DataModels.Interns;

namespace Data.Interfaces
{
    public interface IInterns
    {
        public Task<List<Intern>> GetAllInternsAsync();
        public Task<List<Intern>> GetInternsByDirectionIdAsync(Guid directionId);
        public Task<List<Intern>> GetInternsByProjectIdAsync(Guid projectId);
        public Task<Intern> GetInternAsync(Guid Id);
        public Task CreateInternAsync(Intern intern);
        public Task UpdateInternAsync(Intern intern);
    }
}
