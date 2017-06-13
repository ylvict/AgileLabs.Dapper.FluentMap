using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AgileLabs.Dapper.FluentMap
{
    public class SortDefinition<TEntity>
    {
        public static SortDefinition<TEntity> Empty
        {
            get
            {
                return new SortDefinition<TEntity>();
            }
        }

        public List<Tuple<Expression<Func<TEntity, object>>, bool>> SortFilds { get; private set; }

        public SortDefinition()
        {
            SortFilds = new List<Tuple<Expression<Func<TEntity, object>>, bool>>();
        }

        public SortDefinition(Expression<Func<TEntity, object>> field, bool useAsc) : this()
        {
            SortFilds.Add(new Tuple<Expression<Func<TEntity, object>>, bool>(field, useAsc));
        }

        public string ToSql()
        {
            var exp = new EntityExpression<TEntity>().BuildOrderSql(SortFilds);

            return exp;
        }

        public SortDefinition<TEntity> AddRange<TEntity>(SortDefinition<TEntity>[] sorts)
        {
            var first = sorts.First();
            foreach (var defination in sorts.Skip(1))
            {
                first.SortFilds.AddRange(defination.SortFilds);
            }
            return first;
        }
    }
}
