using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;

namespace WebApi.Controllers
{
    [Route("api/Payment")]
    [ApiController]
    [Authorize]
    public class cPayment : ControllerBase
    {
        private readonly dPayment _dPayment;

        public cPayment(dPayment dPayment)
        {
            _dPayment = dPayment;

        }

        [HttpGet("GetCurrencys")]
        public async Task<IActionResult> GetCurrencys()
        {
            try
            {
                var _response = await _dPayment.GetCurrencys();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetPaymentTypes")]
        public async Task<IActionResult> GetPaymentTypes()
        {
            try
            {
                var _response = await _dPayment.GetPaymentTypes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetDocumentTypes")]
        public async Task<IActionResult> GetDocumentTypes()
        {
            try
            {
                var _response = await _dPayment.GetDocumentTypes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetDocumentConcepts")]
        public async Task<IActionResult> GetDocumentConcepts()
        {
            try
            {
                var _response = await _dPayment.GetDocumentConcepts();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetDocumentStatus")]
        public async Task<IActionResult> GetDocumentStatus()
        {
            try
            {
                var _response = await _dPayment.GetDocumentStatus();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("GetAccountReceivables")]
        public async Task<IActionResult> GetAccountReceivables(Int32 userId, Int32 supplierId, Int32 dealerId, string? typeCode, string? conceptCode, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId, DateTime? paymentDate)
        {
            try
            {
                var _response = await _dPayment.GetAccountReceivables(userId, supplierId, dealerId, typeCode, conceptCode, rowfrom, filter, fromDate, upToDate, statusId, paymentDate);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetBankAccounts")]
        public async Task<IActionResult> GetBankAccounts(Int32 supplierId)
        {
            try
            {
                var _response = await _dPayment.GetBankAccounts(supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("GetPayments")]
        public async Task<IActionResult> GetPayments(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId, int? currencyId,int? typeId)
        {
            try
            {
                // 1. Obtener la lista de pagos (PaymentDetails)
                var paymentsResponse = await _dPayment.GetPayments(userId, supplierId, dealerId, rowfrom, filter, fromDate, upToDate, statusId, currencyId, typeId);

                if (paymentsResponse.Data == null || !paymentsResponse.Data.Any())
                    return Ok(new List<PaymentFull>());

                // 2. Obtener todas las cuentas/settlements relacionados 
                // Pasamos null en PaymentId para que, según tu lógica, traiga toda la lista filtrada
                var accountsResponse = await _dPayment.GetAccountByPayment(userId, supplierId, dealerId, null, filter, fromDate, upToDate, statusId, currencyId, typeId, null);

                // 3. Unir (Anclar) los modelos usando LINQ
                var result = paymentsResponse.Data.Select(p => new PaymentFull
                {
                    PaymentDetail = p,
                    AccountPreview = accountsResponse.Data?
                        .Where(a => a.PaymentId == p.PaymentId) // Suponiendo que AccountPreview tiene una propiedad PaymentId
                        .ToList() ?? new List<AccountPreview>()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetPaymentStatus")]
        public async Task<IActionResult> GetPaymentStatus(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId, int? currencyId, int? typeId)
        {
            try
            {
                var _response = await _dPayment.GetPaymentStatus(userId, supplierId, dealerId, rowfrom, filter, fromDate, upToDate, statusId, currencyId, typeId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetAccountByPayment")]
        public async Task<IActionResult> GetAccountByPayment(Int32 userId, Int32 rowfrom, Int32 PaymentId)
        {
            try
            {
                var _response = await _dPayment.GetAccountByPayment(userId, null, null, rowfrom, null, null, null, null, null, null, PaymentId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetPaymentDetailsById")]
        public async Task<IActionResult> GetPaymentDetailsById(Int32 userId, Int32 rowfrom, int? PaymentDetailId)
        {
            try
            {
                var _response = await _dPayment.GetPaymentDetailsById(userId, rowfrom, PaymentDetailId);
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

                var _response = await _dPayment.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("PostPayment")]
        public async Task<IActionResult> PostPayment(Int32 userId, Models.PostPaymentDetail payment)
        {


            try
            {
                var _response = await _dPayment.PostPayment(payment, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



    }
}
