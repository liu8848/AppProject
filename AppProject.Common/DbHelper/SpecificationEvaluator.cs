namespace AppProject.Common.DbHelper;

public class SpecificationEvaluator<T> where T:class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T>? specification)
    {
        var query = inputQuery;
        if (specification?.Criteria is not null) query = query.Where(specification.Criteria);

        return query;
    }
}