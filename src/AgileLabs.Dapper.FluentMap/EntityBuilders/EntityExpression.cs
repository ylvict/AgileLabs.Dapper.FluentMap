using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static AgileLabs.Dapper.FluentMap.AgileMapper;

namespace AgileLabs.Dapper.FluentMap
{
    /// <summary>
    /// Represents a typed SQL expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class EntityExpression<TEntity>
    {
        private readonly StringBuilder _whereBuilder = new StringBuilder();
        private readonly DynamicParameters _parameters = new DynamicParameters();
        //private int _parameterIndex;

        /// <summary>
        /// Builds a SQL expression for the specified filter expression.
        /// </summary>
        /// <param name="expression">The filter expression on the entity.</param>
        /// <returns>The current <see cref="DommelMapper.SqlExpression&lt;TEntity&gt;"/> instance.</returns>
        public virtual EntityExpression<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            if (expression != null)
            {
                AppendToWhere("and", expression);
            }
            return this;
        }

        public virtual EntityExpression<TEntity> Where(ICondition condition, string oprand = "and")
        {
            if (condition != null)
            {
                switch (condition.GetType().Name)
                {
                    case "ExpressionCondition`1":
                        HandleCondition(oprand, (ExpressionCondition<TEntity>)condition);
                        break;
                    //case "CombinCondition`1":
                    //    HandleCondition(oprand, (CombinCondition<TEntity>)condition);
                    //    break;
                    default:
                        break;
                }

            }
            return this;
        }

        private void HandleCondition<TEntity>(string oprand, ExpressionCondition<TEntity> condition)
        {
            if (condition != null && condition.Expression != null)
            {
                AppendToWhere(oprand ?? "and", condition.Expression);
            }
        }
        //private void HandleCondition<TEntity>(string oprand = "and", CombinCondition<TEntity> condition)
        //{
        //    if (condition != null)
        //    {
        //        switch (condition.Oprand)
        //        {
        //            case WhereOprand.And:
        //                Where(condition.Left);
        //                Where(condition.Right);
        //                break;
        //            default:
        //                break;
        //        }

        //    }
        //}

        #region Order
        /// <summary>
        /// Builds a SQL expression for the specified filter expression.
        /// </summary>
        /// <param name="expression">The filter expression on the entity.</param>
        /// <param name="orderByAsc">Order by type.</param>
        /// <returns></returns>
        private string BuildOrderSqlInternal(Expression<Func<TEntity, object>> expression, bool orderByAsc)
        {
            var sql = "";
            if (expression == null)
            {
                return sql;
            }

            MemberExpression memberExpression = null;

            var predicateBodyNodeType = expression.Body.NodeType;
            if (predicateBodyNodeType == ExpressionType.Convert ||
                predicateBodyNodeType == ExpressionType.ConvertChecked)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                memberExpression = unaryExpression?.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression?.Expression == null)
            {
                return sql;
            }

