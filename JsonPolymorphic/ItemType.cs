using ppe.shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JsonPolymorphic;

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