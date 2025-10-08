using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BHK.Retrieval.Attendance.Shared.Options;

namespace BHK.Retrieval.Attendance.WPF.Configuration.DI;

/// <summary>
/// Configuration bootstrapper for the application
/// </summary>
public static class ConfigurationBootstrapper
{
    /// <summary>
    /// Configure application configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>IConfiguration instance</returns>
    public static IConfiguration ConfigureAppConfiguration(this IServiceCollection services)
    {
        // Get environment from environment variable or default to Production
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                         ?? Environment.GetEnvironmentVariable("ENVIRONMENT") 
                         ?? "Production";

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Register IConfiguration as singleton
        services.AddSingleton<IConfiguration>(configuration);

        return configuration;
    }

    /// <summary>
    /// Configure options pattern with validation
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure all options with validation
        services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<DeviceOptions>(configuration.GetSection(DeviceOptions.SectionName));
        services.Configure<SharePointOptions>(configuration.GetSection(SharePointOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<ReportOptions>(configuration.GetSection(ReportOptions.SectionName));
        services.Configure<UIOptions>(configuration.GetSection(UIOptions.SectionName));

        // Add validation for critical options
        services.PostConfigure<DatabaseOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException("Database connection string cannot be empty");
            }
        });

        services.PostConfigure<ApplicationOptions>(options =>
        {
            if (string.IsNullOrWhiteSpace(options.ApplicationName))
            {
                throw new InvalidOperationException("Application name cannot be empty");
            }
        });

        return services;
    }

    /// <summary>
    /// Get strongly-typed options for immediate use
    /// </summary>
    /// <typeparam name="T">Options type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <param name="sectionName">Configuration section name</param>
    /// <returns>Configured options instance</returns>
    public static T GetOptions<T>(this IServiceCollection services, IConfiguration configuration, string sectionName) 
        where T : class, new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }
}