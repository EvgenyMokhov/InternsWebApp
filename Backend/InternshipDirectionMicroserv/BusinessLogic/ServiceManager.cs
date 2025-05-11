using BusinessLogic.Services;

namespace BusinessLogic
{
    public class ServiceManager
    {
        public DirectionService Directions { get; private set; }
        public ServiceManager(IServiceProvider provider) => Directions = new(provider);
    }
}
