using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Where条件扩展
    /// 用于分步实现不确定条件个数的逻辑运算组织   
    /// </summary>
    public static class Where
    {
        /// <summary>
        /// Enumerable的Contains方法
        /// </summary>
        private static readonly MethodInfo containsMethod;

        /// <summary>
        /// 获取表示式参数名
        /// </summary>
        public static readonly string ParamterName = "item";

        static Where()
        {
            var q = from m in typeof(Enumerable).GetMethods()
                    where m.Name == "Contains" && m.IsGenericMethod
                    let parameters = m.GetParameters()
                    where parameters.Length == 2
                    let pLast = parameters.Last()
                    where pLast.ParameterType.IsGenericType == false
                    select m;

            Where.containsMethod = q.FirstOrDefault();
        }

        /// <summary>
        /// 返回默认为True的条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>()
        {
            return item => true;
        }

        /// <summary>
        /// 返回默认为False的条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>()
        {
            return item => false;
        }


        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="expRight">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, bool>> expRight)
        {
            var candidateExpr = Expression.Parameter(typeof(T), Where.ParamterName);
            var left = new ParameterReplacer(candidateExpr).Replace(expLeft.Body);
            var right = new ParameterReplacer(candidateExpr).Replace(expRight.Body);

            var body = Expression.AndAlso(left, right);
            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        /// <summary>
        /// 或逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="expRight">表达式2</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, bool>> expRight)
        {
            var candidateExpr = Expression.Parameter(typeof(T), Where.ParamterName);
            var left = new ParameterReplacer(candidateExpr).Replace(expLeft.Body);
            var right = new ParameterReplacer(candidateExpr).Replace(expRight.Body);

            var body = Expression.OrElse(left, right);
            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }


        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="fieldName">键2</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expLeft, string fieldName, object value, Operator op)
        {
            var expRight = Where.GeneratePredicate<T>(fieldName, value, op);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">键</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, TKey value, Operator op)
        {
            var expRight = Where.GeneratePredicate<T, TKey>(keySelector, value, op);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 与逻辑运算
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expLeft">表达式1</param>
        /// <param name="keySelector">键选择</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndIn<T, TKey>(this Expression<Func<T, bool>> expLeft, Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var expRight = Where.GenerateAggregateOrPredicate<T, TKey>(keySelector, values);
            return expLeft.And(expRight);
        }

        /// <summary>
        /// 将数组转换为Or的表示式合集
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="keySelector">键选择</param>
        /// <param name="values">包含的值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GenerateAggregateOrPredicate<T, TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var p = keySelector.Parameters.Single();
            var equals = values.Select(value => (Expression)Expression.Equal(keySelector.Body, Expression.Constant(value, typeof(TKey))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.OrElse(accumulate, equal));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        /// <summary>
        /// 生成In操作的表示式
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <param name="keySelector">键选择</param>
        /// <param name="values">包含的值</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GenerateContainsPredicate<T, TKey>(Expression<Func<T, TKey>> keySelector, IEnumerable<TKey> values)
        {
            var method = Where.containsMethod.MakeGenericMethod(typeof(TKey));
            var callBody = Expression.Call(null, method, Expression.Constant(values, typeof(IEnumerable<TKey>)), keySelector.Body);
            var paramExp = keySelector.Parameters.Single();
            return Expression.Lambda(callBody, paramExp) as Expression<Func<T, bool>>;
        }

        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <exception cref="MissingFieldException"></exception>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GeneratePredicate<T>(string fieldName, object value, Operator op)
        {
            var field = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (field == null)
            {
                throw new MissingFieldException(typeof(T).Name, fieldName);
            }

            var paramExp = Expression.Parameter(typeof(T), Where.ParamterName);
            var memberExp = Expression.MakeMemberAccess(paramExp, field);
            return Where.GeneratePredicate<T>(paramExp, memberExp, value, op);
        }

        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">键选择</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GeneratePredicate<T, TKey>(Expression<Func<T, TKey>> keySelector, TKey value, Operator op)
        {
            return Where.GeneratePredicate<T>(keySelector.Parameters.First(), (MemberExpression)keySelector.Body, value, op);
        }


        /// <summary>
        /// 生成表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramExp">参数</param>
        /// <param name="memberExp">成员</param>
        /// <param name="value">值</param>
        /// <param name="op">操作符</param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GeneratePredicate<T>(ParameterExpression paramExp, MemberExpression memberExp, object value, Operator op)
        {
            switch (op)
            {
                case Operator.Contains:
                case Operator.EndWith:
                case Operator.StartsWith:
                    var method = typeof(string).GetMethod(op.ToString(), new Type[] { typeof(string) });
                    var callBody = Expression.Call(memberExp, method, Expression.Constant(value, typeof(string)));
                    return Expression.Lambda(callBody, paramExp) as Expression<Func<T, bool>>;

                default:
                    var valueType = (memberExp.Member as PropertyInfo).PropertyType;
                    var valueExp = Expression.Constant(value, valueType);
                    var expMethod = typeof(Expression).GetMethod(op.ToString(), new Type[] { typeof(Expression), typeof(Expression) });

                    var symbolBody = expMethod.Invoke(null, new object[] { memberExp, valueExp }) as Expression;
                    return Expression.Lambda(symbolBody, paramExp) as Expression<Func<T, bool>>;
            }
        }

        /// <summary>
        /// 表达式参数类型转换
        /// </summary>
        /// <typeparam name="TNew">新类型</typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static Expression<Func<TNew, bool>> Cast<TNew>(this LambdaExpression expression)
        {
            var candidateExpr = Expression.Parameter(typeof(TNew), Where.ParamterName);
            var parameterReplacer = new ParameterReplacer(candidateExpr);

            var body = parameterReplacer.Replace(expression.Body);
            return Expression.Lambda<Func<TNew, bool>>(body, candidateExpr);
        }


        /// <summary>
        /// 参数替换对象
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            /// <summary>
            /// 表达式的参数
            /// </summary>
            public ParameterExpression ParameterExpression { get; private set; }

            /// <summary>
            /// 参数替换对象
            /// </summary>
            /// <param name="exp">表达式的参数</param>
            public ParameterReplacer(ParameterExpression exp)
            {
                this.ParameterExpression = exp;
            }

            /// <summary>
            /// 将表达式调度到此类中更专用的访问方法之一
            /// </summary>
            /// <param name="exp">表达式</param>
            /// <returns></returns>
            public Expression Replace(Expression exp)
            {
                return this.Visit(exp);
            }

            /// <summary>
            /// 获取表达式的参数
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            protected override Expression VisitParameter(ParameterExpression p)
            {
                return this.ParameterExpression;
            }
        }
    }
}
