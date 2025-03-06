using Microsoft.EntityFrameworkCore;
using UserIpService.Database;
using UserIpService.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DB config
        var configuration = builder.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
                  .UseLazyLoadingProxies());
        
        builder.Services.AddScoped<IUserIpMainService, UserIpService.Services.UserIpMainService>();

        // Minimal API
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // auto migration when start
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Endpoint для добавления UserIp
        app.MapPost("/users/{userId}/ips", async (long userId, string ipAddress, IUserIpMainService service) =>
        {
            await service.AddUserIp(userId, ipAddress);
            return Results.Ok();
        });

        // Endpoint для поиска пользователей по IP префиксу
        app.MapGet("/users/byip/{ipPrefix}", async (string ipPrefix, IUserIpMainService service) =>
        {
            var userIds = await service.FindUsersByIpPrefix(ipPrefix);
            return Results.Ok(userIds);
        });

        // Endpoint для получения IP адресов пользователя
        app.MapGet("/users/{userId}/ips", async (long userId, IUserIpMainService service) =>
        {
            var ips = await service.GetUserIpAddresses(userId);
            return Results.Ok(ips);
        });

        // Endpoint для получения последней связи пользователя и IP
        app.MapGet("/users/{userId}/lastconnection", async (long userId, IUserIpMainService service) =>
        {
            var lastConnection = await service.GetLastUserConnection(userId);
            return Results.Ok(lastConnection);
        });


        app.Run();
    }
}