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
    public class cBrand : ControllerBase
    {

        private readonly dBrand _dBrand;

        public cBrand(dBrand dBrand)
        {
            _dBrand = dBrand;
          
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int? SupplierId)
        {
           
            try
            {
                List<Models.Brand> _brands = await _dBrand.GetAll(SupplierId);
                return StatusCode(StatusCodes.Status200OK, _brands);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }
}
