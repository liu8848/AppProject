using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AppProject.Repository.Base;

public interface IBaseRepository<TEntity> where TEntity : class
{
    DbSet<TEntity> _dbSet { get; }

    
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition);

    Task<EntityEntry<TEntity>> InsertAsync(TEntity entity);
}