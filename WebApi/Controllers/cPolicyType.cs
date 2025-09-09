using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;


namespace WebApi.Controllers
{
    [Route("api/PolicyType")]
    [ApiController]
    public class cPolicyType : ControllerBase
    {

      
        private readonly dPolicyType _dPolicyType;
        private readonly dBrand _dBrand;
        public cPolicyType(dPolicyType dPolicyType, dBrand dBrand)
        {
            _dPolicyType = dPolicyType;
            _dBrand = dBrand;
        }



        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? filter, Int32 rowFrom, Int32 userId, Int32? supplierId, Int32? brandId)

        {

            try
            {
                var _response = await _dPolicyType.GetAll(filter, rowFrom, userId, supplierId, brandId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private MemoryStream ConvertToExcel(List<Models.PolicyType> _policiesTypes)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("TIPOS DE POLIZAS");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "DESCRIPCION";
                worksheet.Cell(1, 3).Value = "KM";
                worksheet.Cell(1, 4).Value = "TOLERANCIA KM";
                worksheet.Cell(1, 5).Value = "TOPE KM";
                worksheet.Cell(1, 6).Value = "MESES";
                worksheet.Cell(1, 7).Value = "TOLERANCIA MESES";
                worksheet.Cell(1, 8).Value = "TOPE MESES";
                worksheet.Cell(1, 9).Value = "PLANTA";
                worksheet.Cell(1, 10).Value = "MARCA";
                worksheet.Cell(1, 11).Value = "ACTIVA";
                
                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:K1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;
              
                worksheet.Range("A1:K1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _policiesTypes.Count; i++)
                {
                    var _policyType = _policiesTypes[i];
                    worksheet.Cell(i + 2, 1).Value = _policyType.Id;
                    worksheet.Cell(i + 2, 2).Value = _policyType.Description;
                    worksheet.Cell(i + 2, 3).Value = _policyType.Km;
                    worksheet.Cell(i + 2, 4).Value = _policyType.GapKm;
                    worksheet.Cell(i + 2, 5).Value = _policyType.TopKm;
                    worksheet.Cell(i + 2, 6).Value = _policyType.Months;
                    worksheet.Cell(i + 2, 7).Value = _policyType.GapMonths;
                    worksheet.Cell(i + 2, 8).Value = _policyType.TopMonths;
                    worksheet.Cell(i + 2, 9).Value = _policyType.SupplierId;
                    worksheet.Cell(i + 2, 10).Value = _policyType.BrandName;
                    worksheet.Cell(i + 2, 11).Value = _policyType.IsActive != false ? "SI" : "NO";

                }
                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Centra contenido de las columnas 
                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                // 9. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;

            }

        }

        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(Int32 userId, Int32? supplierId, string? filter )

        {

            try
            {

                List<PolicyType> _policiesTypes = await _dPolicyType.GetExport(filter, userId, supplierId);
                MemoryStream _excel = ConvertToExcel(_policiesTypes);
                string _fileName = "TiposPolizas.xlsx";

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


        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 userId, Int32 policyTypeId)

        {

            try
            {

                var _response = await _dPolicyType.GetOne(policyTypeId, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("PostPolicyType")]
        public async Task<IActionResult> Post_PolicyType(Int32 userId, List<Models.PolicyType> policyTypes)
        {
           

            try
            {
                var _response = await _dPolicyType.Post_PolicyType(policyTypes, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }



        private async Task<List<PolicyType>> ReadExcelToPolicyTypes(IFormFile file)
        {

          
            var policyTypes = new List<PolicyType>();
            var id = 0;
            string brandName = "";

            var _brands = await _dBrand.GetAll(null);

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {
                        brandName = row.Cell(10).GetValue<string>()?.Trim();
                        id = _brands.Exists(x => x.Name.ToUpper() == brandName.ToUpper()) ? (int)_brands.Find(x => x.Name.ToUpper() == brandName.ToUpper()).Id : 0;
                        int fila = row.RowNumber(); // Ej: 2
                        string rowRef = $"{fila}"; // Ej: "A2"

                        policyTypes.Add(new PolicyType
                        {
                            // Mapear las celdas de Excel a las propiedades del modelo
                            // Ajusta según tu estructura real
                            Id = row.Cell(1).GetValue<int>(),
                            Description = row.Cell(2).GetValue<string>(),
                            Km = row.Cell(3).GetValue< int >(),
                            GapKm = row.Cell(4).GetValue<int>(),
                            TopKm = row.Cell(5).GetValue<int>(),
                            Months = row.Cell(6).GetValue<int>(),
                            GapMonths= row.Cell(7).GetValue<int>(),
                            TopMonths= row.Cell(8).GetValue< int >(),
                            SupplierId = row.Cell(9).GetValue<int>(),
                            BrandId = id,
                            IsActive = row.Cell(11).GetValue<string>() == "SI" ? true : false,
                            RowReference = rowRef
                        });
                    }
                }
            }

            return policyTypes;
        }

        
        [HttpPost("Import")]
        public async Task<IActionResult> Import(Int32 userId,IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                // Leer el archivo Excel y convertirlo a List<PolicyType>
                List<PolicyType> policyTypes = await ReadExcelToPolicyTypes(file);

                // Llamar al método existente de tu capa de servicio
                var response = await _dPolicyType.Post_PolicyType(policyTypes, userId);

                return StatusCode(response.Processed ?
                    StatusCodes.Status200OK : StatusCodes.Status409Conflict,
                    response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        

        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                var _response = await _dPolicyType.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


    }
}
