using AgileLabs.Dapper.FluentMap.Resolver;
using Dapper.FluentMap.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileLabs.Dapper.FluentMap
{
   /// <summary>
   /// Defines methods for configured Dapper.FluentMap.Dommel.
   /// </summary>
    public static class AgileMapConfigurationExtensions
    {
        /// <summary>
        /// Configures the specified configuration for Dapper.FluentMap.Dommel.
        /// </summary>
        /// <param name="config">The Dapper.FluentMap configuration.</param>
        /// <returns>The Dapper.FluentMap configuration.</returns>
        public static FluentMapConfiguration ForAgileMap(this FluentMapConfiguration config)
        {
            AgileMapper.SetColumnNameResolver(new AgileColumnNameResolver());
            AgileMapper.SetKeyPropertyResolver(new AgileKeyPropertyResolver());
            AgileMapper.SetTableNameResolver(new AgileTableNameResolver());
            AgileMapper.SetPropertyResolver(new AgilePropertyResolver());
            return config;
        }
    }
}
