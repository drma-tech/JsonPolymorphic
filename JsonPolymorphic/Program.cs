using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

var services = new ServiceCollection()
          .AddDbContext<MyDbContext>()
          .BuildServiceProvider();

using var dbContext = services.GetRequiredService<MyDbContext>();

var type = new ItemTypeItem { Description = "type 01", Role = CompositionRole.Item };
var model = new ItemModelItem { ShortDescription = "model 01", ItemType = type, Role = CompositionRole.Item };
var item = new Item { SerialNumber = "12345", ItemModel = model };

model.ItemType = type;
item.ItemModel = model;

dbContext.Items.Add(item);
await dbContext.SaveChangesAsync();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("try 1 (preserve -> works) or 2 (IgnoreCycles -> don't work)");

var dbItem = dbContext.Items.First();

var preserveOptions = new JsonSerializerOptions
{
    ReferenceHandler = ReferenceHandler.Preserve,
    AllowOutOfOrderMetadataProperties = true,
    WriteIndented = true
};

var cycleOptions = new JsonSerializerOptions
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    AllowOutOfOrderMetadataProperties = true,
    WriteIndented = true
};

var options = Console.ReadLine();

if (options == "1")
{
    var json = JsonSerializer.Serialize(dbItem, preserveOptions);
    var newItem = JsonSerializer.Deserialize<Item>(json, preserveOptions);
    Console.WriteLine(newItem?.SerialNumber);
    return;
}
else if (options == "2")
{
    var json = JsonSerializer.Serialize(dbItem, cycleOptions);
    var newItem = JsonSerializer.Deserialize<Item>(json, cycleOptions);
    Console.WriteLine(newItem?.SerialNumber);
    return;
}

public class MyDbContext() : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseInMemoryDatabase("TestDB");
    }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemConfigurations).Assembly); //get all configurations from the assembly
    }
}

[Table(nameof(Item))]
public partial class Item
{
    [Key]
    public int ItemId { get; set; }

    public int ItemModelId { get; set; }

    public string? SerialNumber { get; set; }

    public virtual ItemModel? ItemModel { get; set; }
}

public class ItemConfigurations : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> entity)
    {
        entity.HasKey(e => e.ItemId);

        entity.HasOne(d => d.ItemModel).WithMany(p => p.Items).HasForeignKey(d => d.ItemModelId).OnDelete(DeleteBehavior.NoAction);
    }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Role))]
[JsonDerivedType(typeof(ItemModelItem), (int)CompositionRole.Item)]
[JsonDerivedType(typeof(ItemModelComponent), (int)CompositionRole.Component)]
[Table(nameof(ItemModel))]
public abstract class ItemModel
{
    [Key]
    public int ItemModelId { get; set; }

    public int ItemTypeId { get; set; }

    public string? ShortDescription { get; set; }

    public virtual ItemType? ItemType { get; set; }

    [JsonIgnore]
    [Display(Name = "Role")]
    public CompositionRole Role { get; set; }

    [Display(Name = "Items")]
    public virtual ICollection<Item> Items { get; set; } = [];
}

public partial class ItemModelItem : ItemModel
{
}

public partial class ItemModelComponent : ItemModel
{
}

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

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Role))]
[JsonDerivedType(typeof(ItemTypeItem), (int)CompositionRole.Item)]
[JsonDerivedType(typeof(ItemTypeComponent), (int)CompositionRole.Component)]
[Table(nameof(ItemType))]
public abstract class ItemType
{
    [Key]
    public int ItemTypeId { get; set; }

    public string? Description { get; set; }

    [JsonIgnore]
    [Display(Name = "Role")]
    public CompositionRole Role { get; set; }

    public virtual ICollection<ItemModel> ItemModels { get; set; } = [];
}

public partial class ItemTypeItem : ItemType
{
}

public partial class ItemTypeComponent : ItemType
{
}

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

public enum CompositionRole
{
    Item = 1,

    Component = 2
}