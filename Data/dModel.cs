using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Models;
using Newtonsoft.Json;
using Util;

namespace Data
{
    public  class dModel
    {

        private readonly SemaphoreSlim _semaphore;
        public dModel()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);

        }

        public async Task<Response<List<Models.Model>>> GetAll(Int32 userId, Int32? supplierId, Int32? rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId,rowFrom, filter, 0);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Model>>> _GetAll(Int32 userId, Int32? supplierId, Int32? rowFrom, string? filter,  Int32 modelId=0)
        {
            Response<List<Models.Model>> _response = new Response<List<Models.Model>>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", null);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Total", "TOTAL");
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("PolicyTypeName", "VPOLICYTYPE");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MODELS", _parameter);
                _response.Data = _data.GetList<Models.Model>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.Model>> _GetOne(Int32 userId, Int32 modelId )
        {
            Response<Models.Model> _response = new Response<Models.Model>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@IROWFROM", 0);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", modelId);
                _parameter.AddSqlParameter("@IDSUPPLIER", 0);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Total", "TOTAL");
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("PolicyTypeName", "VPOLICYTYPE");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MODELS", _parameter);
                _response.Data = _data.GetItem<Models.Model>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
   
        public async Task<Response<Models.Model>> GetOne(Int32 userId,Int32 modelId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(userId,modelId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<List<Model>> GetExport(Int32 userId, Int32? supplierId,string? _filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Model>)(await _GetAll(userId, supplierId,null, _filter)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }


       


        public async Task<Response<Models.Result>> Post_Models(List<Model> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Models(_list,userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Models (List<Model> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);
                
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

      

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_MODELS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Models.Result>> Post_Actions(List<Models.Action> _list, Int32 userId)
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

        private async Task<Response<Models.Result>> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);  

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_MODELS_ACTIONS", _parameter);
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
