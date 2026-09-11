using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/Printer")]
    [ApiController]

    public class cPrinter : ControllerBase
    {

        private readonly dPrinter _dPrinter;

        public cPrinter(dPrinter dPrinter)
        {
            _dPrinter = dPrinter;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int? supplierId)
        {

            try
            {
                var _response = await _dPrinter.GetAll(supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

    }
}