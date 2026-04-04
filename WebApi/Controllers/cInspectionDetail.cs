using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/InspectionDetail")]
    [ApiController]
    [Authorize]
    public class cInspectionDetail : ControllerBase
    {
        private readonly dInspectionDetail _dDetailInspection;

        public cInspectionDetail(dInspectionDetail dDetailInspection)
        {
            _dDetailInspection = dDetailInspection;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? InspectionId = null, Int32? AreaId = null, Int32? FaseId = null, Int32? FeatureTypeId = null)
        {
            try
            {
                var _response = await _dDetailInspection.GetAll(InspectionId, AreaId, FaseId, FeatureTypeId);
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

        [HttpPost("Post_InspectionsDetail")]
        public async Task<IActionResult> Post_DetailInspections(List<Models.InspectionDetail> _details)
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