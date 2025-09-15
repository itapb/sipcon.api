using System.Configuration.Provider;
using System.IO.Packaging;
using System.Net.Mail;
using System.Reflection;
using ClosedXML.Excel;
using ClosedXML.Graphics;
using Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/Inventory")]
    [ApiController]
    [Authorize]
    public class cInventory : Controller
    {

        private readonly dInventory _dInventory;

        public cInventory(dInventory dInventory)
        {
            _dInventory = dInventory;
        }


        #region "Inventory"

        [HttpGet("/api/Inventory/GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32 supplierId ,Int32 rowFrom, string? filter )
        {
            try
            {
                Models.Response<List<Inventory>> _response = await _dInventory.GetAll(userId, supplierId, rowFrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        #endregion

        #region "Movements"

        [HttpGet("/api/Movements/GetMovements")]
        public async Task<IActionResult> GetMovements(Int32 userId, Int32 supplierId,string typeId, Int32 rowfrom, string? filter)
        {
            try
            {
                Models.Response<List<Movement>> _response = await _dInventory.GetMovements(userId, supplierId, typeId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/GetMovement")]
        public async Task<IActionResult> GetMovement(Int32 userId, Int32  movementId)
        {
            try
            {
                Models.Response<Movement> _response = await _dInventory.GetMovement(userId, movementId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/NewMovementWithContext")]
        public async Task<IActionResult> NewMovementWithContext(Int32 userId, Int32 supplierId, string typeId)
        {
            try
            {
                
                Movement _new = new Movement();
                _new.Id = 0;
                _new.SupplierId = supplierId;
                _new.TypeId = typeId;

                List<Movement> _list = new List<Movement>();
                _list.Add(_new);

                Response<Result> _resp = await _dInventory.PostMovements(_list, userId);
                Result _resul = new Result();
                _resul = (Result)(_resp.Data);

                var _get= await _dInventory.GetMovement(userId, _resul.LastId);
                _new = (Movement)(_get.Data);

                List<MovementDetails> _details = new List<MovementDetails>();

                MovementWithContext _mov = new MovementWithContext();
                _mov.Movement = _new;
                _mov.Details = _details;

                Response<MovementWithContext> _response = new Response<MovementWithContext>();
                _response.Data = _mov;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/GetMovementWithContext")]
        public async Task<IActionResult> GetMovementWithContext(Int32 userId, Int32 movementId)
        {
            try
            {

                var _get = await _dInventory.GetMovement(userId, movementId);
                var _details = await _dInventory.GetMovementDetails(userId, movementId, null);

                MovementWithContext _mov = new MovementWithContext();
                _mov.Movement = (Movement)(_get.Data);
                try
                {
                    _mov.Details = (List<MovementDetails>)(_details.Data);
                }
                catch
                {

                }
              

                Response<MovementWithContext> _response = new Response<MovementWithContext>();
                _response.Data = _mov;
                _response.Total = _get.Total;
                _response.Processed = _get.Processed;
                _response.Message = _get.Message;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/GetMovementDetails")]
        public async Task<IActionResult> GetMovementDetails(Int32 userId, Int32 movementId, Int32? detailId)
        {
            try
            {
                Models.Response<List<MovementDetails>> _response = await _dInventory.GetMovementDetails(userId, movementId, detailId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/GetMovementDetailsByUser")]
        public async Task<IActionResult> GetMovementDetailsByUser(Int32 userId, string movementType, string mode)
        {
            try
            {
                Models.Response<List<MovementDetails>> _response = await _dInventory.GetMovementDetailsByUser(userId, movementType, mode);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/GetValidLocation")]
        public async Task<IActionResult> GetValidLocation(Int32 movementDetailId, string locationName)
        {
            try
            {
                Models.Response<Models.Location> _response = await _dInventory.GetValidLocation(movementDetailId, locationName);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Movements/PostMovements")]
        public async Task<IActionResult> PostMovements(List<Models.Movement> movements, Int32 userId)
        {
            try
            {
                Response<Result> _response = await _dInventory.PostMovements(movements, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Movements/PostMovementDetail")]
        public async Task<IActionResult> PostMovementDetail(List<Models.NewMovementDetail> detail, Int32 userId)
        {
            try
            {
                Response<Result> _response = await _dInventory.PostMovementDetail(detail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Movements/PostMovementDetailMobile")]
        public async Task<IActionResult> PostMovementDetailMobile(Models.MovementDetails movementDetail, Int32 userId)
        {
            try
            {
                Response<Result> _response = await _dInventory.PostMovementDetailMobile(movementDetail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Movements/PostMovementActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                Response<Result> _response = await _dInventory.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        //private List<NewMovementDetail> ExceltoMovementDetail(IFormFile file, Int32 movementId)
        //{
        //    var _list = new List<NewMovementDetail>();

        //    using (var stream = new MemoryStream())
        //    {
        //        file.CopyTo(stream);

        //        using (var workbook = new XLWorkbook(stream))
        //        {
        //            var worksheet = workbook.Worksheet(1); // Primera hoja
        //            var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

        //            foreach (var row in rows)
        //            {

        //                _list.Add(new NewMovementDetail
        //                {
        //                    // Mapear las celdas de Excel a las propiedades del modelo
        //                    // Ajusta según tu estructura real
        //                    Id = 0,
        //                    InnerCode = row.Cell(1).GetValue<string>(),
        //                    //Description = row.Cell(2).GetValue<string>(),
        //                    RequiredQty = row.Cell(3).GetValue<int>(),
        //                    LocationName = row.Cell(4).GetValue<string>(),
        //                    MovementId = movementId

        //                });
        //            }
        //        }
        //    }

        //    return _list;
        //}

        //[HttpPost("/api/Movements/ImportMovementDetail")]
        //public async Task<IActionResult> ImportMovementDetail(IFormFile file, Int32 userId, Int32 movementId)
        //{

        //    if (file == null || file.Length == 0)
        //        return BadRequest("No se ha proporcionado un archivo válido.");

        //    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //        return BadRequest("Solo se permiten archivos Excel (.xlsx)");

        //    try
        //    {
        //        List<Models.RelatedModel> _models = new List<Models.RelatedModel>();
        //        List<NewMovementDetail> _details = ExceltoMovementDetail(file, movementId);
        //        Response<Result> _response = await _dInventory.PostMovementDetail(_details, userId, true);
        //        return StatusCode(_response.Status, _response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }

        //}

        [HttpPost("/api/Movements/DeleteMovementDetail")]
        public async Task<IActionResult> DeleteMovementDetail(List<Models.Action> _list, Int32 userId)
        {

            try
            {
                Response<Result> _response = await _dInventory.DeleteMovementDetail(_list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpGet("/api/Movements/GetPartialType")]
        public async Task<IActionResult> GetPartialTypes()
        {

            try
            {
                Response<List<PartialType>> _response = await _dInventory.GetPartialTypes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        #endregion


        #region "Packing"

        [HttpGet("/api/Packing/GetCustomersForPacking")]
        public async Task<IActionResult> GetCustomersForPacking(Int32 supplierId)
        {
            try
            {
                var _response = await _dInventory.GetCustomersForPacking(supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("/api/Packing/GetProviderToDeliver")]
        public async Task<IActionResult> GetProviderToDeliver()
        {
            try
            {
                var _response = await _dInventory.GetProviderToDeliver();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetPackages")]
        public async Task<IActionResult> GetPackages(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.GetPackages( supplierId,  customerId,  userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetPackagesFromGuide")]
        public async Task<IActionResult> GetPackagesFromGuide(Int32 guideId)
        {
            try
            {
                var _response = await _dInventory.GetPackagesFromGuide(guideId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetPackageDetails")]
        public async Task<IActionResult> GetPackageDetails(Int32 packageId)
        {
            try
            {
                var _response = await _dInventory.GetPackageDetails(packageId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetPackageDetailByPart")]
        public async Task<IActionResult> GetPackageDetailByPart(Int32 packageId,string scanCode)
        {
            try
            {
                var _response = await _dInventory.GetPackageDetailByPart(packageId, scanCode);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetPackageDetailsPending")]
        public async Task<IActionResult> GetPackageDetailsPending(Int32 packageId)
        {
            try
            {
                var _response = await _dInventory.GetPackageDetailsPending(packageId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetGuides")]
        public async Task<IActionResult> GetGuides(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.GetGuides(supplierId, customerId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("/api/Packing/NewPackage")]
        public async Task<IActionResult> NewPackage(Int32 supplierId, Int32 customerId)
        {

            try
            {
                Models.Package package = new Models.Package();
                package.SupplierId = supplierId;
                package.CustomerId = customerId;
                package.Closed = false;
                package.Id = 0;

                var _response = await _dInventory.PostPackage(package);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Packing/ClosePackage")]
        public async Task<IActionResult> ClosePackage(Int32 packageId, decimal weight)
        {

            try
            {
                Models.Package package = new Models.Package();
                package.Id= packageId;
                package.Weight = weight;
                package.Closed = true;

                var _response = await _dInventory.PostPackage(package);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Packing/PostPackageDetail")]
        public async Task<IActionResult> PostPackageDetail(Models.PackageDetail packageDetail)
        {

            try
            {

                var _response = await _dInventory.PostPackageDetail(packageDetail);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Packing/DeletePackageDetail")]
        public async Task<IActionResult> DeletePackageDetail(Int32 packageId, Int32 partId)
        {

            try
            {
                Models.PackageDetail packageDetail = new Models.PackageDetail();
                packageDetail.Id = 0;
                packageDetail.PackageId = packageId;
                packageDetail.PartId = partId;
                packageDetail.Quantity = 0;

                var _response = await _dInventory.PostPackageDetail(packageDetail);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Packing/NewGuide")]
        public async Task<IActionResult> NewGuide(Int32 supplierId, Int32 customerId, Int32 userId)
        {

            try
            {
                Models.Guide guide = new Models.Guide();
                guide.Id = 0;
                guide.CustomerId = customerId;
                guide.SupplierId = supplierId;
                guide.UserId = userId;
                guide.Delivered = false;

                var _response = await _dInventory.PostGuide(guide);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Packing/CloseGuide")]
        public async Task<IActionResult> CloseGuide(Int32 guideId, Int32 providerId, string? guideNumber, Int32 userId)
        {

            try
            {
                Models.Guide guide = new Models.Guide();
                guide.Id = guideId;
                guide.ProviderId = providerId;
                guide.UserId = userId;
                guide.Number = guideNumber;
                guide.Closed = true;

                var _response = await _dInventory.PostGuide(guide);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Packing/AddPackge")]
        public async Task<IActionResult> AddPackge(string packageCode, Int32 guideId, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.AddPackge( packageCode,  guideId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Packing/DeletePackge")]
        public async Task<IActionResult> DeletePackge(string packageCode, Int32 guideId, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.DeletePackge(packageCode, guideId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        #endregion

        #region "Guide Module"


        [HttpGet("/api/Guide/GetAllGuides")]
        public async Task<IActionResult> GetAllGuides(Int32 userId, Int32 supplierId, int rowfrom, string? filter)
        {
            try
            {
                var _response = await _dInventory.GetAllGuides(userId, supplierId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Guide/GetGuideWithContext")]
        public async Task<IActionResult> GetGuideWithContext(Int32 guideId)
        {
            try
            {

                var _get = await _dInventory.GetOneGuide(guideId);
                var _details = await _dInventory.GetGuideDetails(guideId);

                GuideWithContext _guide = new GuideWithContext();

                //var temp = (List<Guide>)(_get.Data);
                //_guide.Guide = temp[0];
                _guide.Guide = (Guide)(_get.Data);
                _guide.GuideDetails = (List<GuideDetails>)(_details.Data);

                Response<GuideWithContext> _response = new Response<GuideWithContext>();
                _response.Data = _guide;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("/api/Guide/PostGuideDetail")]
        public async Task<IActionResult> PostGuideDetails(List<GuideDetails> _list, int userId)
        {
            try
            {

                var _response = await _dInventory.PostGuideDetails(_list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Guide/PostGuidesActions")]
        public async Task<IActionResult> PostGuideActions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                var _response = await _dInventory.PostGuidesActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Guide/PostGuideNumber")]
        public async Task<IActionResult> PostGuideNumber(Int32 userId,Int32 guideId, string guideNumber)
        {

            try
            {
                var _response = await _dInventory.PostGuideNumber(userId, guideId, guideNumber);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        private MemoryStream ConvertToExcel(List<Models.Inventory> _inventories)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("ALMACENES");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "SERIAL";
                worksheet.Cell(1, 3).Value = "IDPLANTA";
                worksheet.Cell(1, 4).Value = "PARTE";
                worksheet.Cell(1, 5).Value = "STOCK";
                worksheet.Cell(1, 6).Value = "UBICACION";
                worksheet.Cell(1, 7).Value = "ZONA";
                worksheet.Cell(1, 8).Value = "ALMACEN";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // 6. Llenar los datos
                for (int i = 0; i < _inventories.Count; i++)
                {
                    var _contacto = _inventories[i];

                    worksheet.Cell(i + 2, 1).Value = _contacto.PartId;
                    worksheet.Cell(i + 2, 2).Value = _contacto.PartInnerCode;
                    worksheet.Cell(i + 2, 3).Value = _contacto.SupplierId;
                    worksheet.Cell(i + 2, 4).Value = _contacto.PartName;
                    worksheet.Cell(i + 2, 5).Value = _contacto.Stock;
                    worksheet.Cell(i + 2, 6).Value = _contacto.LocationName;
                    worksheet.Cell(i + 2, 7).Value = _contacto.ZoneName;
                    worksheet.Cell(i + 2, 8).Value = _contacto.WarehouseName;

                }

                // 7. Ajustar el ancho de las columnas al contenido
                worksheet.Columns().AdjustToContents();

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; //Importante: rebobinar el stream
                return stream;


                //// 9. Retornar el archivo Excel
                //return System.IO.File(
                //    content,
                //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                //    "PersonasExportadas.xlsx");
            }

        }


        //[HttpGet("ExportGuide")]
        //public async Task<IActionResult> GetExportGuide(Int32 userId, Int32 movementId)
        //{
        //    try
        //    {
        //        Models.Response response = await _dInventory.GetMovement(userId, movementId);

        //        // 2. Validar que la respuesta es exitosa y contiene datos
        //        if (response.Status != StatusCodes.Status200OK || response.Data == null)
        //        {
        //            return NotFound("No se encontró Guia de Movimiento");
        //        }

        //        // 3. Convertir los datos correctamente
        //        List<Movement> movement = new List<Movement>();

        //        if (response.Data is Movement singleMovement)
        //        {
        //            // Si es un solo objeto
        //            movement.Add(singleMovement);
        //        }
        //        else if (response.Data is IEnumerable<Movement> movementList)
        //        {
        //            // Si ya es una lista/enumerable
        //            movement = movementList.ToList();
        //        }
        //        else if (response.Data is JArray jsonArray)
        //        {
        //            // Si viene como JArray (JSON)
        //            movement = jsonArray.ToObject<List<Movement>>();
        //        }
        //        else
        //        {
        //            // Si no reconocemos el formato
        //            return StatusCode(StatusCodes.Status500InternalServerError,
        //                "Formato de datos de Servicio no reconocido");
        //        }

        //        // 4. Validar que tenemos datos
        //        if (!movement.Any())
        //        {
        //            return NotFound("No se encontraron datos válidos de servicio");
        //        }

        //        // 5. Generar el PDF
        //        byte[] pdfBytes = GeneratePdfReport(movement, movementId, userId);
        //        string fileName = $"Orden_{movement.FirstOrDefault()?.Id?.ToString() ?? "reporte"}.pdf";

        //        return File(pdfBytes, "application/pdf", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

        //[Obsolete]
        //private byte[] GeneratePdfReport(List<Movement> movement, int movementId, Int32 userId)
        //{
        //    QuestPDF.Settings.License = LicenseType.Community;

        //    var selectedMovement = movement.FirstOrDefault(m => m.Id == movementId);

        //    var response = _dInventory.GetMovementDetails(userId, movementId, null).GetAwaiter().GetResult();
        //    var MovementDetailsList = new List<MovementDetails>();



        //    if (response.Data is IEnumerable<MovementDetails> detailList)
        //        MovementDetailsList = detailList.ToList();
        //    else if (response.Data is JArray jsonArray)
        //        MovementDetailsList = jsonArray.ToObject<List<MovementDetails>>();

        //    // Aplicamos el filtro según el tipo del movimiento
        //    var movementType = selectedMovement?.TypeId;
        //    List<MovementDetails> filteredDetails = new();

        //    if (movementType != null)
        //    {
        //        var typeMap = new Dictionary<string, string>
        //                        {
        //                            { "D", "E" },
        //                            { "R", "P" },
        //                            { "T", "E" }
        //                        };

        //        if (typeMap.TryGetValue(movementType, out string expectedDetailType))
        //        {
        //            filteredDetails = MovementDetailsList
        //                .Where(detail => detail.MovementId == movementId && detail.TypeId == expectedDetailType)
        //                .ToList();
        //        }
        //    }

        //    int totalProductos = filteredDetails
        //    .Select(d => d.Id) // Asumiendo que existe un campo ProductId
        //    .Distinct()
        //    .Count();

        //    int? totalUnidades = selectedMovement?.TypeId switch
        //    {
        //        "T" or "D" => filteredDetails.Sum(d => d.RealQty),
        //        "R" => filteredDetails.Sum(d => d.RequiredQty),
        //        _ => 0
        //    };
        //    // Asumiendo que existe un campo Quantity


        //    var document = Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Margin(50);
        //            page.Size(PageSizes.Ledger.Portrait());

        //            page.Header().Element(header =>
        //            {
        //                header.Column(col =>
        //                {
        //                    col.Item().Row(row =>
        //                    {
        //                        // LOGO (izquierda)
        //                        row.ConstantItem(110).Image($@"\\{Environment.MachineName}{Util.Setting.ImagesUrl}\mdv logo.PNG", ImageScaling.FitWidth);

        //                        // TÍTULO (centro)
        //                        string tituloMovimiento = selectedMovement?.TypeId switch
        //                        {
        //                            "D" => "GUÍA DE DESPACHO",
        //                            "R" => "GUÍA DE RECEPCIÓN",
        //                            "T" => "GUÍA DE TRASLADO",
        //                            _ => "GUÍA DE MOVIMIENTO" // valor por defecto
        //                        };

        //                        row.RelativeItem()
        //                           .AlignCenter()
        //                           .Text(tituloMovimiento)
        //                           .FontSize(20)
        //                           .Bold();


        //                        // ID y fecha (derecha)
        //                        row.ConstantItem(130).Column(right =>
        //                        {
        //                            right.Item().AlignRight().Text($"Numero de Guia: {selectedMovement.GuideNumber ?? "N/A"}");
        //                            right.Item().AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}");
        //                        });
        //                    });

        //                    // BLOQUE DE INFORMACIÓN debajo del título
        //                    col.Item().PaddingTop(10).PaddingBottom(5).Border(1).Padding(3).Column(info =>
        //                    {

        //                        info.Item().Row(r =>
        //                        {
        //                            r.ConstantItem(75).Text("Proveedor:").Bold();
        //                            r.RelativeItem().Text(selectedMovement?.SupplierName ?? "N/A").WrapAnywhere();
        //                        });

        //                        info.Item().Row(r =>
        //                        {
        //                            r.ConstantItem(75).Text("Cliente:").Bold();
        //                            r.RelativeItem().Text(selectedMovement?.ContactName ?? "N/A").WrapAnywhere();
        //                        });

        //                        info.Item().Row(r =>
        //                        {
        //                            r.ConstantItem(75).Text("Responsable:").Bold();
        //                            r.RelativeItem().Text($"{selectedMovement?.ManagerName ?? "N/A"}").WrapAnywhere();

        //                        });


        //                    });
        //                });
        //            });

        //            page.Content().Column(column =>
        //            {
        //                column.Item().LineHorizontal(1);

        //                column.Item().Table(table =>
        //                {
        //                    table.ColumnsDefinition(columns =>
        //                    {
        //                        columns.RelativeColumn(1); // Código
        //                        columns.RelativeColumn(3); // Descripción
        //                        columns.RelativeColumn(1); // Cantidad
        //                    });

        //                    table.Header(header =>
        //                    {
        //                        header.Cell().Element(CellStyle).Text("Código");
        //                        header.Cell().Element(CellStyle).Text("Producto");
        //                        header.Cell().Element(CellStyle).Text("Cantidad");

        //                        IContainer CellStyle(IContainer container) =>
        //                            container.DefaultTextStyle(x => x.Bold()).Padding(5).Background("#EEE");
        //                    });

        //                    foreach (var movementdetail in filteredDetails)
        //                    {
        //                        table.Cell().Element(CellStyle).Text(movementdetail.PartInnerCode);
        //                        table.Cell().Element(CellStyle).Text(movementdetail.PartDescription);

        //                        var cantidadMostrar = selectedMovement?.TypeId switch
        //                        {
        //                            "T" or "D" => movementdetail.RealQty,
        //                            "R" => movementdetail.RequiredQty,
        //                            _ => 0
        //                        };
        //                        table.Cell().Element(CellStyle).Text(cantidadMostrar.ToString());


        //                        IContainer CellStyle(IContainer container) =>
        //                            container.Padding(5);
        //                    }

        //                    table.Footer(footer =>
        //                    {

        //                        footer.Cell().ColumnSpan(2).BorderTop(1).AlignRight().Element(CellStyle).Text($"Total productos: {totalProductos}");
        //                        footer.Cell().ColumnSpan(1).BorderTop(1).Element(CellStyle).Text($"Total unidades: {totalUnidades}");

        //                        IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.Bold()).Padding(5).Background("#EEE"); ;
        //                    });
        //                    column.Item().LineHorizontal(1);
        //                });

        //                column.Item().PaddingTop(25).Text("Observaciones: ______________________________________________________________________________________________________________");
        //                column.Item().PaddingTop(25).AlignCenter().Text("DOCUMENTO NO FISCAL");
        //            });


        //        });
        //    });

        //    return document.GeneratePdf();
        //}



        #endregion

        #region"BackOrder"

        [HttpGet("/api/BackOrder/GetAll")]
        public async Task<IActionResult> GetBackOrders(Int32 userId, Int32 supplierId, Int32 rowFrom, string? filter)
        {
            try
            {
                var _response = await _dInventory.GetBackOrders(userId, supplierId, rowFrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/BackOrder/PostActions")]
        public async Task<IActionResult> PostBackorder_Actions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                var _response = await _dInventory.PostBackorder_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("/api/BackOrder/PostBackOrder")]
        public async Task<IActionResult> PostBackorder(Models.PostBackOrder backOrder, Int32 userId)
        {

            try
            {
                var _response = await _dInventory.PostBacKOrder(backOrder, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        #endregion

        #region "Adjustment"

        //#### GET ALL
        
        [HttpGet("/api/Adjustment/GetAdjustments")]
         public async Task<IActionResult> GetAdjustments(Int32 userId, Int32 supplierId, Int32 rowFrom, string? filter)
         {
            try
            {
             var _response = await _dInventory.GetAdjustments(userId, supplierId, rowFrom, filter);
             return StatusCode(_response.Status, _response);
            }
              catch (Exception ex)
              {
               return StatusCode(StatusCodes.Status409Conflict, ex.Message);
               }
         }

        //#### GET ONE
        [HttpGet("/api/Adjustment/GetAdjustment")]
        public async Task<IActionResult> GetAdjustment(Int32 userId, Int32 adjustmentId)
        {

            try
            {
                var _response = await _dInventory.GetAdjustment(userId, adjustmentId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        //#### GET DETAILS
        [HttpGet("/api/Adjustment/GetAdjustmentDetails")]
        public async Task<IActionResult> GetAdjustmentDetails(Int32 adjustmentId)
        {

            try
            {
                var _response = await _dInventory.GetAdjustmentDetails(adjustmentId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        //#### GET WITH CONTEXT
        [HttpGet("/api/Adjustment/GetAdjustmentWithContext")]
        public async Task<IActionResult> GetAdjustmentWithContext(Int32 userId, Int32 adjustmentId)
        {
            try
            {

                var _get = await _dInventory.GetAdjustment(userId, adjustmentId);
                var _details = await _dInventory.GetAdjustmentDetails(adjustmentId);

                AdjustmentWithContext _req = new AdjustmentWithContext();
                _req.Adjustment = (Adjustment)(_get.Data);
                _req.Details = (List<AdjustmentDetails>)(_details.Data);

                Response<AdjustmentWithContext> _response = new Response<AdjustmentWithContext>();
                _response.Data = _req;
                _response.Total = _get.Total;
                _response.Processed = _get.Processed;
                _response.Message = _get.Message;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        //#### GET REASONS

        [HttpGet("/api/Adjustment/GetReasons")]
        public async Task<IActionResult> GetReasons()
        {

            try
            {
                var _response = await _dInventory.GetReasons();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        //#### GET NEW WITH CONTEXT

        [HttpGet("/api/Adjustment/NewAdjustmentWithContext")]
        public async Task<IActionResult> NewAdjustmentWithContext(Int32 userId, Int32 supplierId)
        {
            try
            {
                Adjustment _new = new Adjustment();
                _new.Id = 0; 
                _new.SupplierId = supplierId;
                _new.UserId = userId;
                _new.StatusId = 2;

                List<Adjustment> _list = new List<Adjustment>();
                _list.Add(_new);

                var _resp = await _dInventory.PostAdjustment(_new, userId);
                Result _resul = new Result();
                _resul = (Result)(_resp.Data);

                var _get = await _dInventory.GetAdjustment( userId, _resul.LastId);
                _new = (Adjustment)(_get.Data);


                List<AdjustmentDetails> _details = new List<AdjustmentDetails>();

                AdjustmentWithContext _req = new AdjustmentWithContext();
                _req.Adjustment = _new;
                _req.Details = _details;

                Response<AdjustmentWithContext> _response = new Response<AdjustmentWithContext>();
                _response.Data = _req;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        //#### POST CABECERA

        [HttpPost("/api/Adjustment/PostAdjustment")]
          public async Task<IActionResult> PostAdjustment(Models.Adjustment adjustment, Int32 userId)
          {

              try
              {
                  var _response = await _dInventory.PostAdjustment(adjustment, userId);
                  return StatusCode(_response.Status, _response); 
              }
              catch (Exception ex)
              {
                  return StatusCode(StatusCodes.Status409Conflict, ex.Message);
              }
          }

        //#### POST DETAILS
        [HttpPost("/api/Adjustment/PostAdjustmentDetails")]
        public async Task<IActionResult> PostAdjustmentDetails(Models.AdjustmentDetails detail, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.PostAdjustmentDetails(detail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        //#### POST ACTIONS
        [HttpPost("/api/Adjustment/PostAdjustmentActions")]
        public async Task<IActionResult> PostAdjustmentActions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.PostAdjustmentActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        #endregion
    }
}
