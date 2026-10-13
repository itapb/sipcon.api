using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    // PARA GUARDAR CONTEO (normal o botón +)
    public class StockCountDetail
    {
        // Id = NULL o 0 → botón (+)
        // Id > 0 → conteo normal
        public int? Id { get; set; }

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

    // MODELO PARA RESPUESTA DE GETTYPES
    public class InventoryTypeOption
    {
        public string Tipo { get; set; } = string.Empty;           // 'FISICO' o 'CICLICO'
        public int IdInventoryCount { get; set; }
        public int IdCountType { get; set; }
        public int IdStatus { get; set; }
        public string VDescription { get; set; } = string.Empty;
        public int TieneAsignacion { get; set; }                    // 1 = tiene, 0 = no
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
        public int IdInventoryDetail { get; set; }
        public int IdLocation { get; set; }
        public int IdPart { get; set; }
        public int? IQuantityOld { get; set; }
        public int? IQuantityNew { get; set; }
        public string? IForm { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public string PartDescription { get; set; } = string.Empty;
        public int? IdAssign { get; set; }                          // Solo en preinventario
        public string? VAssignType { get; set; }                    // Solo en preinventario
        public string Modo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }

    // MODELO DE RESPUESTA GENÉRICA
    public class ServiceResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    // MODELO PARA FILA DE IMPORTACIÓN
    public class ImportStockCountRow
    {
        public string LocationCode { get; set; } = string.Empty;
        public string PartCode { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }


    // MODELO PARA RESULTADO DE IMPORTACIÓN
    public class ImportStockCountResult
    {
        public int? Fila { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}