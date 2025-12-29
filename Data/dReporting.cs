using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models;
using Util;

namespace Data
{
    public class dReporting
    {

        private readonly SemaphoreSlim _semaphore;

        public dReporting() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<string> GetAllJson( string? filter, int? rowFrom, int userId,int? supplierId,int? dealerId,DateTime? fromDate,DateTime? upToDate,int? estatusId,int reportingId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await GetAllJsonAsync(filter, rowFrom, userId, supplierId, dealerId, fromDate, upToDate, estatusId, reportingId)
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<string> GetAllJsonAsync(string? filter, Int32? rowFrom, Int32 userId, Int32? supplierId, Int32? dealerId,DateTime? fromDate, DateTime? upToDate, int? estatusId,int ReportingId)
        {
            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@IDUSER", userId); 
                parameter.AddSqlParameter("@IDDEALER", dealerId ?? (object)DBNull.Value); 
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId ?? (object)DBNull.Value); 
                parameter.AddSqlParameter("@IROWFROM", rowFrom ?? (object)DBNull.Value); 
                parameter.AddSqlParameter("@VFILTER", filter ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@IDREPORTING", ReportingId);
                parameter.AddSqlParameter("@DFROMDATE", fromDate ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@DUPTODATE", upToDate ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@IDESTATUS", estatusId ?? (object)DBNull.Value);

                var data = Util.Data.GetInstance();
                // Aquí asumo que tienes un método que devuelve texto plano
                string jsonResult = await data.ExecuteScalarAsync<string>(
                    "USP_GET_REPORTS", parameter
                ).ConfigureAwait(false);

                return jsonResult;
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }
        }


    }
}
