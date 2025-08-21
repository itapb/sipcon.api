using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace Models
{
    public class Contact : Record
    {

        [SwaggerIgnore]
        public Int32 Total { get; set; } = 0;

        [Required]
        public string? Vat { get; set; } = string.Empty;

        [Required]
        public string? FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; } = string.Empty;

        [Required]
        public string? Address { get; set; } = string.Empty;

        [Required]
        public string? Phone1 { get; set; } = string.Empty;
        public string? Phone2 { get; set; } = string.Empty;
        public Int32? BrandId { get; set; } = 0;

        [SwaggerIgnore]
        public string? BrandName { get; set; } = string.Empty;

        [Required]
        public string? Email { get; set; } = string.Empty;
        [SwaggerIgnore]
        public string? Login { get; set; } = string.Empty;
        [SwaggerIgnore]
        public string? Password { get; set; } = string.Empty;
        public string? Reference { get; set; } = string.Empty;

        [Required]
        public bool? IsCustomer { get; set; } = false;

        [Required]
        public bool? IsSupplier { get; set; } = false;

        [Required]
        public bool? IsUser { get; set; } = false;

        [Required]
        public bool? IsDealer { get; set; } = false;

        [Required]
        public bool? IsProvider { get; set; } = false;

        [Required]
        public Int32? CityId { get; set; } = 0;

        [SwaggerIgnore]
        public string? CityName { get; set; } = string.Empty;

        [SwaggerIgnore]
        public string? State { get; set; } = string.Empty;

        [Required]
        public bool? Male { get; set; } = false;

        [Required]
        public DateTime? Birthday { get; set; } = DateTime.Now;

        [SwaggerIgnore]
        public string? Type { get; set; } = string.Empty;

        [Required]
        public Int32? PayMethodId { get; set; } = 0;

        [Required]
        public Int32? GroupId { get; set; } = 0;

        [Required]
        public Int32? AgentId { get; set; } = 0;

        
        [SwaggerIgnore]
        public string? AgentName { get; set; } = string.Empty;

        [Required]
        public bool? Blocked { get; set; } = false;

    }
}
