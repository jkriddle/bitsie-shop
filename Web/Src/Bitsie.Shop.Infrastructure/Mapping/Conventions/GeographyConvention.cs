using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using GeoAPI.Geometries;

namespace Bitsie.Shop.Infrastructure.Mapping.Conventions
{
    [Serializable]
    public class GeographyConvention : IPropertyConvention 
    {
        public void Apply(IPropertyInstance instance)
        {
            if (typeof(IGeometry).IsAssignableFrom(instance.Property.PropertyType))
            {
                instance.CustomType(typeof(GeographyType));
                instance.CustomSqlType("GEOGRAPHY");
            }
        }
    }
}
