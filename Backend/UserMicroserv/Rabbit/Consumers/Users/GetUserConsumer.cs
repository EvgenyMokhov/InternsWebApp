using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Users.Requests;
using Rabbit.Users.Responses;

namespace RabbitMQ.Consumers.Users
{
    public class GetUserConsumer : IConsumer<GetUserRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetUserConsumer> logger;
        public GetUserConsumer(IServiceProvider provider, ILogger<GetUserConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }
        public async Task Consume(ConsumeContext<GetUserRequest> context)
        {
            GetUserResponse response = new() { ResponseData = await serviceManager.Users.GetUserAsync(context.Message.UserId) };
            await context.RespondAsync(response);
        }
    }
}
