using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using Data;
using Data.Implementations;
using Data.Interfaces;
using RabbitMQ.Consumers.Users;

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
builder.Services.AddScoped<IUsers, Users>();
builder.Services.AddScoped<IUser_logs, Users_logs>();
builder.Services.AddScoped<DataManager>();
var connection = builder.Configuration["CONNECTIONSTRING_MSSQL"];
builder.Services.AddDbContext<UsersDbContext>(options =>
{
    options.UseSqlServer(connection);
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateUserConsumer>().Endpoint(endp => endp.Name = "create-user-requests");
    x.AddConsumer<GetAllUsersConsumer>().Endpoint(endp => endp.Name = "get-all-users-requests");
    x.AddConsumer<GetUserConsumer>().Endpoint(endp => endp.Name = "get-user-requests");
    x.AddConsumer<UpdateUserConsumer>().Endpoint(endp => endp.Name = "update-user-requests");

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
