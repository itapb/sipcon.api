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

        [HttpPost("Update_Rate")]
        public async Task<IActionResult> Update_Rate(int id, decimal nRate)
        {
            try
            {
                var response = await _dRate.Update(id, nRate);

                if (!response.Processed)
                    return StatusCode(StatusCodes.Status400BadRequest, response.Message);

                if (response.Data == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Tasa no encontrada");

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}