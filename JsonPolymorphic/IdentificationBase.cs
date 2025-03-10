using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JsonPolymorphic
{
    public abstract class IdentificationBase<TId>
    {
        [JsonIgnore]
        [NotMapped]
        public abstract TId IdentificationId { get; }

        [JsonIgnore]
        [NotMapped]
        public abstract string? IdentificationName { get; }

        [JsonIgnore]
        [NotMapped]
        public virtual string? IdentificationGroup { get; }

        /// <summary>
        /// Mark as false if you want to hide in scenarios such as editing record.
        /// Note: Inactive items should be loaded into the combo normally, as they will be filtered automatically.
        /// For instance: if "AlwaysAvailable = s.Active", then doesnt apply filter by active
        /// </summary>
        [NotMapped]
        public virtual bool AlwaysAvailable { get; set; } = true;
    }
}