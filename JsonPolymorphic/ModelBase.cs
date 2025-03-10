using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JsonPolymorphic
{
    public abstract class ModelBase : IdentificationBase<int>
    {
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        public abstract bool IsNew { get; }

        [NotMapped]
        [Display(Name = "#")]
        public int Actions { get; set; }
    }
}