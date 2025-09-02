using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WebApi.Controllers
{
    [Route("api/Print")]
    [ApiController]
    public class cPrintqueue : ControllerBase
    {

        private readonly dPrintqueue _dPrintqueue;

        public cPrintqueue(dPrintqueue dPrintqueue)
        {
            _dPrintqueue = dPrintqueue;
        }



        [HttpPost("RequestPrint")]
        public async Task<IActionResult> Post_PrintQueue(List<Models.Printqueue> Printqueue, int userId)
        {

            try
            {
                var _response = await _dPrintqueue.Post_PrintQueue(Printqueue, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
    }
}
