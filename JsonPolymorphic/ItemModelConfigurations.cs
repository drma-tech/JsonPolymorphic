using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ppe.shared.Models;

namespace JsonPolymorphic
{
    public class ItemModelConfigurations : IEntityTypeConfiguration<ItemModel>
    {
        public void Configure(EntityTypeBuilder<ItemModel> entity)
        {
            entity.HasKey(e => e.ItemModelId);

            entity.Property(e => e.ShortDescription).HasMaxLength(40);
            entity.Property(e => e.Description).HasMaxLength(40);
            entity.Property(e => e.JsonData).HasMaxLength(4000);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.CustomerId, e.ShortDescription }).IsUnique();

            entity.HasDiscriminator(e => e.Role)
                .HasValue<ItemModelItem>(CompositionRole.Item)
                .HasValue<ItemModelComponent>(CompositionRole.Component);

            entity.Property(e => e.Role).HasDefaultValue(CompositionRole.Item);

            entity.HasOne(d => d.ItemType).WithMany(p => p.ItemModels).HasForeignKey(d => d.ItemTypeId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}