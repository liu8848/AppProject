using System.Linq.Expressions;
using AppProject.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AppProject.Repository.Base;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
{
    private readonly AppProjectDbContext _dbContext;

    public BaseRepository(AppProjectDbContext dbContext)
    {
        _dbContext = dbContext??throw new ArgumentNullException($"{typeof(AppProjectDbContext).FullName}");
    }


    public DbSet<TEntity> _dbSet => _dbContext.Set<TEntity>();

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await _dbSet.FirstOrDefaultAsync(condition);
    }

    public async Task<EntityEntry<TEntity>> InsertAsync(TEntity entity)
    {
        var entityEntry = await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entityEntry;
    }
}