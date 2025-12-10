
using Azure;
using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;



namespace WebApi.Controllers
{
    [Route("api/Part")]
    [ApiController]
    [Authorize]
    public class cPart : Controller
    {
        private readonly dPart _dPart;
        private readonly dContact _dContact;
        private readonly dBrand _dBrand;

        public cPart(dPart dPart, dContact dContact, dBrand dBrand)
        {
            _dPart = dPart;
            _dContact = dContact;
            _dBrand = dBrand;   
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
        {

            try
            {
                var _response = await _dPart.GetAll(userId, supplierId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetFamilies")]
        public async Task<IActionResult> GetFamilies()
        {

            try
            {
                var _response = await _dPart.GetFamily();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpGet("GetSubFamilies")]
        public async Task<IActionResult> GetSubFamilies()
        {

            try
            {
                var _response = await _dPart.GetSubFamily();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


        [HttpGet("GetUms")]
        public async Task<IActionResult> GetUm()
        {

            try
            {
                var _response = await _dPart.GetUm();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetTaxes")]
        public async Task<IActionResult> GetTaxes()
        {

            try
            {
                var _response = await _dPart.GetTaxes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpGet("GetPartTypes")]
        public async Task<IActionResult> GetPartTypes()
        {

            try
            {
                var _response  = await _dPart.GetPartType();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 userId,Int32 partId )
        {

            try
            {
                var _response = await _dPart.GetOne(userId,partId );
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }



        [HttpGet("GetByModel")]
        public async Task<IActionResult> GetByModel(Int32 userId, Int32 supplierId, Int32? rowfrom, string? filter, Int32? modelId, bool? isSell = null)
        {

            try
            {
                var _response = await _dPart.GetByModel(userId, supplierId, rowfrom, filter, modelId, isSell);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetOneWithContext")]
        public async Task<IActionResult> GetOneWithContext(Int32 userId,Int32 partId )
        {
            Models.Response<PartWithContext> _response = new Models.Response<PartWithContext>();
            try
            {
                PartWithContext _data = new PartWithContext();

                _data.Part = (Part)((await _dPart.GetOne(userId, partId)).Data);
                _data.Types = (List<PartType>)((await _dPart.GetPartType()).Data);
                _data.Families = (List<Family>)((await _dPart.GetFamily()).Data);
                _data.SubFamilies = (List<SubFamily>)((await _dPart.GetSubFamily()).Data);
                _data.Taxes = (List<Tax>)((await _dPart.GetTaxes()).Data);
                _data.Ums = (List<Um>)((await _dPart.GetUm()).Data);
                _data.Suppliers = (List<Contact>)((await _dContact.GetAll_by( false,true )));
                _data.Brands = await _dBrand.GetAll(null);
                _data.Models = (List<RelatedModel>)((await _dPart.GetModels(partId)).Data);

                _response.Data = _data;
                _response.Total = 1;

                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                _response.Processed = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status409Conflict, _response);
            }


        }


        private MemoryStream ConvertToExcel(List<Models.Part> _parts)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("PARTES");

                // 4. Agregar los encabezados
                //worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 1).Value = "CODPARTE";
                worksheet.Cell(1, 2).Value = "PARTE";
                worksheet.Cell(1, 3).Value = "PRECIO $";
                worksheet.Cell(1, 4).Value = "COSTO $";
                worksheet.Cell(1, 5).Value = "IMPUESTO";
                worksheet.Cell(1, 6).Value = "ACTIVO";
                worksheet.Cell(1, 7).Value = "CODFABRICANTE";
                worksheet.Cell(1, 8).Value = "CODALTERNO";
                worksheet.Cell(1, 9).Value = "CODREEMPLAZO";
                worksheet.Cell(1, 10).Value = "CODBARRA";
                worksheet.Cell(1, 11).Value = "TIPO";
                worksheet.Cell(1, 12).Value = "FAMILIA";
                worksheet.Cell(1, 13).Value = "SUBFAMILIA";
                worksheet.Cell(1, 14).Value = "MARCA";
                worksheet.Cell(1, 15).Value = "UM";
                worksheet.Cell(1, 16).Value = "PLANTA";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:G1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // 6. Llenar los datos
                for (int i = 0; i < _parts.Count; i++)
                {
                    var _item = _parts[i];

                   // worksheet.Cell(i + 2, 1).Value = _item.Id;
                    worksheet.Cell(i + 2, 1).Value = _item.InnerCode;
                    worksheet.Cell(i + 2, 2).Value = _item.Description;
                    worksheet.Cell(i + 2, 3).Value = _item.Price;
                    worksheet.Cell(i + 2, 4).Value = _item.Cost;
                    worksheet.Cell(i + 2, 5).Value = _item.TaxName;
                    worksheet.Cell(i + 2, 6).Value = _item.IsActive == true ? "SI" : "NO";
                    worksheet.Cell(i + 2, 7).Value = _item.MasterCode;
                    worksheet.Cell(i + 2, 8).Value = _item.AlterCode;
                    worksheet.Cell(i + 2, 9).Value = _item.ReplacementCode;
                    worksheet.Cell(i + 2, 10).Value = _item.BarCode;
                    worksheet.Cell(i + 2, 11).Value = _item.TypeName;
                    worksheet.Cell(i + 2, 12).Value = _item.FamilyName;
                    worksheet.Cell(i + 2, 13).Value = _item.SubFamilyName;
                    worksheet.Cell(i + 2, 14).Value = _item.BrandName;
                    worksheet.Cell(i + 2, 15).Value = _item.UmName;
                    worksheet.Cell(i + 2, 16).Value = _item.SupplierReference;


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

                List<Part> _parts = await _dPart.GetExport(userId,  supplierId, filter);
                MemoryStream _excel = ConvertToExcel(_parts);
                string _fileName = "Repuestos.xlsx";

                return File(
                 _excel,
                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 _fileName);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        
        
        
        //  ReadExcelTo

        private async Task<List<Part>> ExceltoPost(IFormFile file,Int32? supplierId)
        {
            var _list = new List<Part>();
            var _plantas = new List<Contact>();
            var response = new Models.Response<Models.Result>();

            _plantas = await _dContact.GetAll_by(false, true);


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados


                    decimal priceValue;
                    decimal costValue;
                    foreach (var row in rows)
                    {
                        int fila = row.RowNumber(); // Ej: 2
                        string rowRef = $"{fila}";
                        try
                        {
                            _list.Add(new Part
                            {

                                InnerCode = row.Cell(1).GetValue<string>(),
                                Description = row.Cell(2).GetValue<string>(),
                                Price = string.IsNullOrWhiteSpace(row.Cell(3).GetString()) ? 0 : row.Cell(3).GetValue<decimal>(),
                                Cost = string.IsNullOrWhiteSpace(row.Cell(4).GetString()) ? 0 : row.Cell(4).GetValue<decimal>(),
                                TaxId = row.Cell(5).GetValue<string>().ToUpper() switch
                                {
                                    "EXENTO" => 1,
                                    "GRAVABLE" => 2,
                                    _ => throw new Exception($"Valor inválido en TaxId: '{row.Cell(5).GetValue<string>()}'. Se esperaba 'EXENTO' o 'GRAVADO'.")
                                },
                                IsActive = row.Cell(6).GetValue<string>().ToUpper() switch
                                {
                                    "SI" => true,
                                    "NO" => false,
                                    _ => throw new Exception($"Valor inválido en ACTIVO. Se esperaba 'SI' o 'NO'. FILA-{rowRef}")
                                },

                                MasterCode = row.Cell(7).GetValue<string>(),
                                SupplierId = supplierId,
                                RowReference = rowRef

                            });
                        }
                        catch (Exception ex)
                        {
                            response.SetError(ex);

                        }

                    }
                }
            }

            return _list;
        }




        [HttpPost("Import")]
        public async Task<IActionResult> PostImport(IFormFile file, Int32 userId,Int32 supplierId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                List<Models.RelatedModel> _models = new List<Models.RelatedModel>();
                List<Part> _parts = await ExceltoPost(file,supplierId);
                var _response = await _dPart.Import_Parts(_parts, _models,userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("PostPart")]
        public async Task<IActionResult> PostPart(PartRequest partRequest, Int32 userId)
        {
            try
            {
                List<Part> _parts = new List<Part>();
                _parts.Add(partRequest.Part);
                var _response = await _dPart.Post_Parts(_parts, partRequest.Models, userId,true);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostParts")]
        public async Task<IActionResult> PostParts(List <Models.Part> parts, Int32 userId)
        {
            try
            {
                List<RelatedModel > _models = new List<RelatedModel>();
                var _response = await _dPart.Post_Parts(parts, _models, userId,true);
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
                var _response = await _dPart.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }



    }
}
