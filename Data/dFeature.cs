using Models;
using System.Data;
using Util;

namespace Data
{
    public class dFeature
    {
        // test data
        private readonly SemaphoreSlim _semaphore;

        public dFeature()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.Feature>>> GetAll(Int32? DealerId, Int32? SupplierId, Int32? AreaId, Int32? FeatureTypeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(DealerId, SupplierId, AreaId, FeatureTypeId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Feature>> GetOne(Int32 FeatureId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(FeatureId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> Post_Features(List<Feature> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Features(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Feature>>> _GetAll(Int32? DealerId, Int32? SupplierId, Int32? AreaId, Int32? FeatureTypeId)
        {
            Response<List<Models.Feature>> _response = new Response<List<Models.Feature>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDDEALER", DealerId);
                _parameter.AddSqlParameter("@IDSUPPLIER", SupplierId);
                _parameter.AddSqlParameter("@IDAREA", AreaId);
                _parameter.AddSqlParameter("@IDFEATURETYPE", FeatureTypeId);

                Mapping _mapping = new Mapping(); 
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("FeatureTypeId", "IDFEATURETYPE");
                _mapping.AddItem("FeatureType", "FEATURETYPE");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("Model", "MODEL");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("Fase", "FASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("Area", "AREA");
                _mapping.AddItem("DealerName", "DEALER");
                _mapping.AddItem("SupplierName", "SUPPLIER");
                _mapping.AddItem("Brand", "VBRAND");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURES", _parameter);
                _response.Data = _data.GetList<Models.Feature>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.Feature>> _GetOne(Int32 FeatureId)
        {
            Response<Models.Feature> _response = new Response<Models.Feature>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", FeatureId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("FeatureTypeId", "IDFEATURETYPE");
                _mapping.AddItem("FeatureType", "FEATURETYPE");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("Model", "MODEL");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("Fase", "FASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("Area", "AREA");
                _mapping.AddItem("DealerName", "DEALER");
                _mapping.AddItem("SupplierName", "SUPPLIER");
                _mapping.AddItem("Brand", "VBRAND");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURE_BY_ID", _parameter);
                _response.Data = _data.GetItem<Models.Feature>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Result> _Post_Features(List<Feature> _list)
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
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_FEATURE", _mapping, _parameter);
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