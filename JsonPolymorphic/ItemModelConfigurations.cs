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

            entity.HasDiscriminator(e => e.Role)
                .HasValue<ItemModelItem>(CompositionRole.Item)
                .HasValue<ItemModelComponent>(CompositionRole.Component);

            entity.HasOne(d => d.ItemType).WithMany(p => p.ItemModels).HasForeignKey(d => d.ItemTypeId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}