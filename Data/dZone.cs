using System.Data;
using Models;
using Util;

namespace Data
{
    public class dZone
    {
        private readonly SemaphoreSlim _semaphore;
        public dZone()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Zone>>> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId,  supplierId,  rowfrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Zone>>> _GetAll(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, Int32? zoneId = null )
        {
            Response<List<Models.Zone>> _response = new Response<List<Models.Zone>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", zoneId);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("WarehouseId", "IDWAREHOUSE");
                _mapping.AddItem("WarehouseName", "VWAREHOUSE");
                _mapping.AddItem("Size", "VSIZE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("IsCrossDocking", "BCROSSDOCKING");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ZONES", _parameter);
                _response.Data = _data.GetList<Models.Zone>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<Models.Zone>> _getOne(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, Int32? zoneId = null)
        {
            Response<Models.Zone> _response = new Response<Models.Zone>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", zoneId);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("WarehouseId", "IDWAREHOUSE");
                _mapping.AddItem("WarehouseName", "VWAREHOUSE");
                _mapping.AddItem("Size", "VSIZE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("IsCrossDocking", "BCROSSDOCKING");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ZONES", _parameter);
                _response.Data = _data.GetItem<Models.Zone>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        //have to refactor get one into its own private method
        public async Task<Response<Models.Zone>> GetOne(Int32 zoneId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getOne(userId, null, null, null, zoneId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

    

        public async Task<List<Zone>> GetExport(Int32 userId, Int32? supplierId, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Zone>)(await _GetAll(userId, supplierId, null , filter)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        

        public async Task<Response<Result>> Post_Zones(List<Models.Zone> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Zones(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _Post_Zones(List<Zone> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ZONES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Result>> Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Actions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();   



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ZONES_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }



    }
}
