using HandiPapi;
using HandiPapi.DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;



public partial class Program
{
    private const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";

    public static async Task Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(
                   path: "c:\\handipapi\\logs\\log-.txt",
                   outputTemplate: "{TimeStamp:yyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:Ij}{NewLine}{Exception}",
                   restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information).CreateLogger();

            Log.Information("Application is starting");
            
            var webhost = CreateHostBuilder(args).Build();
            
            using (var scope = webhost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
                try
                {
                    logger.LogInformation("Starting up");
                    var context = services.GetRequiredService<DatabaseContext>();
                    var apply = await ApplyMigrations(services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occured during migration");
                }
            }
            
            await webhost.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application Failed to Start");
        }
        finally
        {
            Log.CloseAndFlush();
        }


    }

    private static async Task<int> ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        await using var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await dbContext.Database.MigrateAsync();

        return 1;
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
            .ConfigureAppConfiguration((context, config) =>{
                var env = context.HostingEnvironment;
                config.SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable(AspNetCoreEnvironment)}.json", optional: true)
                    .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
                    .AddJsonFile($"secrets/appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();
            })
            .UseStartup<Startup>();
        }
        );

}



