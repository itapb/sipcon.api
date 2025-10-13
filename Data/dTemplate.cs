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

        public async Task<Response<Result>> ImportInventoryTamplate(List<Models.InventoryTamplate> _list, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _importInventoryTamplate(_list, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _importInventoryTamplate(List<Models.InventoryTamplate> _list, Int32 supplierId)
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
    }
}
