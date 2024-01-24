using System.Linq.Expressions;
using AppProject.Common.DbHelper;
using AppProject.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AppProject.Repository.Base;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
{
    #region 查询相关接口

    public virtual ValueTask<TEntity?> GetAsync(object key)
    {
        return _dbSet.FindAsync(key);
    }
    public async Task<IReadOnlyList<TEntity>> GetAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public bool Any(Expression<Func<TEntity, bool>>? condition = null)
    {
        return null != condition ? _dbSet.Any(condition) : _dbSet.Any();
    }

    private readonly AppProjectDbContext _dbContext;
    
    public DbSet<TEntity> _dbSet => _dbContext.Set<TEntity>();

    public BaseRepository(AppProjectDbContext dbContext)
    {
        _dbContext = dbContext??throw new ArgumentNullException($"{typeof(AppProjectDbContext).FullName}");
    }

    public IQueryable<TEntity> GetAsQueryable()
    {
        return _dbSet.AsQueryable();
    }
    
    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(condition);
    }

    public async Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> condition)
    {
        return await _dbSet.AsNoTracking().Where(condition).ToListAsync();
    }

    public int Count(Expression<Func<TEntity, bool>> condition)
    {
        return _dbSet.Where(condition).Count();
    }
    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await _dbSet.Where(condition).CountAsync();
    }
    
    #endregion

    #region 添加相关接口
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task AddRange(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.AddRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    #endregion

    #region 更新相关接口

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region 删除相关接口

    public async Task DeleteAsync(object key)
    {
        var entity = await GetAsync(key);
        if (entity is not null) await DeleteAsync(entity);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion

    public async Task<EntityEntry<TEntity>> InsertAsync(TEntity entity)
    {
        var entityEntry = await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entityEntry;
    }
    
}