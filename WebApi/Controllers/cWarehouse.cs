using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApi.Controllers
{
    [Route("api/Warehouse")]
    [ApiController]
    public class cWarehouse : ControllerBase
    {

        private readonly dWarehouse _dWarehouse;
        private readonly dContact _dContact;

        public cWarehouse(dWarehouse dWarehouse, dContact dContact)
        {
            _dWarehouse = dWarehouse;
            _dContact = dContact;   
        }


        //getall
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId, Int32 rowFrom, string? filter)
        {

            try
            {
                var _response = await _dWarehouse.GetAll(userId, supplierId, rowFrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }
        //getone
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 warehouseId, Int32 userId)
        {

            try
            {
                var _response = await _dWarehouse.GetOne(warehouseId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        //Exportar

        private MemoryStream ConvertToExcel(List<Models.Warehouse> _locations)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("ALMACENES");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "ALMACEN";
               // worksheet.Cell(1, 3).Value = "IDPLANTA";
                worksheet.Cell(1, 3).Value = "PLANTA";
               // worksheet.Cell(1, 5).Value = "MARCA";
                worksheet.Cell(1, 4).Value = "ACTIVO";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // 6. Llenar los datos
                for (int i = 0; i < _locations.Count; i++)
                {
                    var _contacto = _locations[i];
                    worksheet.Cell(i + 2, 1).Value = _contacto.Id;
                    worksheet.Cell(i + 2, 2).Value = _contacto.Name;
                    //worksheet.Cell(i + 2, 3).Value = _contacto.SupplierId;
                    worksheet.Cell(i + 2, 3).Value = _contacto.SupplierName;
                    //worksheet.Cell(i + 2, 5).Value = _contacto.BrandName;
                    worksheet.Cell(i + 2, 4).Value = _contacto.IsActive == true ? "SI" : "NO";

                }

                // 7. Ajustar el ancho de las columnas al contenido
                worksheet.Columns().AdjustToContents();

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;


            }

        }

        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(Int32 userId, Int32? supplierId, string? filter)
        {
            try
            {

                List<Warehouse> _warehouses = await _dWarehouse.GetExport(userId, supplierId, filter);
                MemoryStream _excel = ConvertToExcel(_warehouses);
                string _fileName = "Almacenes.xlsx";

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



        private async Task<List<Warehouse>> ExceltoPost(IFormFile file)
        {
            var _list = new List<Warehouse>();
            var _plantas = new List<Contact>();
            var id = 0;
            var supplierReference = "";

            _plantas = await _dContact.GetAll_by( false ,true);

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
                        supplierReference = row.Cell(3).GetValue<string>();

                        if (_plantas.Exists(x => x.Reference.ToUpper() == supplierReference.ToUpper()))
                        {
                            id = (int)_plantas.Find(x => x.Reference.ToUpper() == supplierReference.ToUpper()).Id;
                        }
                        _list.Add(new Warehouse
                        {
                            // Mapear las celdas de Excel a las propiedades del modelo
                            // Ajusta según tu estructura real
                            Id = row.Cell(1).GetValue<int>(),
                            Name = row.Cell(2).GetValue<string>(),
                            SupplierId = id,
                            IsActive = row.Cell(4).GetValue<string>() == "SI" ? true : false,

                        });
                    }
                }
            }

            return _list;
        }

        [HttpPost("Import")]
        public async Task<IActionResult> PostImport(IFormFile file, Int32 userId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<Warehouse> _wh = await ExceltoPost(file);
                var _response = await _dWarehouse.Post_Warehouses(_wh, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }



        [HttpPost("PostWarehouses")]
        public async Task<IActionResult> Post_Warehouses(List<Models.Warehouse> warehouses, Int32 userId)
        {
            try
            {
                var _response = await _dWarehouse.Post_Warehouses(warehouses, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        //actions
        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions,Int32 userId)
        {

            try
            {
                var _response = await _dWarehouse.Post_Actions(actions,userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
    }
}
