using JsonPolymorphic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ppe.shared.Models;

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