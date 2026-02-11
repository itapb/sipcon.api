using ClosedXML.Excel;
using ClosedXML.Graphics;
using Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Configuration.Provider;
using System.IO.Packaging;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using Colors = QuestPDF.Helpers.Colors;




namespace WebApi.Controllers
{

    [Route("api/Inventory")]
    [ApiController]
    [Authorize]
    public class cInventory : Controller
    {

        private readonly dInventory _dInventory;
        private readonly dContact _dContact;
        private readonly dAttachment _dAttachment;
        public cInventory(dInventory dInventory, dContact dContact, dAttachment dAttachment)
        {
            _dInventory = dInventory;
            _dContact = dContact;
            _dAttachment = dAttachment;

        }


        #region "Inventory"

        [HttpGet("/api/Inventory/GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32 supplierId, Int32 rowFrom, string? filter, bool? withStock = true, string? locationType = null)
        {
            try
            {
                Models.Response<List<Inventory>> _response = await _dInventory.GetAll(userId, supplierId, rowFrom, filter, withStock, locationType);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        private MemoryStream ConvertToExcelInventory(List<Models.Inventory> _inventory)
        {

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("INVENTORY");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "CODIGO";
                worksheet.Cell(1, 3).Value = "DESCRIPCION";
                worksheet.Cell(1, 4).Value = "TAMAÑO PARTE";
                worksheet.Cell(1, 5).Value = "EXISTENCIA";
                worksheet.Cell(1, 6).Value = "PRECIO";
                worksheet.Cell(1, 7).Value = "ALMACEN";
                worksheet.Cell(1, 8).Value = "UBICACION";
                worksheet.Cell(1, 9).Value = "ZONA";
                worksheet.Cell(1, 10).Value = "TAMAÑO ZONA";
                worksheet.Cell(1, 11).Value = "PLANTA";
                worksheet.Cell(1, 12).Value = "ACTIVA";

                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:J1").SetAutoFilter();
                for (int i = 0; i < _inventory.Count; i++)
                {
                    var _inventor = _inventory[i];
                    worksheet.Cell(i + 2, 1).Value = _inventor.Id;
                    worksheet.Cell(i + 2, 2).Value = _inventor.PartInnerCode;
                    worksheet.Cell(i + 2, 3).Value = _inventor.PartName;
                    worksheet.Cell(i + 2, 4).Value = _inventor.PartSize;
                    worksheet.Cell(i + 2, 5).Value = _inventor.Stock;
                    worksheet.Cell(i + 2, 6).Value = _inventor.Price;
                    worksheet.Cell(i + 2, 7).Value = _inventor.WarehouseName;
                    worksheet.Cell(i + 2, 8).Value = _inventor.LocationName;
                    worksheet.Cell(i + 2, 9).Value = _inventor.ZoneName;
                    worksheet.Cell(i + 2, 10).Value = _inventor.ZoneSize;
                    worksheet.Cell(i + 2, 11).Value = _inventor.SupplierName;
                    worksheet.Cell(i + 2, 11).Value = _inventor.IsActive != false ? "SI" : "NO";


                }
                worksheet.Columns().AdjustToContents();

                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;


                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }

        }


        [HttpGet("/api/Inventory/Export")]
        public async Task<IActionResult> GetExportInventory(Int32 userId, Int32 supplierId)
        {
            try
            {
                List<Inventory> _response = await _dInventory.GetExport(userId, supplierId);
                MemoryStream _excel = ConvertToExcelInventory(_response);
                string _fileName = "Inventory.xlsx";

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

        #endregion

        #region "Movements"

        [HttpGet("/api/Movements/GetMovements")]
        public async Task<IActionResult> GetMovements(Int32 userId, Int32 supplierId, string typeId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                Models.Response<List<Movement>> _response = await _dInventory.GetMovements(userId, supplierId, typeId, rowfrom, filter, fromDate, upToDate, estatusId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Movements/ReceptionExport")]
        public async Task<IActionResult> GetExportMovementsReception(Int32 userId, Int32 supplierId, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                List<Movement> _response = await _dInventory.GetExportMovementsReception(userId, supplierId, filter, fromDate, upToDate, estatusId);
                MemoryStream _excel = ConvertToExcelMovements(_response);
                string _fileName = "Recepcion.xlsx";

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
        [HttpGet("/api/Movements/RelocationExport")]
        public async Task<IActionResult> GetExportMovementsRelocation(Int32 userId, Int32 supplierId, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                List<Movement> _response = await _dInventory.GetExportMovementsRelocation(userId, supplierId, filter, fromDate, upToDate, estatusId);
                MemoryStream _excel = ConvertToExcelMovements(_response);
                string _fileName = "Traslado.xlsx";

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

        private MemoryStream ConvertToExcelMovements(List<Models.Movement> _movements)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("MOVIMIENTOS");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value = "FECHA";
                worksheet.Cell(1, 3).Value = "PROVEEDOR";
                worksheet.Cell(1, 4).Value = "REFERENCIA";
                worksheet.Cell(1, 5).Value = "ESTATUS";
                worksheet.Cell(1, 6).Value = "USUARIO";

                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:F1").SetAutoFilter();

                for (int i = 0; i < _movements.Count; i++)
                {
                    var _movement = _movements[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _movement.Id;
                    worksheet.Cell(row, 2).Value = _movement.Created;
                    worksheet.Cell(row, 3).Value = _movement.SupplierReference;
                    worksheet.Cell(row, 4).Value = _movement.Reference;
                    worksheet.Cell(row, 5).Value = _movement.StatusName;
                    worksheet.Cell(row, 6).Value = _movement.CreatedBy;
                }

                worksheet.Columns().AdjustToContents();

                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }

        [HttpGet("/api/Movements/GetMovement")]
        public async Task<IActionResult> GetMovement(Int32 userId, Int32 movementId)
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


                var _get2 = (Movement)(await _dInventory.getLast(userId, supplierId, typeId)).Data;

                if (_get2.Id is null || _get2.Id == 0)
                {

                    Response<Result> _resp = await _dInventory.PostMovements(_list, userId);
                    if (_resp.Processed == false)
                    {
                        throw new Exception(_resp.Message);
                    }

                    Result _resul = new Result();
                    _resul = (Result)(_resp.Data);

                    var _get = await _dInventory.GetMovement(userId, _resul.LastId);
                    _new = (Movement)(_get.Data);

                }
                else
                {
                    _new = _get2;
                }


                List<MovementDetails> _details = new List<MovementDetails>();

                MovementWithContext _mov = new MovementWithContext();
                _mov.Movement = _new;
                _mov.Details = _details;

                Models.Response<MovementWithContext> _response = new Models.Response<MovementWithContext>();
                _response.Data = _mov;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                Response<MovementWithContext> _response = new Response<MovementWithContext>();
                _response.SetError(ex);

                return StatusCode(StatusCodes.Status409Conflict, _response);

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


                Models.Response<MovementWithContext> _response = new Models.Response<MovementWithContext>();
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

        [HttpGet("/api/Movements/GetMovementDetailsPicking")]
        public async Task<IActionResult> GetMovementDetailsPicking(Int32 userId, Int32? supplierId, Int32? dealerId, Int32? rowfrom, string? filter, bool? pending, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                Models.Response<List<MovementDetails>> _response = await _dInventory.GetMovementDetailsPicking(userId, supplierId, dealerId, rowfrom, filter, pending, fromDate, upToDate, estatusId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        [HttpGet("/api/Movements/GetMovementDetailsByUser")]
        public async Task<IActionResult> GetMovementDetailsByUser(Int32 userId, Int32? supplierId, string movementType, string mode)
        {
            try
            {
                Models.Response<List<MovementDetails>> _response = await _dInventory.GetMovementDetailsByUser(userId, supplierId, movementType, mode);
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
                Models.Response<Result> _response = await _dInventory.PostMovements(movements, userId);
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
                Models.Response<Result> _response = await _dInventory.PostMovementDetail(detail, userId);
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
                Models.Response<Result> _response = await _dInventory.PostMovementDetailMobile(movementDetail, userId);
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
                Models.Response<Result> _response = await _dInventory.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("/api/Movements/PostMovementDetailsPickingActions")]
        public async Task<IActionResult> PostMovementDetailsPickingActions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                Models.Response<Result> _response = await _dInventory.PostMovementDetailsPickingActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private List<NewMovementDetail> ExceltoMovementDetail(IFormFile file, Int32 movementId)
        {
            var _list = new List<NewMovementDetail>();
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
                        int fila = row.RowNumber(); // Ej: 2
                        string rowRef = $"{fila}";
                        try
                        {
                            _list.Add(new NewMovementDetail
                            {
                                // Mapear las celdas de Excel a las propiedades del modelo
                                // Ajusta según tu estructura real
                                Id = 0,
                                InnerCode = row.Cell(1).GetValue<string>(),
                                PartName = row.Cell(2).GetValue<string>(),
                                RequiredQty = string.IsNullOrWhiteSpace(row.Cell(3).GetString()) ? 0 : row.Cell(3).GetValue<int>(),
                                LocationName = row.Cell(4).GetValue<string>(),
                                DestinationName = row.Cell(5).GetValue<string>(),
                                MovementId = movementId,
                                RowReference = rowRef,
                                Cost = row.Cell(6).GetValue<decimal>(),
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

        [HttpPost("/api/Movements/ImportMovementDetail")]
        public async Task<IActionResult> ImportMovementDetail(IFormFile file, Int32 userId, Int32 movementId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                List<Models.RelatedModel> _models = new List<Models.RelatedModel>();
                List<NewMovementDetail> _details = ExceltoMovementDetail(file, movementId);
                Models.Response<Result> _response = await _dInventory.PostMovementDetail(_details, userId, true);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Movements/DeleteMovementDetail")]
        public async Task<IActionResult> DeleteMovementDetail(List<Models.Action> _list, Int32 userId)
        {

            try
            {
                Models.Response<Result> _response = await _dInventory.DeleteMovementDetail(_list, userId);
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
                Models.Response<List<PartialType>> _response = await _dInventory.GetPartialTypes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }



        //--------------------------------------------------------------------------------------------------
        [HttpPost("/api/Movements/PostMovementDetailsTransferActions")]
        public async Task<IActionResult> PostMovementDetailsTransferActions(List<Models.Action> actions, Int32 userId)
        {

            try
            {
                Models.Response<Result> _response = await _dInventory.PostMovementDetailsTransferActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        [HttpGet("/api/Movements/GetMovementDetailsTransfer")]
        public async Task<IActionResult> GetMovementDetailsTransfer(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, bool? pending, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                Models.Response<List<MovementDetails>> _response = await _dInventory.GetMovementDetailsTransfer(userId, supplierId, rowfrom, filter, pending, fromDate, upToDate, estatusId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        //--------------------------------------------------------------------------------------------------
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
                var _response = await _dInventory.GetPackages(supplierId, customerId, userId);
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
        public async Task<IActionResult> GetPackageDetailByPart(Int32 packageId, string scanCode)
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

        /*----------------------------------------------GetPackageList------------------------------------------------------*/

        [HttpGet("/api/Packing/GetPackageList")]
        public async Task<IActionResult> GetPackageList(Int32 supplierId, string packageCode)
        {
            try
            {
                var _response = await _dInventory.GetPackagesList(supplierId, packageCode);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Packing/GetPackingList")]
        public async Task<IActionResult> GetPackingList(Int32 supplierId)
        {
            try
            {
                var _response = await _dInventory.GetPackingList(supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        /*------------------------------------------------------------------------------------------------------------*/


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
                package.Id = packageId;
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

                var _response = await _dInventory.PostGuide(guide, userId);
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

                var _response = await _dInventory.PostGuide(guide, userId);
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
                var _response = await _dInventory.AddPackge(packageCode, guideId);
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

        [HttpPost("/api/Packing/OpenPackage")]
        public async Task<IActionResult> OpenPackage(int packageId, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.OpenPackage(packageId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Packing/PrintPackge")]
        public async Task<IActionResult> PostPackagePrint(Int32 packageId, Int32 userId)
        {

            try
            {
                Models.Printqueue _print = new Models.Printqueue();
                _print.Id = packageId;
                _print.RecordId = packageId;
                _print.Type = "PK";

                var _response = await _dInventory.PostPackagePrint(_print, userId);
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
        public async Task<IActionResult> GetAllGuides(Int32 userId, Int32 supplierId, Int32 dealerId, int rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                var _response = await _dInventory.GetAllGuides(userId, supplierId, dealerId, rowfrom, filter, fromDate, upToDate, estatusId);
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

                Models.Response<GuideWithContext> _response = new Models.Response<GuideWithContext>();
                _response.Data = _guide;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        private async Task<string> GetFirstAttachmentFilePath(string moduleName, int? recordId)
        {
            var _response = await _dAttachment.GetAll(moduleName, recordId);
            if (_response?.Data == null) return null;

            var attachment = ((List<Models.Attachment>)_response.Data).FirstOrDefault();
            if (attachment == null || string.IsNullOrEmpty(attachment.FileName)) return null;

            string attachmentUrl = $@"\\{Environment.MachineName}{Util.Setting.AttachmentUrl}\";
            var _modules = moduleName;

            return Path.Combine(attachmentUrl, _modules, attachment.RecordId.ToString(), attachment.FileName);
        }

       
        
        [Obsolete]
        private async Task<byte[]> GeneratePdfReport(Models.Guide guide, int guideId, int userId)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var response = _dInventory.GetGuideDetails(guideId).GetAwaiter().GetResult();
            var guideDetailsList = new List<GuideDetails>();

            guideDetailsList = response.Data.ToList();


            int? supplierId = guide.SupplierId;
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", supplierId);

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.Ledger.Portrait());

                    page.Header().Element(header =>
                    {
                        header.Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                // LOGO (izquierda)
                                row.ConstantItem(110).Image(supplierImagePath, ImageScaling.FitWidth);

                                // TÍTULO (centro)

                                row.RelativeItem()
                                   .AlignCenter()
                                   .Text($"GUIA DE DESPACHO")
                                   .FontSize(20)
                                   .Bold();


                                // ID y fecha (derecha)
                                row.ConstantItem(150).Column(right =>
                                {
                                    right.Item().AlignRight().Text($"NUMERO DE GUIA: {guideId}");
                                    right.Item().AlignRight().Text($"FECHA: {guide.CreatedDate:dd/MM/yyyy}");
                                });
                            });

                            // BLOQUE DE INFORMACIÓN debajo del título
                            col.Item().PaddingTop(10).PaddingBottom(5).Border(1).Padding(3).Column(info =>
                            {

                                info.Item().Row(r =>
                                {
                                    r.ConstantItem(120).Text("PROVEEDOR:").Bold();
                                    r.RelativeItem().Text(guide?.SupplierName ?? "N/A").WrapAnywhere();
                                });

                                info.Item().Row(r =>
                                {
                                    r.ConstantItem(120).Text("CLIENTE:").Bold();
                                    r.RelativeItem().Text(guide?.CustomerName?? "N/A").WrapAnywhere();
                                });

                                info.Item().Row(r =>
                                {
                                    r.ConstantItem(120).Text("PROVEEDOR DE ENVIO:").Bold();
                                    r.RelativeItem().Text($"{guide?.ProviderName ?? "N/A"}").WrapAnywhere();

                                });


                            });
                        });
                    });

                    page.Content().Column(column =>
                    {
                        column.Item().LineHorizontal(1);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Código
                                columns.RelativeColumn(3); // Descripción
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(1); // PRECIOUNITARIO
                                columns.RelativeColumn(1); // SUBTOTAL
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("CODIGO");
                                header.Cell().Element(CellStyle).Text("DESCRIPCION");
                                header.Cell().Element(CellStyle).Text("CANTIDAD");
                                header.Cell().Element(CellStyle).Text("NRO BULTO");
                                header.Cell().Element(CellStyle).Text("NRO PEDIDO");

                                IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.Bold()).Padding(5).Background("#EEE").AlignCenter(); ;
                            });

                            int totalunidades = 0;
                            int totalBultos = 0;
                            foreach (var detail in guideDetailsList)
                            {
                                table.Cell().Element(CellStyle).Text(detail.PartInnercode);
                                table.Cell().Element(CellStyle).Text(detail.PartDescription);
                                table.Cell().Element(CellStyle).Text(detail.Quantity.ToString());
                                table.Cell().Element(CellStyle).Text($"{detail.PackageCode.ToString()}");
                                table.Cell().Element(CellStyle).Text($"{detail.SaleOrderId.ToString()}");
                                IContainer CellStyle(IContainer container) => container.Padding(5).AlignCenter();
                                totalunidades += (int)detail.Quantity;
                                totalBultos = (int)detail.PackageNumber;
                            }

                            table.Footer(footer =>
                            {

                                footer.Cell().ColumnSpan(5).BorderTop(1).AlignRight().Element(CellStyle).Text($"TOTAL UNIDADES: {totalunidades}        TOTAL DE BULTOS: {totalBultos.ToString()}");


                                IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.Bold()).Padding(5).Background("#EEE"); ;
                            });
                            column.Item().LineHorizontal(1);
                        });

                        column.Item().PaddingTop(25).Text("Observaciones: ______________________________________________________________________________________________________________");
                        column.Item().PaddingTop(25).AlignCenter().Text("DOCUMENTO NO FISCAL");
                    });


                });
            });

            return document.GeneratePdf();
        }

        [HttpGet("ExportPdfGuide")]
        public async Task<IActionResult> ExportPdfGuide(int guideId, int userId)
        {
            try
            {
                Models.Response<Models.Guide> response = await _dInventory.GetOneGuide(guideId);

                // 2. Validar que la respuesta es exitosa y contiene datos
                if (response.Status != StatusCodes.Status200OK || response.Data == null)
                {
                    response.SetError(new Exception("DATOS DE PEDIDO NO ENCONTRADOS"));
                    return StatusCode(response.Status, response);
                }

                // 3. Convertir los datos correctamente
                Guide guide;

                if (response.Data is Guide guideSingle)
                {
                    // Asignar directamente el modelo
                    guide = guideSingle;
                }
                else
                {
                    // Si no reconocemos el formato
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Formato de datos de Servicio no reconocido");
                }

                // 4. Validar que tenemos datos
                if (guide == null)
                {
                    return NotFound("No se encontraron datos válidos de servicio");
                }

                // 5. Generar el PDF
                byte[] pdfBytes = await GeneratePdfReport(guide, guideId, userId);
                string fileName = $"Guia_Num{guideId.ToString() ?? "reporte"}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
        public async Task<IActionResult> PostGuideNumber(Int32 userId, Int32 guideId, string guideNumber)
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

        //[HttpPost("/api/Guide/Claim")]
        //public async Task<IActionResult> postClaim(Int32 guideId, Int32 userId)
        //{
        //    try
        //    {

        //        var _response = await _dInventory.PostClaim(guideId, userId);
        //        return StatusCode(_response.Status, _response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }
        //}
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






        #endregion


        #region"BackOrder"


        [HttpGet("/api/BackOrder/GetAll")]
        public async Task<IActionResult> GetBackOrders(Int32 userId, Int32 supplierId, Int32? dealerId, Int32? rowFrom, string? filter, DateTime? startdate, DateTime? enddate, bool stockOnly = false)
        {
            try
            {
                var _response = await _dInventory.GetBackOrders(userId, supplierId, dealerId, rowFrom, filter, startdate, enddate, stockOnly);
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
        public async Task<IActionResult> PostBackorder(Models.BackOrder backOrder, Int32 userId)
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



        private MemoryStream ConvertToExcel(List<Models.BackOrder> _backOrders)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("BACKORDER");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "CODIGO";
                worksheet.Cell(1, 3).Value = "DESCRIPCION";
                worksheet.Cell(1, 4).Value = "TIPO";
                worksheet.Cell(1, 5).Value = "CANTIDAD";
                worksheet.Cell(1, 6).Value = "NRO PEDIDO";
                worksheet.Cell(1, 7).Value = "FECHA DE CREACION";
                worksheet.Cell(1, 8).Value = "FECHA DE LLEGADA";
                worksheet.Cell(1, 9).Value = "CONCESIONARIO";
                worksheet.Cell(1, 10).Value = "PLANTA";
                worksheet.Cell(1, 11).Value = "ACTIVA";
                worksheet.Cell(1, 12).Value = "PRECIO";
                worksheet.Cell(1, 13).Value = "COSTO";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:J1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _backOrders.Count; i++)
                {
                    var _backOrder = _backOrders[i];
                    worksheet.Cell(i + 2, 1).Value = _backOrder.Id;
                    worksheet.Cell(i + 2, 2).Value = _backOrder.PartInnerCode;
                    worksheet.Cell(i + 2, 3).Value = _backOrder.PartDescription;
                    worksheet.Cell(i + 2, 4).Value = _backOrder.TypeName;
                    worksheet.Cell(i + 2, 5).Value = _backOrder.Quantity;
                    worksheet.Cell(i + 2, 6).Value = _backOrder.SaleOrderNumber;
                    worksheet.Cell(i + 2, 7).Value = _backOrder.CreatedDate;
                    worksheet.Cell(i + 2, 8).Value = _backOrder.Arrival;
                    worksheet.Cell(i + 2, 9).Value = _backOrder.DealerName;
                    worksheet.Cell(i + 2, 10).Value = _backOrder.SupplierRef;
                    worksheet.Cell(i + 2, 11).Value = _backOrder.IsActive != false ? "SI" : "NO";
                    worksheet.Cell(i + 2, 12).Value = _backOrder.Price;
                    worksheet.Cell(i + 2, 13).Value = _backOrder.Cost;


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

        [HttpGet("/api/BackOrder/Export")]
        public async Task<IActionResult> GetExportBackOrder(Int32 userId, Int32 supplierId, Int32 dealerId)
        {
            try
            {
                List<BackOrder> _response = await _dInventory.GetExportBackOrders(userId, supplierId, dealerId);
                MemoryStream _excel = ConvertToExcel(_response);
                string _fileName = "BackOrder.xlsx";

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


        [HttpPost("/api/BackOrder/Import")]
        public async Task<IActionResult> PostImportBackOrder(IFormFile file, Int32 userId, string? supplierId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {
                List<Models.BackOrder> _BackOrder = await ExceltoPostBackOrder(file, supplierId);

                if (_BackOrder == null || !_BackOrder.Any())
                    return BadRequest("El archivo no contiene datos válidos para procesar.");

                var _response = await _dInventory.PostImportBackOrders(_BackOrder, userId);

                if (_response.Data?.UpdatedRows == 0 && _response.Data?.InsertedRows == 0)
                {
                    _response.Message = "No se realizaron actualizaciones. Verifique que los IDs existan y haya cambios en los datos.";
                }

                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new
                {
                    message = "Error en la importación",
                    detail = ex.Message
                });
            }
        }

        private async Task<List<BackOrder>> ExceltoPostBackOrder(IFormFile file, string? supplierId)
        {
            var _list = new List<BackOrder>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        if (row.IsEmpty()) continue;

                        try
                        {
                            _list.Add(new BackOrder
                            {
                                Id = row.Cell(1).GetValue<int>(),
                                Quantity = row.Cell(5).GetValue<int>(),
                                Arrival = row.Cell(8).GetValue<DateTime>(),
                                SupplierId = supplierId
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error en fila {row.RowNumber()}: {ex.Message}");
                            throw new Exception($"Error en fila {row.RowNumber()}: Formato de datos inválido");
                        }
                    }
                }
            }

            return _list;
        }



        #endregion

        #region "Adjustment"

        //#### GET ALL

        [HttpGet("/api/Adjustment/GetAdjustments")]
        public async Task<IActionResult> GetAdjustments(Int32 userId, Int32 supplierId, Int32 rowFrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            try
            {
                var _response = await _dInventory.GetAdjustments(userId, supplierId, rowFrom, filter, fromDate, upToDate, estatusId);
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

                Models.Response<AdjustmentWithContext> _response = new Models.Response<AdjustmentWithContext>();
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

        //#### GET EXPORT

        private MemoryStream ConvertToExcelAdjustment(List<Models.Adjustment> _adjustments)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AJUSTES");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value = "FECHA";
                worksheet.Cell(1, 3).Value = "USUARIO";
                worksheet.Cell(1, 4).Value = "ESTATUS";
                worksheet.Cell(1, 5).Value = "OBSERVACION";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:E1").SetAutoFilter();

                for (int i = 0; i < _adjustments.Count; i++)
                {
                    var _adjustment = _adjustments[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _adjustment.Id;
                    worksheet.Cell(row, 2).Value = _adjustment.DCreated;
                    worksheet.Cell(row, 3).Value = _adjustment.UserLogin;
                    worksheet.Cell(row, 4).Value = _adjustment.StatusName;
                    worksheet.Cell(row, 5).Value = _adjustment.Comment;
                }

                worksheet.Columns().AdjustToContents();

                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }


        [HttpGet("/api/Adjustment/Export")]
        public async Task<IActionResult> GetExportAdjustment(Int32 userId, Int32 supplierId)
        {
            try
            {
                List<Adjustment> _response = await _dInventory.GetExportAdjustment(userId, supplierId);
                MemoryStream _excel = ConvertToExcelAdjustment(_response);
                string _fileName = "Adjustes.xlsx";

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


                var _get2 = (Adjustment)(await _dInventory.getLastAdjustment(userId, supplierId)).Data;

                if (_get2.Id is null || _get2.Id == 0)
                {

                    var _resp = await _dInventory.PostAdjustment(_new, userId);

                    if (_resp.Processed == false)
                    {
                        throw new Exception(_resp.Message);
                    }

                    Result _resul = new Result();
                    _resul = (Result)(_resp.Data);

                    var _get = await _dInventory.GetAdjustment(userId, _resul.LastId);
                    _new = (Adjustment)(_get.Data);

                }
                else
                {
                    _new = _get2;
                }




                List<AdjustmentDetails> _details = new List<AdjustmentDetails>();

                AdjustmentWithContext _req = new AdjustmentWithContext();
                _req.Adjustment = _new;
                _req.Details = _details;

                Models.Response<AdjustmentWithContext> _response = new Models.Response<AdjustmentWithContext>();
                _response.Data = _req;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                Response<AdjustmentWithContext> _response = new Response<AdjustmentWithContext>();
                _response.SetError(ex);

                return StatusCode(StatusCodes.Status409Conflict, _response);
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

        //#### DELETE ADJUSTMENTDETAILS

        [HttpPost("/api/Adjustment/DeleteAdjustmentDetails")]
        public async Task<IActionResult> DeleteAdjustmentDetail(List<Models.Action> _list, Int32 userId)
        {

            try
            {
                Models.Response<Result> _response = await _dInventory.DeleteAdjustmentDetail(_list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        #endregion


        #region "Invoice Control"

        [HttpGet("/api/InvoiceControl/GetDispatchedControl")]
        public async Task<IActionResult> GetDispatchedControl(int userId, int supplierId, int? rowfrom, string? filter)
        {
            try
            {
                var _response = await _dInventory.GetDispatchedControl(userId, supplierId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/InvoiceControl/GetDispatchedControlTxt")]
        public async Task<IActionResult> GetDispatchedControlTxt(int userId, int supplierId, int? rowfrom, string? filter, int? pendant)
        {
            try
            {
                var _response = await _dInventory.GetDispatchedControlTxt(userId, supplierId, rowfrom, filter, pendant);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("/api/InvoiceControl/GetInvoice")]
        public async Task<IActionResult> GetInvoice(int userId, int supplierId, int? rowfrom, string? filter, int? pendant)
        {
            try
            {
                var _response = await _dInventory.GetInvoice(userId, supplierId, rowfrom, filter, pendant);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("/api/InvoiceControl/PostActions")]
        public async Task<IActionResult> PostInvoiceControl_Actions(List<Models.Action> actions, Int32 userId, Int32 supplierId)
        {
            try
            {
                var _response = await _dInventory.PostInvoiceControl_Actions(actions, userId, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/InvoiceControl/PostInvoiceControl")]
        public async Task<IActionResult> PostInvoiceControl(List<Models.Invoicecontrol> invoiceControls, Int32 userId)
        {
            try
            {
                var _response = await _dInventory.PostInvoiceControl(invoiceControls, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        private List<InvoiceReport> ExcelToList(IFormFile file)
        {
            var _list = new List<InvoiceReport>();


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
                            _list.Add(new InvoiceReport
                            {
                                Vat = row.Cell(26).GetValue<string>(),
                                PartCode = row.Cell(5).GetValue<string>(),
                                Quantity = row.Cell(7).GetValue<int>(),
                                Reference = row.Cell(36).GetValue<string>(),
                                InvoiceNumber = row.Cell(2).GetValue<string>()

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

        [HttpPost("/api/InvoiceControl/ImportInvoiceReport")]
        public async Task<IActionResult> ImportInvoiceReport(IFormFile file, Int32 userId, Int32 supplierId)
        {

            if (file == null || file.Length == 0)
                return BadRequest("No se ha proporcionado un archivo válido.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos Excel (.xlsx)");

            try
            {

                List<InvoiceReport> _list = ExcelToList(file);

                var _response = await _dInventory.ImportInvoiceReport(_list, userId, supplierId);

                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private byte[] ConvertToTxt(List<Invoicecontrol> records)
        {
            var lines = records.Select(r =>
                $"{r.PartInnerCode}\t{r.PartName}\t{r.Dispatched}\t{r.Price:0.00}");

            return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
        }

        [HttpGet("/api/InvoiceControl/ExportTXT")]
        public async Task<IActionResult> ExportTXTByControl(int userId, int supplierId, int controlId)
        {
            var response = await _dInventory.GetDispatchedControlTxtExport(userId, supplierId, controlId);

            List<Invoicecontrol> groupedRecords = response.Data?.ToList();

            if (groupedRecords == null || !groupedRecords.Any())
                return NotFound($"No hay registros agrupados para el control {controlId}");

            var txtBytes = ConvertToTxt(groupedRecords);
            string fileName = $"{controlId.ToString().PadLeft(10, '0')}.txt";
            return File(txtBytes, "text/plain", fileName);
        }

        [HttpPost("/api/Packing/ClonePackage")]
        public async Task<IActionResult> PostReplicatePackage(int packageId, int replicationCount)
        {
            try
            {
                var _response = await _dInventory.PostReplicatePackage(packageId, replicationCount);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion

        #region "CLAIM"

        [HttpGet("/api/Claim/GetAllClaims")]
        public async Task<IActionResult> GetAllClaims(int userId, int? supplierId, int? dealerId, int? rowFrom, DateTime? fromDate, DateTime? upToDate, int? estatusId, string? filter, int? claimId = null)
        {
            try
            {
                var _response = await _dInventory.GetAllClaims(userId, supplierId, dealerId, rowFrom, fromDate, upToDate, estatusId, filter, claimId);
    
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("/api/Claim/GetClaim")]
        public async Task<IActionResult> GetClaim(int userId, int claimId)
        {
            {
                try
                {
                    var _response = await _dInventory.GetClaim(userId, claimId);
                    return StatusCode(_response.Status, _response);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status409Conflict, ex.Message);
                }
            }
        }

        [HttpGet("/api/Claim/GetClaimDetails")]
        public async Task<IActionResult> GetClaimDetails(Int32 claimId)
        {

            try
            {
                var _response = await _dInventory.GetClaimDetails(claimId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }  
        }

        [HttpGet("/api/Claim/GetClaimWithContext")]
        public async Task<IActionResult> GetClaimWithContext(int userId, int claimId)
        {
            try
            {
                var _get = await _dInventory.GetClaim(userId, claimId);   
                var _details = await _dInventory.GetClaimDetails(claimId);

                if (_get.Data == null)
                {
                    return NotFound("Claim not found");
                }

                ClaimWithContext _req = new ClaimWithContext();
                _req.Claim = _get.Data;  
                _req.Details = (List<ClaimDetails>)_details.Data;

                Models.Response<ClaimWithContext> _response = new Models.Response<ClaimWithContext>();
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

        [HttpPost("/api/Claim/PostClaim")]
        public async Task<IActionResult> PostClaim(List<Models.ClaimPart> claims, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInventory.PostClaim(claims, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Claim/PostClaimDetail")]
        public async Task<IActionResult> PostClaimDetail(List<Models.ClaimDetails> detail, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInventory.PostClaimDetails(detail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Claim/NewClaimWithContext")] 
        public async Task<IActionResult> NewClaimWithContext(Int32 userId, Int32 dealerId)
        {
            try
            {

                ClaimPart _new = new ClaimPart();
                _new.Id = 0;
                _new.DealerId = dealerId; 
                _new.UserId = userId;

                if (dealerId == 0)
                {
                    throw new Exception("Concesionario Invalido");
                }

                List<ClaimPart> _list = new List<ClaimPart>();
                _list.Add(_new);


                var _get2 = (ClaimPart)(await _dInventory.GetlastClaim(userId, dealerId)).Data;

                if (_get2.Id is null || _get2.Id == 0)
                {

                    var _resp = await _dInventory.PostClaim(_list, userId);

                    if (_resp.Processed == false)
                    {
                        throw new Exception(_resp.Message);
                    }
                     
                    Result _resul = new Result();
                    _resul = (Result)(_resp.Data);

                    var _get = await _dInventory.GetClaim(userId,_resul.LastId );
                    _new = (ClaimPart)(_get.Data);


                }
                else
                {
                    _new = _get2;
                }



                List<ClaimDetails> _details = new List<ClaimDetails>();

                ClaimWithContext _req = new ClaimWithContext();
                _req.Claim = _new;
                _req.Details = _details;

                Models.Response<ClaimWithContext> _response = new Models.Response<ClaimWithContext>();
                _response.Data = _req;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                Models.Response<ClaimWithContext> _response = new Models.Response<ClaimWithContext>();
                _response.SetError(ex);

                return StatusCode(StatusCodes.Status409Conflict, _response);
            }
        }

        [HttpPost("/api/Claim/PostActionsClaim")]
        public async Task<IActionResult> Post_ActionsClaim(List<Models.Action> actions, Int32 userId)
        {

            try
            {

                var _response = await _dInventory.Post_ActionsClaim(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }



        [HttpPost("/api/Claim/PostActionsClaimDetails")]
        public async Task<IActionResult> Post_ActionsDetails(List<Models.ActionClaim> _list, Int32 userId)
        {

            try
            {

                var _response = await _dInventory.Post_ActionsDetails(_list, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("/api/Claim/DeleteClaimDetails")]
        public async Task<IActionResult> DeleteClaimDetail(List<Models.Action> _list, Int32 userId)
        {

            try
            {
                Models.Response<Result> _response = await _dInventory.DeleteClaimDetail(_list, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        //[HttpPost("/api/Claim/PostClaimActions")]
        //public async Task<IActionResult> PostActions(List<Models.Action> actions, Int32 userId)
        //{

        //    try
        //    {
        //        Models.Response<Result> _response = await _dInventory.PostActions(actions, userId);
        //        return StatusCode(_response.Status, _response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }

        //}

        //[HttpPost("/api/Claim/PostDetailsActions")]
        //public async Task<IActionResult> PostDetailsActions(Int32 userId, List<Models.Action> actions)
        //{

        //    try
        //    { 
        //        var _response = await _dInventory.PostDetailsActions(actions, userId);
        //        return StatusCode(_response.Status, _response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status409Conflict, ex.Message);
        //    }

        //}

        #endregion
    }
}
