using BusinessLogic.Services;
using RabbitMQ;

namespace BusinessLogic
{
    public class ServiceManager
    {
        public Interns Interns { get; set; }
        public Directions Directions { get; set; }
        public Projects Projects { get; set; }
        public ServiceManager(RabbitManager rabbit)
        {
            Interns = new(rabbit);
            Directions = new(rabbit);
            Projects = new(rabbit);
        }
    }
}
