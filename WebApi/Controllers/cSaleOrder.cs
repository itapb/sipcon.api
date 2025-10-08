using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/SaleOrder")]
    [ApiController]

    [Authorize]
    public class cSaleOrder : ControllerBase
    {
        private readonly dSaleOrder _dSaleOrder;

        public cSaleOrder(dSaleOrder dSaleOrder)
        {
            _dSaleOrder = dSaleOrder;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId,Int32? dealerId, Int32 rowfrom, string? filter)
        {

            try
            {
                var _response = await _dSaleOrder.GetAll(userId, supplierId,dealerId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 saleOrderId, Int32 userId)
        {

            try
            {
                var _response = await _dSaleOrder.GetOne(saleOrderId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOneWithContext")]
        public async Task<IActionResult> GetOneWithContext(Int32 userId, Int32 saleOrderId)
        {
            try
            {

                var _get = await _dSaleOrder.GetOne(saleOrderId, userId);
                var _details = await _dSaleOrder.GetDetails(saleOrderId);

                SaleOrderWithContext _req = new SaleOrderWithContext();
                _req.SaleOrder = (SaleOrder)(_get.Data);
                _req.Details = (List<SaleOrderDetail>)(_details.Data);

                Response<SaleOrderWithContext> _response = new Response<SaleOrderWithContext>();
                _response.Data = _req;
                _response.Total = _get.Total;
                _response.Processed = _get.Processed;
                _response.Message = _get.Message;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

/*
        [HttpGet("GetReasons")]
        public async Task<IActionResult> GetReasons()
        {

            try
            {
                var _response = await _dSaleOrder.GetReasons();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        */

        [HttpGet("GetSaleOrderTypes")]
        public async Task<IActionResult> GetSaleOrderTypes()
        {

            try
            {
                var _response = await _dSaleOrder.GetSaleOrderTypes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(Int32 saleOrderId)
        {

            try
            {
                var _response = await _dSaleOrder.GetDetails(saleOrderId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetReportFail")]
        public async Task<IActionResult> GetReportFail(Int32 userId, Int32? dealerId,String type, Int32 rowfrom)
        {

            try
            {
                var _response = await _dSaleOrder.GetReportFail(userId, dealerId,type, rowfrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }



        [HttpGet("NewSaleOrderWithContext")]
        public async Task<IActionResult> NewSaleOrderWithContext(Int32 userId, Int32 dealerId, Int32 typeId)
        {
            try
            {

                SaleOrder _new = new SaleOrder();
                _new.Id = 0;
                _new.DealerId = dealerId;
                _new.SupplierId = 0;
                _new.TypeId = typeId;

                if (dealerId == 0)
                {
                    throw new Exception("Concesionario Invalido!.");
                }


                List<SaleOrder> _list = new List<SaleOrder>();
                _list.Add(_new);

                var _get2 = (SaleOrder)(await _dSaleOrder.getLast(userId, dealerId)).Data;

                if (_get2.Id is null || _get2.Id == 0)
                {

                    Response <Result> _resp = await _dSaleOrder.PostSaleOrder(_list, userId);
                    if (_resp.Processed == false)
                    {
                        throw new Exception(_resp.Message);
                    }

                    Result _resul = new Result();
                    _resul = (Result)(_resp.Data);

                    var _get = await _dSaleOrder.GetOne(_resul.LastId, userId);
                    _new = (SaleOrder)(_get.Data);


                }
                else
                {
                    _new = _get2;
                }



                List<SaleOrderDetail> _details = new List<SaleOrderDetail>();

                SaleOrderWithContext _req = new SaleOrderWithContext();
                _req.SaleOrder = _new;
                _req.Details = _details;

                Response<SaleOrderWithContext> _response = new Response<SaleOrderWithContext>();
                _response.Data = _req;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                Response<SaleOrderWithContext> _response = new Response<SaleOrderWithContext>();
                _response.SetError(ex);

                return StatusCode(StatusCodes.Status409Conflict, _response);
            }
        }



        [HttpPost("PostSaleOrder")]
        public async Task<IActionResult> PostSaleOrder(List<Models.SaleOrder> saleOrders, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.PostSaleOrder(saleOrders, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }




        [HttpPost("PostDetail")]
        public async Task<IActionResult> PostDetail(Models.SaleOrderDetail detail, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.PostDetail(detail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        [HttpPost("DeleteDetails")]
        public async Task<IActionResult> DeleteDetails(List<Models.Detail> details, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.DeleteDetails(details, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

    }


}
