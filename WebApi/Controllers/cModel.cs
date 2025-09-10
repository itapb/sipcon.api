using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;
using Model = Models.Model;
using Microsoft.AspNetCore.Authorization;



namespace WebApi.Controllers
{
    [Route("api/Model")]
    [ApiController]
    [Authorize]
    public class cModel : ControllerBase
    {
        

        private readonly dModel _dModel;
        private readonly dBrand _dBrand;
        private readonly dPolicyType _dPolicyType;

        public cModel(dModel dModel, dBrand dBrand, dPolicyType dPolicyType)
        {
            _dModel = dModel;
            _dBrand = dBrand;
            _dPolicyType=dPolicyType;
        }

 
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId, Int32 rowFrom, string? filter  )
        {
           
            try
            {
                var _response = await _dModel.GetAll(userId, supplierId, rowFrom,filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }    

        }
    
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 userId,Int32 modelId)
        {
        
            try
            {
                var _response  = await _dModel.GetOne(userId,modelId );
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        private MemoryStream ConvertToExcel(List<Models.Model> _models)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("MODELOS");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "NOMBRE";
                worksheet.Cell(1, 3).Value = "DESCRIPCION";
                worksheet.Cell(1, 4).Value = "MARCA";
                worksheet.Cell(1, 5).Value = "TIPO DE POLIZA";
                worksheet.Cell(1, 6).Value = "ACTIVA";
           
                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;
                worksheet.Range("A1:F1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _models.Count; i++)
                {
                    var _model = _models[i];
                    worksheet.Cell(i + 2, 1).Value = _model.Id;
                    worksheet.Cell(i + 2, 2).Value = _model.Name;
                    worksheet.Cell(i + 2, 3).Value = _model.Description;
                    worksheet.Cell(i + 2, 4).Value = _model.BrandName;
                    worksheet.Cell(i + 2, 5).Value = _model.PolicyTypeName;
                    worksheet.Cell(i + 2, 6).Value = _model.IsActive != false ? "SI" : "NO";

                }
                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Centra contenido de las columnas 
                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;

            }

        }

   
        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(Int32 userId, Int32? supplierId, string? _filter )
        {
            
            try
            {

                List<Models.Model> _models = await _dModel.GetExport(userId,supplierId, _filter);
                MemoryStream _excel = ConvertToExcel(_models);
                string _fileName = "Modelos.xlsx";

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


        private async Task<List<Models.PolicyType>> GetPolicyTypeAsync(Int32 userId, Int32 supplierId)
        {

            var _responsePolicyType = await _dPolicyType.GetAll(null,0,userId,supplierId,null);
            var _list = (List<Models.PolicyType>)_responsePolicyType.Data;
            return _list;

        }


        private async Task<List<Models.Model>> ReadExcelToModels(IFormFile file,Int32 userId,Int32 supplierId)
        {
            var models = new List<Models.Model>();
            var _brands = await _dBrand.GetAll(null);
            var _policyTypes = await GetPolicyTypeAsync(userId,supplierId);
            var idbrand = 0;
            var idPolicyType = 0;
            string brandName = "";
            string policyTypeName = "";
            

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                       
                    {
                        brandName = row.Cell(4).GetValue<string>()?.Trim();
                        policyTypeName = row.Cell(5).GetValue<string>()?.Trim();

                        idbrand = _brands.Exists(x => x.Name.ToUpper() == brandName.ToUpper()) ? (int)_brands.Find(x => x.Name.ToUpper() == brandName.ToUpper()).Id : 0;
                        idPolicyType = _policyTypes.Exists(x => x.Description.ToUpper() == policyTypeName.ToUpper()) ? (int)_policyTypes.Find(x => x.Description.ToUpper() == policyTypeName.ToUpper()).Id : 0;


                        models.Add(new Model
                        {
                            // Mapear las celdas de Excel a las propiedades del modelo
                            // Ajusta según tu estructura real
                            Id = row.Cell(1).GetValue<int>(),
                            Name = row.Cell(2).GetValue<string>(),
                            Description = row.Cell(3).GetValue<string>(),
                            BrandId = idbrand,
                            PolicyTypeId = idPolicyType,
                            IsActive = row.Cell(6).GetValue<string>() == "SI" ? true : false
                    
                        });
                    }
                }
            }

            return models;
        }


        
        [HttpPost("Import")]
        public async Task<IActionResult> Import(IFormFile file, Int32 userId,Int32 supplierId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                // Leer el archivo Excel y convertirlo a List<PolicyType>
                List<Model> models = await ReadExcelToModels(file, userId, supplierId);

                // Llamar al método existente de tu capa de servicio
                var response = await _dModel.Post_Models(models, userId);

                return StatusCode(response.Processed ?
                    StatusCodes.Status200OK : StatusCodes.Status409Conflict,
                    response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

     
        [HttpPost("PostModels")]
        public async Task<IActionResult> Post_Models(List<Models.Model> models, Int32 userId )
        {

            try
            {
                var _response = await _dModel.Post_Models(models, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                var _response = await _dModel.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


       

    }
}
