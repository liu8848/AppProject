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
        Criteria = criteria is not null ? Criteria.AndAlso(criteria) : criteria;
    }
}

public static class ExpressionExtension
{
    public static Expression<Func<T, bool>>
        AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter((typeof(T)));
        
        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0],parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left ?? throw new InvalidOperationException(),
                right ?? throw new InvalidOperationException()), parameter);
        
    }
    
    private class ReplaceExpressionVisitor:ExpressionVisitor
    {
        private readonly Expression _newValue;
        private readonly Expression _oldValue;

        public ReplaceExpressionVisitor(Expression newValue, Expression oldValue)
        {
            _newValue = newValue;
            _oldValue = oldValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}