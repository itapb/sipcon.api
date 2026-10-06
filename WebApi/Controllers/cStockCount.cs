using Microsoft.AspNetCore.Mvc;
using Models;
using Data;

namespace WebApi.Controllers
{
    [Route("api/StockCount")]
    [ApiController]
    public class cStockCount : ControllerBase
    {
        private readonly dStockCount _dStockCount;

        public cStockCount(dStockCount dStockCount)
        {
            _dStockCount = dStockCount;
        }

        // Verificar si existe cabecera activa
        [HttpGet("CheckActiveInventory")]
        public async Task<IActionResult> CheckActiveInventory(
            int userId,
            int supplierId)
        {
            try
            {
                var _response = await _dStockCount.CheckActiveInventory(userId, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }


        // Traer tipos de inventario disponibles
        [HttpGet("GetInventoryTypes")]
        public async Task<IActionResult> GetInventoryTypes(
            int userId,
            int supplierId)
        {
            try
            {
                var _response = await _dStockCount.GetInventoryTypes(userId, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        // Traer detalles de inventario final Estatus 6
        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(
            int userId,
            int supplierId,
            int inventoryId)
        { 
            try
            {
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
            int userId,
            int supplierId,
            int inventoryId,
            string locationCode)
        {
            try
            {
                var _response = await _dStockCount.ValidateLocation(userId, supplierId, inventoryId, locationCode);


                return Ok(_response);
            }
            catch (Exception ex)
            {
                // Solo en caso de excepción real devolver error
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }

        // GET: Validar repuesto
        [HttpGet("ValidatePart")]
        public async Task<IActionResult> ValidatePart(
            int userId,
            int supplierId,
            int locationId,
            string partCode)
        {
            try
            {
                var _response = await _dStockCount.ValidatePart(userId, supplierId, locationId, partCode);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { error = ex.Message });
            }
        }

        [HttpPost("PostStockCount")]
        public async Task<IActionResult> PostStockCount(
           int userId,
           int supplierId,
           [FromBody] StockCountDetail data)
        {
            try
            {
                var _response = await _dStockCount.PostStockCount(userId, supplierId, data);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { error = ex.Message });
            }
        }

        // POST: Botón (+) para agregar repuesto no asignado en inventario final
        [HttpPost("AddUnassignedPart")]
        public async Task<IActionResult> AddUnassignedPart(
            int userId,
            int supplierId,
            [FromBody] UnassignedPartRequest data)
        {
            try
            {
                var _response = await _dStockCount.AddUnassignedPart(userId, supplierId, data);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { error = ex.Message });
            }
        }

       
    }
}