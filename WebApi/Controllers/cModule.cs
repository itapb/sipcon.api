using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/Module")]
    [ApiController]
    [Authorize]
    public class cModule : ControllerBase
    {

        private readonly dModule _dModule;

        public cModule(dModule dModule)
        {
            _dModule = dModule;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? moduleName, Int32 userId)
        {

            try
            {
                List<Models.Module> _modules = await _dModule.GetAll(moduleName, userId);
                return StatusCode(StatusCodes.Status200OK, _modules);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

    }
}
