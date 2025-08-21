using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PartRequest
    {
        [Required]
        public Part Part { get; set; } = new Part();

        [Required]
        public List<RelatedModel> Models { get; set; } = new List<RelatedModel>();
    }
}
