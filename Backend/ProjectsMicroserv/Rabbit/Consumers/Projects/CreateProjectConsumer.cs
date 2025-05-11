using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Projects.Requests;
using Rabbit.Projects.Responses;

namespace RabbitMQ.Consumers.Projects
{
    public class CreateProjectConsumer : IConsumer<CreateProjectRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<CreateProjectConsumer> logger;
        public CreateProjectConsumer(IServiceProvider provider, ILogger<CreateProjectConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<CreateProjectRequest> context)
        {
            CreateProjectResponse response = new();
            await serviceManager.Projects.CreateProjectAsync(context.Message.RequestData);
            await context.RespondAsync(response);
        }
    }
}
