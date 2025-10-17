using ClosedXML.Excel;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace WebApi.Controllers
{
    [Route("api/Templates")]
    [ApiController]
    [Authorize]

    public class cTemplate: ControllerBase
    {

        private readonly dTemplate _dTemplate;

        public cTemplate(dTemplate dTemplate)
        {
            _dTemplate = dTemplate;
        }



        private List<InventoryTamplate> ExcelToInventoryTamplate(IFormFile file)
        {
            var _list = new List<InventoryTamplate>();


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {

                        try
                        {
                            _list.Add(new InventoryTamplate
                            {
                                InnerCode = row.Cell(1).GetValue<string>(),
                                MasterCode = row.Cell(2).GetValue<string>(),
                                Description = row.Cell(3).GetValue<string>(),
                                Price = row.Cell(4).GetValue<decimal>(),
                                Cost = row.Cell(5).GetValue<decimal>(),
                                Tax = row.Cell(6).GetValue<string>(),
                                Type = row.Cell(7).GetValue<string>(),
                                Family = row.Cell(8).GetValue<string>(),
                                SubFamily = row.Cell(9).GetValue<string>(),
                                Size = row.Cell(10).GetValue<string>(),
                                Warehouse = row.Cell(11).GetValue<string>(),
                                Zone = row.Cell(12).GetValue<string>(),
                                Location = row.Cell(13).GetValue<string>(),
                                Stock = row.Cell(15).GetValue<int>()

                            });
                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            }

            return _list;
        }

        private List<LocationTemplate> ExcelToLocationTamplate(IFormFile file)
        {
            var _list = new List<LocationTemplate>();


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {

                        try
                        {
                            _list.Add(new LocationTemplate
                            {
                               
                                Warehouse = row.Cell(1).GetValue<string>(),
                                Zone = row.Cell(2).GetValue<string>(),
                                Location = row.Cell(3).GetValue<string>(),
                                Size = row.Cell(4).GetValue<string>()


                            });
                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            }

            return _list;
        }

        private List<PartModelTemplate> ExcelToPartModelTamplate(IFormFile file)
        {
            var _list = new List<PartModelTemplate>();


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {

                        try
                        {
                            _list.Add(new PartModelTemplate
                            {

                                InnerCode = row.Cell(1).GetValue<string>(),
                                ModelName = row.Cell(3).GetValue<string>()
  

                            });
                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            }

            return _list;
        }

        private List<ContactTemplate> ExcelToContactTamplate(IFormFile file)
        {
            var _list = new List<ContactTemplate>();


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {

                        try
                        {
                            _list.Add(new ContactTemplate
                            {

                                Reference = row.Cell(1).GetValue<string>(),
                                Name = row.Cell(2).GetValue<string>(),
                                Vat = row.Cell(3).GetValue<string>(),
                                Address = row.Cell(4).GetValue<string>(),
                                CityName = row.Cell(5).GetValue<string>(),
                                SupplierName = row.Cell(6).GetValue<string>()


                            });
                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            }

            return _list;
        }


        [HttpPost("/api/Templates/ImportInventoryTemplate")]
        public async Task<IActionResult> ImportInventoryTemplate(IFormFile file, Int32 supplierId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<InventoryTamplate> _list = ExcelToInventoryTamplate(file);

                var _response = await _dTemplate.ImportInventoryTemplate(_list, supplierId);

                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("/api/Templates/ImportLocationTemplate")]
        public async Task<IActionResult> ImportLocationTemplate(IFormFile file, Int32 supplierId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<LocationTemplate> _list = ExcelToLocationTamplate(file);

                var _response = await _dTemplate.ImportLocationTemplate(_list, supplierId);

                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("/api/Templates/ImportPartModelTemplate")]
        public async Task<IActionResult> ImportPartModelTemplate(IFormFile file, Int32 supplierId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<PartModelTemplate> _list = ExcelToPartModelTamplate(file);

                var _response = await _dTemplate.ImportPartModelTemplate(_list, supplierId);

                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Templates/ImportContactTemplate")]
        public async Task<IActionResult> ImportContactTemplate(IFormFile file)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<ContactTemplate> _list = ExcelToContactTamplate(file);

                var _response = await _dTemplate.ImportContactTemplate(_list);

                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

    }
}
