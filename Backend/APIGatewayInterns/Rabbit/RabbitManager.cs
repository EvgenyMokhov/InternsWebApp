using RabbitMQ.Services;

namespace RabbitMQ
{
    public class RabbitManager
    {
        public Interns Interns { get; set; }
        public Projects Projects { get; set; }
        public Directions Directions { get; set; }
        public Users Users { get; set; }
        public RabbitManager(Interns interns, Projects projects, Directions directions, Users users)
        {
            Interns = interns;
            Projects = projects;
            Directions = directions;
            Users = users;
        }
    }
}
