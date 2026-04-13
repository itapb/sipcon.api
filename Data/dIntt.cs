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
    public class dIntt
    {
        private readonly SemaphoreSlim _semaphore;

        public dIntt()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }
        public async Task<List<Intt>> GetExportPendientesExcel(int userId,
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
                return (List<Intt>)(await _GetInttDataAsync(userId, supplierId, rowFrom, fromDate, upToDate, tipo, filter)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Intt>>> GetInttData(
            int userId,
            int? supplierId,
            int? rowFrom, 
            DateTime? fromDate,
            DateTime? upToDate ,
            string? tipo,
            string? filter = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetInttDataAsync(userId, supplierId, rowFrom, fromDate, upToDate, tipo, filter)
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Intt>>> _GetInttDataAsync(
            int userId,
            int? supplierId,
            int? rowFrom,
            DateTime? fromDate,
            DateTime? upToDate,
            string? tipo,
            string? filter = null)
        {
            var response = new Response<List<Intt>>();

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
                DataTable table = await data.GetDataTable("USP_GET_INTT", parameter);

                var mapping = new Mapping();

                // Detectar qué versión del SP se ejecutó
                if (table.Columns.Contains("TIPO_MOV"))
                {
                    // Column version
                    mapping.AddItem("Id", "ID");
                    mapping.AddItem("MovementType", "TIPO_MOV");
                    mapping.AddItem("CertificateNumber", "NRO_CERTIFICADO");
                    mapping.AddItem("DealerRif", "RIF_CONCESIONARIO");
                    mapping.AddItem("ClientRif", "RIF_CLIENTE");
                    mapping.AddItem("FirstName", "PRIMER_NOMBRE");
                    mapping.AddItem("SecondName", "SEGUNDO_NOMBRE");
                    mapping.AddItem("FirstLastName", "PRIMER_APELLIDO");
                    mapping.AddItem("SecondLastName", "SEGUNDO_APELLIDO");
                    mapping.AddItem("Address", "DIRECCION");
                    mapping.AddItem("Urbanization", "URBANIZACION");
                    mapping.AddItem("House", "CASA");
                    mapping.AddItem("Floor", "PISO");
                    mapping.AddItem("Apartment", "APTO");
                    mapping.AddItem("Municipality", "MUNICIPIO");
                    mapping.AddItem("Phone1Code", "TELF1_COD");
                    mapping.AddItem("Phone1Number", "TELF1_NRO");
                    mapping.AddItem("Phone2Code", "TELF1_COD2");
                    mapping.AddItem("Phone2Number", "TELF1_NRO2");
                    mapping.AddItem("Plate", "PLACA");
                    mapping.AddItem("Brand", "MARCA");
                    mapping.AddItem("State", "ESTADO");
                    mapping.AddItem("Vin", "VIN");
                    mapping.AddItem("InvoiceDate", "FECHA_FACTURA");
                    mapping.AddItem("InvoiceNumber", "NRO_FACTURA");
                    mapping.AddItem("NumberTxt", "VNUMBERTXT");
                }
                else if (table.Columns.Contains("DATOS"))
                {
                    // Versión TXT
                    mapping.AddItem("Datos", "DATOS");
                }

                response.Data = data.GetList<Intt>(mapping, table);
                response.SetGetResponse(table);
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }

            return response;
        }

        public async Task<Response<object>> PostInttActions(string data, int userId, int? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostInttActionsAsync(data, userId, supplierId).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<object>> _PostInttActionsAsync(string data, int userId, int? supplierId)
        {
            var response = new Response<object>();

            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@DATA", data);
                parameter.AddSqlParameter("@IDUSER", userId);
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                var dataUtil = Util.Data.GetInstance();
                DataTable table = await dataUtil.GetDataTable("USP_POST_INTT_ACTION", parameter);

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
        public async Task<Response<List<Intt>>> GetInttExport(int userId, int supplierId, int controlId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetInttExportAsync(userId, supplierId, controlId).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Intt>>> _GetInttExportAsync(int userId, int supplierId, int controlId)
        {
            var response = new Response<List<Intt>>();

            try
            {
                var parameter = new Parameter();
                parameter.AddSqlParameter("@IDUSER", userId);
                parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                parameter.AddSqlParameter("@IDCONTROL", controlId);

                var mapping = new Mapping();
                mapping.AddItem("Datos", "DATOS");

                var data = Util.Data.GetInstance();
                DataTable table = await data.GetDataTable("USP_GET_INTT_EXPORT", parameter);

                response.Data = data.GetList<Intt>(mapping, table);
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