
using Azure;
using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Models;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Controllers
{
    [Route("api/Vehicle")]
    [ApiController]
    public class cVehicle : ControllerBase
    {

      
        private readonly dVehicle _dVehicle;
        private readonly dContact _dContact;
        private readonly dModel _dModel;
        private readonly dColor _dColor;
        public cVehicle(dVehicle dVehicle, dContact dContact, dModel dModel, dColor dColor)
        {
            _dVehicle = dVehicle;
            _dContact = dContact;
            _dModel = dModel;
            _dColor = dColor;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId, Int32? dealerId, Int32 rowFrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {

            try
            {
                var _response = await _dVehicle.GetAll(userId, supplierId, dealerId,rowFrom, filter, fromDate, upToDate, estatusId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetVehiclesInvoiced")]
        public async Task<IActionResult> GetVehiclesInvoiced(Int32 userId, Int32? supplierId, Int32? dealerId, Int32 rowFrom, string? filter)
        {

            try
            {
                var _response = await _dVehicle.GetVehiclesInvoiced(userId, supplierId, dealerId, rowFrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 userId,Int32 vehicleId)
        {

            try
            {
                var _response = await _dVehicle.GetOne(userId,vehicleId );
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOneBy")]
        public async Task<IActionResult> GetOneBy(Int32 userId, Int32? dealerId,string filter, Int32 filterBy)
        {

            try
            {
                var _response = await _dVehicle.GetOneBy(userId, dealerId, filter, filterBy);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetVehicleFullBy")]
        public async Task<IActionResult> GetVehicleFullBy(Int32 userId, string filter, Int32 filterBy,Int32? supplierId)
        {

            try
            {
                Models.Response<Models.VehicleFull> _response = await _dVehicle.GetVehicleFullBy(userId, filter, filterBy,supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetAllAvailables")]
        public async Task<IActionResult> GetAllAvailables(Int32 userId, Int32 dealerId, Int32 rowFrom, string? filter)
        {

            try
            {
                var _response = await _dVehicle.GetAllAvailables(userId, dealerId, rowFrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOneAvailable")]
        public async Task<IActionResult> GetOneAvailable(Int32 userId, Int32 dealerId, string VinOrPlate)
        {

            try
            {
                var _response = await _dVehicle.GetOneAvailable(userId, dealerId, VinOrPlate);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }



        [HttpGet("GetRecordVehicle")]
        public async Task<IActionResult> GetRecordVehicle(String vin, int supplierId, int userId)
        {
            Models.Response<VehicleRecord> _response = new Models.Response<VehicleRecord>();
            String filter = vin;

            try
            {
                VehicleRecord _data = new VehicleRecord();

                _data.Vehicle = (Vehicle)((await _dVehicle.GetVehicleFullBy(userId, filter, 0, supplierId)).Data);
                _data.Customer = (CustomerVehicleRecord)((await _dVehicle.GetCustomerRecordBy(userId, filter, 0, supplierId)).Data);
                _data.Policy = (policyVehicleRecord)((await _dVehicle.GetPolicyRecordFullBy(userId, filter, 0, supplierId)).Data);
                _data.EstatusRecord = (List<EstatusRecord>)((await _dVehicle.GetEstatusRecord(vin, supplierId, userId)).Data);
                _data.ServiceRecord = (List<ServiceRecord>)((await _dVehicle.GetServiceRecord(vin,supplierId,userId)).Data);
               
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

        private async Task<List<Models.Contact>> GetContactsAsync()
        {
            List<Models.Contact> contacts = await _dContact.GetAll_by(true, true);
            return contacts;
        }

        private async Task<List<Models.Color>> GetColorAsync()
        {

            var _responseColor = await _dColor.GetAll();
            var _list = (List<Models.Color>)_responseColor.Data;
            return _list;

        }

        private async Task<List<Models.Model>> GetModelAsync(Int32 userId, Int32? supplierId)
        {

            var _responseModel = await _dModel.GetAll(userId, supplierId,null,null);
            var _list = (List<Models.Model>)_responseModel.Data;
            return _list;

        }




        private MemoryStream ConvertToExcel(List<Models.Vehicle> _vehicles)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("VEHICULOS");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "VIN";
                worksheet.Cell(1, 2).Value = "SERIALMOTOR";
                worksheet.Cell(1, 3).Value = "COLOR";
                worksheet.Cell(1, 4).Value = "MODELO";
                worksheet.Cell(1, 5).Value = "AÑO";
                worksheet.Cell(1, 6).Value = "PLACA";
                worksheet.Cell(1, 7).Value = "ACTIVA";
                worksheet.Cell(1, 8).Value = "CONCESIONARIO";
                worksheet.Cell(1, 9).Value = "ESTATUS";
                worksheet.Cell(1, 10).Value = "PLANTA";
                


                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:J1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _vehicles.Count; i++)
                {
                    var _vehicle = _vehicles[i];
                    worksheet.Cell(i + 2, 1).Value = _vehicle.Vin;
                    worksheet.Cell(i + 2, 2).Value = _vehicle.EngineSerial;
                    worksheet.Cell(i + 2, 3).Value = _vehicle.ColorName;
                    worksheet.Cell(i + 2, 4).Value = _vehicle.ModelName;
                    worksheet.Cell(i + 2, 5).Value = _vehicle.Year;
                    worksheet.Cell(i + 2, 6).Value = _vehicle.Plate;
                    worksheet.Cell(i + 2, 7).Value = _vehicle.IsActive != false ? "SI" : "NO"; 
                    worksheet.Cell(i + 2, 8).Value = _vehicle.DealerReference;
                    worksheet.Cell(i + 2, 9).Value = _vehicle.EstatusName;
                    worksheet.Cell(i + 2, 10).Value = _vehicle.SupplierReference;
                   


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
        public async Task<IActionResult> GetExport(Int32 userId, Int32? supplierId, Int32? dealerId, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {

            try
            {

                List<Vehicle> _vehicles = await _dVehicle.GetExport(userId,  supplierId,  dealerId, filter, fromDate, upToDate, estatusId);
                MemoryStream _excel = ConvertToExcel(_vehicles);
                string _fileName = "Vehiculos.xlsx";

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


        private async Task<List<Vehicle>> ReadExcelToVehicles(IFormFile file, Int32 userId, Int32 supplierId)
        {
            var vehicles = new List<Vehicle>();
            var _contacts = await GetContactsAsync();
           // List<Models.Contact> _contacts = await _dContact.GetDealers(supplierId);
            var _colors = await GetColorAsync();
            var _models = await GetModelAsync(userId, supplierId);
            string colorName = "";
            string modelName = "";
        // string referenceSupplier = "";
            string referenceDealer = "";
            Int32 dealerId = 0;
        //    Int32 supplierId2 = 0;
            Int32 colorId = 0;
            Int32 modelId = 0;
            var response = new Models.Response<Models.Result>();


            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Primera hoja
                    var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados


                    foreach (var row in rows)
                    {

                        colorName = row.Cell(3).GetValue<string>()?.Trim();
                        modelName = row.Cell(4).GetValue<string>()?.Trim();
                        // referenceSupplier = row.Cell(9).GetValue<string>()?.Trim();
                        referenceDealer = row.Cell(8).GetValue<string>()?.Trim();

                        dealerId = _contacts.Exists(x => x.Reference.ToUpper() == referenceDealer.ToUpper()) ? (int)_contacts.Find(x => x.Reference.ToUpper() == referenceDealer.ToUpper()).Id : 0;
                        // supplierId2 = _contacts.Exists(x => x.Reference.ToUpper() == referenceSupplier.ToUpper()) ? (int)_contacts.Find(x => x.Reference.ToUpper() == referenceSupplier.ToUpper()).Id : 0;
                        colorId = _colors.Exists(x => x.Name.ToUpper() == colorName.ToUpper()) ? (int)_colors.Find(x => x.Name.ToUpper() == colorName.ToUpper()).Id : 0;
                        modelId = _models.Exists(x => x.Name.ToUpper() == modelName.ToUpper()) ? (int)_models.Find(x => x.Name.ToUpper() == modelName.ToUpper()).Id : 0;
                        int fila = row.RowNumber(); // Ej: 2
                        string rowRef = $"{fila}"; // Ej: "A2"
                        try
                        {

                            vehicles.Add(new Vehicle
                            {

                                Vin = row.Cell(1).GetValue<string>(),
                                EngineSerial = row.Cell(2).GetValue<string>(),
                                ColorId = colorId,
                                ModelId = modelId,
                                Year = string.IsNullOrWhiteSpace(row.Cell(5).GetString()) ? 0 : row.Cell(5).GetValue<int>(),
                                Plate = row.Cell(6).GetString(),
                                IsActive = row.Cell(7).GetValue<string>().ToUpper() switch
                                {
                                    "SI" => true,
                                    "NO" => false,
                                    _ => throw new Exception($"Valor inválido en ACTIVO. Se esperaba 'SI' o 'NO'. FILA-{rowRef}")
                                },
                                DealerId = dealerId,
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

            return vehicles;
        }

        [HttpPost("PostVehicles")]
        public async Task<IActionResult> Post_Vehicles(List<Models.Vehicle> vehicles, Int32 userId)
        {

            try
            {
                var _response = await _dVehicle.Post_Vehicles(vehicles, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("Import")]
        public async Task<IActionResult> ImportVehicles(IFormFile file, Int32 userId, Int32 supplierId )
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                // Leer el archivo Excel y convertirlo a List<PolicyType>
                List<Vehicle> vehicles = await ReadExcelToVehicles(file,userId, supplierId);

                // Llamar al método existente de tu capa de servicio
                var response = await _dVehicle.Import_Vehicles(vehicles, userId);

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
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                var _response = await _dVehicle.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


    }
}
