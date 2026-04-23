using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Rate  
    {
        [JsonPropertyName("fechaActualizacion")]
        public DateTime DDate { get; set; }

        [JsonPropertyName("promedio")]
        public decimal NRate { get; set; }

    }
}
