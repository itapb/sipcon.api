using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DeliveredGuides
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        public string Number { get; set; }
    }
}
