using Models;
using System.Data;
using Util;

namespace Data
{
    public class dAttachment
    {
        // test data
        private readonly SemaphoreSlim _semaphore;
        public dAttachment()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);

        }

        public async Task<Response<List<Models.Attachment>>> GetAll(string? moduleName, Int32? recordId, Int32? attachmentId = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(moduleName,recordId, attachmentId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Attachment>>> _GetAll(string? moduleName,Int32? recordId, Int32? attachmentId = null)
        {
            Response<List<Models.Attachment>> _response = new Response<List<Models.Attachment>>();
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@ID", attachmentId);
                _parameter.AddSqlParameter("@VMODULE", moduleName);
                _parameter.AddSqlParameter("@IDRECORD", recordId);
                
               

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("FileName", "VFILENAME");
                _mapping.AddItem("RecordId", "IDRECORD");
                _mapping.AddItem("ModuleId", "IDMODULE");
                _mapping.AddItem("ModuleName", "VMODULE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ATTACHMENT", _parameter);
                _response.Data = _data.GetList<Models.Attachment>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Response<List<Models.Attachment>>> GetOne(Int32 attachmentId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(null,null, attachmentId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response<Models.Result>> Post_Attachment(Models.Attachment _attachment, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Attachment(_attachment, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Attachment(Models.Attachment _attachment, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_attachment);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ATTACHMENT", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response<Models.Result>> Delete_Attachment(Int32 attachmentId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Delete_Attachment(attachmentId, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Delete_Attachment(Int32 attachmentId, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@ID", attachmentId);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_DELETE_ATTACHMENT", _parameter);
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
