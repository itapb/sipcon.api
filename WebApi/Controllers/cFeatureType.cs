using Microsoft.AspNetCore.Mvc;
using Data;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{

    [Route("api/FeatureType")]
    [ApiController]
    [Authorize]
    public class cFeatureType : ControllerBase
    {

        private readonly dFeatureType _dFeatureType;

        public cFeatureType(dFeatureType dFeatureType)
        {
            _dFeatureType = dFeatureType;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? DealerId = null, Int32? SupplierId = null, Int32? AreaId = null)
        {

            try
            {
                var _response = await _dFeatureType.GetAll(DealerId, SupplierId, AreaId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 FeatureTypeId)
        {
            try
            {
                var _response = await _dFeatureType.GetOne(FeatureTypeId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("Post_FeatureTypes")]
        public async Task<IActionResult> Post_FeatureTypes(List<Models.FeatureType> _featureTypes)
        {
            try
            {
                var _response = await _dFeatureType.Post_FeatureTypes(_featureTypes);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}