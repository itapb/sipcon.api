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

        [HttpGet("ReportsOptions")]
        public async Task<IActionResult> ReportsOptions(int userId, int reportId, int rowFrom)
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