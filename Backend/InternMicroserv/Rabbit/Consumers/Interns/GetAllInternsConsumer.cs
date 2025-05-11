using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace RabbitMQ.Consumers.Interns
{
    public class GetAllInternsConsumer : IConsumer<GetAllInternsRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetAllInternsConsumer> logger;
        public GetAllInternsConsumer(IServiceProvider provider, ILogger<GetAllInternsConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetAllInternsRequest> context)
        {
            GetAllInternsResponse response = new() { ResponseData = await serviceManager.Interns.GetAllInternsAsync() };
            await context.RespondAsync(response);
        }
    }
}
