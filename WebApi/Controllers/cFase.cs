using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/Fase")]
    [ApiController]
    [Authorize]
    public class cFase : ControllerBase
    {
        private readonly dFase _dFase;

        public cFase(dFase dFase)
        {
            _dFase = dFase;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? AreaId = null, Int32? SupplierId = null, Int32? DealerId = null)
        {
            try
            {
                var _response = await _dFase.GetAll(AreaId, SupplierId, DealerId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 FaseId)
        {
          try
          {
            var _response = await _dFase.GetOne(FaseId);
            return StatusCode(_response.Status, _response);
          }
          catch (Exception ex)
          {
            return StatusCode(StatusCodes.Status409Conflict, ex.Message);
          }
        }

    [HttpPost("Post_Fases")]
        public async Task<IActionResult> Post_Fases(List<Models.Fase> _fases)
        {
            try
            {
              var _response = await _dFase.Post_Fases(_fases);
              return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
              return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
