using System.Linq.Expressions;
using AppProject.Repository.Base;

namespace AppProject.IService.Base;

public interface IBaseService<TEntity> where TEntity:class
{
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition);

    Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> condition);
}