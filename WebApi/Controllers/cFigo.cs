using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        #region "CxC"
        [HttpGet("ReportCxC")]
        public async Task<IActionResult> GetReportCxC(DateTime? Date, string Currency, string? filter)
        {
            try
            {
                DateTime FinalDate = Date ?? DateTime.Today;
                var _response = await _dFigo.GetReportCxC(FinalDate, Currency, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("ExportReportCxC")]
        public async Task<IActionResult> ExportReportCxC(DateTime? Date, string Currency, string? filter)
        {
            try
            {
                DateTime FinalDate = Date ?? DateTime.Today;
                var _response = await _dFigo.GetReportCxC(FinalDate, Currency, filter);

                if (_response.Data == null)
                {
                    return StatusCode(_response.Status, _response);
                }

                MemoryStream _excel = ExportExcel.ConvertToExcelReportCxC(_response.Data, FinalDate);
                string _fileName = $"ReporteCxC_{FinalDate:yyyyMMdd}.xlsx";

                return File(
                    _excel,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("ExportReportCxCPdf")]
        public async Task<IActionResult> ExportReportCxCPdf(DateTime? Date, string Currency, string? filter)
        {
            try
            {
                DateTime FinalDate = Date ?? DateTime.Today;

                var _response = await _dFigo.GetReportCxC(FinalDate, Currency, filter);

                if (_response.Data == null)
                {
                    return StatusCode(_response.Status, _response);
                }

                byte[] _pdfBytes = ExportPDF.ConvertToPdfReportCxC(_response.Data, FinalDate);
                string _fileName = $"ReporteCxC_{FinalDate:yyyyMMdd}.pdf";

                return File(
                    _pdfBytes,
                    "application/pdf",
                    _fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion "CxC"

        #region "VENTAS"
        [HttpPost("ExtractDaily")]
        public async Task<IActionResult> ExtractDailySales(DateTime? date)
        {
            try
            {
                DateTime processDate = date ?? DateTime.Today;
                var response = await _dFigo.ExtractAndInsertSales(processDate);
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