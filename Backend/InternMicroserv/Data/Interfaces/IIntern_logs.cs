using DataModels.Interns;

namespace Data.Interfaces
{
    public interface IIntern_logs
    {
        public Task LogAsync(Intern_log log);
    }
}
