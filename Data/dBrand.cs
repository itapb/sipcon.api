using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models;
using Util;

namespace Data
{
    public class dBrand
    {

        private readonly SemaphoreSlim _semaphore;

        public dBrand() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<List<Models.Brand>> GetAll(int? SupplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
              return await _GetAll(SupplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<List<Models.Brand>> _GetAll(int? SupplierId=null)
        {

            List<Models.Brand> _list = new List<Models.Brand>();
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", SupplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.Brand>("USP_GET_BRANDS", _mapping,_parameter);

            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _list;

        }


    }
}
