using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AgileLabs.Dapper.FluentMap.Mapping
{
    /// <summary>
    /// Represents a non-typed mapping of an entity for Dommel.
    /// </summary>
    public interface IAgileEntityMap : IEntityMap
    {
        /// <summary>
        /// Gets the table name for the current entity.
        /// </summary>
        string TableName { get; }
    }

    /// <summary>
    /// Represents the typed mapping of an entity for Dommel.
    /// </summary>
    /// <typeparam name="TEntity">The type of an entity.</typeparam>
    public abstract class AgileEntityMap<TEntity> : EntityMapBase<TEntity, AgilePropertyMap>, IAgileEntityMap
        where TEntity : class
    {
        /// <summary>
        /// Gets the <see cref="IPropertyMap"/> implementation for the current entity map.
        /// </summary>
        /// <param name="info">The information about the property.</param>
        /// <returns>An implementation of <see cref="Dapper.FluentMap.Mapping.IPropertyMap"/>.</returns>
        protected override AgilePropertyMap GetPropertyMap(PropertyInfo info)
        {
            return new AgilePropertyMap(info);
        }

        /// <summary>
        /// Gets the table name for this entity map.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Sets the table name for the current entity.
        /// </summary>
        /// <param name="tableName">The name of the table in the database.</param>
        protected void ToTable(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// Sets the table name for the current entity.
        /// </summary>
        /// <param name="tableName">The name of the table in the database.</param>
        /// <param name="schemaName">The name of the table schema in the database.</param>
        protected void ToTable(string tableName, string schemaName)
        {
            TableName = $"{schemaName}.{tableName}";
        }
    }
}
