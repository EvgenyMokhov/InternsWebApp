using BusinessLogic.Services;

namespace BusinessLogic
{
    public class ServiceManager
    {
        public ProjectService Projects { get; private set; }
        public ServiceManager(IServiceProvider provider) => Projects = new(provider);
    }
}
