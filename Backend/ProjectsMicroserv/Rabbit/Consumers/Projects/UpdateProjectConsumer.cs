using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Projects.Requests;
using Rabbit.Projects.Responses;

namespace RabbitMQ.Consumers.Projects
{
    public class UpdateProjectConsumer : IConsumer<UpdateProjectRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<UpdateProjectConsumer> logger;
        public UpdateProjectConsumer(IServiceProvider provider, ILogger<UpdateProjectConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateProjectRequest> context)
        {
            UpdateProjectResponse response = new();
            await serviceManager.Projects.UpdateProjectAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
