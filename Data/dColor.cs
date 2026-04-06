using System.Data;
using Models;
using Util;

namespace Data
{
    public class dColor
    {
        // test data
        private readonly SemaphoreSlim _semaphore;
        public dColor() {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);

        }

        public async Task<Response<List<Models.Color>>> GetAll()
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

        private async Task<Response<List<Models.Color>>> _GetAll()
        {
            Response<List<Models.Color>> _response = new Response<List<Models.Color>>();
            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_COLORS");
                _response.Data = _data.GetList<Models.Color>(_mapping, _table);
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
