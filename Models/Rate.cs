using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Models
{
    public class Rate  
    {
        [Required]
        public int? Id { get; set; } = 0;
        [Required]
        public decimal? NRate { get; set; }
        [SwaggerIgnore]
        public DateTime? DDate { get; set; } 
    }
}
