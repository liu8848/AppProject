using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AppProject.Repository.Base;

public interface IBaseRepository<TEntity> where TEntity : class
{
    DbSet<TEntity> _dbSet { get; }

    #region 查询

    IQueryable<TEntity> GetAsQueryable();
    
    #endregion

    #region 添加
    Task<long> Add(TEntity entity);
    Task<List<long>> Add(List<TEntity> entities);
    #endregion

    #region 删除
    Task<bool> DeleteById(object id);
    Task<bool> Delete(TEntity entity);
    Task<bool> DeleteByIds(object[] ids);
    #endregion

    #region 更新
    Task<bool> Update(TEntity entity);
    Task<bool> Update(List<TEntity> entities);
    #endregion
    
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition);

    Task<EntityEntry<TEntity>> InsertAsync(TEntity entity);
}