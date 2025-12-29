using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{
    [Route("api/Brand")]
    [ApiController]
    [Authorize]
    public class cReporting : ControllerBase
    {

        private readonly dReporting _dReporting;

        public cReporting(dReporting dReporting)
        {
            _dReporting = dReporting;

        }


        [HttpGet("GetAllJson")]
        public async Task<IActionResult> GetAllJson(string? filter, int? rowFrom, int userId, int? supplierId, int? dealerId, DateTime? fromDate, DateTime? upToDate, int? estatusId, int reportingId)
        {
            try
            {
                string json = await _dReporting.GetAllJson(
                    filter, rowFrom, userId, supplierId, dealerId, fromDate, upToDate, estatusId, reportingId
                );

                // Devuelves el JSON crudo
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
