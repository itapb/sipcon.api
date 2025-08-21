using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;


namespace WebApi.Controllers
{
    [Route("api/Claim")]
    [ApiController]
    public class cClaim : ControllerBase
    {
        private readonly dSaleOrder _dOrder;

        public cClaim(dSaleOrder dOrder)
        {
            _dOrder = dOrder;
        }

        //[HttpGet("GetAll")]
        //public async Task<IActionResult> GetAll(Int32 userId, Int32? dealerId, Int32 rowfrom, string? filter)
        //{

        //    try
        //    {
        //        Response _response = await _dOrder.GetAll(userId, dealerId, rowfrom, filter, "c");
        //        return StatusCode(_response.Status, _response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }


        //}

        //[HttpGet("GetOne")]
        //public async Task<IActionResult> GetOne(Int32 orderId, Int32 userId)
        //{

        //    try
        //    {
        //        Response _response = await _dOrder.GetOne(orderId, userId, "c");
        //        return StatusCode(_response.Status, _response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }

        //}
        //[HttpPost("PostClaim")]
        //public async Task<IActionResult> Post_Order(List<Models.Requisition> orders, Int32 userId)
        //{
        //    try
        //    {
        //        Response _response = await _dOrder.Post_Order(orders, userId, "c"); // defined type as 'c' for indicating post as claim
        //        return StatusCode(_response.Status, _response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    } 
            
        //}



    }
}
