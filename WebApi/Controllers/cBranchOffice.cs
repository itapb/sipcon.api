using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/BranchOffice")]
    [ApiController]
    [Authorize]
    public class cBranchOffice : ControllerBase
    {
        private readonly dBranchOffice _dBranchOffice;

        public cBranchOffice(dBranchOffice dBranchOffice)
        {
            _dBranchOffice = dBranchOffice;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32? ContactId = null, bool? IsActive = null)
        {
            try
            {
                var _response = await _dBranchOffice.GetAll(ContactId, IsActive);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostBranchOffice")]
        public async Task<IActionResult> Post_BranchOffice(List<Models.BranchOffice> _list)
        {
            try
            {
                var _response = await _dBranchOffice.Post_BranchOffice(_list);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
    }
}

