using System.Data;
using Models;
using Util;

namespace Data
{
     
    public class dAssessment
    {
        // test data
        private readonly SemaphoreSlim _semaphore;
        public dAssessment()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);

        }

        public async Task<Response<List<Models.Assessment>>> GetAll(string moduleName, Int32 recordId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(moduleName, recordId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Assessment>>> _GetAll(string moduleName, Int32 recordId)
        {
            Response<List<Models.Assessment>> _response = new Response<List<Models.Assessment>>();
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDRECORD", recordId);
                _parameter.AddSqlParameter("@VMODULE", moduleName);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("RecordId", "IDRECORD");
                _mapping.AddItem("ModuleName", "VMODULE");
                _mapping.AddItem("ModuleId", "IDMODULE");
                _mapping.AddItem("UserDealerName", "VUSERDEALERNAME");
                _mapping.AddItem("UserDealerLastName", "VUSERDEALERLASTNAME");
                _mapping.AddItem("UserSupplierName", "VUSERSUPPLIERNAME");
                _mapping.AddItem("UserSupplierLastName", "VUSERSUPPLIERLASTNAME");
                _mapping.AddItem("StartDate", "DSTARTDATE");
                _mapping.AddItem("EndDate", "DENDDATE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ASSESSMENT", _parameter);
                _response.Data = _data.GetList<Models.Assessment>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Response<Models.Result>> Post_Assessment(Assessment _assessment, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Assessment(_assessment, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Assessment(Assessment _assessment, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_assessment);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ASSESSMENT", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<Models.Result>> Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Actions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ASSESSMENT_ACTIONS", _parameter);
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
