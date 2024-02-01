using AppProject.Model;
using AppProject.Model.Entities.Identities;
using AppProject.Repository.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppProject.Repository.Context;

public class AppProjectDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,long>
{
    public AppProjectDbContext(DbContextOptions options) : base(options)
    {
    }

    #region 实体映射表

    public DbSet<TestModel> TestModels => Set<TestModel>();
    
    #endregion


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new QueryCommandInterceptor());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = "Anonymous";
                    entry.Entity.CreatedTime=DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = "Anonymous";
                    entry.Entity.LastModified=DateTime.UtcNow;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}