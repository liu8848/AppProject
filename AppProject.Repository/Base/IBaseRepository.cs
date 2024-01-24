using System.Linq.Expressions;
using AppProject.Common.DbHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AppProject.Repository.Base;

public interface IBaseRepository<TEntity> where TEntity : class
{
    DbSet<TEntity> _dbSet { get; }

    #region 查询

    ValueTask<TEntity?> GetAsync(object key);
    IQueryable<TEntity> GetAsQueryable();
    Task<IReadOnlyList<TEntity>> GetAsync();
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition);
    Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> condition);
    
    int Count(Expression<Func<TEntity, bool>> condition);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> condition);

    bool Any(Expression<Func<TEntity, bool>>? condition = null);
    
    #endregion

    #region 添加

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRange(List<TEntity> entities, CancellationToken cancellationToken = default);
    
    #endregion

    #region 更新
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    #endregion

    #region 删除
    Task DeleteAsync(object key);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    #endregion

    Task<EntityEntry<TEntity>> InsertAsync(TEntity entity);
}