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
        public async Task<Response<Result>> ExtractAndInsertSales(DateTime date)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ExtractAndInsertSales(date);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //traer las ventas de repuestos desde FIGO e insertarlas
        private async Task<Response<Result>> _ExtractAndInsertSales(DateTime date)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                // 1. Extraer datos desde FIGO
                var Params = new List<OracleParameter>
        {
            new OracleParameter("FECHA_EMISION", OracleDbType.Date) { Value = date.Date }
        };

                DataTable extractedData = await _oracleDB.GetDataTable(FigoQueries.ExtractSalesRepuestos, Params);

                if (extractedData.Rows.Count == 0)
                {
                    _response.Message = $"No se encontraron ventas de repuestos para la fecha {date:yyyy-MM-dd}";
                    _response.Processed = true;
                    _response.Status = 200;
                    return _response;
                }

                // 2. Convertir DataTable a List<SalesFigo>
                List<Models.SalesFigo> salesList = new List<Models.SalesFigo>();
                foreach (DataRow row in extractedData.Rows)
                {
                    salesList.Add(new Models.SalesFigo
                    {
                        invoiceNumber = row["INVOICENUMBER"].ToString(),
                        dInvoiceDate = Convert.ToDateTime(row["DINVOICEDATE"]),
                        vVAT = row["VVAT"].ToString(),
                        vInnerCode = row["VINNERCODE"].ToString(),
                        iQuantity = Convert.ToInt32(row["IQUANTITY"]),
                        idSupplier = Convert.ToInt32(row["IDSUPPLIER"])

                    });
                }

                // 3. Convertir lista a JSON
                string jsonSales = Util.Json.ConvertToJsonString(salesList);

                // 4. Ejecutar SP en SIPCON
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", jsonSales);
                _parameter.AddSqlParameter("@IDUSER", 0);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_SALES_FIGO", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response<Result>> ExtractAndInsertTransit()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ExtractAndInsertTransit();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _ExtractAndInsertTransit()
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                // 1. Extraer datos desde FIGO (sin parámetros)
                DataTable extractedData = await _oracleDB.GetDataTable(FigoQueries.ExtractTransitRepuestos, null);

                if (extractedData.Rows.Count == 0)
                {
                    _response.Message = "No se encontraron repuestos con tránsito pendiente";
                    _response.Processed = true;
                    _response.Status = 200;
                    return _response;
                }

                // 2. Convertir DataTable a List<FigoTransitRepuestos>
                List<Models.FigoTransitRepuestos> transitList = new List<Models.FigoTransitRepuestos>();
                foreach (DataRow row in extractedData.Rows)
                {
                    transitList.Add(new Models.FigoTransitRepuestos
                    {
                        CodigoRepuesto = row["CODIGO_REPUESTO"].ToString(),
                        CantidadTransito = Convert.ToInt32(row["CANTIDAD_TRANSITO"])
                    });
                }

                // 3. Convertir lista a JSON
                string jsonTransit = Util.Json.ConvertToJsonString(transitList);

                // 4. Ejecutar SP
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", jsonTransit);
                _parameter.AddSqlParameter("@IDUSER", 0);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_TRAFFIC_FIGO", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

           
                //  verificar si actualizó algo      
                if (_response.Data != null && _response.Data.UpdatedRows == 0 && _response.Data.InsertedRows == 0)
                {
                    _response.Message = "No se encontraron coincidencias en la tabla PART para actualizar";
                    _response.Processed = true;
                    _response.Status = 200;
                }
                else if (_response.Data != null && _response.Data.UpdatedRows > 0)
                {
                    _response.Message = $"Tránsito actualizado correctamente: {_response.Data.UpdatedRows} repuestos";
                    _response.Processed = true;
                    _response.Status = 200;
                }
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
    }

}