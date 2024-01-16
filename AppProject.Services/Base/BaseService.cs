using AppProject.IService.Base;
using AppProject.Repository.Base;

namespace AppProject.Services.Base;

public class BaseService<TEntity>:IBaseService<TEntity> where TEntity:class,new()
{
    private readonly IBaseRepository<TEntity> _baseRepository;

    public BaseService(IBaseRepository<TEntity> baseRepository)
    {
        _baseRepository = baseRepository??throw new ArgumentNullException($"{typeof(IBaseRepository<TEntity>).FullName}");
    }
    
    
}