using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Models
{
    public class FIGO_Report
    {
        public int Id { get; set; }
        public string? NameReport { get; set; }
        public int AccessGroupId { get; set; }
        public Boolean IsPdfReport { get; set; }
    }

    public class FIGO_Filters
    {
        public int Id { get; set; }
        public string? Field { get; set; }
        public string? FieldType { get; set; }
        public string? ActionType { get; set; }
        public int ReportFigoId { get; set; }
    }

    public class FIGO_Query
    {
        public int Id { get; set; }
        public string? Query { get; set; }
        public string? Type { get; set; }
    }

    public class FIGO_Options
    {
        public int ReportFigoId { get; set; }
        public int FilterReportId { get; set; }
        public int FilterOptionId { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
    }

    public class FIGO_MastersID
    {
        public string? Id { get; set; }
    }

    public class SalesFigo
    {
        public int? Id { get; set; } = 0;
        public string? invoiceNumber { get; set; }
        public DateTime dInvoiceDate { get; set; }
        public string? vVAT { get; set; }
        public string? vInnerCode { get; set; }
        public int iQuantity { get; set; }

        public int idSupplier { get; set; }
    }

    public class FigoTransitRepuestos
    {
        public string? CodigoRepuesto { get; set; }
        public int CantidadTransito { get; set; }
    }

    public class FIGO_Parts
    {
        public int Id { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Stock { get; set; }
        public string? Um { get; set; }
    }

    public class FIGO_MasterSales
    {
        public string? Id { get; set; }
        public string CompanyId { get; set; }
        public string? CompanyTaxId { get; set; }
        public string? CompanyName { get; set; }
        public string? DocumentType { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? NoteNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? ClientTaxId { get; set; }
        public string? ClientName { get; set; }
        public string? ProductName { get; set; }
        public string? ProductId { get; set; }
        public string? Year { get; set; }
        public string? Vin { get; set; }
        public string? EngineNumber { get; set; }
        public string? LicensePlate { get; set; }
        public string? Color { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? PlatePrice { get; set; }
        public decimal? UnitPlatePrice { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? TotalSales { get; set; }
        public decimal? Cost { get; set; }
        public decimal? ExchangeRate { get; set; }
    }

    public class FIGO_ReportCxC
    {
        [Column("ID_REGISTRO")]
        public long Id { get; set; }

        [Column("EMPRESA")]
        public string? DealerName { get; set; }

        [Column("RIF")]
        public string? Vat { get; set; }

        [Column("ORGANIZACION")]
        public string? Client { get; set; }

        [Column("TELEFONO")]
        public string? Phone { get; set; }

        [Column("ZONA")]
        public string? Zone { get; set; }

        [Column("OCURRENCIA")]
        public string? Occurrence { get; set; }

        [Column("DOCUMENTO")]
        public string? Document { get; set; }

        [Column("EMISION")]
        public string? IssueDate { get; set; }

        [Column("VENCIMIENTO")]
        public string? DueDate { get; set; }

        [Column("DIAS_VENCIDOS")]
        public int OverdueDays { get; set; }

        [Column("MONEDA")]
        public string? Currency { get; set; }

        [Column("MONEDA_DOCUMENTO")]
        public string? DocumentCurrency { get; set; }

        [Column("VENCIDO")]
        public decimal OverdueAmount { get; set; }

        [Column("POR_VENCER")]
        public decimal CurrentAmount { get; set; }

        [Column("TOTAL_DEUDA")]
        public decimal TotalDebt { get; set; }

        [Column("TASA")]
        public decimal ExchangeRate { get; set; }

        [Column("PRODUCTO")]
        public string? Product { get; set; }

        [Column("SERIAL")]
        public string? SerialNumber { get; set; }

        [Column("GRUPO_PRODUCTO")]
        public string? ProductGroup { get; set; }

        [Column("FECHA_ORDEN_DATE")]
        public string? OrdenDate { get; set; }
    }

    public class FIGO_PartsStock
    {
        [Column("VSUPPLIER")]
        public string? Supplier { get; set; }

        [Column("VINVENTORY")]
        public string? Inventory { get; set; }

        [Column("IDPRODUCT")]
        public string? ProductId { get; set; }

        [Column("VPRODUCT")]
        public string? Product { get; set; }

        [Column("IQTY")]
        public int Qty { get; set; }
    }

    public class FIGO_ModelFeatures
    {
        public int? SupplierId { get; set; }
        public string? ModelCode { get; set; }
        public string? ModelName { get; set; }
        public decimal? ModelWeight { get; set; }
        public int? Capacity { get; set; }
        public int? AxleNumber { get; set; }
        public int? WheeleDiameter { get; set; }
        public string? ModelClass { get; set; }
        public string? ModelType { get; set; }
        public string? ModelUse { get; set; }
        public int? SeatsNumber { get; set; }
        public string? FuelType { get; set; }
    }

    public class FIGO_VehicleFileIntt
    {
        public int? SupplierId { get; set; }
        public string? Vin { get; set; }
        public string? FileNumber { get; set; }
        public DateTime? FileDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? DuaNumber { get; set; }
        public DateTime? DuaDate { get; set; }
        public int? ModelYear { get; set; }
        public int? ManufactureYear { get; set; }
    }
    public class FIGO_InventoryParts
    {
        public string? Supplier { get; set; }
        public string? Inventory { get; set; }
        public string? ProductId { get; set; }
        public string Product { get; set; }
        public int Qty { get; set; }
    }
}
