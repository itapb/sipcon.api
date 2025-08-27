using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Models;
using Util;

namespace Data
{
    public class dComment
    {
        // test data
        private readonly SemaphoreSlim _semaphore;
        public dComment()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);

        }

        public async Task<Response> GetAll(string moduleName, Int32 recordId)
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

        private async Task<Response> _GetAll(string moduleName, Int32 recordId)
        {
            Response _response = new Response(true);
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDRECORD", recordId);
                _parameter.AddSqlParameter("@VMODULE", moduleName);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Content", "VCONTENT");
                _mapping.AddItem("RecordId", "IDRECORD");
                _mapping.AddItem("ModuleName", "VMODULE");
                _mapping.AddItem("ModuleId", "IDMODULE");
                _mapping.AddItem("UserName", "VFIRSTNAME");
                _mapping.AddItem("UserLastName", "VLASTNAME");
                _mapping.AddItem("UserType", "VUSERTYPE");
                _mapping.AddItem("DateComment", "DCREATED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_COMMENTS", _parameter);
                _response.Data = _data.GetList<Models.Comment>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Response> Post_Comment(Comment _comment, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Comment(_comment, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_Comment(Comment _comment, Int32 userId)
        {
            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_comment);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_COMMENT", _parameter);
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
