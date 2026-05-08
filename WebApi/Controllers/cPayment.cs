using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.ComponentModel;
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
        public async Task<IActionResult> GetBankAccounts(Int32 supplierId, Int32? idCurrency)
        {
            try
            {
                var _response = await _dPayment.GetBankAccounts(supplierId, idCurrency);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("GetBank")]
        public async Task<IActionResult> GetBank()
        {
            try
            {
                var _response = await _dPayment.GetBank();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetPayments")]
        public async Task<IActionResult> GetPayments(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId, int? currencyId, int? typeId)
        {
            try
            {
                // 1. Obtener la lista de pagos que YA SON de tipo PaymentFull
                var paymentsResponse = await _dPayment.GetPayments(userId, supplierId, dealerId, rowfrom, filter, fromDate, upToDate, statusId, currencyId, typeId);

                if (paymentsResponse.Data == null)
                    return Ok(new Response<List<PaymentFull>> { Data = new List<PaymentFull>(), Message = "No hay datos", Status = 200 });

                // 2. Obtener cuentas
                var accountsResponse = await _dPayment.GetAccountByPayment(userId, supplierId, dealerId, null, filter, fromDate, upToDate, statusId, currencyId, typeId, null);

                // 3. Crear el Lookup para optimización (O(1))
                var accountsLookup = accountsResponse.Data?.ToLookup(a => a.PaymentId);

                // 4. Hidratar la propiedad AccountPreview directamente
                // Como 'paymentsResponse.Data' ya es List<PaymentFull>, simplemente iteramos
                foreach (var payment in paymentsResponse.Data)
                {
                    payment.AccountPreview = accountsLookup?[payment.PaymentId].ToList() ?? new List<AccountPreview>();
                }

                // 5. Envolver el resultado final
                var finalResponse = new Response<List<PaymentFull>>
                {
                    Data = paymentsResponse.Data, // Ya es la lista enriquecida
                    Message = paymentsResponse.Message,
                    Processed = paymentsResponse.Processed,
                    Status = paymentsResponse.Status,
                    Total = paymentsResponse.Total
                };

                return Ok(finalResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new Response<List<PaymentFull>> { Message = ex.Message, Status = 409 });
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


        [HttpPost("PostPayDetailsActions")]
        public async Task<IActionResult> PostPayDetailsActions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                var _response = await _dPayment.PostPayDetails_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private MemoryStream ConvertToExcel_Payment(List<Models.PaymentDetails> _paymentDetails)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("VEHICULOS EN CAMPAÑA");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "NUM";
                worksheet.Cell(1, 2).Value = "REFERENCIA";
                worksheet.Cell(1, 3).Value = "MONEDA";
                worksheet.Cell(1, 4).Value = "TIPO";
                worksheet.Cell(1, 5).Value = "FECHA";
                worksheet.Cell(1, 6).Value = "BANCO EMISOR";
                worksheet.Cell(1, 7).Value = "CONCESIONARIO";
                worksheet.Cell(1, 8).Value = "BANCO DESTINO";
                worksheet.Cell(1, 9).Value = "CUENTA DESTINO";
                worksheet.Cell(1, 10).Value = "MONTO";
                worksheet.Cell(1, 11).Value = "TASA DE PAGO (BCV)";
                worksheet.Cell(1, 12).Value = "FECHA DE TASA (BCV) ";
                worksheet.Cell(1, 13).Value = "ESTATUS";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:N1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:N1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _paymentDetails.Count; i++)
                {
                    var _paymentdetail= _paymentDetails[i];
                    worksheet.Cell(i + 2, 1).Value = _paymentdetail.Id;
                    worksheet.Cell(i + 2, 2).Value = _paymentdetail.Reference;
                    worksheet.Cell(i + 2, 3).Value = _paymentdetail.CurrencyName;
                    worksheet.Cell(i + 2, 4).Value = _paymentdetail.TypeName;
                    worksheet.Cell(i + 2, 5).Value = _paymentdetail.Date;
                    worksheet.Cell(i + 2, 6).Value = _paymentdetail.BankOriginName;
                    worksheet.Cell(i + 2, 7).Value = _paymentdetail.DealerName;
                    worksheet.Cell(i + 2, 8).Value = _paymentdetail.BankName;
                    worksheet.Cell(i + 2, 9).Value = _paymentdetail.AccountNumber;
                    worksheet.Cell(i + 2, 10).Value = _paymentdetail.Amount;
                    worksheet.Cell(i + 2, 11).Value = _paymentdetail.Rate;
                    worksheet.Cell(i + 2, 12).Value = _paymentdetail.DateRate;
                    worksheet.Cell(i + 2, 13).Value = _paymentdetail.StatusName;



                }
                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Centra contenido de las columnas 
                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                // 9. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;

            }

        }


        [HttpGet("ExportPayment")]
        public async Task<IActionResult> GetExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId, int? currencyId, int? typeId)

        {

            try
            {

                List<PaymentDetails> _PaymentDetails = await _dPayment.GetExportPaymentDetails(userId, supplierId, dealerId, filter, fromDate, upToDate, statusId, currencyId, typeId);
                MemoryStream _excel = ConvertToExcel_Payment(_PaymentDetails);
                string _fileName = $"Lista_Detalles_Pago.xlsx";

                return File(
                 _excel,
                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 _fileName);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }



        private MemoryStream ConvertToExcel_AccountReceivable(List<Models.AccountReceivable> _AccountReceivables)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("VEHICULOS EN CAMPAÑA");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "NUMERO";
                worksheet.Cell(1, 2).Value = "REFERENCIA";
                worksheet.Cell(1, 3).Value = "CONCESIONARIO";
                worksheet.Cell(1, 4).Value = "FECHA EMISION";
                worksheet.Cell(1, 5).Value = "FECHA VENCIMIENTO";
                worksheet.Cell(1, 6).Value = "TIPO";  
                worksheet.Cell(1, 7).Value = "CONCEPTO";
                worksheet.Cell(1, 8).Value = "MONTO REF";
                worksheet.Cell(1, 9).Value = "SALDO PENDIENTE REF";
                worksheet.Cell(1, 10).Value = "MONTO BS(BCV)";
                worksheet.Cell(1, 11).Value = "SALDO PENDIENTE BS(BCV)";
                worksheet.Cell(1, 12).Value = "ESTATUS";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:M1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:M1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _AccountReceivables.Count; i++)
                {
                    var _AccountReceivable = _AccountReceivables[i];
                    worksheet.Cell(i + 2, 1).Value = _AccountReceivable.Number;
                    worksheet.Cell(i + 2, 2).Value = _AccountReceivable.Reference;
                    worksheet.Cell(i + 2, 3).Value = _AccountReceivable.DealerName;
                    worksheet.Cell(i + 2, 4).Value = _AccountReceivable.DocumentDate;
                    worksheet.Cell(i + 2, 5).Value = _AccountReceivable.DocumentDueDate;
                    worksheet.Cell(i + 2, 6).Value = _AccountReceivable.TypeName;
                    worksheet.Cell(i + 2, 7).Value = _AccountReceivable.ConceptName;
                    worksheet.Cell(i + 2, 8).Value = _AccountReceivable.Amount;
                    worksheet.Cell(i + 2, 9).Value = _AccountReceivable.Balance;
                    worksheet.Cell(i + 2, 10).Value = _AccountReceivable.AmountBs;
                    worksheet.Cell(i + 2, 11).Value = _AccountReceivable.BalanceBs;
                    worksheet.Cell(i + 2, 12).Value = _AccountReceivable.StatusName;



                }
                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Centra contenido de las columnas 
                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                // 9. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;

            }

        }

        [HttpGet("ExportAccountReceivable")]
        public async Task<IActionResult> ExportAccountReceivable(Int32 userId, Int32 supplierId, Int32 dealerId, string? typeCode, string? conceptCode, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId, DateTime? paymentDate)

        {

            try
            {

                List<AccountReceivable> _AccountReceivable = await _dPayment.GetExportAccountReceivable(userId, supplierId, dealerId, typeCode, conceptCode, filter, fromDate, upToDate, statusId, paymentDate);
                MemoryStream _excel = ConvertToExcel_AccountReceivable(_AccountReceivable);
                string _fileName = $"Lista_Detalles_Pago.xlsx";

                return File(
                 _excel,
                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 _fileName);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }




        [HttpPost("DeletePaymentDetails")]
        public async Task<IActionResult> DeletePaymentDetails(List<Models.Action> _list, int userId)
        {
            try
            {

                var _response = await _dPayment.Delete_Details(_list, userId);
                return StatusCode(_response.Status, _response);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el archivo: {ex.Message}");
            }
        }


    }
}
