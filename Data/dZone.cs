using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.CustomUI;
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

        public async Task<Response> GetAll(Int32 userId, Int32? supplierId, Int32 rowfrom, string? filter)
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

        private async Task<Response> _GetAll(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, Int32? zoneId = null )
        {
            Response _response = (zoneId == null) ?  new Response(true) : new Response();

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

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ZONES", _parameter);
                _response.Data = (zoneId == null) ? _data.GetList<Models.Zone>(_mapping, _table) : _data.GetItem<Models.Zone>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        //have to refactor get one into its own private method
        public async Task<Response> GetOne(Int32 zoneId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, null, null, null, zoneId);
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

        

        public async Task<Response> Post_Zones(List<Models.Zone> _list, Int32 userId)
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

        private async Task<Response> _Post_Zones(List<Zone> _list, Int32 userId)
        {
            Response _response = new Response();
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

        public async Task<Response> Post_Actions(List<Models.Action> _list, Int32 userId)
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
        private async Task<Response> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

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
