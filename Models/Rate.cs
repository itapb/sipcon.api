using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Models
{
    public class Rate : Record
    {
        public DateTime DDate { get; set; }
        public decimal NRate { get; set; }

    }
}
