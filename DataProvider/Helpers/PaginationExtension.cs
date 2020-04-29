using Models;
using Models.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Dynamic;

namespace DataProvider.Helpers
{
    public static class Extensions
    {
        public static async Task<PaginatedResultModel<T>> Paginate<T>(this IQueryable<T> query, BaseSearchModel search)
        {
            try
            {
                PaginatedResultModel<T> searchResult = new PaginatedResultModel<T>();
                if (!string.IsNullOrEmpty(search.OrderByColumn))
                {
                    if (search.OrderDir == Catalogs.PaginationOrderCatalog.Asc)
                        query = query.OrderBy($"{search.OrderByColumn} ASC");
                    else
                        query = query.OrderBy($"{search.OrderByColumn} DESC");
                }

                try
                {
                    if (search.CalculateTotal)
                    {
                        searchResult.TotalCount = (query == null ? 0 : await query.CountAsync());
                    }
                    if (search.DisablePagination == false)
                    {
                        query = query.Skip((search.CurrentPage - 1) * search.RecordsPerPage).Take(search.RecordsPerPage);
                    }
                    List<T> resultList = await query.ToListAsync() as List<T>;
                    searchResult.Items = resultList ?? new List<T>();


                }
                catch (Exception ex)
                {
                    searchResult.Items = new List<T>();
                }
                return searchResult;
            }
            catch (Exception ex)
            {
                return new PaginatedResultModel<T>();
            }

        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
        public static IEnumerable<t> DistinctByAsyncVer<t>(this IEnumerable<t> list, Func<t, object> propertySelector)
        {
            return list.GroupBy(propertySelector).Select(x => x.First());
        }
        private static PropertyInfo GetPropertyInfo(Type objType, string name)
        {
            var properties = objType.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name == name);
            if (matchedProperty == null)
                throw new ArgumentException("name");

            return matchedProperty;
        }
        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi)
        {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> query, string name, string orderType)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == orderType && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string name, string orderType)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == orderType && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }
    }

}
