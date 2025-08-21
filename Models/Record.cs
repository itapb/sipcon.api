using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;


namespace Models
{
    public class Record
    {

        [Required]
        public int? Id { get; set; } = 0;


        [Required]
        public bool? IsActive { get; set; } = true;


    }
}
