using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    // PARA GUARDAR CONTEO
    public class StockCountDetail
    { 
        [Required]
        public int? IdInventory { get; set; }

        [Required]
        public string? LocationCode { get; set; }

        [Required]
        public string? PartCode { get; set; }

        [Required]
        public int? Quantity { get; set; }
    }

    // MODELO PARA BOTÓN
    public class UnassignedPartRequest
    {
        [Required]
        public int? IdInventory { get; set; }

        [Required]
        public string? LocationCode { get; set; }

        [Required]
        public string? PartCode { get; set; }

        [Required]
        public int? Quantity { get; set; }
    }

    // MODELO PARA RESPUESTA DEL POST
    public class StockCountResponse
    {
        public int Iid { get; set; }
        public int Iinserted { get; set; }
        public int Iupdated { get; set; }
        public string? Message { get; set; }
    }

    // MODELO PARA RESPUESTA DE CHECKACTIVE
    public class ActiveInventoryResponse
    {
        public bool HasActiveInventory { get; set; }
    }

    // MODELO PARA RESPUESTA DE GETTYPES
    public class InventoryTypeOption
    {
        public int IdCountType { get; set; }
        public string VName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string AssignmentType { get; set; } = string.Empty;
        public string AssignmentName { get; set; } = string.Empty;
        public int IdInventoryCount { get; set; }
        public int HasAssignment { get; set; }
        public int IdStatus { get; set; }        // 20 o 6
        public string VDescription { get; set; } = string.Empty;
        public int IdCount { get; set; }           // 0 o 1
    }

    // MODELO PARA VALIDACIÓN DE UBICACIÓN
    public class LocationValidationResponse
    {
        public int LocationId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string? Message { get; set; }
    }

    // MODELO PARA VALIDACIÓN DE REPUESTO
    public class PartValidationResponse
    {
        public int PartId { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public string PartDescription { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string? Message { get; set; }
    }

    // MODELO PARA DETALLES DEL INVENTARIO (GETDETAILS)
    public class InventoryDetailResponse
    {
        public int IdLocation { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int IdPart { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public string PartDescription { get; set; } = string.Empty;
        public int? IdStatus { get; set; }
        public int? IdCount { get; set; }

    }

    // MODELO DE RESPUESTA GENÉRICA
    public class ServiceResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}