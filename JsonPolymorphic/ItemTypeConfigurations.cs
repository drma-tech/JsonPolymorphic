using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JsonPolymorphic
{
    public class ItemTypeConfigurations : IEntityTypeConfiguration<ItemType>
    {
        public void Configure(EntityTypeBuilder<ItemType> entity)
        {
            entity.HasKey(e => e.ItemTypeId);

            entity.HasDiscriminator(e => e.Role)
                .HasValue<ItemTypeItem>(CompositionRole.Item)
                .HasValue<ItemTypeComponent>(CompositionRole.Component);
        }
    }
}