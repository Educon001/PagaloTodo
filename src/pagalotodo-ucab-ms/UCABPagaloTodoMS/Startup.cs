using System.Reflection;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Infrastructure.Database;
using UCABPagaloTodoMS.Infrastructure.Settings;
using UCABPagaloTodoMS.Providers.Implementation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using RestSharp;

namespace UCABPagaloTodoMS;


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
    }

    public void Configure(IApplicationBuilder app)
    {
        var appSettingsSection = Configuration.GetSection("AppSettings");
        _appSettings = appSettingsSection.Get<AppSettings>();

        app.UseHttpsRedirection();
        app.UseRouting();

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