            var property = new EntityExpression<TEntity>().VisitMemberAccess(memberExpression);
            //sql = $" ORDER BY {property} " + (orderByAsc ? "ASC" : "DESC");
            sql = $"{property} " + (orderByAsc ? "ASC" : "DESC");
            return sql;
        }

        public string BuildOrderSql(List<Tuple<Expression<Func<TEntity, object>>, bool>> list)
        {
            if (list == null)
            {
                return string.Empty;
            }

            var orderFields = new List<string>();
            foreach (var item in list)
            {
                orderFields.Add(BuildOrderSqlInternal(item.Item1, item.Item2));
            }
            return $" ORDER BY {string.Join(",", orderFields)}";
        }

        public string BuildOrderSql(Expression<Func<TEntity, object>> expression, bool orderByAsc)
        {
            if (expression == null)
            {
                return string.Empty;
            }

            var orderFields = new List<string>();
            orderFields.Add(BuildOrderSqlInternal(expression, orderByAsc));
            return $" ORDER BY {string.Join(",", orderFields)}";
        }
        #endregion

        private void AppendToWhere(string conditionOperator, Expression expression)
        {
            var sqlExpression = VisitExpression(expression).ToString();
            AppendToWhere(conditionOperator, sqlExpression);
        }

        private void AppendToWhere(string conditionOperator, string sqlExpression)
        {
            if (_whereBuilder.Length == 0)
            {
                _whereBuilder.Append(" where ");
            }
            else
            {
                _whereBuilder.AppendFormat(" {0} ", conditionOperator);
            }

            _whereBuilder.Append(sqlExpression);
        }

        /// <summary>
        /// Visits the expression.
        /// </summary>
        /// <param name="expression">The expression to visit.</param>
        /// <returns>The result of the visit.</returns>
        protected virtual object VisitExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return VisitLambda(expression as LambdaExpression);

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return VisitBinary((BinaryExpression)expression);

                case ExpressionType.Convert:
                case ExpressionType.Not:
                    return VisitUnary((UnaryExpression)expression);

                case ExpressionType.New:
                    return VisitNew((NewExpression)expression);

                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)expression);

                case ExpressionType.Constant:
                    return VisitConstantExpression((ConstantExpression)expression);
            }

            return expression;
        }

        /// <summary>
        /// Processes a lambda expression.
        /// </summary>
        /// <param name="epxression">The lambda expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual object VisitLambda(LambdaExpression epxression)
        {
            if (epxression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var member = epxression.Body as MemberExpression;
                if (member?.Expression != null)
                {
                    return $"{VisitMemberAccess(member)} = '1'";
                }
            }

            return VisitExpression(epxression.Body);
        }

        /// <summary>
        /// Processes a binary expression.
        /// </summary>
        /// <param name="expression">The binary expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual object VisitBinary(BinaryExpression expression)
        {
            object left, right;
            var operand = BindOperant(expression.NodeType);
            if (operand == "and" || operand == "or")
            {
                // Left side.
                var member = expression.Left as MemberExpression;
                if (member != null &&
                    member.Expression != null &&
                    member.Expression.NodeType == ExpressionType.Parameter)
                {
                    left = $"{VisitMemberAccess(member)} = '1'";
                }
                else
                {
                    left = VisitExpression(expression.Left);
                }

                // Right side.
                member = expression.Right as MemberExpression;
                if (member != null &&
                    member.Expression != null &&
                    member.Expression.NodeType == ExpressionType.Parameter)
                {
                    right = $"{VisitMemberAccess(member)} = '1'";
                }
                else
                {
                    right = VisitExpression(expression.Right);
                }
            }
            else
            {
                // It's a single expression.
                left = VisitExpression(expression.Left);
                right = VisitExpression(expression.Right);

                var paramName = left.ToString();// + _parameterIndex++;
                _parameters.Add(paramName, value: right);
                return $"{left} {operand} @{paramName}";
            }

            return $"{left} {operand} {right}";
        }

        /// <summary>
        /// Processes a unary expression.
        /// </summary>
        /// <param name="expression">The unary expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual object VisitUnary(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    var o = VisitExpression(expression.Operand);
                    if (!(o is string))
                    {
                        return !(bool)o;
                    }

                    var memberExpression = expression.Operand as MemberExpression;
                    if (memberExpression != null &&
                        Resolvers.Properties(memberExpression.Expression.Type).Any(p => p.Name == (string)o))
                    {
                        o = $"{o} = '1'";
                    }

                    return $"not ({o})";
                case ExpressionType.Convert:
                    if (expression.Method != null)
                    {
                        return Expression.Lambda(expression).Compile().DynamicInvoke();
                    }
                    break;
            }

            return VisitExpression(expression.Operand);
        }

        /// <summary>
        /// Processes a new expression.
        /// </summary>
        /// <param name="expression">The new expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual object VisitNew(NewExpression expression)
        {
            var member = Expression.Convert(expression, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            return getter();
        }

        /// <summary>
        /// Processes a member access expression.
        /// </summary>
        /// <param name="expression">The member access expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual object VisitMemberAccess(MemberExpression expression)
        {
            if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.Parameter)
            {
                return MemberToColumn(expression);
            }

            var member = Expression.Convert(expression, typeof(object));
            var lambda = Expression.Lambda<Func<object>>(member);
            var getter = lambda.Compile();
            return getter();
        }

        /// <summary>
        /// Processes a constant expression.
        /// </summary>
        /// <param name="expression">The constant expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual object VisitConstantExpression(ConstantExpression expression)
        {
            return expression.Value ?? "null";
        }

        /// <summary>
        /// Proccesses a member expression.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <returns>The result of the processing.</returns>
        protected virtual string MemberToColumn(MemberExpression expression)
        {
            return Resolvers.Column((PropertyInfo)expression.Member);
        }

        /// <summary>
        /// Returns the expression operand for the specified expression type.
        /// </summary>
        /// <param name="expressionType">The expression type for node of an expression tree.</param>
        /// <returns>The expression operand equivalent of the <paramref name="expressionType"/>.</returns>
        protected virtual string BindOperant(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.AndAlso:
                    return "and";
                case ExpressionType.OrElse:
                    return "or";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "MOD";
                case ExpressionType.Coalesce:
                    return "COALESCE";
                default:
                    return expressionType.ToString();
            }
        }

        /// <summary>
        /// Returns the current SQL query.
        /// </summary>
        /// <returns>The current SQL query.</returns>
        public string ToSql()
        {
            return _whereBuilder.ToString();
        }

        /// <summary>
        /// Returns the current SQL query.
        /// </summary>
        /// <param name="parameters">When this method returns, contains the parameters for the query.</param>
        /// <returns>The current SQL query.</returns>
        public string ToSql(out DynamicParameters parameters)
        {
            parameters = _parameters;
            return _whereBuilder.ToString();
        }

        /// <summary>
        /// Returns the current SQL query.
        /// </summary>
        /// <returns>The current SQL query.</returns>
        public override string ToString()
        {
            return _whereBuilder.ToString();
        }
    }
}
