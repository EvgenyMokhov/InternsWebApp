using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Data;
using Data.Implementations;
using Data.Interfaces;
using Rabbit.Consumers.Interns;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using RabbitMQ.Consumers.Interns;

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
builder.Services.AddScoped<IInterns, Interns>();
builder.Services.AddScoped<IIntern_logs, Intern_logs>();
builder.Services.AddScoped<DataManager>();
var connection = builder.Configuration["CONNECTIONSTRING_MSSQL"];
builder.Services.AddDbContext<InternsDbContext>(options =>
{
    options.UseSqlServer(connection);
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateInternConsumer>().Endpoint(endp => endp.Name = "create-intern-requests");
    x.AddConsumer<GetAllInternsConsumer>().Endpoint(endp => endp.Name = "get-all-interns-requests");
    x.AddConsumer<GetInternConsumer>().Endpoint(endp => endp.Name = "get-intern-requests");
    x.AddConsumer<UpdateInternConsumer>().Endpoint(endp => endp.Name = "update-intern-requests");
    x.AddConsumer<GetInternsByDirectionConsumer>().Endpoint(endp => endp.Name = "get-interns-by-direction-requests");
    x.AddConsumer<GetInternsByProjectConsumer>().Endpoint(endp => endp.Name = "get-interns-by-project-requests");
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
