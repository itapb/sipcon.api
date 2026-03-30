using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using System.Data;
using Util;

namespace Data
{
    public class dDetailInspection
    {
        private readonly SemaphoreSlim _semaphore;

        public dDetailInspection()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.DetailInspection>>> GetAll(Int32? InspectionId = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(InspectionId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.DetailInspection>> GetOne(Int32 DetailInspectionId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(DetailInspectionId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> Post_DetailInspections(List<DetailInspection> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_DetailInspections(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.DetailInspection>>> _GetAll(Int32? InspectionId)
        {
            Response<List<Models.DetailInspection>> _response = new Response<List<Models.DetailInspection>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                if (InspectionId != null) _parameter.AddSqlParameter("@IDINSPECTION", InspectionId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDDETAILINSPECTION");
                _mapping.AddItem("Value", "BVALUE");
                _mapping.AddItem("Observation", "VOBSERVATION");
                _mapping.AddItem("FileUrl", "VFILEURL");
                _mapping.AddItem("IdInspection", "IDINSPECTION");
                _mapping.AddItem("IdFeature", "IDFEATURE");
                _mapping.AddItem("NameFeature", "FEATURE_NAME");
                _mapping.AddItem("NameFeatureType", "FEATURE_TYPE_NAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DETAIL_INSPECTIONS", _parameter);
                _response.Data = _data.GetList<Models.DetailInspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.DetailInspection>> _GetOne(Int32 DetailInspectionId)
        {
            Response<Models.DetailInspection> _response = new Response<Models.DetailInspection>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", DetailInspectionId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDDETAILINSPECTION");
                _mapping.AddItem("Value", "BVALUE");
                _mapping.AddItem("Observation", "VOBSERVATION");
                _mapping.AddItem("FileUrl", "VFILEURL");
                _mapping.AddItem("IdInspection", "IDINSPECTION");
                _mapping.AddItem("IdFeature", "IDFEATURE");
                _mapping.AddItem("NameFeature", "FEATURE_NAME");
                _mapping.AddItem("NameFeatureType", "FEATURE_TYPE_NAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DETAIL_INSPECTION_BY_ID", _parameter);
                _response.Data = _data.GetItem<Models.DetailInspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Result> _Post_DetailInspections(List<DetailInspection> _list)
        {
            List<Result> _results = new List<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_DETAIL_INSPECTION", _mapping, _parameter);
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _results[0];
        }
    }
}