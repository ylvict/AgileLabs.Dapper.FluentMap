using Dapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AgileLabs.Dapper.FluentMap
{
    public static class EntityBuilder<TEntity>
    {
        public static WhereDefinitionBuilder<TEntity> Where { get; }
        public static SortDefinitionBuilder<TEntity> Sort { get; }

        static EntityBuilder()
        {
            Where = new WhereDefinitionBuilder<TEntity>();
            Sort = new SortDefinitionBuilder<TEntity>();
        }
    }

    public sealed class WhereDefinitionBuilder<TEntity>
    {
        public WhereDefinition<TEntity> Expression(Expression<Func<TEntity, bool>> predicate)
        {
            return new WhereDefinition<TEntity>(predicate);
        }

        //public ICondition And(ICondition expressionCondition, ICondition expressionCondition2)
        //{
        //    return new WhereDefinition<TEntity>(expressionCondition, WhereOprand.And, expressionCondition2).Condition;
        //}

        //public ICondition Or(ICondition expressionCondition, ICondition expressionCondition2)
        //{
        //    return new WhereDefinition<TEntity>(expressionCondition, WhereOprand.Or, expressionCondition2).Condition;
        //}
    }

    //public enum WhereOprand
    //{
    //    And,
    //    Or
    //}

    public class WhereDefinition<TEntity>
    {

        public static WhereDefinition<TEntity> Empty
        {
            get
            {
                return new WhereDefinition<TEntity>();
            }
        }

        public ICondition Condition { get; private set; }

        public WhereDefinition()
        {

        }

        public WhereDefinition(Expression<Func<TEntity, bool>> expression) : this()
        {
            this.Condition = new ExpressionCondition<TEntity>(expression);
        }

        //public WhereDefinition(ICondition left, WhereOprand oprand, ICondition right) : this()
        //{
        //    //var newCondition = new CombinCondition<TEntity>(left.Condition, oprand, right.Condition);
        //    //this.Condition = Condition == null ? newCondition : new CombinCondition<TEntity>(Condition, WhereOprand.And, newCondition);
        //    this.Condition = new CombinCondition<TEntity>(left, oprand, right);
        //}

        public string ToSql(out DynamicParameters parameters)
        {
            return new EntityExpression<TEntity>().Where(this.Condition).ToSql(out parameters);
        }
    }

    public interface ICondition
    {

    }

    public class ExpressionCondition<TEntity> : ICondition
    {
        public Expression<Func<TEntity, bool>> Expression { get; private set; }

        public ExpressionCondition(Expression<Func<TEntity, bool>> expression)
        {
            this.Expression = expression;
        }
    }

    //public class CombinCondition<TEntity> : ICondition
    //{
    //    public ICondition Left { get; set; }
    //    public WhereOprand Oprand { get; set; }
    //    public ICondition Right { get; set; }

    //    public CombinCondition(ICondition left, WhereOprand oprand, ICondition right)
    //    {
    //        this.Left = left;
    //        this.Oprand = oprand;
    //        this.Right = right;
    //    }
    //}
}
