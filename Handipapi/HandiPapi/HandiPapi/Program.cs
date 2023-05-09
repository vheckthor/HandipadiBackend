using HandiPapi.Configurations;
using HandiPapi.DataAccess;
using HandiPapi.Services;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;



public partial class Program
{
    private const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
    private static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable(AspNetCoreEnvironment)}.json", optional: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static IConfiguration Configuration { get; set; }


    public static void Main(string[] args)
    {
        Configuration = GetConfiguration();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers().AddNewtonsoftJson(op =>
            op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Handipapi", Version = "v1" });
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication();
            //builder.Services.AddDbContext<DatabaseContext>();
            builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));
            builder.Services.AddAutoMapper(typeof(MapperInitializer));
            builder.Services.AddTransient<IAuthManager, AuthManager>();
            //builder.services.AddHttpContextAccessor();
            builder.Services.AddScoped<UserManager<ApiUser>, UserManager<ApiUser>>();
            //builder.Services.AddDefaultIdentity<IdentityUser>(options => { })
            builder.Services.AddIdentityCore<ApiUser>
            (options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>();
           // builder.Services.AddIdentity();//.Services.ConfigureIdentity();
            //builder.Services.ConfigureJWT(Configuration);
            var app = builder.Build();

            //services extention
            
            
           



            

            app.Run();




            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.File(
            //        path: "c:\\handipapi\\logs\\log-.txt",
            //        outputTemplate: "{TimeStamp:yyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:Ij}{NewLine}{Exception}",
            //        RollingInterval.Day,
            //        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information).CreateLogger();


            Log.Information("Application is starting");
            CreateHostBuilder(args).Build().Run();
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



    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Program>();
        }
        );

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Handipapi v1"));
            app.UseHttpsRedirection();
        

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.MapControllers();

            
    }
}



