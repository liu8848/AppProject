using AppProject.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AppProject.Repository.Context;

public class AppProjectDbContextFactory:IDesignTimeDbContextFactory<AppProjectDbContext>
{
    public AppProjectDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppProjectDbContext>();

        var connectionString = App.Configuration.GetConnectionString("Default");
        optionsBuilder.UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion);
        return new AppProjectDbContext(optionsBuilder.Options);
    }
}