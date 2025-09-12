using Azure;
using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/LaborTime")]
    [ApiController]
    [Authorize]
    public class cLaborTime : ControllerBase
    {

        private readonly dLaborTime _dLaborTime;
        private readonly dModel _dModel;
        

        public cLaborTime(dLaborTime dLabortime, dModel dModel)
        {
            _dLaborTime = dLabortime;
            _dModel = dModel; 
        }





        [HttpGet("GetAll")]
        public async Task<IActionResult> GetLaborTime(Int32 supplierId, Int32 irowfrom, Int32? modelId, String? filter)
        {

            try
            {
                var _response = await _dLaborTime.GetAll(supplierId,modelId,filter,irowfrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        private MemoryStream ConvertToExcel(List<Models.LaborTime> _laborTimes)
        {
            var modelName = _laborTimes.First().ModelName;

            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add($" MANOS DE OBRA-{modelName}");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "REFERENCIA";
                worksheet.Cell(1, 2).Value = "DESCRIPCION";
                worksheet.Cell(1, 3).Value = "DURACION/HORAS";
                worksheet.Cell(1, 4).Value = "ACTIVA";

                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;
                worksheet.Range("A1:D1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _laborTimes.Count; i++)
                {
                    var _laborTime = _laborTimes[i];
                    worksheet.Cell(i + 2, 1).Value = _laborTime.Reference;
                    worksheet.Cell(i + 2, 2).Value = _laborTime.Description;
                    worksheet.Cell(i + 2, 3).Value = _laborTime.Hours;
                    worksheet.Cell(i + 2, 4).Value = _laborTime.IsActive != false ? "SI" : "NO";
       
                }
                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Centra contenido de las columnas 
                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;

            }

        }

        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(Int32 supplierId, Int32 modelId, String? filter)
        {

            try
            {

                List<LaborTime> _laborTimes = await _dLaborTime.GetExport(supplierId, modelId,filter, null);
                MemoryStream _excel = ConvertToExcel(_laborTimes);
                var modelName = _laborTimes.First().ModelName;
                string _fileName = $"Lista de Mano de Obra :{modelName}.xlsx";

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


        [HttpPost("PostLaborTime")]
        public async Task<IActionResult> Post_LaborTime(List<Models.LaborTime> list, Int32 userId)
        {

            try
            {
                var _response = await _dLaborTime.Post_LaborTime(list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        //  ReadExcelTo

        private  async Task<List<LaborTime>> ExceltoPost(IFormFile file, Int32 modelId)
        {
            var _list = new List<LaborTime>();
            var _models = new List<Models.Model>();


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {

                        int fila = row.RowNumber(); // Ej: 2
                        string rowRef = $"{fila}";

                        _list.Add(new LaborTime
                        {
                            Reference = row.Cell(1).GetValue<string>(),
                            Description = row.Cell(2).GetValue<string>(),
                            Hours = row.Cell(3).GetValue<decimal>(),
                            IsActive = row.Cell(4).GetValue<string>() == "SI" ? true : false,
                            ModelId =modelId,
                            RowReference = rowRef
                        });
                    }
                }
            }

            return _list;
        }


        [HttpPost("Import")]
        public async Task<IActionResult> PostImport(IFormFile file, Int32 userId, Int32 modelId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                List<Models.LaborTime> _labortime = await ExceltoPost(file, modelId); // Correct type usage
                var _response = await _dLaborTime.Post_LaborTime(_labortime, userId); // Adjusted method call
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("PostActions")]
        public async Task<IActionResult> PostActions(List<Models.Action> _list, Int32 userId)
        {

            try
            {
                var _response = await _dLaborTime.PostActions(_list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


    }
}
