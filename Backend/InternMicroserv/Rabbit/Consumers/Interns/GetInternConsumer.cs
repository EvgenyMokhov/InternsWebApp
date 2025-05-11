using BusinessLogic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Rabbit.Interns.Requests;
using Rabbit.Interns.Responses;

namespace RabbitMQ.Consumers.Interns
{
    public class GetInternConsumer : IConsumer<GetInternRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetInternConsumer> logger;
        public GetInternConsumer(IServiceProvider provider, ILogger<GetInternConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetInternRequest> context)
        {
            GetInternResponse response = new() { ResponseData = await serviceManager.Interns.GetInternAsync(context.Message.Id) };
            await context.RespondAsync(response);
        }
    }
}
