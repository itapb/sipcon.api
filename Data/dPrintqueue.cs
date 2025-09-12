using System.Data;
using Models;
using Util;


namespace Data
{
    public class dPrintqueue
    {
        private readonly SemaphoreSlim _semaphore;

        public dPrintqueue()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }


        private async Task<Response<Result>> _Post_PrintQueue(List<Models.Printqueue> _list, int userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PRINTQUEUE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response<Result>> Post_PrintQueue(List<Models.Printqueue> _list, int userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_PrintQueue(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }




    }
}
