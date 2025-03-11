using ppe.shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JsonPolymorphic;

[Table(nameof(Item))]
public partial class Item
{
    [Key]
    public int ItemId { get; set; }

    public int ItemModelId { get; set; }

    public string? SerialNumber { get; set; }

    public virtual ItemModel? ItemModel { get; set; }
}