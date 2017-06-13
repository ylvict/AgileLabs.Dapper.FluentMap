using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AgileLabs.Dapper.FluentMap
{
    public sealed class SortDefinitionBuilder<TEntity>
    {
        public SortDefinitionBuilder() { }
        public SortDefinition<TEntity> Ascending(Expression<Func<TEntity, object>> field)
        {
            return new SortDefinition<TEntity>(field, true);
        }
        public SortDefinition<TEntity> Combine(params SortDefinition<TEntity>[] sorts)
        {
            return SortDefinition<TEntity>.Empty.AddRange(sorts);
        }
        public SortDefinition<TEntity> Combine(IEnumerable<SortDefinition<TEntity>> sorts)
        {
            return SortDefinition<TEntity>.Empty;
        }
        public SortDefinition<TEntity> Descending(Expression<Func<TEntity, object>> field)
        {
            return new SortDefinition<TEntity>(field, false);
        }
    }
}
