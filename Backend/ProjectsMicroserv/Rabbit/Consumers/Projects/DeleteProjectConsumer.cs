using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Projects.Requests;
using Rabbit.Projects.Responses;

namespace RabbitMQ.Consumers.Projects
{
    public class DeleteProjectConsumer : IConsumer<DeleteProjectRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<DeleteProjectConsumer> logger;
        public DeleteProjectConsumer(IServiceProvider provider, ILogger<DeleteProjectConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<DeleteProjectRequest> context)
        {
            DeleteProjectResponse response = new();
            await serviceManager.Projects.DeleteProjectAsync(context.Message.Id);
            await context.RespondAsync(response);
        }
    }
}
