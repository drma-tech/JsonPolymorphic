using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JsonPolymorphic
{
    public class ItemTypeConfigurations : IEntityTypeConfiguration<ItemType>
    {
        public void Configure(EntityTypeBuilder<ItemType> entity)
        {
            entity.HasKey(e => e.ItemTypeId);
            entity.Property(e => e.Description).HasMaxLength(50);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.CustomerId, e.Description }).IsUnique();

            entity.HasDiscriminator(e => e.Role)
                .HasValue<ItemTypeItem>(CompositionRole.Item)
                .HasValue<ItemTypeComponent>(CompositionRole.Component);

            entity.Property(e => e.Role).HasDefaultValue(CompositionRole.Item);
        }
    }
}