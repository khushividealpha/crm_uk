using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Text;

namespace CRMUKMTPApi.Extentions;

public static class WebApplicationBuilderExtensions
{
    public static Serilog.Core.Logger AddSerilogServices(this WebApplicationBuilder builder, string dir)
    {
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string logdir = Path.Combine(dir, "Logs");
        if (!Directory.Exists(logdir)) Directory.CreateDirectory(logdir);

        var logger = new LoggerConfiguration()
                     .WriteTo.Console()
                     .WriteTo.File(Path.Combine(logdir, "Log.txt"), rollingInterval: RollingInterval.Day)
                     .MinimumLevel.Information()
                     .MinimumLevel.Override("Microsoft", LogEventLevel.Information) // Disable Microsoft logs below Warning
                     .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning) // Disable EF Core SQL logs below Warning
                     .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Services.AddSerilog(logger);
        return logger;
    }
    public static WebApplicationBuilder AddSwaggerGenUI(this WebApplicationBuilder builder, string title)
    {
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = "v1",
                Description = title
            });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Firebase JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

        });
        return builder;
    }
    public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(option =>
               {
                   option.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration["jwt:Issuer"],
                       ValidAudience = builder.Configuration["jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:Key"]))
                   };
               });
        return builder;
    }
    public static WebApplicationBuilder AddCorsPolicies(this WebApplicationBuilder builder, string policyName)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(policyName,
                builder => builder
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyHeader()
                    .AllowCredentials()
         );
        });
        return builder;
    }
}

