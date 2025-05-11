using BusinessLogic.Services;

namespace BusinessLogic
{
    public class ServiceManager
    {
        public InternsService Interns { get; private set; }
        public ServiceManager(IServiceProvider provider) => Interns = new(provider);
    }
}
