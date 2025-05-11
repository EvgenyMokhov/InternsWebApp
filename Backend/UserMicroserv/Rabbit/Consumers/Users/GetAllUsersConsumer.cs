using MassTransit;
using BusinessLogic;
using Microsoft.Extensions.Logging;
using Rabbit.Users.Requests;
using Rabbit.Users.Responses;

namespace RabbitMQ.Consumers.Users
{
    public class GetAllUsersConsumer : IConsumer<GetAllUsersRequest>
    {
        private readonly ServiceManager serviceManager;
        private readonly ILogger<GetAllUsersConsumer> logger;
        public GetAllUsersConsumer(IServiceProvider provider, ILogger<GetAllUsersConsumer> logger)
        {
            serviceManager = new(provider);
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<GetAllUsersRequest> context)
        {
            GetAllUsersResponse response = new() { ResponseData = await serviceManager.Users.GetAllUsersAsync() };
            await context.RespondAsync(response);
        }
    }
}
