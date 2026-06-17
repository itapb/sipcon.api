using Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Util;

namespace Data
{
    public class dFigo
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly OracleDB _oracleDB;

        public dFigo(OracleDB oracle)
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);
            _oracleDB = oracle;
        }

        public async Task<Response<List<Models.FIGO_ReportCxC>>> GetReportCxC(DateTime Date, string Currency, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReportCxC(Date, Currency, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.FIGO_Report>>> GetReportsFigo(int userId, int rowFrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReportsFigo(userId, rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FIGO_ReportCxC>>> _GetReportCxC(DateTime Date, string Currency, string? filter)
        {
            Response<List<Models.FIGO_ReportCxC>> _response = new Response<List<Models.FIGO_ReportCxC>>();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID_REGISTRO");
                _mapping.AddItem("DealerName", "EMPRESA");
                _mapping.AddItem("Vat", "RIF");
                _mapping.AddItem("Client", "ORGANIZACION");
                _mapping.AddItem("Phone", "TELEFONO");
                _mapping.AddItem("Zone", "ZONA");
                _mapping.AddItem("Occurrence", "OCURRENCIA");
                _mapping.AddItem("Document", "DOCUMENTO");
                _mapping.AddItem("IssueDate", "EMISION");
                _mapping.AddItem("DueDate", "VENCIMIENTO");
                _mapping.AddItem("OverdueDays", "DIAS_VENCIDOS");
                _mapping.AddItem("Currency", "MONEDA");
                _mapping.AddItem("DocumentCurrency", "MONEDA_DOCUMENTO");
                _mapping.AddItem("OverdueAmount", "VENCIDO");
                _mapping.AddItem("CurrentAmount", "POR_VENCER");
                _mapping.AddItem("TotalDebt", "TOTAL_DEUDA");
                _mapping.AddItem("ExchangeRate", "TASA");
                _mapping.AddItem("Product", "PRODUCTO");
                _mapping.AddItem("SerialNumber", "SERIAL");
                _mapping.AddItem("ProductGroup", "GRUPO_PRODUCTO");

                string FormatDate = Date.ToString("MM/dd/yyyy"); // Es necesario este formato para poder filtra con las tablas de FIGO

                var Params = new List<OracleParameter>
                {
                    new OracleParameter("FECHA_CORTE", OracleDbType.Varchar2) { Value = FormatDate },
                    new OracleParameter("MONEDA_REPORTE", OracleDbType.Varchar2) { Value = Currency.ToUpper() },
                    new OracleParameter("BUSQUEDA", OracleDbType.Varchar2) { Value = filter }
                };

                string Query = FigoQueries.ReportCxC;

                DataTable _table = await _oracleDB.GetDataTable(Query, Params);

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = _data.GetList<Models.FIGO_ReportCxC>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<List<Models.FIGO_Report>>> _GetReportsFigo(int userId, int rowFrom)
        {
            Response<List<Models.FIGO_Report>> _response = new Response<List<Models.FIGO_Report>>();
            try
            {
                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("NameReport", "VNAME");
                _mapping.AddItem("AccessGroupId", "IDACCESSGROUP");
                _mapping.AddItem("IsPdfReport", "BPDFREPORT");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORT_FIGO", _parameter);

                _response.Data = _data.GetList<Models.FIGO_Report>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
    }
}