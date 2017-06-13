﻿using AgileLabs.Dapper.FluentMap.Mapping;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;
using System;
using System.Linq;
using System.Reflection;

namespace AgileLabs.Dapper.FluentMap.Resolver
{
    /// <summary>
    /// Implements the <see cref="DommelMapper.IKeyPropertyResolver"/> interface by using the configured mapping.
    /// </summary>
    public class AgileKeyPropertyResolver : AgileMapper.IKeyPropertyResolver
    {
        /// <inheritdoc/>
        public PropertyInfo ResolveKeyProperty(Type type)
        {
            bool isIdentity;
            return ResolveKeyProperty(type, out isIdentity);
        }

        /// <inheritdoc/>
        public PropertyInfo ResolveKeyProperty(Type type, out bool isIdentity)
        {
            IEntityMap entityMap;
            if (!FluentMapper.EntityMaps.TryGetValue(type, out entityMap))
            {
                return AgileMapper.Resolvers.Default.KeyPropertyResolver.ResolveKeyProperty(type, out isIdentity);
            }

            var mapping = entityMap as IAgileEntityMap;
            if (mapping != null)
            {
                var keyPropertyMaps = entityMap.PropertyMaps.OfType<AgilePropertyMap>().Where(e => e.Key).ToList();

                if (keyPropertyMaps.Count == 1)
                {
                    var keyPropertyMap = keyPropertyMaps[0];
                    isIdentity = keyPropertyMap.Identity;
                    return keyPropertyMap.PropertyInfo;
                }

                if (keyPropertyMaps.Count > 1)
                {
                    var msg = string.Format("Found multiple key properties on type '{0}'. This is not yet supported. The following key properties were found:{1}{2}",
                                            type.FullName,
                                            Environment.NewLine,
                                            string.Join(Environment.NewLine, keyPropertyMaps.Select(t => t.PropertyInfo.Name)));

                    throw new Exception(msg);
                }
            }

            // Fall back to the default mapping strategy.
            return AgileMapper.Resolvers.Default.KeyPropertyResolver.ResolveKeyProperty(type, out isIdentity);
        }
    }
}
