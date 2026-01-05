using Data;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;


namespace WebApi.Controllers
{
    [Route("api/Reporting")]
    [ApiController]
    [Authorize]
    public class cReporting : ControllerBase
    {

        private readonly dReporting _dReporting;

        public cReporting(dReporting dReporting)
        {
            _dReporting = dReporting;

        }


        [HttpGet("GetReportingType")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? rowFrom)
        {

            try
            {
                var _response = await _dReporting.GetReportingType(userId, rowFrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetReports")]
        public async Task<IActionResult> GetAllJson( string? filter, int? rowFrom, int userId, int? supplierId, int? dealerId, DateTime? fromDate, DateTime? upToDate, int reportingId)
        {
            try
            {
                var response = await _dReporting.GetAllJson(
                    filter, rowFrom, userId, supplierId, dealerId, fromDate, upToDate, reportingId
                );

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
