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
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ComputerShop.Models.Models;

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
builder.Services.AddSwaggerGen(x =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme()
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **_ONLY_** your JWT Bearer token in the text box below",
        Reference = new OpenApiReference()
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    x.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {jwtSecurityScheme,Array.Empty<string>()}
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireClaim("Admin");
    });
});

//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
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

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.RegisterHealthChecks();
app.MapHealthChecks("/healthz");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
