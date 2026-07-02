using Data;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApi.ExportFiles;


namespace WebApi.Controllers
{
    [Route("api/Figo")]
    [ApiController]
    [Authorize]
    public class cFigo : ControllerBase
    {
        private readonly dFigo _dFigo;

        public cFigo(dFigo dFigo)
        {
            _dFigo = dFigo;
        }

        #region "Reports"

        [HttpGet("ReportsFigo")]
        public async Task<IActionResult> GetReportsFigo(int userId, int rowFrom)
        {
            try
            {
                var _response = await _dFigo.GetReportsFigo(userId, rowFrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("ReportsFilters")]
        public async Task<IActionResult> ReportsFilters(int userId, int reportId, int rowFrom)
        {
            try
            {
                var _response = await _dFigo.ReportsFilters(userId, reportId, rowFrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("ReportsOptions")]
        public async Task<IActionResult> ReportsOptions(int userId, int reportId, int? rowFrom)
        {
            try
            {
                var _response = await _dFigo.ReportsOptions(userId, reportId, rowFrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetReportsContent")]
        public async Task<IActionResult> GetAllJson(int userId,int supplierId, int rowfrom, int reportId, string jsonParameters,string? filter)
        {
            try
            {
                var response = await _dFigo.GetAllJson(userId, supplierId,rowfrom, reportId, jsonParameters,filter);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        #endregion "Reports"




        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(int userId, int supplierId, int reportId, string jsonParameters, string? filter)
        {
            try
            {
                // 1. Traemos la data del reporte como siempre
                var response = await _dFigo.GetAllJson(userId, supplierId, null, reportId, jsonParameters, filter);
                List<Dictionary<string, object>> genericList = response.Data;

                // 2. NUEVO: Traemos la configuración de los filtros/campos para este reporte
                // (Usa el valor correspondiente para 'rowFrom', por ejemplo 0 o el que requiera tu método)
                var filtersResponse = await _dFigo.ReportsFilters(userId, reportId, 0);

                List<string> columnsToTotal = new List<string>();

                // 1. Validamos que la respuesta no sea nula y que la propiedad Data contenga elementos
                if (filtersResponse != null && filtersResponse.Data != null)
                {
                    columnsToTotal = filtersResponse.Data
                        .Where(f => f.ActionType == "T" && !string.IsNullOrEmpty(f.Field))
                        .Select(f => f.Field)
                        .ToList();
                }

                // 3. Pasar la lista genérica Y la lista de columnas a totalizar al método ConvertToExcel
                // NOTA: Necesitamos ajustar la firma de tu método ConvertToExcel para que reciba este parámetro
                MemoryStream _excel = ExportExcel.ConvertToExcelFigo(genericList, columnsToTotal);

                string _fileName = "Reporte.xlsx";

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

        [HttpGet("ExportPdf")]
        public async Task<IActionResult> GetExportPdf(int userId, int supplierId,int reportId, string jsonParameters, string? filter)
        {
            try
            {
                var response = await _dFigo.GetAllJson( userId, supplierId, null, reportId, jsonParameters, filter);
                List<Dictionary<string, object>> genericList = response.Data;
                if (!response.Processed || response.Data == null)
                    return BadRequest();

                byte[] pdf = ExportPDF.PdfFactory( reportId, genericList, jsonParameters);

                return File(pdf, "application/pdf", $"Reporte_{reportId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        #region "VENTAS"
        [HttpPost("ExtractDaily")]
        public async Task<IActionResult> ExtractDailySales(DateTime? date)
        {
            try
            {
                var response = await _dFigo.ExtractAndInsertSales();
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        #endregion "VENTAS" 

        #region "TRÁNSITO"
        [HttpPost("ExtractTransit")]
        public async Task<IActionResult> ExtractTransitRepuestos()
        {
            try
            {
                var response = await _dFigo.ExtractAndInsertTransit();
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
        #endregion "TRÁNSITO"
    }
}