using MiCake.Cord.Storage;
using Microsoft.EntityFrameworkCore;

namespace MiCake.EntityFrameworkCore.StorageInterpretor.Strategy
{
    internal class PropertyConfigStrategy : IConfigModelBuilderStrategy
    {
        public ModelBuilder Config(ModelBuilder modelBuilder, IConventionStoreEntity storeEntity, Type efModelType)
        {
            var properties = storeEntity.GetProperties();

            if (!properties.Any())
                return modelBuilder;

            var entityBuilder = modelBuilder.Entity(efModelType);

            foreach (var property in properties)
            {
                if (property is not IConventionStoreProperty conventionProperty)
                {
                    continue;
                }

                var propertyName = property.Name;
                var efcoreProperty = entityBuilder.Property(propertyName);

                //consider splitting into multiple Strategy classes?
                if (conventionProperty.IsConcurrency.HasValue && conventionProperty.IsConcurrency.Value)
                    efcoreProperty.IsConcurrencyToken(true);

                if (conventionProperty.IsNullable.HasValue)
                    efcoreProperty.IsRequired(!conventionProperty.IsNullable.Value);

                if (conventionProperty.MaxLength.HasValue)
                    efcoreProperty.HasMaxLength(conventionProperty.MaxLength.Value);

                // config default value.
                if (conventionProperty.DefaultValue != null && conventionProperty.DefaultValue.HasValue)
                {
                    var defaultValueConfig = conventionProperty.DefaultValue.Value;

                    if (defaultValueConfig.ValueType == StorePropertyDefaultValueType.ClrValue)
                    {
                        efcoreProperty.HasDefaultValue(defaultValueConfig.DefaultValue);
                    }
                    else if (defaultValueConfig.ValueType == StorePropertyDefaultValueType.SqlValue)
                    {
                        efcoreProperty.HasDefaultValueSql(defaultValueConfig.DefaultValue as string);
                    }

                    if (defaultValueConfig.SetOpportunity == StorePropertyDefaultValueSetOpportunity.Add)
                    {
                        efcoreProperty.ValueGeneratedOnAdd();
                    }
                    else if (defaultValueConfig.SetOpportunity == StorePropertyDefaultValueSetOpportunity.Update)
                    {
                        efcoreProperty.ValueGeneratedOnUpdate();
                    }
                    else if (defaultValueConfig.SetOpportunity == StorePropertyDefaultValueSetOpportunity.AddAndUpdate)
                    {
                        efcoreProperty.ValueGeneratedOnAddOrUpdate();
                    }
                }
            }

            return modelBuilder;
        }
    }
}
