using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApi.Controllers
{
    [Route("api/Zone")]
    [ApiController]
    public class cZone : ControllerBase
    {

        private readonly dZone _dZone;
        private readonly dWarehouse _dWarehouse;

        public cZone(dZone dZone, dWarehouse dWarehouse)
        {
            _dZone = dZone;
            _dWarehouse = dWarehouse;   
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
        {

            try
            {
                var _response = await _dZone.GetAll(userId, supplierId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 zoneId, Int32 userId)
        {

            try
            {
                var _response = await _dZone.GetOne(zoneId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        private MemoryStream ConvertToExcel(List<Models.Zone> _locations)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("ZONAS");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "ZONA";
                worksheet.Cell(1, 3).Value = "TAMAÑO";
                worksheet.Cell(1, 4).Value = "ALMACEN";
                worksheet.Cell(1, 5).Value = "ACTIVO";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // 6. Llenar los datos
                for (int i = 0; i < _locations.Count; i++)
                {
                    var _contacto = _locations[i];
                    worksheet.Cell(i + 2, 1).Value = _contacto.Id;
                    worksheet.Cell(i + 2, 2).Value = _contacto.Name;
                    worksheet.Cell(i + 2, 3).Value = _contacto.Size;
                    worksheet.Cell(i + 2, 4).Value = _contacto.WarehouseName;
                    worksheet.Cell(i + 2, 5).Value = _contacto.IsActive == true ? "SI" : "NO" ;

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

                List<Zone> _zones = await _dZone.GetExport(userId, supplierId, filter);
                MemoryStream _excel = ConvertToExcel(_zones);
                string _fileName = "Zonas.xlsx";

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




        private async Task<List<Zone>> ExceltoPost(IFormFile file, Int32 userId, Int32? supplierId)
        {
            var _list = new List<Zone>();
            var _warehouses = new List<Warehouse>();
            var id = 0;
            var warehouseName = "";

            _warehouses = await _dWarehouse.GetExport(userId, supplierId,null);

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
                        warehouseName = row.Cell(4).GetValue<string>();
                        int fila = row.RowNumber(); // Ej: 2
                        string rowRef = $"{fila}"; // Ej: "A2"

                        if (_warehouses.Exists(x => x.Name.ToUpper() == warehouseName.ToUpper()))
                        {
                            id = (int)_warehouses.Find(x => x.Name.ToUpper() == warehouseName.ToUpper()).Id;
                        }

                        _list.Add(new Zone
                        {
                            // Mapear las celdas de Excel a las propiedades del modelo
                            // Ajusta según tu estructura real
                            Id = row.Cell(1).GetValue<int>(),
                            Name = row.Cell(2).GetValue<string>(),
                            Size = row.Cell(3).GetValue<string>(),
                            WarehouseId = id,
                           // WarehouseName = row.Cell(4).GetValue<string>(),
                            IsActive = row.Cell(5).GetValue<string>() == "SI" ? true : false,
                            RowReference = rowRef

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

                List<Zone> _zones = await ExceltoPost(file, userId, supplierId);
                var _response = await _dZone.Post_Zones(_zones, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }




        [HttpPost("PostZones")]
        public async Task<IActionResult> Post_Zones(List<Models.Zone> zones, Int32 userId)
        {
            try
            {
                var _response = await _dZone.Post_Zones(zones, userId);
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
                var _response = await _dZone.Post_Actions(actions,userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
    }
}
