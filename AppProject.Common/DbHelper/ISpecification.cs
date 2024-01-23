using System.Linq.Expressions;

namespace AppProject.Common.DbHelper;

public interface ISpecification<T>
{
    //查询条件子句
    Expression<Func<T,bool>> Criteria { get; }
}