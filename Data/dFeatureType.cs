using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using System.Data;
using Util;

namespace Data
{
    public class dFeatureType
    {
        // test data
        private readonly SemaphoreSlim _semaphore;

        public dFeatureType()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.FeatureType>>> GetAll()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.FeatureType>> GetOne(Int32 FeatureTypeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(FeatureTypeId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> Post_FeatureTypes(List<FeatureType> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_FeatureTypes(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FeatureType>>> _GetAll()
        {
            Response<List<Models.FeatureType>> _response = new Response<List<Models.FeatureType>>();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDFEATURETYPE");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IdFase", "IDFASE");
                _mapping.AddItem("NameFase", "FASE_NAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURE_TYPES");
                _response.Data = _data.GetList<Models.FeatureType>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.FeatureType>> _GetOne(Int32 FeatureTypeId)
        {
            Response<Models.FeatureType> _response = new Response<Models.FeatureType>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", FeatureTypeId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDFEATURETYPE");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IdFase", "IDFASE");
                _mapping.AddItem("NameFase", "FASE_NAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURE_TYPE_BY_ID", _parameter);
                _response.Data = _data.GetItem<Models.FeatureType>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Result> _Post_FeatureTypes(List<FeatureType> _list)
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
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_FEATURE_TYPE", _mapping, _parameter);
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