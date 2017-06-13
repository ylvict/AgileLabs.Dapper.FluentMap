using Dapper.FluentMap.Mapping;
using System.Reflection;

namespace AgileLabs.Dapper.FluentMap.Mapping
{
    /// <summary>
    /// Represents mapping of a property for Dommel.
    /// </summary>
    public class AgilePropertyMap : PropertyMapBase<AgilePropertyMap>, IPropertyMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgilePropertyMap"/> class
        /// with the specified <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="info">The information about the property.</param>
        public AgilePropertyMap(PropertyInfo info) : base(info)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this property is a primary key.
        /// </summary>
        public bool Key { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this primary key is an identity.
        /// </summary>
        public bool Identity { get; set; }

        /// <summary>
        /// Specifies the current property as key for the entity.
        /// </summary>
        /// <returns>The current instance of <see cref="AgilePropertyMap"/>.</returns>
        public AgilePropertyMap IsKey()
        {
            Key = true;
            return this;
        }

        /// <summary>
        /// Specifies the current property as an identity.
        /// </summary>
        /// <returns>The current instance of <see cref="AgilePropertyMap"/>.</returns>
        public AgilePropertyMap IsIdentity()
        {
            Identity = true;
            return this;
        }
    }
}