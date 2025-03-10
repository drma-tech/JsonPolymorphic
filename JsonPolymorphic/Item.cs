using ppe.shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JsonPolymorphic;

[Table(nameof(Item))]
public partial class Item : ModelBase
{
    [Key]
    public int ItemId { get; set; }

    public int ItemModelId { get; set; }

    public string? SerialNumber { get; set; }

    public virtual ItemModel? ItemModel { get; set; }

    [JsonIgnore]
    public override bool IsNew => ItemId == 0;

    [JsonIgnore]
    public override int IdentificationId { get => ItemId; }

    [JsonIgnore]
    public override string? IdentificationName { get => SerialNumber; }
}