using MediatR;
using ComputerShop.BL.CommandHandlers;
using ComputerShop.Extensions;
using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using ComputerShop.HealthChecks;
using ComputerShop.Middleware;
using ComputerShop.Models.Configurations;

//logger
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
//Register Serilog
builder.Logging.AddSerilog(logger);

//Kafka
builder.Services.Configure<KafkaProducerSettings>(builder
    .Configuration.GetSection(nameof(KafkaProducerSettings)));

builder.Services.Configure<KafkaConsumerSettings>(builder
    .Configuration.GetSection(nameof(KafkaConsumerSettings)));

//Mongo
builder.Services.Configure<MongoDBSettings>(builder
    .Configuration.GetSection(nameof(MongoDBSettings)));

//Fluent validation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

// Add services to the container.
builder.Services.RegisterRepositories()
        .RegisterHostedService()
        .AddAutoMapper(typeof(Program));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//healthchecks
builder.Services.AddHealthChecks()
    .AddCheck<SQLHealthCheck>("SQL Server");

//MediatR
builder.Services.AddMediatR(typeof(AddComputerCommandHandler).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.RegisterHealthChecks();
app.MapHealthChecks("/healthz");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
