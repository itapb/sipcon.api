using System.Data;
using Models;
using Util;


namespace Data
{
    public class dPrinter
    {
        private readonly SemaphoreSlim _semaphore;

        public dPrinter()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);
        }


        public async Task<Response<List<Models.Printer>>> GetAll(int? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Printer>>> _GetAll(int? supplierId)
        {
            Response<List<Models.Printer>> _response = new Response<List<Models.Printer>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "PRINTER");
                _mapping.AddItem("IdSupplier", "IDSUPPLIER");
                _mapping.AddItem("BDefault", "BDEFAULT");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PRINTER", _parameter);
                _response.Data = _data.GetList<Models.Printer>(_mapping, _table);
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