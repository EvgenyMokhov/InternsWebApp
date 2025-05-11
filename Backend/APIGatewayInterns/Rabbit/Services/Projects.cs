using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rabbit.Projects;
using Rabbit.Projects.Requests;
using Rabbit.Projects.Responses;

namespace RabbitMQ.Services
{
    public class Projects
    {
        private readonly IServiceProvider Provider;
        private readonly ILogger<Projects> Logger;
        public Projects(IServiceProvider provider, ILogger<Projects> logger)
        {
            Provider = provider;
            Logger = logger;
        }

        public async Task CreateProjectAsync(ProjectDto project, Guid transactionId)
        {
            IRequestClient<CreateProjectRequest> requestClient = Provider.GetRequiredService<IRequestClient<CreateProjectRequest>>();
            await requestClient.GetResponse<CreateProjectResponse>(new() { RequestData = project, TransactionId = transactionId });
        }

        public async Task UpdateProjectAsync(ProjectDto project, Guid transactionId)
        {
            IRequestClient<UpdateProjectRequest> requestClient = Provider.GetRequiredService<IRequestClient<UpdateProjectRequest>>();
            await requestClient.GetResponse<UpdateProjectResponse>(new() { RequestData = project, TransactionId = transactionId });
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync(Guid transactionId)
        {
            IRequestClient<GetAllProjectsRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetAllProjectsRequest>>();
            return (await requestClient.GetResponse<GetAllProjectsResponse>(new() { TransactionId = transactionId })).Message.ResponseData;
        }

        public async Task<ProjectDto> GetProjectAsync(Guid id, Guid transactionId)
        {
            IRequestClient<GetProjectRequest> requestClient = Provider.GetRequiredService<IRequestClient<GetProjectRequest>>();
            try
            {
                return (await requestClient.GetResponse<GetProjectResponse>(new() { Id = id, TransactionId = transactionId })).Message.ResponseData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.Split(':')[1]);
            }
        }
    }
}
