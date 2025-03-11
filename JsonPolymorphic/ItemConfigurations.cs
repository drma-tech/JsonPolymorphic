using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JsonPolymorphic
{
    public class ItemConfigurations : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> entity)
        {
            entity.HasKey(e => e.ItemId);

            entity.HasOne(d => d.ItemModel).WithMany(p => p.Items).HasForeignKey(d => d.ItemModelId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}