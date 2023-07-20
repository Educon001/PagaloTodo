using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Infrastructure.Database;
using UCABPagaloTodoMS.Infrastructure.Settings;
using UCABPagaloTodoMS.Providers.Implementation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RestSharp;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UCABPagaloTodoMS.Application.Handlers.Queries;
using UCABPagaloTodoMS.Application.Handlers.Queries.Services;
using UCABPagaloTodoMS.Authorization;

namespace UCABPagaloTodoMS;

[ExcludeFromCodeCoverage]
public class Startup
{
    

    private AppSettings _appSettings;
    private readonly string _allowAllOriginsPolicy = "AllowAllOriginsPolicy";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        VersionNumber = "v" + Assembly.GetEntryAssembly()!
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
        Folder = "docs";
    }
    private string Folder { get; }
    private string VersionNumber { get; }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(_allowAllOriginsPolicy,
                builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
        });
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var appSettingsSection = Configuration.GetSection("AppSettings");
        _appSettings = appSettingsSection.Get<AppSettings>();
        services.Configure<AppSettings>(appSettingsSection);
        services.AddTransient<IUCABPagaloTodoDbContext, UCABPagaloTodoDbContext>();
        services.AddProviders(Configuration, Folder, _appSettings, environment);
        services.AddMediatR(
                typeof(GetServicesQueryHandler).GetTypeInfo().Assembly);
        services.AddMemoryCache();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aA1$Bb2&Cc3^Dd4#Ee5!Ff6*Gg7(Hh8)Ii9Jj0")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.AdminPolicy, policy =>
                policy.RequireClaim("UserType","admin"));
        
            options.AddPolicy(AuthorizationPolicies.ConsumerPolicy, policy =>
                policy.RequireClaim("UserType", "consumer"));
        
            options.AddPolicy(AuthorizationPolicies.ProviderPolicy, policy =>
                policy.RequireClaim("UserType", "provider"));
            
            options.AddPolicy(AuthorizationPolicies.AdminOrConsumerPolicy, policy =>
            {
                policy.RequireClaim("UserType", "admin", "consumer");
            });

            options.AddPolicy(AuthorizationPolicies.AdminOrProviderPolicy, policy =>
            {
                policy.RequireClaim("UserType", "admin", "provider");
            });

            options.AddPolicy(AuthorizationPolicies.ConsumerOrProviderPolicy, policy =>
            {
                policy.RequireClaim("UserType", "consumer", "provider");
            });

            options.AddPolicy(AuthorizationPolicies.AllPolicies, policy =>
            {
                policy.RequireClaim("UserType", "admin", "consumer", "provider");
            });
        });
        
    }

    public void Configure(IApplicationBuilder app)
    {
        var appSettingsSection = Configuration.GetSection("AppSettings");
        _appSettings = appSettingsSection.Get<AppSettings>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();


        if (_appSettings.RequireSwagger)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/{documentname}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./" + Folder + "/swagger.json", $"UCABPagaloTodo Microservice ({VersionNumber})");
                c.InjectStylesheet(_appSettings.SwaggerStyle);
                c.DisplayRequestDuration();
                c.RoutePrefix = string.Empty;
            });
        }


        if (_appSettings.RequireAuthorization)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        if (_appSettings.RequireControllers)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/ready",
                    new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
            });
        }
        
        
        
    }
}
