using Data;
using Data.Implementations;
using Data.Interfaces;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Consumers.Projects;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, cfg) =>
{
    cfg
    .Enrich.WithProperty("Application", "Direction0")
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
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProjects, Projects>();
builder.Services.AddScoped<IProject_logs, Project_logs>();
builder.Services.AddScoped<DataManager>();
var connection = builder.Configuration["CONNECTIONSTRING_MSSQL"];
builder.Services.AddDbContext<ProjectsDbContext>(options =>
{
    options.UseSqlServer(connection);
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateProjectConsumer>().Endpoint(endp => endp.Name = "create-project-requests");
    x.AddConsumer<GetAllProjectsConsumer>().Endpoint(endp => endp.Name = "get-all-projects-requests");
    x.AddConsumer<GetProjectConsumer>().Endpoint(endp => endp.Name = "get-project-requests");
    x.AddConsumer<UpdateProjectConsumer>().Endpoint(endp => endp.Name = "update-project-requests");
    x.AddConsumer<DeleteProjectConsumer>().Endpoint(endp => endp.Name = "delete-project-requests");
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RABBITMQ_HOST"], builder.Configuration["RABBITMQ_VHOST"], h =>
        {
            h.Username(builder.Configuration["RABBITMQ_USER"]);
            h.Password(builder.Configuration["RABBITMQ_PASSWORD"]);
        });
        cfg.ExchangeType = ExchangeType.Fanout;
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
