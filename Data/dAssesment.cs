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
     
    public class dAssessment
    {
        // test data
        private readonly SemaphoreSlim _semaphore;
        public dAssessment()
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


        public async Task<Response> Post_Assessment(Assessment _assessment, Int32 userId)
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

        private async Task<Response> _Post_Assessment(Assessment _assessment, Int32 userId)
        {
            Response _response = new Response();

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


        public async Task<Response> Post_Actions(List<Models.Action> _list, Int32 userId)
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

        private async Task<Response> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
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
