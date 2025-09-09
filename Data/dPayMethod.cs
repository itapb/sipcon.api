using System.Data;
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

        public async Task<Response<List<Models.PayMethod>>> GetAll(bool? isDealer)
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

        private async Task<Response<List<Models.PayMethod>>> _GetAll(bool? isDealer)
        {
            Response<List<Models.PayMethod>> _response = new Response<List<Models.PayMethod>>();
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
