using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Util;

namespace Data
{
    public class dWarehouse
    {
        private readonly SemaphoreSlim _semaphore;
        public dWarehouse()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);

        }

        public async Task<Response<List<Warehouse>>> GetAll(Int32 userId, Int32? supplierId, Int32 rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId, rowFrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Warehouse>>> _GetAll(Int32 userId, Int32? supplierId,Int32? rowFrom, string? filter , Int32? warehouseId = null )
        {
            Response<List<Warehouse>> _response = new Response<List<Warehouse>>();

            try
            {

                Parameter _parameter = new Parameter();
               
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", warehouseId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Sell", "BSELL");

       
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_WAREHOUSES", _parameter);
                _response.Data = _data.GetList<Models.Warehouse>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Warehouse>> _getOne(Int32 userId, Int32? supplierId, Int32? rowFrom, string? filter, Int32? warehouseId = null)
        {
            Response<Warehouse> _response = new Response<Warehouse>();

            try
            {

                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", warehouseId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Sell", "BSELL");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_WAREHOUSES", _parameter);
                _response.Data =  _data.GetItem<Models.Warehouse>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        //have to refactor get one into its own private method
        public async Task<Response<Warehouse>> GetOne(Int32 warehouseId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getOne(userId, null, null, null, warehouseId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<List<Warehouse>> GetExport(Int32 userId, Int32? supplierId, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Warehouse>)(await _GetAll(userId, supplierId, null, filter)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        


        public async Task<Response<Result>> Post_Warehouses(List<Models.Warehouse> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Warehouses(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _Post_Warehouses(List<Warehouse> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_WAREHOUSES", _parameter);
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
                DataTable _table = await _data.GetDataTable("USP_POST_WAREHOUSES_ACTIONS", _parameter);
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
