using QK.Framework.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QK.Framework.Core.Extensions
{
    /// <summary>
    /// 查询表达式扩展
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        ///  运算符
        ///  l  模糊查询 
        ///  g  时间大于
        ///  e  时间小于
        ///  d  数字等于
        ///  b  数字大于
        ///  s  数字小于
        ///  n  不等于
        ///  od or等于查询
        ///  o  or模糊查询
        /// </summary>
        public static IQueryable<T> Filter<T>(this IQueryable<T> query, List<DataFilter> filters)
        {
            if (filters == null || filters.Count == 0)
                return query;
            var props = typeof(T).GetProperties();
            if (props == null || props.Length == 0)
                return query;

            Expression predicateBody = null;
            ParameterExpression pe = Expression.Parameter(typeof(T), "obj");
            foreach (var filter in filters)
            {
                var prop = props.Where(p => p.Name.ToLower() == filter.field.ToLower()).FirstOrDefault();
                if (prop == null)
                    continue;

                Expression left = Expression.Property(pe, prop.Name);
                Expression right = null;
                if (typeof(int?) == prop.PropertyType || typeof(int) == prop.PropertyType)
                {
                    right = Expression.Constant(Convert.ToInt32(filter.value), prop.PropertyType);
                }
                else if (typeof(string) == prop.PropertyType)
                {
                    right = Expression.Constant(filter.value, prop.PropertyType);
                }
                else if (typeof(DateTime?) == prop.PropertyType)
                {
                    right = Expression.Constant(Convert.ToDateTime(filter.value), prop.PropertyType);
                }
                else if (prop.PropertyType.BaseType == typeof(Enum))
                {
                    right = Expression.Constant(Enum.Parse(prop.PropertyType, filter.value), prop.PropertyType);
                }

                if (right == null)
                    return query;

                Expression expression = null;
                switch (filter.comparison)
                {
                    case "l":
                        expression = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), right);
                        break;
                    case "g":
                    case "b":
                        expression = Expression.GreaterThan(left, right);
                        break;
                    case "e":
                    case "s":
                        expression = Expression.LessThanOrEqual(left, right);
                        break;
                    case "d":
                        expression = Expression.Equal(left, right);
                        break;
                    case "n":
                        expression = Expression.NotEqual(left, right);
                        break;
                    case "o":
                        break;
                }
                if (predicateBody == null)
                {
                    predicateBody = expression;
                }
                else
                {
                    predicateBody = Expression.AndAlso(predicateBody, expression);
                }
            }
            return query.Where(Expression.Lambda<Func<T, bool>>(predicateBody, new ParameterExpression[] { pe }));
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, List<SortData> sorts)
        {
            if (sorts == null || sorts.Count == 0)
                return query;
            var props = typeof(T).GetProperties();

            sorts = sorts.OrderBy(m => m.level).ToList();

            try
            {
                ParameterExpression pe = Expression.Parameter(typeof(T));

                foreach (var m in sorts)
                {
                    var prop = props.Where(p => p.Name.ToLower() == m.field.ToLower()).FirstOrDefault();
                    var tmp = prop.GetType();

                    var epr = Expression.PropertyOrField(pe, prop.Name);
                    if (prop.PropertyType == typeof(string))
                    {
                        query = ToSort<T, string>(m.type, query, Expression.Lambda<Func<T, string>>(epr, pe));
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        query = ToSort<T, int>(m.type, query, Expression.Lambda<Func<T, int>>(epr, pe));
                    }
                    else if (prop.PropertyType == typeof(DateTime?))
                    {
                        query = ToSort<T, DateTime?>(m.type, query, Expression.Lambda<Func<T, DateTime?>>(epr, pe));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return query;
        }

        private static IQueryable<T> ToSort<T, TKey>(SortType type, IQueryable<T> query, Expression<Func<T, TKey>> lambdaExpression)
        {
            switch (type)
            {
                case SortType.ASC:
                    query = query.OrderBy(lambdaExpression);
                    break;
                case SortType.DESC:
                    query = query.OrderByDescending(lambdaExpression);
                    break;
            }

            return query;
        }
    }
}
