using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{

    [Route("api/Figo")]
    [ApiController]
    [Authorize]
    public class cFigo : ControllerBase
    {

        private readonly dFigo _dFigo;

        public cFigo(dFigo dFigo)
        {
            _dFigo = dFigo;
        }

        [HttpGet("ReportCxC")]
        public async Task<IActionResult> GetReportCxC(DateTime? Date, string Currency, string? filter)
        {
            try
            {
                DateTime FinalDate = Date ?? DateTime.Today;
                var _response = await _dFigo.GetReportCxC(FinalDate, Currency, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
    }
}
