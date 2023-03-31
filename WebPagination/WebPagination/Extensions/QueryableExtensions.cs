using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Linq.Expressions;
using System.Reflection;
using WebPagination.Models;

namespace DataTableExample.Controllers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, TableSearchModel model, Order order)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");
            PropertyInfo property;
            Expression propertyAccess;

            property = typeof(T).GetProperty(model.Columns[order.Column].Data);
            propertyAccess = Expression.MakeMemberAccess(parameter, property);

            bool ascending = order.Dir == "asc";

            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable),
                                                             ascending ? "OrderBy" : "OrderByDescending",
                                                             new[] { type, property.PropertyType }, source.Expression,
                                                             Expression.Quote(orderByExp));

            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, TableSearchModel model)
        {

            if (model.Order.Count > 0)
            {
                if (model.Order.Count == 1)
                {

                    return source.OrderByDynamic(model, model.Order[0]);
                }
                else
                {
                    var order = model.Order[0];
                    model.Order.Remove(order);
                    return source.OrderByDynamic(model, order).OrderByDynamic(model);
                }
            }
            return source;
        }

        public static IQueryable<T> WhereAndOr<T>(this IQueryable<T> source, string column, object value, JoinCondition joinCondition)
        {
            var list = new List<SearchCriteria>();
            list.Add(new SearchCriteria(column, value, joinCondition));
            return source.WhereAndOr(list);
        }

        public static IQueryable<T> WhereAndOr<T>(this IQueryable<T> source, TableSearchModel model, List<SearchCriteria> extraCriterias = null)
        {
            try
            {
                var list = new List<SearchCriteria>();

                var searchablecols = model.Columns.Where(c => c.Searchable == true).ToList();
                var searchString = "";

                if (model.Search != null && !string.IsNullOrEmpty(model.Search.Value))
                {
                    searchString = model.Search.Value.Trim();
                }

                foreach (var col in searchablecols)
                {
                    if (!string.IsNullOrEmpty(col.Search.Value))
                    {
                        var st = col.Search.Value.Replace("^", "").Replace("$", "").Replace("\\", "").Trim();
                        if (st.Length > 0)
                        {
                            list.Add(new SearchCriteria(col.Data.ToUpper(), st.ToUpper(), JoinCondition.And));
                        }
                    }
                    if (!string.IsNullOrEmpty(searchString))
                    {
                        list.Add(new SearchCriteria(col.Data.ToUpper(), searchString.ToUpper(), JoinCondition.Or));
                    }
                }

                if (extraCriterias != null)
                {
                    list.AddRange(extraCriterias);
                }

                return source.WhereAndOr(list);

            }
            catch (Exception ex)
            {
                //do nothing
                var a = ex;
            }

            return source;
        }

        public static IQueryable<T> WhereAndOr<T>(this IQueryable<T> source, List<SearchCriteria> criterias)
        {
            if (criterias.Count() == 0)
                return source;

            LambdaExpression lambda;
            Expression resultCondition = null;

            // Create a member expression pointing to given column
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "p");

            foreach (var searchCriteria in criterias)
            {
                try
                {
                    if (string.IsNullOrEmpty(searchCriteria.PropertyName))
                    {
                        continue;
                    }

                    Expression memberAccess = MemberExpression.Property(parameter, searchCriteria.PropertyName);


                    if (memberAccess.Type == typeof(DateTime))
                    {
                        memberAccess = Date_ToString.ReplaceParameter(memberAccess);
                    }
                    else if (memberAccess.Type != typeof(string))
                    {
                        var m = typeof(object).GetMethods().Where(x => x.Name == "ToString").FirstOrDefault();
                        memberAccess = Expression.Call(memberAccess, m);
                    }

                    var containsMethod = typeof(string).GetMethods().Where(x => x.Name == "Contains").FirstOrDefault();

                    Expression condition = Expression.Call(memberAccess, containsMethod, Expression.Constant(searchCriteria.PropertyValue));

                    if (searchCriteria.JoinCondition == JoinCondition.And)
                    {
                        resultCondition = resultCondition != null ? Expression.And(resultCondition, condition) : condition;
                    }
                    else if (searchCriteria.JoinCondition == JoinCondition.Or)
                    {
                        resultCondition = resultCondition != null ? Expression.Or(resultCondition, condition) : condition;
                    }

                }
                catch (Exception ex)
                {
                    //Do Nothing
                    var a = ex;
                }
            }

            lambda = Expression.Lambda(resultCondition, parameter);

            MethodCallExpression result = Expression.Call(
                       typeof(Queryable), "Where",
                       new[] { source.ElementType },
                       source.Expression,
                       lambda);

            return source.Provider.CreateQuery<T>(result);

        }

        public static JsonResult GetPagedList<T>(this IQueryable<T> source, TableSearchModel model)
        {
            var list = source.GetIPagedList(model);

            return new JsonResult(new { draw = model.Draw, recordsTotal = list.TotalItemCount, recordsFiltered = list.TotalItemCount, data = list.ToList() });
        }

        public static IPagedList<T> GetIPagedList<T>(this IQueryable<T> source, TableSearchModel model)
        {
            var pageSize = 10;
            var pageNumber = 1;
            if (model.Length > 0)
            {
                pageSize = model.Length;
            }
            if (model.Start > 0)
            {
                pageNumber = (model.Start / pageSize) + 1;
            }
            try
            {
                return source.WhereAndOr(model).OrderByDynamic(model).ToPagedList(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return source.ToPagedList(pageNumber, pageSize);
            }

        }

        public enum JoinCondition
        {
            And = 0,
            Or = 1
        }

        public class SearchCriteria
        {
            public SearchCriteria(string propertyName, object propertyValue, JoinCondition joinCondition)
            {
                PropertyName = propertyName;
                PropertyValue = propertyValue;
                JoinCondition = joinCondition;
            }

            public string PropertyName { get; set; }
            public object PropertyValue { get; set; }
            public JoinCondition JoinCondition { get; set; }
        }

        public static Expression ReplaceParameter(this LambdaExpression expression, Expression target)
        {
            return new ParameterReplacer { Source = expression.Parameters[0], Target = target }.Visit(expression.Body);
        }

        public class ParameterReplacer : ExpressionVisitor
        {
            public ParameterExpression Source;
            public Expression Target;
            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == Source ? Target : base.VisitParameter(node);
            }
        }

        public static readonly Expression<Func<DateTime, string>> Date_ToString = date =>
        date.Year.ToString() + "-" +
        date.Month.ToString() + "-" +
        date.Day.ToString();

    }
}
