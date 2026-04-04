using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/Inspection")]
    [ApiController]
    [Authorize]
    public class cInspection : ControllerBase
    {
        private readonly dInspection _dInspection;

        public cInspection(dInspection dInspection)
        {
            _dInspection = dInspection;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? AreaId = null)
        {
            try
            {
                var _response = await _dInspection.GetAll(AreaId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 InspectionId)
        {
            try
            {
                var _response = await _dInspection.GetOne(InspectionId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("Post_Inspections")]
        public async Task<IActionResult> Post_Inspections(List<Models.Inspection> _inspections)
        {
            try
            {
                var _response = await _dInspection.Post_Inspections(_inspections);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}