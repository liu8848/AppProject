using AppProject.Model;
using AppProject.Model.Attributes;
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
    
    #endregion


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var mappedEntities = GetAllMappedEntities();
        mappedEntities.ForEach(t=>modelBuilder.Entity(t));
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


    /// <summary>
    /// 获取所有映射实体类型
    /// </summary>
    /// <returns></returns>
    private List<Type> GetAllMappedEntities()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        var types = assemblies.SelectMany(t=>t.GetTypes())
            .Where(t=>
                t.GetCustomAttributes(typeof(TableEntityAttribute),inherit:false).Length>0
                &&t.IsClass&&!t.IsAbstract)
            .ToList();
        return types;
    }
}