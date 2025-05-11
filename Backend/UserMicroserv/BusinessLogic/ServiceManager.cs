using BusinessLogic.Services;

namespace BusinessLogic
{
    public class ServiceManager
    {
        public UserService Users { get; private set; }
        public ServiceManager(IServiceProvider provider) => Users = new(provider);
    }
}
