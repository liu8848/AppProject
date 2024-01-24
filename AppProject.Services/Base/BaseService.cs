using System.Linq.Expressions;
using AppProject.IService.Base;
using AppProject.Model;
using AppProject.Repository.Base;

namespace AppProject.Services.Base;

public class BaseService<TEntity>:IBaseService<TEntity> where TEntity:class,new()
{
    public IBaseRepository<TEntity> _repository { get; set; }
    
    public BaseService(IBaseRepository<TEntity> repository=null)
    {
        _repository = repository;
    }


    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await _repository.GetAsync(condition);
    }

    public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await _repository.GetList(condition);
    }
}