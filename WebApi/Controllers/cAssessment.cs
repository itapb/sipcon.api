using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.ComponentModel;


namespace WebApi.Controllers
{

    [Route("api/Assessment")]
    [ApiController]

    public class cAssessment : ControllerBase
    {

        private readonly dAssessment _dAssessment;

        public cAssessment(dAssessment dAssessment)
        {
            _dAssessment = dAssessment;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 recordId,String moduleName)
        {

            try
            {
                Response _response = await _dAssessment.GetAll(moduleName,recordId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpPost("PostAssessment")]
        public async Task<IActionResult> Post_Assessment(Models.Assessment assessment, Int32 userId)
        {


            try
            {
                Models.Response _response = await _dAssessment.Post_Assessment(assessment, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                Response _response = await _dAssessment.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        //comentario de prueba




    }
}

