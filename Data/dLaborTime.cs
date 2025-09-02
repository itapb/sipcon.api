using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Models;
using Util;


namespace Data
{
    public class dLaborTime
    {

        private readonly SemaphoreSlim _semaphore;

        public dLaborTime() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.LaborTime>>> GetAll(Int32 supplierId,Int32? modelId, String? filter, Int32 irowfrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(supplierId, modelId, filter, irowfrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<List<LaborTime>> GetExport(Int32 supplierId, Int32 modelId, String? filter, Int32? irowfrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                var response = await _GetAll(supplierId, modelId,filter,irowfrom);
                if (response.Data is List<LaborTime> laborTimesList)
                {
                    return laborTimesList;
                }
                return new List<LaborTime>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.LaborTime>>> _GetAll(Int32 supplierId, Int32? modelId, String? filter, Int32? irowfrom)
        {
            Response<List<Models.LaborTime>> _response = new Response<List<Models.LaborTime>>();
            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDMODEL", modelId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", irowfrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODELNAME");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Hours", "NHOURS");
                _mapping.AddItem("Price", "NPRICE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LABORTIME", _parameter);
                _response.Data = _data.GetList<Models.LaborTime>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        public async Task<Response<Models.Result>> Post_LaborTime(List<Models.LaborTime> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_LaborTime(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_LaborTime(List<LaborTime> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_LABORTIME", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response<Models.Result>> Delete_LaborTime(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Delete_LaborTime(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Delete_LaborTime(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

    
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_DELETE_LABORTIME", _parameter);
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
