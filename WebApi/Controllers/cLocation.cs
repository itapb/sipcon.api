using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using ClosedXML.Excel;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApi.Controllers
{
    [Route("api/Location")]
    [ApiController]
    public class cLocation : ControllerBase
    {
        private readonly dLocation _dLocation;
        private readonly dZone  _dZone;

        public cLocation(dLocation dLocation, dZone dZone)
        {
            _dLocation = dLocation;
            _dZone = dZone; 
        }



        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
        {

            try
            {
                var _response = await _dLocation.GetAll(userId, supplierId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 locationId, Int32 userId)
        {

            try
            {
                var _response = await _dLocation.GetOne(locationId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


        private MemoryStream ConvertToExcel(List<Models.Location> _locations)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("UBICACIONES");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "UBICACION";
               // worksheet.Cell(1, 3).Value = "IDZONA";
                worksheet.Cell(1, 3).Value = "ZONA";
                worksheet.Cell(1, 4).Value = "TIPO";
                worksheet.Cell(1, 5).Value = "MAPEO";
                // worksheet.Cell(1, 5).Value = "ALMACEN";
                worksheet.Cell(1, 6).Value = "ACTIVO";


                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // 6. Llenar los datos
                for (int i = 0; i < _locations.Count; i++)
                {

                    var _loc = _locations[i];
                    worksheet.Cell(i + 2, 1).Value = _loc.Id;
                    worksheet.Cell(i + 2, 2).Value = _loc.Name;
                   // worksheet.Cell(i + 2, 3).Value = _loc.ZoneId;
                    worksheet.Cell(i + 2, 3).Value = _loc.ZoneName;
                    worksheet.Cell(i + 2, 4).Value = _loc.TypeId;
                    worksheet.Cell(i + 2, 5).Value = _loc.Mapping;
                    //  worksheet.Cell(i + 2, 5).Value = _loc.WarehouseName;
                    worksheet.Cell(i + 2, 6).Value = _loc.IsActive == true ? "SI" : "NO"    ; 

                    
                }

                // 7. Ajustar el ancho de las columnas al contenido
                worksheet.Columns().AdjustToContents();

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;


                //// 9. Retornar el archivo Excel
                //return System.IO.File(
                //    content,
                //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                //    "PersonasExportadas.xlsx");
            }

        }

        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(Int32 userId, Int32? supplierId, string? filter)
        {

            try
            {

                List <Location> _locations = await _dLocation.GetExport(userId,  supplierId, filter);
                MemoryStream _excel = ConvertToExcel(_locations);
                string _fileName = "Ubicaciones.xlsx";

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


        //  ReadExcelTo

        private async  Task<List<Location>> ExceltoPost(IFormFile file, Int32 userId, Int32? supplierId)
        {
            var _list = new List<Location>();
            var _zones = new List<Zone>();
            var id = 0;
            var zoneName = "";

            _zones = await _dZone.GetExport(userId, supplierId,null);

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {
                        id = 0;
                        zoneName = row.Cell(3).GetValue<string>();

                        if (_zones.Exists(x  => x.Name.ToUpper() == zoneName.ToUpper()))
                        {
                            id = (int)_zones.Find(x => x.Name.ToUpper() == zoneName.ToUpper()).Id;
                        }

                        _list.Add(new Location
                        {
                            // Mapear las celdas de Excel a las propiedades del modelo
                            // Ajusta según tu estructura real
                            Id = row.Cell(1).GetValue<int>(),
                            Name = row.Cell(2).GetValue<string>(),
                            ZoneId = id,
                            //ZoneName = row.Cell(4).GetValue<string>(),
                            //WarehouseName = row.Cell(5).GetValue<string>(),
                            IsActive = row.Cell(4).GetValue<string>() == "SI" ? true : false,
                           
                        });
                    }
                }
            }

            return _list;
        }




        [HttpPost("Import")]
        public async Task<IActionResult> PostImport(IFormFile file, Int32 userId, Int32? supplierId )
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<Location> _locations = await ExceltoPost(file, userId, supplierId);
                var _response = await _dLocation.Post_Locations(_locations, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }





        [HttpPost("PostLocations")]
        public async Task<IActionResult> Post_Locations(List<Models.Location> locations, Int32 userId)
        {
            try
            {
                var _response = await _dLocation.Post_Locations(locations, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions,Int32 userId)
        {

            try
            {
                var _response = await _dLocation.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
    }
}
