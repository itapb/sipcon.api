using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public async Task<Response<List<Models.ReportType>>> GetReportingType(Int32 userId, Int32? rowFrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReportingType(userId,rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.ReportType>>> _GetReportingType(Int32 userId, Int32? rowFrom)
        {
            Response<List<Models.ReportType>> _response = new Response<List<Models.ReportType>>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Total", "TOTAL");
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORTING_TYPE", _parameter);
                _response.Data = _data.GetList<Models.ReportType>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response<List<Dictionary<string, object>>>> GetAllJson( string? filter, int? rowFrom, int userId,int? supplierId,int? dealerId,DateTime? fromDate,DateTime? upToDate,int reportingId, int? estatusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await GetAllJsonAsync(filter, rowFrom, userId, supplierId, dealerId, fromDate, upToDate, reportingId,estatusId)
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Dictionary<string, object>>>> GetAllJsonAsync( string? filter, int? rowFrom, int userId, int? supplierId, int? dealerId, DateTime? fromDate, DateTime? upToDate,int reportingId, int? estatusId)
        {
            var response = new Response<List<Dictionary<string, object>>>();
            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@IDUSER", userId);
                parameter.AddSqlParameter("@IDDEALER", dealerId ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@IROWFROM", rowFrom ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@VFILTER", filter ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@IDREPORTING", reportingId);
                parameter.AddSqlParameter("@DFROMDATE", fromDate ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@DUPTODATE", upToDate ?? (object)DBNull.Value);
                parameter.AddSqlParameter("@IESTATUS", estatusId ?? (object)DBNull.Value);



                var data = Util.Data.GetInstance();
                DataTable table = await data.GetDataTable("USP_GET_REPORTS", parameter);

                var rows = new List<Dictionary<string, object>>();
                foreach (DataRow row in table.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    int total = 0;
                    foreach (DataColumn col in table.Columns)
                    {
                        // 🔹 Excluir la columna TOTAL del diccionario
                        if (col.ColumnName.Equals("TOTAL", StringComparison.OrdinalIgnoreCase))
                        {
                            if (row[col] != DBNull.Value)
                                total = Convert.ToInt32(row[col]);
                            continue; // no se agrega al diccionario
                        }

                        if (col.ColumnName.Equals("IDSUPPLIER", StringComparison.OrdinalIgnoreCase) || col.ColumnName.Equals("IDDEALER", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }


                            dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }
                    rows.Add(dict);
                }


                response.Data = rows;
                response.SetGetResponse(table);
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }

            return response;
        }




    }
}
