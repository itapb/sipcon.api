using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;


namespace WebApi.Controllers
{

    [Route("api/Comment")]
    [ApiController]

    public class cComment : ControllerBase
    {

        private readonly dComment _dComment;

        public cComment(dComment dComment)
        {
            _dComment = dComment;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 recordId,String moduleName)
        {

            try
            {
                var _response = await _dComment.GetAll(moduleName,recordId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpPost("PostComment")]
        public async Task<IActionResult> Post_Comment(Models.Comment comment, Int32 userId)
        {


            try
            {
                var _response = await _dComment.Post_Comment(comment, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


    



    }
}

