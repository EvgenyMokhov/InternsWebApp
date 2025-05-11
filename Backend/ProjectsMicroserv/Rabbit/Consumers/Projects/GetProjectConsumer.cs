using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Projects.Requests;
using Rabbit.Projects.Responses;

namespace RabbitMQ.Consumers.Projects
{
    public class GetProjectConsumer : IConsumer<GetProjectRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetProjectConsumer> logger;
        public GetProjectConsumer(IServiceProvider provider, ILogger<GetProjectConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetProjectRequest> context)
        {
            GetProjectResponse response = new() { ResponseData = await serviceManager.Projects.GetProjectAsync(context.Message.Id) };
            await context.RespondAsync(response);
        }
    }
}
