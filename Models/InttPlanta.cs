using Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class InttPlanta : Record
    {
        [SwaggerIgnore]
        public Int32 Total { get; set; } = 0;

        // Campos de la vista 
        public string? VTIPO_MOV { get; set; } = string.Empty;
        public string? VRECORD_NUMBER { get; set; } = string.Empty;
        public string? VUPDATE_NUMBER { get; set; } = string.Empty;
        public string? VBRAND { get; set; } = string.Empty;
        public string? VSERIE { get; set; } = string.Empty;
        public string? VMODEL { get; set; } = string.Empty;
        public string? VMODELYEAR { get; set; } = string.Empty;
        public string? VVIN { get; set; } = string.Empty;
        public string? VSERIAL { get; set; } = string.Empty;
        public string? VPLATE { get; set; } = string.Empty;
        public string? VCOLOR1 { get; set; } = string.Empty;
        public string? VCOLOR2 { get; set; } = string.Empty;
        public string? VWEIGHT { get; set; } = string.Empty;
        public string? VCAPACITYTYPE { get; set; } = string.Empty;
        public string? VCAPACITY { get; set; } = string.Empty;
        public string? VAXLENUMBER { get; set; } = string.Empty;
        public string? VWHEELDIAMETER { get; set; } = string.Empty;
        public string? VCLASS { get; set; } = string.Empty;
        public string? VTYPE { get; set; } = string.Empty;
        public string? VUSE { get; set; } = string.Empty;
        public string? VCERTIFICATEDATE { get; set; } = string.Empty;
        public string? VRIF { get; set; } = string.Empty;
        public string? VPORT { get; set; } = string.Empty;
        public string? VFILENUMBER { get; set; } = string.Empty;
        public string? DFILEDATE { get; set; } = string.Empty;
        public string? VINVOICENUMBER { get; set; } = string.Empty;
        public string? DINVOICEDATE { get; set; } = string.Empty;
        public string? VCERTIFICATENUMBER { get; set; } = string.Empty;
        public string? VMANUFACTUREYEAR { get; set; } = string.Empty;
        public string? VSERIALVIN { get; set; } = string.Empty;
        public string? VSERIALCHASIS { get; set; } = string.Empty;
        public string? VSELLINVOICENUMBER { get; set; } = string.Empty;
        public string? VSELLINVOICEDATE { get; set; } = string.Empty;
        public string? VHOMONUMBER { get; set; } = string.Empty;
        public string? VHOMODATE { get; set; } = string.Empty;
        public string? VSERVICE { get; set; } = string.Empty;
        public string? VSEATSNUMBER { get; set; } = string.Empty;
        public string? VRAFANUMBER { get; set; } = string.Empty;
        public string? VRAFADATE { get; set; } = string.Empty;
        public string? VRAFASEC { get; set; } = string.Empty;
        public string? VSERIALCARRO { get; set; } = string.Empty;
        public string? VFUELTYPE { get; set; } = string.Empty;

        // Campo del correlativo
        public string? VNUMBERPLANTATXT { get; set; } = string.Empty;

        // Modo TXT (exportación)
        public string? Datos { get; set; } = string.Empty;
    }
}