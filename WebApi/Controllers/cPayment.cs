using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using ClosedXML.Excel;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

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
        public async Task<IActionResult> GetAccountReceivables(Int32 userId, Int32 supplierId, Int32 dealerId, string? typeCode, string? conceptCode, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId)
        {
            try
            {
                var _response = await _dPayment.GetAccountReceivables(userId, supplierId, dealerId, typeCode, conceptCode, rowfrom, filter, fromDate, upToDate, statusId);
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
        public async Task<IActionResult> GetPayments(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId)
        {
            try
            {
                var _response = await _dPayment.GetPayments(userId, supplierId, dealerId,rowfrom, filter, fromDate, upToDate, statusId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetPaymentStatus")]
        public async Task<IActionResult> GetPaymentStatus(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, DateTime? fromDate, DateTime? upToDate)
        {
            try
            {
                var _response = await _dPayment.GetPaymentStatus(userId, supplierId, dealerId, rowfrom, fromDate, upToDate);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

    }
}
