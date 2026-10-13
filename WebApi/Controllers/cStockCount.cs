using ClosedXML.Excel;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApi.Controllers
{
    [Route("api/StockCount")]
    [ApiController]
    [Authorize]
    public class cStockCount : ControllerBase
    {
        private readonly dStockCount _dStockCount;

        public cStockCount(dStockCount dStockCount)
        {
            _dStockCount = dStockCount;
        }

        // Traer tipos de inventario disponibles (FÍSICO / CÍCLICO)
        [HttpGet("GetInventoryTypes")]
        public async Task<IActionResult> GetInventoryTypes(
            [FromQuery] int userId,
            [FromQuery] int supplierId)
        {
            try
            {
                if (userId <= 0 || supplierId <= 0)
                    return BadRequest(new { error = "userId y supplierId son obligatorios" });

                var _response = await _dStockCount.GetInventoryTypes(userId, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        // Traer detalles de inventario
        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(
            [FromQuery] int userId,
            [FromQuery] int supplierId,
            [FromQuery] int inventoryId)
        {
            try
            {
                if (userId <= 0 || supplierId <= 0 || inventoryId <= 0)
                    return BadRequest(new { error = "userId, supplierId e inventoryId son obligatorios" });

                var _response = await _dStockCount.GetDetails(userId, supplierId, inventoryId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }


        // GET: Validar ubicación
        [HttpGet("ValidateLocation")]
        public async Task<IActionResult> ValidateLocation(
        [FromQuery] int userId,
        [FromQuery] int supplierId,
        [FromQuery] int inventoryId,
        [FromQuery] string locationCode,
        [FromQuery] bool isUnassigned = false)
        {
            try
            {
                if (userId <= 0 || supplierId <= 0 || inventoryId <= 0)
                    return BadRequest(new { error = "userId, supplierId e inventoryId son obligatorios" });

                if (string.IsNullOrWhiteSpace(locationCode))
                    return BadRequest(new { error = "locationCode es obligatorio" });

                var _response = await _dStockCount.ValidateLocation(userId, supplierId, inventoryId, locationCode, isUnassigned);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }

        // GET: Validar repuesto
        [HttpGet("ValidatePart")]
        public async Task<IActionResult> ValidatePart(
            [FromQuery] int userId,
            [FromQuery] int supplierId,
            [FromQuery] int locationId,
            [FromQuery] string partCode)
        {
            try
            {
                if (userId <= 0 || supplierId <= 0 || locationId <= 0)
                    return BadRequest(new { error = "userId, supplierId y locationId son obligatorios" });

                if (string.IsNullOrWhiteSpace(partCode))
                    return BadRequest(new { error = "partCode es obligatorio" });

                var _response = await _dStockCount.ValidatePart(userId, supplierId, locationId, partCode);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }

        // POST: Guardar conteo (normal o botón +)
        [HttpPost("PostStockCount")]
        public async Task<IActionResult> PostStockCount(
            [FromQuery] int userId,
            [FromQuery] int supplierId,
            [FromBody] StockCountDetail data)
        {
            try
            {
                if (userId <= 0 || supplierId <= 0)
                    return BadRequest(new { error = "userId y supplierId son obligatorios" });

                if (data == null)
                    return BadRequest(new { error = "El cuerpo de la solicitud es obligatorio" });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var _response = await _dStockCount.PostStockCount(userId, supplierId, data);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { error = ex.Message });
            }
        }

        // POST: Importar conteo desde Excel
        [HttpPost("ImportStockCount")]
        public async Task<IActionResult> ImportStockCount(IFormFile file, Int32 userId, Int32 supplierId, Int32 inventoryId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                List<ImportStockCountRow> _rows = await ExceltoImportStockCount(file);

                if (_rows == null || !_rows.Any())
                    return BadRequest("El archivo no contiene filas válidas");

                string jsonData = Util.Json.ConvertToJsonString(_rows);

                var _response = await _dStockCount.ImportStockCount(userId, supplierId, inventoryId, jsonData);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        private async Task<List<ImportStockCountRow>> ExceltoImportStockCount(IFormFile file)
        {
            var _list = new List<ImportStockCountRow>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        try
                        {
                            string locationCode = row.Cell(1).GetValue<string>()?.Trim();
                            string partCode = row.Cell(2).GetValue<string>()?.Trim();
                            string quantityStr = row.Cell(4).GetString()?.Trim();

                            if (string.IsNullOrWhiteSpace(locationCode) &&
                                string.IsNullOrWhiteSpace(partCode) &&
                                string.IsNullOrWhiteSpace(quantityStr))
                                continue;

                            decimal quantity = 0;
                            if (!string.IsNullOrWhiteSpace(quantityStr))
                            {
                                if (!decimal.TryParse(quantityStr, out quantity))
                                {
                                    quantity = -1;
                                }
                            }

                            _list.Add(new ImportStockCountRow
                            {
                                LocationCode = locationCode ?? string.Empty,
                                PartCode = partCode ?? string.Empty,
                                Quantity = quantity
                            });
                        }
                        catch
                        {
                            _list.Add(new ImportStockCountRow
                            {
                                LocationCode = string.Empty,
                                PartCode = string.Empty,
                                Quantity = -1
                            });
                        }
                    }
                }
            }

            return _list;
        }


    }
}