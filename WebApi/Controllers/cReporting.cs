using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;


namespace WebApi.Controllers
{
    [Route("api/Reporting")]
    [ApiController]
    [Authorize]
    public class cReporting : ControllerBase
    {

        private readonly dReporting _dReporting;

        public cReporting(dReporting dReporting)
        {
            _dReporting = dReporting;

        }


        [HttpGet("GetReportingType")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? rowFrom)
        {

            try
            {
                var _response = await _dReporting.GetReportingType(userId, rowFrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetReports")]
        public async Task<IActionResult> GetAllJson( string? filter, int? rowFrom, int userId, int? supplierId, int? dealerId, DateTime? fromDate, DateTime? upToDate, int reportingId, int? estatusId)
        {
            try
            {
                var response = await _dReporting.GetAllJson(filter, rowFrom, userId, supplierId, dealerId, fromDate, upToDate, reportingId,estatusId);

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        private MemoryStream ConvertToExcel(List<Dictionary<string, object>> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DATA");

                if (data.Any())
                {
                    // Encabezados dinámicos
                    var headers = data.First().Keys.ToList();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                    }

                    var headerRange = worksheet.Range(1, 1, 1, headers.Count);
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Font.Bold = true;
                    headerRange.SetAutoFilter();

                    // Filas dinámicas
                    int row = 2;
                    foreach (var dict in data)
                    {
                        for (int col = 0; col < headers.Count; col++)
                        {
                            var cell = worksheet.Cell(row, col + 1);
                            var value = dict[headers[col]];

                            if (value == null)
                            {
                                cell.Value = string.Empty;
                            }
                            else if (value is DateTime dt)
                            {
                                cell.Value = dt;
                            }
                            else if (value is int i)
                            {
                                cell.Value = i;
                            }
                            else if (value is decimal d)
                            {
                                cell.Value = d;
                            }
                            else if (value is bool b)
                            {
                                cell.Value = b ? "SI" : "NO";
                            }
                            else
                            {
                                cell.Value = value.ToString();
                            }
                        }
                        row++;
                    }



                    worksheet.Columns().AdjustToContents();
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }


        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(string? filter, int userId, int? supplierId, int? dealerId, DateTime? fromDate, DateTime? upToDate, int reportingId, int? estatusId)

        {

            try
            {

                var response = await _dReporting.GetAllJson(filter, null, userId, supplierId, dealerId, fromDate, upToDate, reportingId,estatusId);

                // Convertir la respuesta a una lista dinámica
                List<Dictionary<string, object>> genericList = response.Data;

                // Pasar la lista genérica al método ConvertToExcel
                MemoryStream _excel = ConvertToExcel(genericList);


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

        


    }
}
