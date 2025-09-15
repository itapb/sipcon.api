using System.Data;
using Models;
using Util;

namespace Data
{
    public class dLocation
    {
        private readonly SemaphoreSlim _semaphore;
        public dLocation()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.Location>>> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId, rowfrom, filter );
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Location>>> _GetAll(Int32 userId, Int32? supplierId,Int32? rowfrom, string? filter , Int32? locationId = null )
        {
            Response<List<Models.Location>> _response =  new Response<List<Models.Location>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", locationId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("ZoneId", "IDZONE");
                _mapping.AddItem("ZoneName", "VZONE");
                _mapping.AddItem("WarehouseName", "VWAREHOUSE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Mapping", "IMAPPING");
                _mapping.AddItem("TypeId", "VTYPE");
                _mapping.AddItem("TypeName", "VTYPENAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LOCATIONS", _parameter);
                _response.Data = _data.GetList<Models.Location>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<Models.Location>> _getOne(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, Int32? locationId = null)
        {
            Response<Models.Location> _response =  new Response<Models.Location>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", locationId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("ZoneId", "IDZONE");
                _mapping.AddItem("ZoneName", "VZONE");
                _mapping.AddItem("WarehouseName", "VWAREHOUSE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Mapping", "IMAPPING");
                _mapping.AddItem("TypeId", "VTYPE");
                _mapping.AddItem("TypeName", "VTYPENAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LOCATIONS", _parameter);
                _response.Data = _data.GetItem<Models.Location>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        //have to refactor get one into its own private method
        public async Task<Response<Models.Location>> GetOne(Int32 locationId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getOne(userId, null, null, null, locationId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

   
        public async Task<List<Location>> GetExport(Int32 userId, Int32? supplierId, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Location>)(await _GetAll(userId, supplierId, null, filter )).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

       


        public async Task<Response<Result>> Post_Locations(List<Location> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Locations(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _Post_Locations(List<Location> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_LOCATIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        public async Task<Response<Result>> Post_Actions(List<Models.Action> _list,Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Actions(_list,userId);
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
                DataTable _table = await _data.GetDataTable("USP_POST_LOCATIONS_ACTIONS", _parameter);
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
