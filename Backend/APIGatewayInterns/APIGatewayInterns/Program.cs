using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using MassTransit;
using Rabbit.Direction.Requests;
using Rabbit.Interns.Requests;
using Rabbit.Projects.Requests;
using Rabbit.Users.Requests;
using RabbitMQ;
using RabbitMQ.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, cfg) =>
{
    cfg
    .Enrich.WithProperty("Application", "ApiGateway")
    .WriteTo.Console();
    //.WriteTo.Elasticsearch(new[] { new Uri(builder.Configuration["Elastic:Url"]) }, opts =>
    //{
    //    opts.TextFormatting = new EcsTextFormatterConfiguration();
    //    opts.DataStream = new DataStreamName("logs", "dotnet", "default");
    //    opts.BootstrapMethod = BootstrapMethod.Failure;
    //    opts.MinimumLevel = LogEventLevel.Information;
    //}, transport =>
    //{
    //    transport.Authentication(new ApiKey(builder.Configuration["Elastic:ApiKey"]));
    //});
}, preserveStaticLogger: false);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RabbitManager>();
builder.Services.AddScoped<Interns>();
builder.Services.AddScoped<Users>();
builder.Services.AddScoped<Directions>();
builder.Services.AddScoped<Projects>();
builder.Services.AddMassTransit(x => 
{
    x.AddRequestClient<GetInternRequest>(new Uri("exchange:get-intern-requests"));
    x.AddRequestClient<GetAllInternsRequest>(new Uri("exchange:get-all-interns-requests"));
    x.AddRequestClient<GetInternsByDirectionRequest>(new Uri("exchange:get-interns-by-direction-requests"));
    x.AddRequestClient<GetInternsByProjectRequest>(new Uri("exchange:get-interns-by-project-requests"));
    x.AddRequestClient<CreateInternRequest>(new Uri("exchange:create-intern-requests"));
    x.AddRequestClient<UpdateInternRequest>(new Uri("exchange:update-intern-requests"));

    x.AddRequestClient<GetProjectRequest>(new Uri("exchange:get-project-requests"));
    x.AddRequestClient<GetAllProjectsRequest>(new Uri("exchange:get-all-projects-requests"));
    x.AddRequestClient<CreateProjectRequest>(new Uri("exchange:create-project-requests"));
    x.AddRequestClient<UpdateProjectRequest>(new Uri("exchange:update-project-requests"));
    x.AddRequestClient<DeleteProjectRequest>(new Uri("exchange:delete-project-requests"));

    x.AddRequestClient<GetDirectionRequest>(new Uri("exchange:get-direction-requests"));
    x.AddRequestClient<GetAllDirectionsRequest>(new Uri("exchange:get-all-directions-requests"));
    x.AddRequestClient<CreateDirectionRequest>(new Uri("exchange:create-direction-requests"));
    x.AddRequestClient<UpdateDirectionRequest>(new Uri("exchange:update-direction-requests"));
    x.AddRequestClient<DeleteDirectionRequest>(new Uri("exchange:delete-direction-requests"));

    x.AddRequestClient<GetUserRequest>(new Uri("exchange:get-user-requests"));
    x.AddRequestClient<GetAllUsersRequest>(new Uri("exchange:get-all-users-requests"));
    x.AddRequestClient<CreateUserRequest>(new Uri("exchange:create-user-requests"));
    x.AddRequestClient<UpdateUserRequest>(new Uri("exchange:update-user-requests"));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RABBITMQ_HOST"], builder.Configuration["RABBITMQ_VHOST"], h =>
        {
            h.Username(builder.Configuration["RABBITMQ_USER"]);
            h.Password(builder.Configuration["RABBITMQ_PASSWORD"]);
        });
        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
