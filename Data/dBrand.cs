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

        public async Task<List<Models.Brand>> GetAll()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
              return await _GetAll();
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<List<Models.Brand>> _GetAll()
        {

            List<Models.Brand> _list = new List<Models.Brand>();
            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.Brand>("USP_GET_BRANDS", _mapping);

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
