using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;


namespace WebApi.Controllers
{

    [Route("api/PayMethod")]
    [ApiController]

    public class cPayMethod : ControllerBase
    {

        private readonly dPayMethod _dPayMethod;

        public cPayMethod(dPayMethod dPayMethod)
        {
            _dPayMethod = dPayMethod;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(bool? isDealer)
        {

            try
            {
                Response _response = await _dPayMethod.GetAll(isDealer);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


    }
}
