using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Projects.Requests;
using Rabbit.Projects.Responses;

namespace RabbitMQ.Consumers.Projects
{
    public class GetAllProjectsConsumer : IConsumer<GetAllProjectsRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetAllProjectsConsumer> logger;
        public GetAllProjectsConsumer(IServiceProvider provider, ILogger<GetAllProjectsConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetAllProjectsRequest> context)
        {
            GetAllProjectsResponse response = new() { ResponseData = await serviceManager.Projects.GetAllProjectAsync() };
            await context.RespondAsync(response);
        }
    }
}
