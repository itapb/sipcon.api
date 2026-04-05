using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{

    [Route("api/Area")]
    [ApiController]
    [Authorize]
    public class cArea : ControllerBase
    {

        private readonly dArea _dArea;

        public cArea(dArea dArea)
        {
            _dArea = dArea;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? DealerId = null, Int32? SupplierId = null, bool IsActive = true)
        {
            try
            {
                var _response = await _dArea.GetAll(DealerId, SupplierId, IsActive);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 AreaId)
        {
          try
          {
            var _response = await _dArea.GetOne(AreaId);
            return StatusCode(_response.Status, _response);
          }
          catch (Exception ex)
          {
            return StatusCode(StatusCodes.Status409Conflict, ex.Message);
          }
        }

    [HttpPost("Post_Areas")]
        public async Task<IActionResult> Post_Areas(List<Models.Area> _areas, Int32 userId)
        {
            try
            {
              var _response = await _dArea.Post_Areas(_areas, userId);
              return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
              return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
