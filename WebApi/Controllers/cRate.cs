using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/Rate")]
    [ApiController]
    [Authorize]
    public class cRate : ControllerBase
    {
        private readonly dRate _dRate;

        public cRate(dRate dRate)
        {
            _dRate = dRate;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int? rowFrom = null, string? filter = null)
        {
            try
            {
                var response = await _dRate.GetAll(rowFrom, filter);

                if (response.Processed)
                    return StatusCode(StatusCodes.Status200OK, response);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("/api/Rate/Post")]
        public async Task<IActionResult> PostRate(Models.Rate rate, Int32 userId)
        {

            try
            {
                var _response = await _dRate.PostRate(rate, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
    }
}