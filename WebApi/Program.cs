// /////////////////////////////////////////////////////////////////////////////
// PLEASE DO NOT RENAME OR REMOVE ANY OF THE CODE BELOW. 
// YOU CAN ADD YOUR CODE TO THIS FILE TO EXTEND THE FEATURES TO USE THEM IN YOUR WORK.
// /////////////////////////////////////////////////////////////////////////////

using WebApi.Helpers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation;
using WebApi.Validators;
_.__();

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
  builder.WebHost.UseUrls("http://localhost:3000");
  builder.WebHost.ConfigureLogging((context, logging) =>
  {
    var config = context.Configuration.GetSection("Logging");
    logging.AddConfiguration(config);
    logging.AddConsole();
    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
    logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
  });

  var services = builder.Services;
  services.AddControllers();
  services.AddEndpointsApiExplorer();
    builder.Services.AddValidatorsFromAssemblyContaining<CreatePlayerRequestValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<TeamRequirementRequestValidator>();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "PlayerGen API", Version = "v1" });

        // Add Bearer token support
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter Bearer token.\r\n\r\nExample: 'Bearer your_token_here'"
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    });

    services.AddSqlite<DataContext>("DataSource=webApi.db");

  services.AddDataProtection().UseCryptographicAlgorithms(
      new AuthenticatedEncryptorConfiguration
      {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
      });
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
  var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
  dataContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// configure HTTP request pipeline
{
  app.MapControllers();
}

app.Run();

public partial class Program { }
