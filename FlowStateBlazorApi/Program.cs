using FlowStateBlazor.Data.Context;
using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using FlowStateBlazor.Data.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace FlowStateBlazorApi;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        // https://nblumhardt.com/2020/10/bootstrap-logger/
        // Initialize early, without access to configuration or services
        //Log.Logger = new LoggerConfiguration()
        //    .WriteTo.Console()
        //    .WriteTo.File(@"log.txt")
        //    .CreateBootstrapLogger();

        // https://stackoverflow.com/questions/49744852/use-serilog-with-azure-log-stream
        Log.Logger = new LoggerConfiguration()
        .WriteTo.File(@"C:\home\LogFiles\Application\RseWebServer.txt",
            fileSizeLimitBytes: 1_000_000,
            rollOnFileSizeLimit: true,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1))
        .CreateBootstrapLogger();

        //Log.Logger = new LoggerConfiguration()
        //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        //    .Enrich.FromLogContext()
        //    .WriteTo.Console()
        //    .CreateLogger(); 

        Log.Information("Starting up");

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                // You might notice that the console sink is specified in both logger configurations.
                // This is because the bootstrap logger is wholly reconfigured:
                // its initial set of sinks and other pipeline components are completely shut down, and a new pipeline is set up.
                loggerConfiguration.WriteTo.Console()
                    // We have access to configuration and services from the host
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services);

                //loggerConfiguration
                //    .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces)
            });

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigureApp(app, app.Environment);

            await InitDb(app);

            app.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An exception occurred while creating the web host");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<FlowStateContext, MyFlowStateContextFactory>();

        services.AddScoped<FlowStateContext>(p => p.GetRequiredService<IDbContextFactory<FlowStateContext>>().CreateDbContext());

        services.AddControllers();

        // Learn more about configuring OpenAPI at
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0
        services.AddOpenApi();
    }

    /// <summary>
    /// Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public static void ConfigureApp(WebApplication app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            app.MapOpenApi();

            // Launch the app and navigate to https://localhost:<port>/swagger to view the Swagger UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });

            // add Scalar.AspNetCore package
            //app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapGet("/", () => "SwaggerEndpoint are at /openapi/v1.json");

        app.MapControllers();
    }

    /// <summary>
    /// Seed the database with information
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task InitDb(IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                //Seed Default Users
                var factory = services.GetRequiredService<IDbContextFactory<FlowStateContext>>();
                using (var context = factory.CreateDbContext())
                {
                    FlowGraphDescriptionService service = new FlowGraphDescriptionService(context);

                    int count = await service.CountAsync();
                    if (count == 0)
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        var description = JsonSerializer.Serialize("", options);

                        var fgd = new FlowGraphDescription
                        {
                            Name = "Name1",
                            Description = "Description1",
                            JsonFlowSerialized = description!
                        };

                        await service.AddAsync(fgd);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }
        }
    }
}
