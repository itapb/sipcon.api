using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Data
{
    public class dModule
    {

        private readonly SemaphoreSlim _semaphore;
        public dModule()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);

        }

        public async Task<List<Models.Module>> GetAll(string? moduleName, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(moduleName, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<Models.Module>> _GetAll(string? moduleNme , Int32 userId)
        {
            List<Models.Module> _response = new List<Models.Module>();

            try
            {


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VMODULE", moduleNme);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDMODULE");
                _mapping.AddItem("Name", "VMODULE");
                _mapping.AddItem("ActionName", "VACTION");
                _mapping.AddItem("ActionDisplay", "VDISPLAY");

                Util.Data _data = Util.Data.GetInstance();
                _response = await _data.ExecuteReaderAsync<Models.Module>("USP_GET_MODULES", _mapping, _parameter);

            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
               throw;
            }

            return _response;

        }

   

    }
}
