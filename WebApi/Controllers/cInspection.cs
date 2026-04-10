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
 
namespace WebApi.Controllers
{
    [Route("api/Inspection")]
    [ApiController]
    [Authorize]
    public class cInspection : ControllerBase
    {
        

        private readonly dInspection _dInspection;  

        public cInspection(dInspection dInspection, dModel dModel)
        { 
            _dInspection = dInspection;
        }

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
        public async Task<IActionResult> GetAllAreas(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
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
        public async Task<IActionResult> GetAreaExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
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
        public async Task<IActionResult> GetOneFase(int userId, int fasesId)
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
        public async Task<IActionResult> GetAllFases(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
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
                worksheet.Cell(1, 1).Value  = "NRO DOCUMENTO";
                worksheet.Cell(1, 2).Value  = "FASE"; 
                worksheet.Cell(1, 3).Value  = "ACTIVO";    
                worksheet.Cell(1, 4).Value  = "AREA";
                worksheet.Cell(1, 5).Value  = "ACTUALIZACION";
                worksheet.Cell(1, 6).Value  = "ACTUALIZADO POR";

                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:F1").SetAutoFilter();

                for (int i = 0; i < _fase.Count; i++)
                {
                    var _fases = _fase[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value  = _fases.Id;
                    worksheet.Cell(row, 2).Value  = _fases.Name; 
                    worksheet.Cell(row, 3).Value  = _fases.IsActive != false ? "SI" : "NO"; 
                    worksheet.Cell(row, 4).Value  = _fases.AreaName;
                    worksheet.Cell(row, 5).Value  = _fases.Updated;
                    worksheet.Cell(row, 6).Value  = _fases.UpdatedBy;
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
        public async Task<IActionResult> GetFaseExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
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
        public async Task<IActionResult> GetOneFeatureType(int userId, int featureTypeId)
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
        public async Task<IActionResult> GetAllFeatureType(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
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

                    worksheet.Cell(row, 1).Value  = _featureTypes.Id;
                    worksheet.Cell(row, 2).Value  = _featureTypes.Name; 
                    worksheet.Cell(row, 3).Value  = _featureTypes.IsActive != false ? "SI" : "NO"; 
                    worksheet.Cell(row, 4).Value  = _featureTypes.FaseName;
                    worksheet.Cell(row, 5).Value  = _featureTypes.AreaName;
                    worksheet.Cell(row, 6).Value  = _featureTypes.Updated;
                    worksheet.Cell(row, 7).Value  = _featureTypes.UpdatedBy;
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
        public async Task<IActionResult> GetFeatureTypeExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
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
        public async Task<IActionResult> GetOneFeature(int userId, int featureId)
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
        public async Task<IActionResult> GetAllFeature(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
        {
            try
            {
                var _response = await _dInspection.GetAllFeature(userId, supplierId, dealerId, rowFrom, filter, active);
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
        public async Task<IActionResult> GetFeatureExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
        {
            try
            {
                List<Feature> _response = await _dInspection.GetFeatureExport(userId, supplierId, dealerId, filter, active);
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


        #endregion
    }
}
