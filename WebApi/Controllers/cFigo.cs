using Data;
using DocumentFormat.OpenXml.Bibliography;
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

        [HttpGet("GetReportsContent")]
        public async Task<IActionResult> GetAllJson(int userId,int supplierId, int rowfrom, int reportId, string jsonParameters)
        {
            try
            {
                var response = await _dFigo.GetAllJson(userId, supplierId,rowfrom, reportId, jsonParameters);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        #endregion "Reports"




        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(int userId, int supplierId, int reportId, string jsonParameters)

        {

            try
            {

                var response = await _dFigo.GetAllJson(userId, supplierId, null,reportId, jsonParameters);

                // Convertir la respuesta a una lista dinámica
                List<Dictionary<string, object>> genericList = response.Data;

                // Pasar la lista genérica al método ConvertToExcel
                MemoryStream _excel = ExportExcel.ConvertToExcel(genericList);


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

        //[HttpGet("ExportReportCxCPdf")]
        //public async Task<IActionResult> ExportReportCxCPdf(DateTime? Date, string Currency, string? filter)
        //{
        //    try
        //    {
        //        DateTime FinalDate = Date ?? DateTime.Today;

        //        var _response = await _dFigo.GetReportCxC(FinalDate, Currency, filter);

        //        if (_response.Data == null)
        //        {
        //            return StatusCode(_response.Status, _response);
        //        }

        //        byte[] _pdfBytes = ExportPDF.ConvertToPdfReportCxC(_response.Data, FinalDate);
        //        string _fileName = $"ReporteCxC_{FinalDate:yyyyMMdd}.pdf";

        //        return File(
        //            _pdfBytes,
        //            "application/pdf",
        //            _fileName
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }
        //}

    }
}