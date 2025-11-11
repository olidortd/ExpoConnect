using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExpoConnect.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connStr = Environment.GetEnvironmentVariable("EXPOCONNECT_CONNECTION")
                      ?? "Host=localhost;Port=5432;Database=expo_connect_db;Username=postgres;Password=postgres123";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr, b => b.MigrationsAssembly("ExpoConnect.Infrastructure"))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new AppDbContext(options);
    }
}
