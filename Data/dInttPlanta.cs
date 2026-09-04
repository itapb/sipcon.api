using Microsoft.Data.SqlClient;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Util;

namespace Data
{
    public class dInttPlanta
    {
        private readonly SemaphoreSlim _semaphore;

        public dInttPlanta()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        
        public async Task<Response<List<InttPlanta>>> GetPlantaData(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? tipo,
            string? filter = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPlantaDataAsync(userId, supplierId, rowFrom, fromDate, upToDate, tipo, filter)
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<InttPlanta>>> _GetPlantaDataAsync(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? tipo,
            string? filter = null)
        {
            var response = new Response<List<InttPlanta>>();

            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@IDUSER", userId);
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                parameter.AddSqlParameter("@IROWFROM", rowFrom);
                parameter.AddSqlParameter("@DFROMDATE", fromDate);
                parameter.AddSqlParameter("@DUPTODATE", upToDate);
                parameter.AddSqlParameter("@VTIPO", tipo);
                parameter.AddSqlParameter("@VFILTER", filter);

                var data = Util.Data.GetInstance();
                DataTable table = await data.GetDataTable("USP_GET_INTT_PLANTA_LIST", parameter);

                var mapping = new Mapping();

                // Detectar qué versión del SP se ejecutó (modo TXT o modo columnas)
                if (table.Columns.Contains("VTIPO_MOV"))
                {
                    mapping.AddItem("Id", "ID");
                    mapping.AddItem("VTIPO_MOV", "VTIPO_MOV");
                    mapping.AddItem("VRECORD_NUMBER", "VRECORD_NUMBER");
                    mapping.AddItem("VUPDATE_NUMBER", "VUPDATE_NUMBER");
                    mapping.AddItem("VBRAND", "VBRAND");
                    mapping.AddItem("VSERIE", "VSERIE");
                    mapping.AddItem("VMODEL", "VMODEL");
                    mapping.AddItem("VMODELYEAR", "VMODELYEAR");
                    mapping.AddItem("VVIN", "VVIN");
                    mapping.AddItem("VSERIAL", "VSERIAL");
                    mapping.AddItem("VPLATE", "VPLATE");
                    mapping.AddItem("VCOLOR1", "VCOLOR1");
                    mapping.AddItem("VCOLOR2", "VCOLOR2");
                    mapping.AddItem("VWEIGHT", "VWEIGHT");
                    mapping.AddItem("VCAPACITYTYPE", "VCAPACITYTYPE");
                    mapping.AddItem("VCAPACITY", "VCAPACITY");
                    mapping.AddItem("VAXLENUMBER", "VAXLENUMBER");
                    mapping.AddItem("VWHEELDIAMETER", "VWHEELDIAMETER");
                    mapping.AddItem("VCLASS", "VCLASS");
                    mapping.AddItem("VTYPE", "VTYPE");
                    mapping.AddItem("VUSE", "VUSE");
                    mapping.AddItem("VCERTIFICATEDATE", "VCERTIFICATEDATE");
                    mapping.AddItem("VRIF", "VRIF");
                    mapping.AddItem("VPORT", "VPORT");
                    mapping.AddItem("VFILENUMBER", "VFILENUMBER");
                    mapping.AddItem("DFILEDATE", "DFILEDATE");
                    mapping.AddItem("VINVOICENUMBER", "VINVOICENUMBER");
                    mapping.AddItem("DINVOICEDATE", "DINVOICEDATE");
                    mapping.AddItem("VCERTIFICATENUMBER", "VCERTIFICATENUMBER");
                    mapping.AddItem("VMANUFACTUREYEAR", "VMANUFACTUREYEAR");
                    mapping.AddItem("VSERIALVIN", "VSERIALVIN");
                    mapping.AddItem("VSERIALCHASIS", "VSERIALCHASIS");
                    mapping.AddItem("VSELLINVOICENUMBER", "VSELLINVOICENUMBER");
                    mapping.AddItem("VSELLINVOICEDATE", "VSELLINVOICEDATE");
                    mapping.AddItem("VHOMONUMBER", "VHOMONUMBER");
                    mapping.AddItem("VHOMODATE", "VHOMODATE");
                    mapping.AddItem("VSERVICE", "VSERVICE");
                    mapping.AddItem("VSEATSNUMBER", "VSEATSNUMBER");
                    mapping.AddItem("VRAFANUMBER", "VRAFANUMBER");
                    mapping.AddItem("VRAFADATE", "VRAFADATE");
                    mapping.AddItem("VRAFASEC", "VRAFASEC");
                    mapping.AddItem("VSERIALCARRO", "VSERIALCARRO");
                    mapping.AddItem("VFUELTYPE", "VFUELTYPE");
                    mapping.AddItem("VNUMBERPLANTATXT", "VNUMBERPLANTATXT");
                }
                else if (table.Columns.Contains("DATOS"))
                {
                    // Versión TXT
                    mapping.AddItem("Datos", "DATOS");
                }

                response.Data = data.GetList<InttPlanta>(mapping, table);
                response.SetGetResponse(table);
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }

            return response;
        }

        // Método para exportar a Excel
        public async Task<List<InttPlanta>> GetExportPendientesExcel(
        int userId,
        int? supplierId,
        int? rowFrom,
        DateTime? fromDate,
        DateTime? upToDate,
        string? tipo,
        string? filter = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
        
                var response = await _GetPlantaDataAsync(userId, supplierId, -1, fromDate, upToDate, tipo, filter);
                return response.Data ?? new List<InttPlanta>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // Método para acciones
        public async Task<Response<object>> PostPlantaActions(string data, int userId, int? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostPlantaActionsAsync(data, userId, supplierId).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<object>> _PostPlantaActionsAsync(string data, int userId, int? supplierId)
        {
            var response = new Response<object>();

            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@DATA", data);
                parameter.AddSqlParameter("@IDUSER", userId);
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                var dataUtil = Util.Data.GetInstance();
                DataTable table = await dataUtil.GetDataTable("USP_POST_INTT_PLANTA_ACTION", parameter);

                response.Data = new
                {
                    IID = table.Rows.Count > 0 ? Convert.ToInt32(table.Rows[0]["IID"]) : 0,
                    IUPDATED = table.Rows.Count > 0 ? Convert.ToInt32(table.Rows[0]["IUPDATED"]) : 0
                };
                response.SetGetResponse(table);
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }

            return response;
        }

        // Método para exportar TXT
        public async Task<Response<List<InttPlanta>>> GetPlantaExport(int userId, int supplierId, int controlId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPlantaExportAsync(userId, supplierId, controlId).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<InttPlanta>>> _GetPlantaExportAsync(int userId, int supplierId, int controlId)
        {
            var response = new Response<List<InttPlanta>>();

            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@IDUSER", userId);
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                parameter.AddSqlParameter("@IDCONTROL", controlId);

                var mapping = new Mapping();
                mapping.AddItem("Datos", "DATOS");

                var data = Util.Data.GetInstance(   );
                DataTable table = await data.GetDataTable("USP_GET_INTT_PLANTA_EXPORT", parameter);

                response.Data = data.GetList<InttPlanta>(mapping, table);
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