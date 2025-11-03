// Data/DesignTimeDbContextFactory.cs
using ExpoConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ExpoConnect.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        
        var cs = Environment.GetEnvironmentVariable("EXPOCONNECT_CONNECTION_STRING");

        
        cs ??= "Host=localhost;Port=5432;Database=expo_connect_db;Username=postgres;Password=postgres123";

        var opt = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs, npg =>
            {
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            })
            .Options;

        return new AppDbContext(opt);
    }
}
