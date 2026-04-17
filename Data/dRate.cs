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

        public async Task<Response<Rate>> Update(int id, decimal nRate)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Update(id, nRate);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Rate>> _Update(int id, decimal nRate)
        {
            Response<Rate> _response = new Response<Rate>();

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@ID", id);
                _parameter.AddSqlParameter("@NRATE", nRate);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("DDate", "DDATE");
                _mapping.AddItem("NRate", "NRATE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_UPDATE_RATES", _parameter);
                _response.Data = _data.GetItem<Rate>(_mapping, _table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
    }
}