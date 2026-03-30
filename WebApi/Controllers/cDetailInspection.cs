using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/DetailInspection")]
    [ApiController]
    [Authorize]
    public class cDetailInspection : ControllerBase
    {
        private readonly dDetailInspection _dDetailInspection;

        public cDetailInspection(dDetailInspection dDetailInspection)
        {
            _dDetailInspection = dDetailInspection;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? InspectionId = null)
        {
            try
            {
                var _response = await _dDetailInspection.GetAll(InspectionId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 DetailInspectionId)
        {
            try
            {
                var _response = await _dDetailInspection.GetOne(DetailInspectionId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("Post_DetailInspections")]
        public async Task<IActionResult> Post_DetailInspections(List<Models.DetailInspection> _details)
        {
            try
            {
                var _response = await _dDetailInspection.Post_DetailInspections(_details);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}