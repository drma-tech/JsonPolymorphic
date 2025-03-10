using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JsonPolymorphic
{
    public class ItemConfigurations : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> entity)
        {
            entity.HasKey(e => e.ItemId);

            entity.Property(e => e.SerialNumber).HasMaxLength(20);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => new { e.CustomerId, e.SerialNumber });

            entity.HasOne(d => d.ItemModel).WithMany(p => p.Items).HasForeignKey(d => d.ItemModelId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}