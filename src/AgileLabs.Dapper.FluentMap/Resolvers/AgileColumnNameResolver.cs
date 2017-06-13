using AgileLabs.Dapper.FluentMap.Mapping;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using System.Linq;
using System.Reflection;

namespace AgileLabs.Dapper.FluentMap.Resolver
{
    /// <summary>
    /// Implements the <see cref="AgileMapper.IColumnNameResolver"/> interface by using the configured mapping.
    /// </summary>
    public class AgileColumnNameResolver : AgileMapper.IColumnNameResolver
    {
        /// <inheritdoc/>
        public string ResolveColumnName(PropertyInfo propertyInfo)
        {
            if (propertyInfo.DeclaringType != null)
            {
                IEntityMap entityMap;
                if (FluentMapper.EntityMaps.TryGetValue(propertyInfo.DeclaringType, out entityMap))
                {
                    var mapping = entityMap as IAgileEntityMap;
                    if (mapping != null)
                    {
                        var propertyMaps = entityMap.PropertyMaps.Where(m => m.PropertyInfo.Name == propertyInfo.Name).ToList();
                        if (propertyMaps.Count == 1)
                        {
                            return propertyMaps[0].ColumnName;
                        }
                    }
                }
            }

            return AgileMapper.Resolvers.Default.ColumnNameResolver.ResolveColumnName(propertyInfo);
        }
    }
}
