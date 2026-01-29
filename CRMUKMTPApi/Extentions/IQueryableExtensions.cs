using System.Linq.Expressions;

namespace CRMUKMTPApi.Extentions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            string methodName = ascending ? "OrderBy" : "OrderByDescending";

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName
                            && m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(typeof(T), property.Type);

            var result = genericMethod.Invoke(null, new object[] { source, lambda });

            return (IQueryable<T>)result;
        }
    }
}
