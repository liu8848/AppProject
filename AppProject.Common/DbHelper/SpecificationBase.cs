using System.Linq.Expressions;

namespace AppProject.Common.DbHelper;

public class SpecificationBase<T>:ISpecification<T>
{
    protected SpecificationBase()
    {
        
    }

    protected SpecificationBase(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }
    
    public Expression<Func<T,bool>> Criteria { get; private set; }

    public void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria=criteria is not null?Criteria
    }
}

public static class ExpressionExtension
{
    public static Expression<Func<Task, bool>>
        AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        Expression.Parameter((typeof(T));
    }
}