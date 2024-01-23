using AppProject.Model;
using AppProject.Repository.Interceptors;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AppProject.Repository.Context;

public class AppProjectDbContext : DbContext
{
    public AppProjectDbContext(DbContextOptions options) : base(options)
    {
    }

    #region 实体映射表

    public DbSet<TestModel> TestModels => Set<TestModel>();

    #endregion


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestModel>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new QueryCommandInterceptor());
    }
}