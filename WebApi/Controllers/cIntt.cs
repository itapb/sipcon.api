using Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/Intt")]
    [ApiController]
    [Authorize]
    public class cIntt : ControllerBase
    {
        private readonly dIntt _dIntt;

        public cIntt(dIntt dIntt)
        {
            _dIntt = dIntt;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? tipo,
            string? filter = null) // 'PENDIENTES', 'GENERADOS'
        {
            try
            {
                var response = await _dIntt.GetInttData(
                    userId, supplierId, rowFrom
                    , fromDate, upToDate, tipo, filter);

                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetPendientes")]
        public async Task<IActionResult> GetPendientes(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? filter = null)
        {
            try
            {
                var response = await _dIntt.GetInttData(
                    userId, supplierId, rowFrom
                    , fromDate, upToDate, "PENDIENTES", filter);

                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetGenerados")]
        public async Task<IActionResult> GetGenerados(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? filter = null)
        {
            try
            {
                var response = await _dIntt.GetInttData(
                    userId, supplierId, rowFrom,
                     fromDate, upToDate, "GENERADOS", filter);

                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    

         [HttpPost("PostActions")]
            public async Task<IActionResult> PostActions(
        [FromQuery] int userId,
        [FromQuery] int? supplierId,
        [FromBody] List<Models.Action> actions)
        {
            try
            {
                if (actions == null || actions.Count == 0)
                    return BadRequest("No hay acciones para procesar");

                var data = System.Text.Json.JsonSerializer.Serialize(actions);
                var response = await _dIntt.PostInttActions(data, userId, supplierId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("ExportTXT")]
        public async Task<IActionResult> ExportTXT(
         int userId,
         int supplierId,
         int controlId)
        {
            try
            {
                var response = await _dIntt.GetInttExport(userId, supplierId, controlId);

                if (response.Data == null || response.Data.Count == 0)
                    return NotFound($"No hay registros para el control {controlId}");

                var lines = response.Data.Select(r => r.Datos);
                var txtBytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
                string fileName = $"{controlId.ToString().PadLeft(10, '0')}.txt";

                return File(txtBytes, "text/plain", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private MemoryStream ConvertToExcelINTT(List<Models.Intt> _intt)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("INTT");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "PLACA";
                worksheet.Cell(1, 2).Value = "VIN";
                worksheet.Cell(1, 3).Value = "RIF CLIENTE";
                worksheet.Cell(1, 4).Value = "NOMBRE";
                worksheet.Cell(1, 5).Value = "APELLIDO";
                worksheet.Cell(1, 6).Value = "NRO CERTIFICADO";
                worksheet.Cell(1, 7).Value = "NRO FACTURA";
                worksheet.Cell(1, 8).Value = "FECHA FACTURA";
                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:H1").SetAutoFilter();
                for (int i = 0; i < _intt.Count; i++)
                {
                    var _intts = _intt[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _intts.Plate;
                    worksheet.Cell(row, 2).Value = _intts.Vin;
                    worksheet.Cell(row, 3).Value = _intts.ClientRif;
                    worksheet.Cell(row, 4).Value = _intts.FirstName;
                    worksheet.Cell(row, 5).Value = _intts.FirstLastName;
                    worksheet.Cell(row, 6).Value = _intts.CertificateNumber;
                    worksheet.Cell(row, 7).Value = _intts.InvoiceNumber;
                    worksheet.Cell(row, 8).Value = _intts.InvoiceDate;
                }

                worksheet.Columns().AdjustToContents();

                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }


        [HttpGet("ExportPendientesExcel")]
        public async Task<IActionResult> GetExportPendientesExcel(Int32 userId, Int32 supplierId, int? rowFrom, DateTime? fromDate, DateTime? upToDate, string? filter = null)
        {
            try
            {
                List<Models.Intt> _response = await _dIntt.GetExportPendientesExcel(userId, supplierId, -1, fromDate, upToDate, "PENDIENTES", filter);
                MemoryStream _excel = ConvertToExcelINTT(_response);
                string _fileName = "PENDIENTESINTT.xlsx";

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

    }
}
