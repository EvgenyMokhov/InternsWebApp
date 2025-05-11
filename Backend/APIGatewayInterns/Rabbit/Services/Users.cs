using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rabbit.Users;
using Rabbit.Users.Requests;
using Rabbit.Users.Responses;

namespace RabbitMQ.Services
{
    public class Users
    {
        private readonly IServiceProvider Provider;
        private readonly ILogger<Users> Logger;
        public Users(IServiceProvider provider, ILogger<Users> logger)
        {
            Provider = provider;
            Logger = logger;
        }

        public async Task<UserDto> GetUserAsync(Guid id, Guid transactionId)
        {
            IRequestClient<GetUserRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetUserRequest>>();
            return (await requestClient.GetResponse<GetUserResponse>(new() { UserId = id, TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task CreateUserAsync(UserDto user, Guid transactionId)
        {
            IRequestClient<CreateUserRequest> requestClient = Provider.GetRequiredService<IRequestClient<CreateUserRequest>>();
            await requestClient.GetResponse<CreateUserResponse>(new() { RequestData = user, TransactionId = transactionId });
        }

        public async Task<List<UserDto>> GetAllUsersAsync(Guid transactionId)
        {
            IRequestClient<GetAllUsersRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetAllUsersRequest>>();
            return (await requestClient.GetResponse<GetAllUsersResponse>(new() { TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task UpdateUserAsync(UserDto user, Guid transactionId)
        {
            IRequestClient<UpdateUserRequest> requestClient = Provider.GetRequiredService<IRequestClient<UpdateUserRequest>>();
            await requestClient.GetResponse<UpdateUserResponse>(new() { RequestData = user, TransactionId = transactionId });
        }
    }
}
