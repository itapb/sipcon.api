using Microsoft.AspNetCore.Identity;
using Models;
using System.Data;
using Util;

namespace Data
{
    public class dRate
    {
        private readonly SemaphoreSlim _semaphore;

        public dRate()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);
        }

        public async Task<Response<List<Rate>>> GetAll(int? rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(rowFrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Rate>>> _GetAll(int? rowFrom, string? filter)
        {
            Response<List<Rate>> _response = new Response<List<Rate>>();

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Total", "TOTAL");
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("DDate", "DDATE");
                _mapping.AddItem("NRate", "NRATE");
                _mapping.AddItem("NAltRate", "NALTERRATE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_RATES", _parameter);
                _response.Data = _data.GetList<Rate>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        /*---------------------------------------------------------------POST RATE---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostRate(Models.Rate _item, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostRate(_item, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Result>> _PostRate(Rate _item, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_item);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_RATES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
    


        public async Task<Response<Models.Result>> Insert_Rate(InsertRate rate)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Insert_Rate(rate);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Insert_Rate(InsertRate rate)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(rate);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_INSERT_RATES", _parameter);
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