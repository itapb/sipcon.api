using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;


namespace WebApi.Controllers
{

    [Route("api/Color")]
    [ApiController]

    public class cColor : ControllerBase
    {

        private readonly dColor _dColor;

        public cColor(dColor dColor)
        {
            _dColor = dColor;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {

            try
            {
                Response _response = await _dColor.GetAll();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


    }
}
