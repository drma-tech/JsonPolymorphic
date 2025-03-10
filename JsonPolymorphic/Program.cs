using JsonPolymorphic;
using Microsoft.Extensions.DependencyInjection;
using ppe.shared.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

var services = new ServiceCollection()
          .AddDbContext<MyDbContext>()
          .BuildServiceProvider();

using var dbContext = services.GetRequiredService<MyDbContext>();

var type = new ItemTypeItem { Description = "type 01", Role = CompositionRole.Item };
var model = new ItemModelItem { ShortDescription = "model 01", Description = "model 01", ItemType = type, Role = CompositionRole.Item };
var item = new Item { SerialNumber = "12345", ItemModel = model };

model.ItemType = type;
item.ItemModel = model;

dbContext.Items.Add(item);
await dbContext.SaveChangesAsync();

// See https://aka.ms/new-console-template for more information
Console.WriteLine("try 1 (preserve -> works) or 2 (IgnoreCycles -> dont work)");

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