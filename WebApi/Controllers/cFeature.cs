using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/Feature")]
    [ApiController]
    [Authorize]
    public class cFeature : ControllerBase
    {
        private readonly dFeature _dFeature;

        public cFeature(dFeature dFeature)
        {
            _dFeature = dFeature;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var _response = await _dFeature.GetAll();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 FeatureId)
        {
            try
            {
                var _response = await _dFeature.GetOne(FeatureId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("Post_Features")]
        public async Task<IActionResult> Post_Features(List<Models.Feature> _features)
        {
            try
            {
                var _response = await _dFeature.Post_Features(_features);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}