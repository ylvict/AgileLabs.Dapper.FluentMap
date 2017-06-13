using AgileLabs.Dapper.FluentMap.Mapping;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using System;

namespace AgileLabs.Dapper.FluentMap.Resolver
{
    /// <summary>
    /// Implements the <see cref="DommelMapper.ITableNameResolver"/> interface by using the configured mapping.
    /// </summary>
    public class AgileTableNameResolver : AgileMapper.ITableNameResolver
    {
        /// <inheritdoc />
        public string ResolveTableName(Type type)
        {
            IEntityMap entityMap;
            if (FluentMapper.EntityMaps.TryGetValue(type, out entityMap))
            {
                var mapping = entityMap as IAgileEntityMap;

                if (mapping != null)
                {
                    return mapping.TableName;
                }
            }

            return AgileMapper.Resolvers.Default.TableNameResolver.ResolveTableName(type);
        }
    }
}
