using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CustomerInsights.Database;
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot cfg = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                           .AddJsonFile("appsettings.Development.json", optional: true)
                                                           .AddUserSecrets<AppDbContextFactory>(optional: true)
                                                           .AddEnvironmentVariables()
                                                           .Build();

        // ConnectionString-Name an dein Setup anpassen
        string cs = cfg.GetConnectionString("customerinsights-db") 
                    ?? cfg.GetConnectionString("DefaultConnection") 
                    ?? Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__CUSTOMERINSIGHTS-DB") 
                    ?? "Host=localhost;Database=ci;Username=postgres;Password=postgres";

        DbContextOptions<AppDbContext> opts = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(cs).Options;
        return new AppDbContext(opts);
    }
}
