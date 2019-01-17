using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure.Mapping.Conventions;
using SharpArch.Domain.DomainModel;
using SharpArch.NHibernate.FluentNHibernate;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    /// <summary>
    /// Generates the automapping for the domain assembly
    /// </summary>
    public class AutoPersistenceModelGenerator : IAutoPersistenceModelGenerator
    {
        public AutoPersistenceModel Generate()
        {
            var mappings = AutoMap.AssemblyOf<User>(new AutomappingConfiguration());
            mappings.IgnoreBase<Entity>();
            mappings.IgnoreBase(typeof(EntityWithTypedId<>));
            mappings.Conventions.Setup(GetConventions());
            mappings.UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();

            return mappings;
        }

        private static Action<IConventionFinder> GetConventions()
        {
            return c =>
                   {
                       c.Add<PrimaryKeyConvention>();
                       c.Add<CustomForeignKeyConvention>();
                       c.Add<HasManyConvention>();
                       c.Add<TableNameConvention>();
                       c.Add<EnumConvention>();
                       c.Add<GeographyConvention>();
                       c.Add<StringColumnLengthConvention>();
                       c.Add<BinaryColumnLengthConvention>();
                   };
        }
    }
}