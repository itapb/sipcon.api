using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{

    [Route("api/PowerBI")]
    [ApiController]
    [Authorize]

    public class cPowerBI : ControllerBase
    {

        private readonly dPowerBI _dPowerBI;

        public cPowerBI(dPowerBI dPowerBI)
        {
            _dPowerBI = dPowerBI;
        }

        [HttpGet("GetReports")]
        public async Task<IActionResult> GetReports(string userId)
        {
            try
            {
                var _response = await _dPowerBI.GetReports(userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetToken")]
        public async Task<IActionResult> GetTokenApi()
        {
            try
            {
                var _response = await _dPowerBI.GetTokenAPI();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
    }
}
