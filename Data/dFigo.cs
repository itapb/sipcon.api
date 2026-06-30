using DocumentFormat.OpenXml.Vml;
using Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.Json;
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

        //public async Task<Response<List<Models.FIGO_ReportCxC>>> GetReportCxC(DateTime Date, string Currency, string? filter)
        //{
        //    await _semaphore.WaitAsync(Util.Setting.TimeOut);
        //    try
        //    {
        //        return await _GetReportCxC(Date, Currency, filter);
        //    }
        //    finally
        //    {
        //        _semaphore.Release();
        //    }
        //}

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

        public async Task<Response<List<Models.FIGO_Filters>>> ReportsFilters(int userId, int reportId, int rowFrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ReportsFilters(userId, reportId, rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.FIGO_Options>>> ReportsOptions(int userId, int reportId, int rowFrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ReportsOptions(userId, reportId, rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Dictionary<string, object>>>> GetAllJson(int userId, int supplierId, int? rowfrom, int reportId, string jsonParameters)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllJsonAsync(userId, supplierId, rowfrom, reportId, jsonParameters)
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.FIGO_Query>> _GetReportQuery(int userId, int? reportId, string? reportName = null, int? rowfrom = 0)
        {
            Response<Models.FIGO_Query> _response = new Response<Models.FIGO_Query>();
            try
            {
                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@VREPORT", reportName ?? (object)DBNull.Value);
                _parameter.AddSqlParameter("@IDREPORT", reportId ?? (object)DBNull.Value);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom ?? 0);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Query", "VCONTENT");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORT_FIGOQUERY", _parameter);
                _response.Data = _data.GetItem<Models.FIGO_Query>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<List<Dictionary<string, object>>>> _GetAllJsonAsync(int userId, int supplierId, int? rowfrom, int reportId, string jsonParameters)
        {
            var response = new Response<List<Dictionary<string, object>>>();

            try
            {
                // 1. Obtener el query crudo desde la BD usando tu método existente
                var queryResponse = await _GetReportQuery(userId, reportId);

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    response.Message = "No se pudo obtener la configuración del reporte.";
                    return response;
                }

                string rawQuery = queryResponse.Data.Query; // Aquí extraemos el SQL crudo de tu objeto

                // 2. Deserializar los parámetros recibidos desde el cliente
                var parametersDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonParameters)
                                     ?? new Dictionary<string, object>();

                // 3. Crear los parámetros de Oracle dinámicamente
                var oracleParams = new List<OracleParameter>();
                foreach (var kvp in parametersDict)
                {
                    // Obtener el valor crudo del JsonElement
                    object valor = kvp.Value;

                    // Si el valor es de tipo JsonElement (común al deserializar a object), extrae su valor real
                    if (valor is JsonElement element)
                    {
                        valor = element.ValueKind switch
                        {
                            JsonValueKind.String => element.GetString(),
                            JsonValueKind.Number => element.GetDecimal(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Null => DBNull.Value,
                            _ => element.GetRawText()
                        };
                    }

                    // Asegurarse de que si es null sea DBNull
                    object finalValue = valor ?? DBNull.Value;

                    // Crear el parámetro
                    oracleParams.Add(new OracleParameter(kvp.Key, finalValue));
                }

                // 4. Agregar parámetros internos del sistema
                oracleParams.Add(new OracleParameter("IDSUPPLIER", supplierId));
                oracleParams.Add(new OracleParameter("IROWFROM", rowfrom));


                // 4. Ejecutar el Query crudo
                Util.Data dataInstance = Util.Data.GetInstance();
                DataTable table = await _oracleDB.GetDataTable(rawQuery, oracleParams);
                int total = 0;
                // 5. Convertir el resultado a List<Dictionary<string, object>> (Tu formato dinámico)
                var rows = new List<Dictionary<string, object>>();
                foreach (DataRow row in table.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    
                    foreach (DataColumn col in table.Columns)
                    {
                        // Excluimos columnas de control si es necesario
                        if (col.ColumnName.Equals("TOTAL", StringComparison.OrdinalIgnoreCase))
                        {
                            if (row[col] != DBNull.Value)
                                total = Convert.ToInt32(row[col]);
                            continue; // no se agrega al diccionario
                        }

                        dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }
                    rows.Add(dict);
                }

                response.Data = rows;
                response.Total = total;
                response.Processed = true;
            }
            catch (Exception ex)
            {
                response.SetError(ex);
            }

            return response;
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

        private async Task<Response<List<Models.FIGO_Filters>>> _ReportsFilters(int userId, int reportId, int rowFrom)
        {
            Response<List<Models.FIGO_Filters>> _response = new Response<List<Models.FIGO_Filters>>();
            try
            {
                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDREPORT", reportId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Field", "VFIELD");
                _mapping.AddItem("FieldType", "VFIELDTYPE");
                _mapping.AddItem("ActionType", "VACTIONTYPE");
                _mapping.AddItem("ReportFigoId", "IDREPORTFIGO");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORT_FILTERS", _parameter);

                _response.Data = _data.GetList<Models.FIGO_Filters>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<List<Models.FIGO_Options>>> _ReportsOptions(int userId, int reportId, int rowFrom)
        {
            Response<List<Models.FIGO_Options>> _response = new Response<List<Models.FIGO_Options>>();
            try
            {
                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDREPORT", reportId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("ReportFigoId", "REPORTFIGOID");
                _mapping.AddItem("FilterReportId", "FILTERREPORTID");
                _mapping.AddItem("FilterOptionId", "FILTEROPTIONSID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Value", "VVALUE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORT_OPTIONS", _parameter);

                _response.Data = _data.GetList<Models.FIGO_Options>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        // Método público (sin parámetros)
        public async Task<Response<Result>> ExtractAndInsertSales()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ExtractAndInsertSales();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _ExtractAndInsertSales()
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                int userId = 1;
                string reportName = "ExtractSalesRepuestos";

                // Obtener el query desde BD
                var queryResponse = await _GetReportQuery(userId, null, reportName, 0);
                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query de ventas desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                // 1. Extraer datos desde FIGO - SIN PARÁMETROS
                DataTable extractedData = await _oracleDB.GetDataTable(queryResponse.Data.Query, null);

                if (extractedData.Rows.Count == 0)
                {
                    _response.Message = "No se encontraron ventas de repuestos para la fecha del día anterior";
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
                int userId = 1;                
                string reportName = "ExtractTransitRepuestos";

                // 1. Obtener el query desde BD
                var queryResponse = await _GetReportQuery(userId, null,reportName);
                
                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query de tránsito desde la BD (VNAME: {reportName})"));
                    return _response;
                }


                // 2. Extraer datos desde FIGO con el query obtenido
                DataTable extractedData = await _oracleDB.GetDataTable(queryResponse.Data.Query, null);

                if (extractedData.Rows.Count == 0)
                {
                    _response.Message = "No se encontraron repuestos con tránsito pendiente";
                    _response.Processed = true;
                    _response.Status = 200;
                    return _response;
                }

                // 3. Convertir DataTable a List<FigoTransitRepuestos>
                List<Models.FigoTransitRepuestos> transitList = new List<Models.FigoTransitRepuestos>();
                foreach (DataRow row in extractedData.Rows)
                {
                    transitList.Add(new Models.FigoTransitRepuestos
                    {
                        CodigoRepuesto = row["CODIGO_REPUESTO"].ToString(),
                        CantidadTransito = Convert.ToInt32(row["CANTIDAD_TRANSITO"])
                    });
                }

                // 4. Convertir lista a JSON
                string jsonTransit = Util.Json.ConvertToJsonString(transitList);

                // 5. Ejecutar SP en SIPCON
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", jsonTransit);
                _parameter.AddSqlParameter("@IDUSER", 0);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_TRAFFIC_FIGO", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

                // 6. Verificar si actualizó algo      
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