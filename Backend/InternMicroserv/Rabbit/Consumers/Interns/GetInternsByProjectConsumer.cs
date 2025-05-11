using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace Rabbit.Consumers.Interns
{
    public class GetInternsByProjectConsumer : IConsumer<GetInternsByProjectRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetInternsByProjectConsumer> logger;
        public GetInternsByProjectConsumer(IServiceProvider provider, ILogger<GetInternsByProjectConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetInternsByProjectRequest> context)
        {
            GetInternsByProjectResponse response = new() { ResponseData = await serviceManager.Interns.GetInternsByProjectAsync(context.Message.ProjectId) };
            await context.RespondAsync(response);
        }
    }
}
