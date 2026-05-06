using Azure;
using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Mail;

namespace WebApi.Controllers
{
    [Route("api/Inspection")]
    [ApiController]
    [Authorize]
    public class cInspection : ControllerBase
    {


        private readonly dInspection _dInspection;
        private readonly dAttachment _dAttachment;

        public cInspection(dInspection dInspection, dAttachment dAttachment)
        {
            _dInspection = dInspection;
            _dAttachment = dAttachment;
        }

        #region "PDI"
        [HttpGet("/api/PDI/AccessGroupPDI/GetAll")]
        public async Task<IActionResult> GetAccessGroupPDI(int userId, int supplierId, int? dealerId)
        {
            try
            {
                var _response = await _dInspection.GetAccessGroupPDI(userId, supplierId, dealerId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        [HttpGet("/api/PDI/Transporter/GetAll")]
        public async Task<IActionResult> GetAllTransportPdi()
        {
            try
            {
                var _response = await _dInspection.GetTransportPDI();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/PDI/Dealer/GetAll")]
        public async Task<IActionResult> GetAllDealerPdi(Int32 supplierId)
        {
            try
            {
                var _response = await _dInspection.GetAllDealer(supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion 

        #region "AREA"

        [HttpGet("/api/Area/GetOne")]
        public async Task<IActionResult> GetOneArea(int userId, int areaId)
        {
            try
            {
                var _response = await _dInspection.GetArea(userId, areaId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Area/GetAll")]
        public async Task<IActionResult> GetAllAreas(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active)
        {
            try
            {
                var _response = await _dInspection.GetAllAreas(userId, supplierId, dealerId, rowFrom, filter, active);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        private MemoryStream ConvertToExcelArea(List<Models.Area> _area)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AREAS");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value = "AREA";
                worksheet.Cell(1, 3).Value = "ACTIVO";
                worksheet.Cell(1, 4).Value = "ACTUALIZACION";
                worksheet.Cell(1, 5).Value = "ACTUALIZADO POR";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:E1").SetAutoFilter();

                for (int i = 0; i < _area.Count; i++)
                {
                    var _areas = _area[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _areas.Id;
                    worksheet.Cell(row, 2).Value = _areas.Name;
                    worksheet.Cell(row, 3).Value = _areas.IsActive != false ? "SI" : "NO";
                    worksheet.Cell(row, 4).Value = _areas.Updated;
                    worksheet.Cell(row, 5).Value = _areas.UpdatedBy;
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


        [HttpGet("/api/Area/Export")]
        public async Task<IActionResult> GetAreaExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active)
        {
            try
            {
                List<Area> _response = await _dInspection.GetAreaExport(userId, supplierId, dealerId, filter, active);
                MemoryStream _excel = ConvertToExcelArea(_response);
                string _fileName = "Area.xlsx";

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
        [HttpPost("/api/Area/Post")]
        public async Task<IActionResult> PostArea(List<Models.Area> AreaId, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInspection.PostAreas(AreaId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Area/PostActions")]
        public async Task<IActionResult> PostAreaActions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dInspection.PostAreaActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion

        #region "FASE"

        [HttpGet("/api/Fase/GetOne")]
        public async Task<IActionResult> GetOneFase(Int32 userId, int fasesId)
        {
            try
            {
                var _response = await _dInspection.GetFase(userId, fasesId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Fase/GetAll")]
        public async Task<IActionResult> GetAllFases(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active)
        {
            try
            {
                var _response = await _dInspection.GetAllFases(userId, supplierId, dealerId, rowFrom, filter, active);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        private MemoryStream ConvertToExcelFase(List<Models.Fase> _fase)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("FASES");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value = "FASE";
                worksheet.Cell(1, 3).Value = "ACTIVO";
                worksheet.Cell(1, 4).Value = "AREA";
                worksheet.Cell(1, 5).Value = "ACTUALIZACION";
                worksheet.Cell(1, 6).Value = "ACTUALIZADO POR";

                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:F1").SetAutoFilter();

                for (int i = 0; i < _fase.Count; i++)
                {
                    var _fases = _fase[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _fases.Id;
                    worksheet.Cell(row, 2).Value = _fases.Name;
                    worksheet.Cell(row, 3).Value = _fases.IsActive != false ? "SI" : "NO";
                    worksheet.Cell(row, 4).Value = _fases.AreaName;
                    worksheet.Cell(row, 5).Value = _fases.Updated;
                    worksheet.Cell(row, 6).Value = _fases.UpdatedBy;
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


        [HttpGet("/api/Fase/Export")]
        public async Task<IActionResult> GetFaseExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active)
        {
            try
            {
                List<Fase> _response = await _dInspection.GetFaseExport(userId, supplierId, dealerId, filter, active);
                MemoryStream _excel = ConvertToExcelFase(_response);
                string _fileName = "Fase.xlsx";

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

        [HttpPost("/api/Fase/Post")]
        public async Task<IActionResult> PostFase(List<Models.Fase> FaseId, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInspection.PostFases(FaseId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Fase/PostActions")]
        public async Task<IActionResult> PostFaseActions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dInspection.PostFaseActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion

        #region "FEATURETYPE"

        [HttpGet("/api/FeatureType/GetOne")]
        public async Task<IActionResult> GetOneFeatureType(Int32 userId, int featureTypeId)
        {
            try
            {
                var _response = await _dInspection.GetFeatureType(userId, featureTypeId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/FeatureType/GetAll")]
        public async Task<IActionResult> GetAllFeatureType(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active)
        {
            try
            {
                var _response = await _dInspection.GetAllFeatureType(userId, supplierId, dealerId, rowFrom, filter, active);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        private MemoryStream ConvertToExcelFeatureType(List<Models.FeatureType> _featureType)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("TIPOS DE CARACTERISTICAS");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value = "TIPO CARACTERISTICA";
                worksheet.Cell(1, 3).Value = "ACTIVO";
                worksheet.Cell(1, 4).Value = "FASE";
                worksheet.Cell(1, 5).Value = "AREA";
                worksheet.Cell(1, 6).Value = "ACTUALIZACION";
                worksheet.Cell(1, 7).Value = "ACTUALIZADO POR";

                var headerRange = worksheet.Range("A1:G1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:G1").SetAutoFilter();

                for (int i = 0; i < _featureType.Count; i++)
                {
                    var _featureTypes = _featureType[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _featureTypes.Id;
                    worksheet.Cell(row, 2).Value = _featureTypes.Name;
                    worksheet.Cell(row, 3).Value = _featureTypes.IsActive != false ? "SI" : "NO";
                    worksheet.Cell(row, 4).Value = _featureTypes.FaseName;
                    worksheet.Cell(row, 5).Value = _featureTypes.AreaName;
                    worksheet.Cell(row, 6).Value = _featureTypes.Updated;
                    worksheet.Cell(row, 7).Value = _featureTypes.UpdatedBy;
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


        [HttpGet("/api/FeatureType/Export")]
        public async Task<IActionResult> GetFeatureTypeExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active)
        {
            try
            {
                List<FeatureType> _response = await _dInspection.GetFeatureTypeExport(userId, supplierId, dealerId, filter, active);
                MemoryStream _excel = ConvertToExcelFeatureType(_response);
                string _fileName = "TiposCaracteristicas.xlsx";

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

        [HttpPost("/api/FeatureType/Post")]
        public async Task<IActionResult> PostFeatureType(List<Models.FeatureType> featureTypeId, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInspection.PostFeatureType(featureTypeId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/FeatureType/PostActions")]
        public async Task<IActionResult> PostFeatureTypeActions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dInspection.PostFeatureTypeActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion

        #region "FEATURE"

        [HttpGet("/api/Feature/GetOne")]
        public async Task<IActionResult> GetOneFeature(Int32 userId, int featureId)
        {
            try
            {
                var _response = await _dInspection.GetFeature(userId, featureId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Feature/GetAll")]
        public async Task<IActionResult> GetAllFeature(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active, int? modelId)
        {
            try
            {
                var _response = await _dInspection.GetAllFeature(userId, supplierId, dealerId, rowFrom, filter, active, modelId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        [HttpGet("/api/FeatureValueType/GetAll")]
        public async Task<IActionResult> GetFeatureValueType(Int32 userId)
        {
            try
            {
                var _response = await _dInspection.GetFeatureValueType(userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        [HttpGet("/api/FeatureOption/GetAll")]
        public async Task<IActionResult> GetFeatureOption(Int32 userId, Int32 featureId)
        {
            try
            {
                var _response = await _dInspection.GetFeatureOption(userId, featureId, null);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        [HttpGet("/api/FeatureOption/True")]
        public async Task<IActionResult> GetFeatureOptionTrue(Int32 userId, Int32 featureId)
        {
            try
            {
                var _response = await _dInspection.GetFeatureOption(userId, featureId, true);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }
        private MemoryStream ConvertToExcelFeature(List<Models.Feature> _feature)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("CARACTERISTICAS");

                // Encabezados (fila 1)
                worksheet.Cell(1, 1).Value = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value = "CARACTERISTICA";
                worksheet.Cell(1, 3).Value = "VALOR POR DEFECTO";
                worksheet.Cell(1, 4).Value = "ACTIVO";
                worksheet.Cell(1, 5).Value = "MODELO";
                worksheet.Cell(1, 6).Value = "TIPO";
                worksheet.Cell(1, 7).Value = "FASE";
                worksheet.Cell(1, 8).Value = "AREA";
                worksheet.Cell(1, 9).Value = "ACTUALIZACION";
                worksheet.Cell(1, 10).Value = "ACTUALIZADO POR";

                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:J1").SetAutoFilter();

                for (int i = 0; i < _feature.Count; i++)
                {
                    var _features = _feature[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _features.Id;
                    worksheet.Cell(row, 2).Value = _features.Name;
                    worksheet.Cell(row, 3).Value = _features.DefaultValue != false ? "SI" : "NO";
                    worksheet.Cell(row, 4).Value = _features.IsActive != false ? "SI" : "NO";
                    worksheet.Cell(row, 5).Value = _features.ModelName;
                    worksheet.Cell(row, 6).Value = _features.FeatureTypeName;
                    worksheet.Cell(row, 7).Value = _features.FaseName;
                    worksheet.Cell(row, 8).Value = _features.AreaName;
                    worksheet.Cell(row, 9).Value = _features.Updated;
                    worksheet.Cell(row, 10).Value = _features.UpdatedBy;
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


        [HttpGet("/api/Feature/Export")]
        public async Task<IActionResult> GetFeatureExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active, int? modelId)
        {
            try
            {
                List<Feature> _response = await _dInspection.GetFeatureExport(userId, supplierId, dealerId, filter, active, modelId);
                MemoryStream _excel = ConvertToExcelFeature(_response);
                string _fileName = "Caracteristicas.xlsx";

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
        [HttpPost("/api/Feature/Post")]
        public async Task<IActionResult> PostFeature(List<Models.Feature> featureId, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInspection.PostFeature(featureId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Feature/PostActions")]
        public async Task<IActionResult> PostFeatureActions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dInspection.PostFeatureActions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/FeatureOption/Post")]
        public async Task<IActionResult> PostFeatureOption(List<Models.FeatureOption> FeatureOptionId, Int32 userId)
        {
            try
            {
                Models.Response<Result> _response = await _dInspection.PostFeatureOption(FeatureOptionId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }
        #endregion

        #region "INSPECTIONS"

        [HttpGet("/api/Inspections/TableGetAll")]
        public async Task<IActionResult> TableGetAll(Int32? supplierId, Int32? rowfrom, string? filter)
        {
            try
            {
                var _response = await _dInspection.TableGetAllInspections(supplierId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Inspections/GetAll")]
        public async Task<IActionResult> GetAll(Int32? AreaId = null, bool? IsCompleted = null)
        {
            try
            {
                var _response = await _dInspection.GetAllInspections(AreaId, IsCompleted);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/Inspections/GetOne")]
        public async Task<IActionResult> GetOne(Int32 InspectionId)
        {
            try
            {
                var _response = await _dInspection.GetOneInspection(InspectionId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/Inspections/Post_Inspections")]
        public async Task<IActionResult> Post_Inspections(List<Models.Inspection> _inspections)
        {
            try
            {
                var _response = await _dInspection.Post_Inspections(_inspections);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("/api/Inspections/GetDealers")]
        public async Task<IActionResult> InspectionGetDealers(string ids)
        {
            try
            {
                var _response = await _dInspection.InspectionGetDealers(ids);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        #endregion

        #region "INSPECTIONDETAILS"

        [HttpGet("/api/InspectionsDetails/GetAll")]
        public async Task<IActionResult> GetAllInspectionsDetails(Int32? InspectionId = null, Int32? AreaId = null, Int32? FaseId = null, Int32? FeatureTypeId = null)
        {
            try
            {
                var _response = await _dInspection.GetAllInspectionsDetails(InspectionId, AreaId, FaseId, FeatureTypeId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("/api/InspectionsDetails/GetOne")]
        public async Task<IActionResult> GetOneInspectionsDetails(Int32 DetailInspectionId)
        {
            try
            {
                var _response = await _dInspection.GetOneInspectionsDetails(DetailInspectionId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/InspectionsDetails/Post_InspectionsDetail")]
        public async Task<IActionResult> Post_InspectionsDetail(List<Models.InspectionDetail> _details)
        {
            try
            {
                var _response = await _dInspection.Post_InspectionDetails(_details);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region "INSPECTION_FASE"

        [HttpGet("/api/InspectionFase/GetAll")]
        public async Task<IActionResult> GetAllInspectionFase(Int32? AreaId = null, Int32? FaseId = null, Int32? InspectionId = null, bool? IsCompleted = null, bool? IsCompletedInspection = null)
        {
            try
            {
                var _response = await _dInspection.GetAllInspectionFase(AreaId, FaseId, InspectionId, IsCompleted, IsCompletedInspection);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("/api/InspectionFase/Post_InspectionFase")]
        public async Task<IActionResult> Post_InspectionFase(List<Models.InspectionFase> _inspectionsFase)
        {
            try
            {
                var _response = await _dInspection.Post_InspectionFase(_inspectionsFase);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #endregion

        #region "FULLINSPECTION"
        [HttpPost("/api/Post_FullInspection")]
        public async Task<IActionResult> Post_FullInspection(List<Models.FullInspection> _fullInspection, Int32 userId, Int32 supplierId)
        {
            try
            {
                var _response = await _dInspection.Post_FullInspection(_fullInspection, userId, supplierId);
                return StatusCode(StatusCodes.Status200OK, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

        #region "EXPORTS"

        [HttpGet("/api/Inspection/ExportPdf")]
        public async Task<IActionResult> GetExportPdf(Int32 InspectionId)
        {
            // Obtengo la información de la cabecera de la inspección
            var response_inspection = await _dInspection.GetOneInspection(InspectionId);

            // Obtengo el valor de cada uno de los features
            var response_detail = await _dInspection.GetAllInspectionsDetails(
                InspectionId,
                AreaId: null,
                FaseId: null,
                FeatureTypeId: null
            );


            // 1. Obtener IDs de los detalles que tienen archivos
            List<int> idsConArchivos = response_detail.Data
                                        .Where(item => item.HasFiles == true)
                                        .Select(item => item.Id ?? 0)
                                        .ToList();

            // 2. Preparar tareas de búsqueda (Detalles + Encabezado de Inspección)
            var tareasAdjuntos = idsConArchivos
                .Select(id => _dAttachment.GetAll("INSPECCION-TIPOS-CARACTERISTICAS", id))
                .ToList();

            // Si la inspección general tiene archivos (Modulo: INSPECCION-INSPECCION)
            if (response_inspection.Data.HasFiles == true)
            {
                tareasAdjuntos.Add(_dAttachment.GetAll("INSPECCION-INSPECCION", InspectionId));
            }

            var resultadosAdjuntos = await Task.WhenAll(tareasAdjuntos);

            // 3. Configuración de rutas para lectura física
            string baseUrl = Util.Setting.AttachmentUrl;
            string attachmentUrl = Path.Combine($"\\\\{Environment.MachineName}", baseUrl);
            var _modules = await _dAttachment.GetModule(null, 1);

            List<byte[]> imagenesParaPdf = new List<byte[]>();
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".bmp" };

            // 4. Aplanar resultados y leer archivos físicos (Solo imágenes)
            var todosLosAdjuntos = resultadosAdjuntos
                                    .Where(r => r.Data != null)
                                    .SelectMany(r => (List<Models.Attachment>)r.Data)
                                    .ToList();

            foreach (var attachment in todosLosAdjuntos)
            {
                string extension = Path.GetExtension(attachment.FileName)?.ToLower();
                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension)) continue;

                string modulePath = _modules.FirstOrDefault(m => m.Id == attachment.ModuleId)?.Name ?? "Unknown";
                string filePath = Path.Combine(attachmentUrl, modulePath, attachment.RecordId.ToString(), attachment.FileName);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    imagenesParaPdf.Add(fileBytes);
                }
            }

            // 5. Agrupación de datos para la tabla
            var datosInspeccion = response_detail.Data
             .GroupBy(d => d.FeatureType ?? "SIN CATEGORÍA")
             .Select(grupo => (
                 Grupo: grupo.Key,
                 Items: grupo.Select(item =>
                 {
                     string observacionFinal;
                     if (item.FeatureValueTypeId == "2")
                         observacionFinal = item.OptionSelected ?? "";
                     else if (item.FeatureValueTypeId == "3")
                         observacionFinal = item.Value?.ToString() ?? "-";
                     else
                         observacionFinal = item.Value == 1 ? "OK" : (item.Observation ?? "-");

                     return (Nombre: item.Feature ?? "Sin Nombre", Obs: observacionFinal, Type: item.FeatureValueTypeId);
                 }).ToList()
             )).ToList();

            try
            {
                byte[] pdfBytes;

                if (response_inspection.Data.Format == "FORMAT_CALIDAD")
                {
                    pdfBytes = await GeneratePdfReportAsync(
                       "INSPECCIÓN DE CALIDAD",
                       BrandId: response_inspection.Data.BrandId ?? 0,
                       SupplierId: response_inspection.Data.SupplierId ?? 0,
                       response_inspection.Data,
                       datosInspeccion,
                       imagenesParaPdf
                   );
                } 
                else
                {
                    pdfBytes = await GenerateDispatchGuidePdfAsync(
                        "GUIA DE DESPACHO",
                        BrandId: response_inspection.Data.BrandId ?? 0,
                        SupplierId: response_inspection.Data.SupplierId ?? 0,
                        response_inspection.Data,
                        datosInspeccion,
                        imagenesParaPdf
                    );
                }

                if (pdfBytes == null || pdfBytes.Length == 0)
                    return NotFound("No se pudo generar el contenido del PDF.");

                return File(pdfBytes, "application/pdf", $"Inspeccion_{InspectionId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
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

        private async Task<byte[]> GeneratePdfReportAsync(
            string titleReport,
            Int32 BrandId,
            Int32 SupplierId,
            Inspection DataInspection, // Cabecera
            List<(string Grupo, List<(string Nombre, string Obs, string Type)> Items)> datosInspeccion, // Features agrupados
            List<byte[]> imagenesParaPdf) //Adjuntos
        {
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", SupplierId);
            string brandImagePath = await GetFirstAttachmentFilePath("RECURSOS-MARCAS", BrandId);

            QuestPDF.Settings.License = LicenseType.Community;

            return QuestPDF.Fluent.Document.Create(container =>
            {
                // --- PÁGINA 1: DATOS E INSPECCIÓN ---
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    // Cabecera
                    page.Header().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(supplierImagePath))
                                col.Item().Height(30).Image(supplierImagePath).FitArea();
                        });

                        row.RelativeItem(2).AlignCenter().AlignMiddle().Column(col =>
                        {
                            col.Item().Text(titleReport).FontSize(16).SemiBold().AlignCenter();
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            if (!string.IsNullOrEmpty(brandImagePath))
                                col.Item().Height(50).Width(60).Image(brandImagePath).FitArea();
                        });
                    });

                    // Contenido Principal
                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        // 1. Fechas
                        column.Item().PaddingBottom(5).Row(row =>
                        {
                            row.RelativeItem().Text(t => { t.Span("Fecha Recepción: ").FontSize(10); t.Span(DataInspection.Created?.ToString("yyyy-MM-dd")).FontSize(10); });
                            row.RelativeItem().AlignRight().Text(t => { t.Span("Fecha Inspección: ").FontSize(10); t.Span(DataInspection.DClose?.ToString("yyyy-MM-dd")).FontSize(10); });
                        });

                        // 2. Características del Vehículo
                        column.Item().PaddingBottom(5).AlignCenter().Text("CARACTERÍSTICAS DEL VEHÍCULO").SemiBold().FontSize(12);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
                            table.Cell().Element(CellStyle).Text(t => { t.Span("VIN: ").SemiBold().FontSize(10); t.Span(DataInspection.Vin).FontSize(10); });
                            table.Cell().Element(CellStyle).Text(t => { t.Span("Placa: ").SemiBold().FontSize(10); t.Span(DataInspection.VehiclePlate).FontSize(10); });
                            table.Cell().Element(CellStyle).Text(t => { t.Span("Marca y Modelo: ").SemiBold().FontSize(10); t.Span($"{DataInspection.Brand}-{DataInspection.Model}").FontSize(10); });
                            table.Cell().Element(CellStyle).Text(t => { t.Span("Color: ").SemiBold().FontSize(10); t.Span(DataInspection.Color).FontSize(10); });
                            table.Cell().Element(CellStyle).Text(t => { t.Span("Año: ").SemiBold().FontSize(10); t.Span(DataInspection.Year).FontSize(10); });
                        });

                        // 3. LISTA DE INSPECCIÓN
                        column.Item().PaddingTop(15).PaddingBottom(15).AlignCenter().Text("LISTA DE INSPECCIÓN").SemiBold().FontSize(12);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); columns.RelativeColumn(1);
                                columns.RelativeColumn(2); columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderStyle).Text("DESCRIPCIÓN");
                                header.Cell().Element(HeaderStyle).Text("OBSERVACIÓN");
                                header.Cell().Element(HeaderStyle).Text("DESCRIPCIÓN");
                                header.Cell().Element(HeaderStyle).Text("OBSERVACIÓN");
                            });

                            foreach (var grupo in datosInspeccion)
                            {
                                table.Cell().ColumnSpan(4).Border(1).Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                                     .Padding(2).AlignCenter().Text(grupo.Grupo.ToUpper()).SemiBold().FontSize(9);

                                for (int i = 0; i < grupo.Items.Count; i += 2)
                                {
                                    var item1 = grupo.Items[i];
                                    var item2 = (i + 1 < grupo.Items.Count) ? grupo.Items[i + 1] : (Nombre: "", Obs: "", Type: "");

                                    table.Cell().Element(RowStyle).Text(item1.Nombre);
                                    table.Cell().Element(RowStyle).AlignCenter().Text(item1.Obs);
                                    table.Cell().Element(RowStyle).Text(item2.Nombre);
                                    table.Cell().Element(RowStyle).AlignCenter().Text(item2.Obs);
                                }
                            }
                        });
                    });

                    // Footer de la primera página
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ").FontSize(10);
                        x.CurrentPageNumber().FontSize(10);
                        x.Span(" de ").FontSize(10);
                        x.TotalPages().FontSize(10);
                    });
                });

                // --- PÁGINAS ADICIONALES: UNA POR FOTO ---
                if (imagenesParaPdf != null && imagenesParaPdf.Any())
                {
                    foreach (var foto in imagenesParaPdf)
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(1, Unit.Centimetre);
                            page.PageColor(QuestPDF.Helpers.Colors.White);

                            page.Header().PaddingBottom(10).Text("ANEXO FOTOGRÁFICO")
                                .SemiBold().FontSize(14).AlignCenter();

                            page.Content().PaddingVertical(20).AlignCenter().AlignMiddle().Column(c =>
                            {
                                c.Item().Border(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Column(imgContainer =>
                                {
                                    // Maximizamos el espacio para la foto centrada
                                    imgContainer.Item().MaxHeight(650).Image(foto).FitArea();
                                });

                                c.Item().PaddingTop(15).AlignCenter().Text("Detalle de Inspección de Unidad")
                                    .FontSize(11).Italic().FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                            });

                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.Span("Página ").FontSize(10);
                                x.CurrentPageNumber().FontSize(10);
                                x.Span(" de ").FontSize(10);
                                x.TotalPages().FontSize(10);
                            });
                        });
                    }
                }
            })
            .GeneratePdf();

            // Estilos Helpers
            static IContainer CellStyle(IContainer container) => container.PaddingVertical(1);
            static IContainer HeaderStyle(IContainer container) => container.Border(1).AlignCenter().Padding(2).DefaultTextStyle(x => x.SemiBold().FontSize(8));
            static IContainer RowStyle(IContainer container) => container.Border(1).PaddingHorizontal(4).PaddingVertical(2).DefaultTextStyle(x => x.FontSize(8));
        }

        private async Task<byte[]> GenerateDispatchGuidePdfAsync(
            string titleReport,
            Int32 BrandId,
            Int32 SupplierId,
            Inspection DataInspection,
            List<(string Grupo, List<(string Nombre, string Obs, string Type)> Items)> datosInspeccion,
            List<byte[]> imagenesParaPdf)
        {
            // Obtención de logos (reutilizando tu lógica existente)
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", SupplierId);
            string brandImagePath = await GetFirstAttachmentFilePath("RECURSOS-MARCAS", BrandId);

            QuestPDF.Settings.License = LicenseType.Community;

            return QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);

                    // --- CABECERA ESTILO "GUIA DE DESPACHO" ---
                    page.Header().Column(headerCol =>
                    {
                        // Fila Superior: Logos y Título
                        headerCol.Item().Border(1).Row(row =>
                        {
                            // Logo Empresa
                            row.RelativeItem().Padding(5).Column(col =>
                            {
                                if (!string.IsNullOrEmpty(supplierImagePath))
                                    col.Item().Height(35).Image(supplierImagePath).FitArea();
                            });

                            // Título y Contacto Central
                            row.RelativeItem(3).BorderLeft(1).BorderRight(1).AlignCenter().Column(col =>
                            {
                                col.Item().PaddingTop(4).Text(titleReport).FontSize(12).SemiBold().AlignCenter();
                                col.Item().PaddingVertical(2).Text("Teléfono: (+58)(424) - 532. 26 39\nDirección: Calle Principal local parcela 53 Parque Industrial del Este Nro S/N caserio las Piedras. Yaritagua Yaracuy").FontSize(8).AlignCenter();
                            });

                            // Logo Marca
                            row.RelativeItem().Padding(5).AlignRight().Column(col =>
                            {
                                if (!string.IsNullOrEmpty(brandImagePath))
                                    col.Item().Height(35).Image(brandImagePath).FitArea();
                            });
                        });

                        // Fila Secundaria: Lugar, Fecha y Control (Basado en imagen_f35954.png)
                        headerCol.Item().BorderHorizontal(1).BorderBottom(1).Row(row =>
                        {
                            row.RelativeItem().Border(1).Column(c => {
                                c.Item().Element(HeaderStyle).Text("LUGAR DE EMISIÓN");
                                c.Item().Element(SubHeaderValueStyle).Text("YARITAGUA");
                            });
                            row.RelativeItem().Border(1).Column(c => {
                                c.Item().Element(HeaderStyle).Text("FECHA DE EMISIÓN");
                                c.Item().Element(SubHeaderValueStyle).Text(DateTime.Now.ToString("dd/MM/yyyy"));
                            });
                            row.RelativeItem().Border(1).Column(c => {
                                c.Item().Element(HeaderStyle).Text("CONTROL");
                                c.Item().Element(SubHeaderValueStyle).Text($"Nº. D-{DataInspection.Id?.ToString("D10")}").FontColor(QuestPDF.Helpers.Colors.Red.Medium).SemiBold();
                            });
                        });
                    });

                    // --- CONTENIDO ---
                    page.Content().Column(column =>
                    {
                        // Información del Vehículo / Cliente
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(1); cols.RelativeColumn(1); cols.RelativeColumn(1); });

                            table.Cell().Element(HeaderStyle).Text("RAZON SOCIAL");
                            table.Cell().Element(HeaderStyle).Text("RIF");
                            table.Cell().Element(HeaderStyle).Text("CIUDAD");
                            table.Cell().Element(HeaderStyle).Text("TELEFONO");

                            table.Cell().Element(RowStyle).Text(DataInspection.NameSupplier); // Ejemplo estático
                            table.Cell().Element(RowStyle).Text(DataInspection.Vin);
                            table.Cell().Element(RowStyle).Text(DataInspection.VehiclePlate);
                            table.Cell().Element(RowStyle).Text(""); // Ejemplo estático
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols => { cols.RelativeColumn(1); });
                            table.Cell().Element(HeaderStyle).Text("DIRECCIÓN DE DESTINO");
                            table.Cell().Element(RowStyle).Text(""); // Ejemplo estático
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols => { cols.RelativeColumn(1); cols.RelativeColumn(1); cols.RelativeColumn(1); cols.RelativeColumn(1); cols.RelativeColumn(1); });
                            table.Cell().Element(HeaderStyleBlue).Text("MODELO");
                            table.Cell().Element(HeaderStyleBlue).Text("AÑO");
                            table.Cell().Element(HeaderStyleBlue).Text("COLOR");
                            table.Cell().Element(HeaderStyleBlue).Text("SERIAL DE CARROCERÍA");
                            table.Cell().Element(HeaderStyleBlue).Text("PLACA");


                            table.Cell().Element(RowStyle).Text(DataInspection.Model);
                            table.Cell().Element(RowStyle).Text(DataInspection.Year);
                            table.Cell().Element(RowStyle).Text(DataInspection.Color);
                            table.Cell().Element(RowStyle).Text(DataInspection.Vin);
                            table.Cell().Element(RowStyle).Text(DataInspection.VehiclePlate);
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols => {
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(4);
                            });

                            // Primera celda de la tabla principal - contiene la subtabla
                            table.Cell().Column(cellContent =>
                            {
                                // Crear subtabla dentro de esta celda
                                cellContent.Item().Table(subTable =>
                                {
                                    subTable.ColumnsDefinition(cols => {
                                        cols.RelativeColumn(1);
                                        cols.RelativeColumn(2);
                                    });

                                    subTable.Cell().ColumnSpan(2).Element(HeaderStyleBlue).Text("CODIGO DE DAÑO");
                                    subTable.Cell().Element(RowStyle).Text("1");
                                    subTable.Cell().Element(RowStyle).Text("DOBLADO");
                                    subTable.Cell().Element(RowStyle).Text("2");
                                    subTable.Cell().Element(RowStyle).Text("ROTO");
                                    subTable.Cell().Element(RowStyle).Text("3");
                                    subTable.Cell().Element(RowStyle).Text("CORTADO");
                                    subTable.Cell().Element(RowStyle).Text("4");
                                    subTable.Cell().Element(RowStyle).Text("PERFORADO");
                                    subTable.Cell().Element(RowStyle).Text("5");
                                    subTable.Cell().Element(RowStyle).Text("RAYADO");
                                    subTable.Cell().Element(RowStyle).Text("6");
                                    subTable.Cell().Element(RowStyle).Text("GOLPEADO");
                                    subTable.Cell().Element(RowStyle).Text("7");
                                    subTable.Cell().Element(RowStyle).Text("DESCONCHADO");
                                    subTable.Cell().Element(RowStyle).Text("8");
                                    subTable.Cell().Element(RowStyle).Text("SUCIO");
                                    subTable.Cell().Element(RowStyle).Text("9");
                                    subTable.Cell().Element(RowStyle).Text("FALTANTE");
                                    subTable.Cell().Element(RowStyle).Text("10");
                                    subTable.Cell().Element(RowStyle).Text("OTROS");

                                    subTable.Cell().ColumnSpan(2).Element(HeaderStyleBlue).Text("TAMAÑO DEL DAÑO");
                                    subTable.Cell().Element(RowStyle).Text("1");
                                    subTable.Cell().Element(RowStyle).Text("MENOS DE 5cms");
                                    subTable.Cell().Element(RowStyle).Text("2");
                                    subTable.Cell().Element(RowStyle).Text("DE 6 A 10cms");
                                    subTable.Cell().Element(RowStyle).Text("3");
                                    subTable.Cell().Element(RowStyle).Text("DE 11 A 20 cms");
                                    subTable.Cell().Element(RowStyle).Text("4");
                                    subTable.Cell().Element(RowStyle).Text("DE 21 A 30 cms");
                                    subTable.Cell().Element(RowStyle).Text("5");
                                    subTable.Cell().Element(RowStyle).Text("MAS DE 30 cms");
                                    subTable.Cell().Element(RowStyle).Text("6");
                                    subTable.Cell().Element(RowStyle).Text("REEMPLAZO DE PIEZA");
                                });
                            });

                            // Segunda celda de la tabla principal
                            table.Cell().Element(RowStyle).Text("");

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(1);
                                });

                                foreach (var grupo in datosInspeccion)
                                 {
                                    table.Cell().ColumnSpan(4).Border(1).Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                                                 .Padding(2).AlignCenter().Text(grupo.Grupo.ToUpper()).SemiBold().FontSize(8);

                                    for (int i = 0; i < grupo.Items.Count; i++)
                                    {
                                        var item = grupo.Items[i];

                                        table.Cell().Column(cellContent =>
                                        {
                                            // Crear subtabla dentro de esta celda
                                            cellContent.Item().Table(subTable =>
                                            {
                                                subTable.ColumnsDefinition(cols =>
                                                {
                                                    cols.RelativeColumn(1);
                                                    cols.RelativeColumn(1);
                                                });

                                                subTable.Cell().ColumnSpan(2).Element(HeaderStyle).Height(15).AlignMiddle().Text(item.Nombre);
                                                subTable.Cell().ColumnSpan(2).Element(RowStyle).Height(9).Text(item.Obs);
                                            });
                                        });
                                    }
                                }
                            });

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols => { cols.RelativeColumn(1); });
                                table.Cell().Element(HeaderStyle).Text("Observaciones");
                                table.Cell().Element(RowStyle).Height(30).Text(DataInspection.Comment);
                            });

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols => 
                                { 
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(1);
                                });

                                table.Cell().Element(HeaderStyle).Text("GERENCIA DE DISTRIBUCIÓN Y LOGÍSTICA");
                                table.Cell().Element(HeaderStyle).Text("TRANSPORTE");
                                table.Cell().Element(HeaderStyle).Text("SEGURIDAD");
                                table.Cell().Element(HeaderStyle).Text("GCONCESIONARIO / CLIENTE FINAL");

                                table.Cell().Element(RowStyle).AlignLeft().Text($"Nombre: {DataInspection.ClosedByName}");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Nombre: {DataInspection.TransporterName}");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Nombre:");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Nombre: {DataInspection.RecepByName}" );

                                table.Cell().Element(RowStyle).AlignLeft().Text($"Cargo: DESPACHADOR");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Cedula: {DataInspection.TransporterByVVTA}");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Cedula:");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"RIF: {DataInspection.RecepByVVTA}");

                                table.Cell().Element(RowStyle).AlignLeft().Text($"Fecha: {DataInspection.DClose}");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Fecha: {DataInspection.DClose}");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Fecha: {DataInspection.DClose}");
                                table.Cell().Element(RowStyle).AlignLeft().Text($"Fecha:");

                                table.Cell().Element(RowStyle).AlignLeft().Height(30).Text("Firma / SELLO:");
                                table.Cell().Element(RowStyle).AlignLeft().Height(30).Text("Firma / SELLO:");
                                table.Cell().Element(RowStyle).AlignLeft().Height(30).Text("Firma / SELLO:");
                                table.Cell().Element(RowStyle).AlignLeft().Height(30).Text("Firma / SELLO:");

                            });
                        });
                    });
                });

                // Páginas de fotos (Anexo)
                if (imagenesParaPdf?.Any() == true)
                {
                    foreach (var foto in imagenesParaPdf)
                    {
                        container.Page(page => {
                            page.Size(PageSizes.A4);
                            page.Margin(1, Unit.Centimetre);
                            page.Content().AlignCenter().Image(foto).FitArea();
                        });
                    }
                }
            }).GeneratePdf();

            // --- ESTILOS LOCALES ---
            static IContainer SubHeaderLabelStyle(IContainer container) => container.AlignCenter().Background(QuestPDF.Helpers.Colors.Grey.Lighten4).PaddingVertical(1).DefaultTextStyle(x => x.FontSize(7).SemiBold());
            static IContainer SubHeaderValueStyle(IContainer container) => container.AlignCenter().PaddingVertical(2).DefaultTextStyle(x => x.FontSize(8));
            static IContainer HeaderStyle(IContainer container) => container.Border(1).Background(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(2).AlignCenter().DefaultTextStyle(x => x.SemiBold().FontSize(6));
            static IContainer HeaderStyleBlue(IContainer container) => container.Border(1).Background(QuestPDF.Helpers.Colors.Blue.Accent3).Padding(2).AlignCenter().DefaultTextStyle(x => x.SemiBold().FontSize(6).FontColor(QuestPDF.Helpers.Colors.White));
            static IContainer RowStyle(IContainer container) => container.Border(1).Padding(2).AlignCenter().DefaultTextStyle(x => x.FontSize(7));
        }
        #endregion
    }
}
