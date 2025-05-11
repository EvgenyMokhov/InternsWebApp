using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog.Events;
using Data;
using Data.Implementations;
using Data.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Elastic.CommonSchema.Serilog;
using RabbitMQ.Consumers.Direction;

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
builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IInternshipDirections, InternshipDirections>();
builder.Services.AddScoped<IInternshipDirection_logs, InternshipDirection_logs>();
builder.Services.AddScoped<DataManager>();
var connection = builder.Configuration["CONNECTIONSTRING_MSSQL"];
builder.Services.AddDbContext<InternshipDirectionDbContext>(options =>
{
    options.UseSqlServer(connection);
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateDirectionConsumer>().Endpoint(endp => endp.Name = "create-direction-requests");
    x.AddConsumer<GetAllDirectionsConsumer>().Endpoint(endp => endp.Name = "get-all-directions-requests");
    x.AddConsumer<GetDirectionConsumer>().Endpoint(endp => endp.Name = "get-direction-requests");
    x.AddConsumer<UpdateDirectionConsumer>().Endpoint(endp => endp.Name = "update-direction-requests");
    x.AddConsumer<DeleteDirectionConsumer>().Endpoint(endp => endp.Name = "delete-direction-requests");
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
