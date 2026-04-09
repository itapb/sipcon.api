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
            _semaphore = new SemaphoreSlim(300, 500);
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




        public async Task<Response<List<Models.Printqueue>>> GetAll(string printerName, bool pair)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(printerName, pair);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Printqueue>>> _GetAll(string printerName, bool pair)
        {
            Response<List<Models.Printqueue>> _response = new Response<List<Models.Printqueue>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VPRINTER", printerName);
                _parameter.AddSqlParameter("@BPAIR", pair);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Type", "VTYPE");
                //_mapping.AddItem("RecordId", "IDRECORD");
                _mapping.AddItem("PrintedDate", "DPRINTED");
                //_mapping.AddItem("ZPL", "VZPL");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Supplier", "VSUPPLIER");
                _mapping.AddItem("CodePair", "VCODEPAIR");
                _mapping.AddItem("DescriptionPair", "VDESCRIPTIONPAIR");
                _mapping.AddItem("IdPair", "IDPAIR");
                _mapping.AddItem("Note", "VNOTE");
                _mapping.AddItem("NotePair", "VNOTEPAIR");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PRINTQUEUE",_parameter);
                _response.Data = _data.GetList<Models.Printqueue>(_mapping, _table);
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
