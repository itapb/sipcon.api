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
    [Route("api/InttPlanta")]
    [ApiController]
    [Authorize]
    public class cInttPlanta : ControllerBase
    {
        private readonly dInttPlanta _dInttPlanta;

        public cInttPlanta(dInttPlanta dInttPlanta)
        {
            _dInttPlanta = dInttPlanta;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? tipo,
            string? filter = null)
        {
            try
            {
                var response = await _dInttPlanta.GetPlantaData(
                    userId, supplierId, rowFrom,
                    fromDate, upToDate, tipo, filter);

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
                var response = await _dInttPlanta.GetPlantaData(
                    userId, supplierId, rowFrom,
                    fromDate, upToDate, "PENDIENTES", filter);

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
                var response = await _dInttPlanta.GetPlantaData(
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
                var response = await _dInttPlanta.PostPlantaActions(data, userId, supplierId);
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
                var response = await _dInttPlanta.GetPlantaExport(userId, supplierId, controlId);

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

        [HttpGet("ExportPendientesExcel")]
        public async Task<IActionResult> GetExportPendientesExcel(
            Int32 userId,
            Int32 supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? filter = null)
        {
            try
            {
                List<Models.InttPlanta> _response = await _dInttPlanta.GetExportPendientesExcel(
                    userId, supplierId, -1, fromDate, upToDate, "PENDIENTES", filter);

                MemoryStream _excel = ConvertToExcelPlanta(_response);
                string _fileName = "PENDIENTESPLANTA.xlsx";

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

        [HttpGet("ExportGeneradosExcel")]
        public async Task<IActionResult> ExportGeneradosExcel(
            int userId,
            int supplierId,
            int controlId)
        {
            try
            {
                // Obtener datos estructurados (modo columnas)
                var response = await _dInttPlanta.GetPlantaData(
                    userId,
                    supplierId,
                    -1,  // rowFrom -1 para modo columnas
                    null,
                    null,
                    "GENERADOS",
                    null);

                if (response.Data == null || response.Data.Count == 0)
                    return NotFound($"No hay registros para el control {controlId}");

                // Filtrar por controlId
                var controlNumber = controlId.ToString().PadLeft(10, '0');
                var filteredData = response.Data
                    .Where(x => x.VNUMBERPLANTATXT == controlNumber)
                    .ToList();

                if (filteredData.Count == 0)
                    return NotFound($"No hay registros para el control {controlId}");

                var excelStream = ConvertToExcelGenerados(filteredData, controlId);
                string fileName = $"CONTROL_{controlNumber}.xlsx";

                return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private MemoryStream ConvertToExcelGenerados(List<InttPlanta> data, int controlId)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"CONTROL_{controlId}");

                worksheet.Cell(1, 1).Value = "NRO CONTROL PLANTA";
                worksheet.Cell(1, 2).Value = "NRO CERTIFICADO";
                worksheet.Cell(1, 3).Value = "RIF CLIENTE";
                worksheet.Cell(1, 4).Value = "MODELO";
                worksheet.Cell(1, 5).Value = "PLACA";
                worksheet.Cell(1, 6).Value = "VIN";
                worksheet.Cell(1, 7).Value = "COLOR";
                worksheet.Cell(1, 8).Value = "FECHA FACTURA";
                worksheet.Cell(1, 9).Value = "NRO FACTURA";

                var headerRange = worksheet.Range("A1:I1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Font.Bold = true;
                worksheet.Range("A1:I1").SetAutoFilter();

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = controlId.ToString().PadLeft(10, '0');
                    worksheet.Cell(row, 2).Value = item.VCERTIFICATENUMBER;
                    worksheet.Cell(row, 3).Value = item.VRIF;
                    worksheet.Cell(row, 4).Value = item.VMODEL;
                    worksheet.Cell(row, 5).Value = item.VPLATE;
                    worksheet.Cell(row, 6).Value = item.VVIN;
                    worksheet.Cell(row, 7).Value = item.VCOLOR1;
                    worksheet.Cell(row, 8).Value = item.VSELLINVOICEDATE;
                    worksheet.Cell(row, 9).Value = item.VSELLINVOICENUMBER;
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

        private MemoryStream ConvertToExcelPlanta(List<InttPlanta> _planta)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("PLANTA");

                worksheet.Cell(1, 1).Value = "NRO CERTIFICADO";
                worksheet.Cell(1, 2).Value = "RIF CLIENTE";
                worksheet.Cell(1, 3).Value = "MODELO";
                worksheet.Cell(1, 4).Value = "PLACA";
                worksheet.Cell(1, 5).Value = "VIN";
                worksheet.Cell(1, 6).Value = "COLOR";
                worksheet.Cell(1, 7).Value = "FECHA FACTURA";
                worksheet.Cell(1, 8).Value = "NRO FACTURA";

                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:H1").SetAutoFilter();

                for (int i = 0; i < _planta.Count; i++)
                {
                    var item = _planta[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = item.VCERTIFICATENUMBER;
                    worksheet.Cell(row, 2).Value = item.VRIF;
                    worksheet.Cell(row, 3).Value = item.VMODEL;
                    worksheet.Cell(row, 4).Value = item.VPLATE;
                    worksheet.Cell(row, 5).Value = item.VVIN;
                    worksheet.Cell(row, 6).Value = item.VCOLOR1;
                    worksheet.Cell(row, 7).Value = item.DINVOICEDATE;
                    worksheet.Cell(row, 8).Value = item.VINVOICENUMBER;
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
    }
}