using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util;
using Models;
using System.Data;

namespace Data
{
    public class dTemplate
    {

      
        private readonly SemaphoreSlim _semaphore;

        public dTemplate()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
         
        }

        public async Task<Response<Result>> ImportInventoryTemplate(List<Models.InventoryTamplate> _list, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _importInventoryTemplate(_list, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Result>> ImportLocationTemplate(List<Models.LocationTemplate> _list, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _importLocationTemplate(_list, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Result>> ImportPartModelTemplate(List<Models.PartModelTemplate> _list, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _importPartModelTemplate(_list, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Result>> ImportContactTemplate(List<Models.ContactTemplate> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _importContactTemplate(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _importInventoryTemplate(List<Models.InventoryTamplate> _list, Int32 supplierId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_TEMPLATE_INVENTORY", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Result>> _importLocationTemplate(List<Models.LocationTemplate> _list, Int32 supplierId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_TEMPLATE_LOCATION", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Result>> _importPartModelTemplate(List<Models.PartModelTemplate> _list, Int32 supplierId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_TEMPLATE_PARTMODEL", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        private async Task<Response<Result>> _importContactTemplate(List<Models.ContactTemplate> _list)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
               // _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_TEMPLATE_CONTACT", _parameter);
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
