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
    [Route("api/License")]
    [ApiController]
    public class cLicense : ControllerBase
    {

      
        private readonly dLicense _dLicense;

        public cLicense(dLicense dLicense)
        {
            _dLicense = dLicense;
        }



        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? filter, Int32 rowFrom, Int32 userId, Int32? supplierId)

        {

            try
            {
                Response _response = await _dLicense.GetAll(filter, rowFrom, userId, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 licenseid,Int32 userId)

        {

            try
            {

                Models.Response _response = await _dLicense.GetOne(licenseid, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        


        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(string? filter, Int32 rowFrom, Int32 licenseId, Int32 userId)

        {

            try
            {
                Response _response = await _dLicense.GetDetails(filter, rowFrom, licenseId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetLicenseType")]
        public async Task<IActionResult> GetServiceType()
        {

            try
            {

                Models.Response _response = await _dLicense.GetLicenseType();
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetDetailBy")]
        public async Task<IActionResult> GetDetailBy(string filter, Int32 userId)

        {

            try
            {

                Models.Response _response = await _dLicense.GetDetailBy(filter, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private MemoryStream ConvertToExcel(List<Models.LicenseDetails> _licenseDetails)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("VEHICULOS EN CAMPAÑA");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "VIN VEHICULO";
                worksheet.Cell(1, 2).Value = "ESTATUS VEHICULO";
                worksheet.Cell(1, 3).Value = "MODELO";
                worksheet.Cell(1, 4).Value = "AÑO";
                worksheet.Cell(1, 5).Value = "NOMBRE CLIENTE";
                worksheet.Cell(1, 6).Value = "APELLIDO CLIENTE";
                worksheet.Cell(1, 7).Value = "CONCESIONARIO";
                worksheet.Cell(1, 8).Value = "ESTATUS LICENCIA";
                worksheet.Cell(1, 9).Value = "ID REPORTE";
                


                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:I1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _licenseDetails.Count; i++)
                {
                    var _licenseDetail = _licenseDetails[i];
                    worksheet.Cell(i + 2, 1).Value = _licenseDetail.Vin;
                    worksheet.Cell(i + 2, 2).Value = _licenseDetail.EstatusVehicle;
                    worksheet.Cell(i + 2, 3).Value = _licenseDetail.Model;
                    worksheet.Cell(i + 2, 4).Value = _licenseDetail.Year;
                    worksheet.Cell(i + 2, 5).Value = _licenseDetail.CustomerName;
                    worksheet.Cell(i + 2, 6).Value = _licenseDetail.CustomerLastName;
                    worksheet.Cell(i + 2, 7).Value = _licenseDetail.DealerName;
                    worksheet.Cell(i + 2, 8).Value = _licenseDetail.EstatusDetail;
                    worksheet.Cell(i + 2, 9).Value = _licenseDetail.ReportId;



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

        [HttpGet("ExportDetails")]
        public async Task<IActionResult> GetExport(string? filter, Int32 userId, Int32 licenseId)

        {

            try
            {

                List<LicenseDetails> _licenseDetails = await _dLicense.GetExportDetails(filter, userId, licenseId);
                MemoryStream _excel = ConvertToExcel(_licenseDetails);
                string firstLicenseName = _licenseDetails.FirstOrDefault()?.LicenseName ?? "SinNombre";
                string _fileName = $"Lista_{firstLicenseName}.xlsx";

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


        [HttpPost("PostLicense")]
        public async Task<IActionResult> Post_PolicyType(Int32 userId, List<Models.License> license)
        {


            try
            {
                Response _response = await _dLicense.Post_License(license, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("PostLicenseDetails")]
        public async Task<IActionResult> Post_PostLicenseDetails(Int32 userId, List<Models.LicenseDetails> licenseDetails)
        {


            try
            {
                Response _response = await _dLicense.Post_LicenseDetails(licenseDetails, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                Response _response = await _dLicense.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("PostDetailsActions")]
        public async Task<IActionResult> Post_DetailsActions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                Response _response = await _dLicense.Post_DetailsActions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        

        private List<LicenseDetails> ExceltoLicenseDetail(IFormFile file, Int32 licenseId)
        {
            var _list = new List<LicenseDetails>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                    foreach (var row in rows)
                    {

                        _list.Add(new LicenseDetails
                        {
                            // Mapear las celdas de Excel a las propiedades del modelo
                            // Ajusta según tu estructura real
                            Id = 0,
                            Vin = row.Cell(1).GetValue<string>(),
                            LicenseId = licenseId
                        });
                    }
                }
            }

            return _list;
        }

        [HttpPost("ImportLicenseDetails")]
        public async Task<IActionResult> ImportLicenseDetails(IFormFile file, Int32 userId, Int32 licenseId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                List<LicenseDetails> _details = ExceltoLicenseDetail(file, licenseId);
                Response _response = await _dLicense.Post_LicenseDetails(_details, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("DeleteLicenseDetail")]
        public async Task<IActionResult> DeleteLicenseDetail(List<Models.Action> _list, Int32 userId)
        {

            try
            {
                Response _response = await _dLicense.DeleteLicenseDetail(_list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }




    }
}
