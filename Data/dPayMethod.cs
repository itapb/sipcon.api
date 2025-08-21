using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Models;
using Util;

namespace Data
{
    public class dPayMethod
    {

        private readonly SemaphoreSlim _semaphore;
        public dPayMethod()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);

        }

        public async Task<Response> GetAll(bool? isDealer)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(isDealer);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetAll(bool? isDealer)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@BDEALER", isDealer);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PAYMETHODS", _parameter);
                _response.Data = _data.GetList<Models.PayMethod>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

    }
}
