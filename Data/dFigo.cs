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

        public async Task<Response<List<Models.FIGO_Options>>> ReportsOptions(int userId, int reportId, int? rowFrom)
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

        public async Task<Response<List<Dictionary<string, object>>>> GetAllJson(int userId, int supplierId, int? rowfrom, int reportId, string jsonParameters, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllJsonAsync(userId, supplierId, rowfrom, reportId, jsonParameters, filter)
                    .ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.FIGO_MastersID>>> GetMasterSaleIds()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetMasterSaleIds();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FIGO_MastersID>>> _GetMasterSaleIds()
        {
            Response<List<Models.FIGO_MastersID>> _response = new Response<List<Models.FIGO_MastersID>>();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MASTERSALEID");

                _response.Data = _data.GetList<Models.FIGO_MastersID>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
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
                _mapping.AddItem("Type", "VTYPE");

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

        private async Task<Response<List<Dictionary<string, object>>>> _GetAllJsonAsync(int userId, int supplierId, int? rowfrom, int reportId, string jsonParameters, string? filter)
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

                string rawQuery = queryResponse.Data.Query!; // Aquí extraemos el SQL crudo de tu objeto
                string rawType = queryResponse.Data.Type!;

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
                oracleParams.Add(new OracleParameter("BUSQUEDA", filter));


                // 4. Ejecutar el Query crudo
                Util.Data dataInstance = Util.Data.GetInstance();
                DataTable table = await _oracleDB.GetDataTable(rawQuery, supplierId, rawType, oracleParams);
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

        private async Task<Response<List<Models.FIGO_Options>>> _ReportsOptions(int userId, int reportId, int? rowFrom = null)
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

                string rawQuery = queryResponse.Data.Query; // Aquí extraemos el SQL crudo de tu objeto
                string rawType = queryResponse.Data.Type;

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query de ventas desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                // 1. Extraer datos desde FIGO - SIN PARÁMETROS
                DataTable extractedData = await _oracleDB.GetDataTable(rawQuery, 0, rawType, null);


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
                Util.Parameter _parameter = new Util.Parameter();
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

        public async Task<Response<List<Models.FIGO_PartsStock>>> GetPartsStock(int supplierId, string type)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPartsStock(supplierId, type);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FIGO_PartsStock>>> _GetPartsStock(int supplierId, string type)
        {
            Response<List<Models.FIGO_PartsStock>> _response = new Response<List<Models.FIGO_PartsStock>>();
            try
            {
                string reportName = "PartsStock";

                var queryResponse = await _GetReportQuery(1, null, reportName, 0);

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query del stock de inventario desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                Mapping _mapping = new Mapping();
                _mapping.AddItem("ProductId", "IDPRODUCT");
                _mapping.AddItem("Product", "VPRODUCT");
                _mapping.AddItem("Qty", "IQTY");

                var Params = new List<OracleParameter>
                {
                    new OracleParameter("IDSUPPLIER", OracleDbType.Int32) { Value = supplierId },
                };

                var rawQuery = queryResponse.Data.Query;

                DataTable _table = await _oracleDB.GetDataTable(rawQuery, 0, type, Params);

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = _data.GetList<Models.FIGO_PartsStock>(_mapping, _table);
                _response.SetGetResponse(_table);
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
                var queryResponse = await _GetReportQuery(userId, null, reportName);

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query de tránsito desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                var rawQuery = queryResponse.Data.Query;
                var rawType = queryResponse.Data.Type;

                // 2. Extraer datos desde FIGO con el query obtenido
                DataTable extractedData = await _oracleDB.GetDataTable(rawQuery, 0, rawType, null);

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

        public async Task<Response<Result>> ExtractAndInsertINNT()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ExtractAndInsertINNT();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _ExtractAndInsertINNT()
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                int userId = 1;
                string reportName = "ExtractAndInsertINNT";
                string reportNameDUA = "ExtractAndInsertINNTDUA";

                // 1. Traer query de la BD
                var queryResponse = await _GetReportQuery(userId, null, reportName, 0);
                var queryReponseDUA = await _GetReportQuery(userId, null, reportNameDUA, 0);

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                if (!queryReponseDUA.Processed || queryReponseDUA.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query desde la BD (VNAME: {reportNameDUA})"));
                    return _response;
                }

                string rawQuery = queryResponse.Data.Query;
                string rawType = queryResponse.Data.Type;

                string rawQueryDua = queryReponseDUA.Data.Query;
                string rawTypeDua = queryReponseDUA.Data.Type;

                DataTable extractedData = await _oracleDB.GetDataTable(rawQuery, 4069, rawType, null);
                DataTable extractedDataDua = await _oracleDB.GetDataTable(rawQueryDua, 4069, rawTypeDua, null);

                if (extractedData.Rows.Count == 0 && extractedDataDua.Rows.Count == 0)
                {
                    _response.Message = "No se encontraron unidades datos maestros de la unidades (INTT)";
                    _response.Processed = true;
                    _response.Status = 200;
                    return _response;
                }

                List<Models.FIGO_ModelFeatures> salesList = new List<Models.FIGO_ModelFeatures>();
                List<Models.FIGO_VehicleFileIntt> salesListDUA = new List<Models.FIGO_VehicleFileIntt>();

                if (extractedData.Rows.Count != 0)
                {
                    foreach (DataRow row in extractedData.Rows)
                    {
                        decimal? ParseDecimal(object val)
                        {
                            if (val == null || val == DBNull.Value) return null;
                            string strVal = val.ToString().Replace(',', '.');
                            if (decimal.TryParse(strVal, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                                return result;
                            return null;
                        }

                        int? ParseInt(object val)
                        {
                            if (val == null || val == DBNull.Value) return null;
                            if (int.TryParse(val.ToString(), out int result))
                                return result;
                            return null;
                        }

                        salesList.Add(new Models.FIGO_ModelFeatures
                        {
                            SupplierId = ParseInt(row["IDSUPPLIER"]),
                            ModelCode = row["VCODE"]?.ToString(),
                            ModelName = row["VNAME"]?.ToString(),
                            ModelWeight = ParseDecimal(row["NWEIGHT"]),
                            Capacity = ParseInt(row["ICAPACITY"]),
                            AxleNumber = ParseInt(row["IAXLENUMBER"]),
                            WheeleDiameter = ParseInt(row["IWHEELDIAMETER"]),
                            ModelClass = row["VCLASS"]?.ToString(),
                            ModelType = row["VTYPE"]?.ToString(),
                            ModelUse = row["VUSE"]?.ToString(),
                            SeatsNumber = ParseInt(row["ISEATSNUMBER"]),
                            FuelType = row["VFUELTYPE"]?.ToString()
                        });
                    }
                }

                if (extractedDataDua.Rows.Count != 0)
                {
                    foreach (DataRow row in extractedDataDua.Rows)
                    {
                        int? ParseInt(object val)
                        {
                            if (val == null || val == DBNull.Value) return null;
                            if (int.TryParse(val.ToString(), out int result))
                                return result;
                            return null;
                        }

                        DateTime? ParseDateTime(object val)
                        {
                            if (val == null || val == DBNull.Value) return null;
                            if (DateTime.TryParse(val.ToString(), out DateTime result))
                                return result;
                            return null;
                        }

                        salesListDUA.Add(new Models.FIGO_VehicleFileIntt
                        {
                            SupplierId = ParseInt(row["IDSUPPLIER"]),
                            Vin = row["VVIN"]?.ToString(),
                            FileNumber = row["DFILENUMBER"]?.ToString(),
                            FileDate = ParseDateTime(row["DFILEDATE"]),
                            InvoiceNumber = row["VINVOICENUMBER"]?.ToString(),
                            InvoiceDate = ParseDateTime(row["DINVOICEDATE"]),
                            DuaNumber = row["VDUANUMBER"]?.ToString(),
                            DuaDate = ParseDateTime(row["DDUADATE"]),
                            ModelYear = ParseInt(row["IMODELYEAR"]),
                            ManufactureYear = ParseInt(row["ID_PRODUCTO"])
                        });
                    }
                }

                // 3. Convertir listas a JSON
                string jsonSales = Util.Json.ConvertToJsonString(salesList);
                string jsonSalesDUA = Util.Json.ConvertToJsonString(salesListDUA);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();
                Util.Data _data = Util.Data.GetInstance();

                // 4. Ejecutar Primer POST (ModelFeatures)
                if (salesList.Count > 0)
                {
                    Parameter parameterModelFeatures = new Parameter();
                    parameterModelFeatures.AddSqlParameter("@DATA", jsonSales);
                    DataTable tableModelFeatures = await _data.GetDataTable("USP_POST_MODELFEATURES_FIGO", parameterModelFeatures);
                    _response.Data = _data.GetItem<Models.Result>(_mapping, tableModelFeatures);
                }

                // 5. Ejecutar Segundo POST (VehicleFileIntt) - Cambia "USP_POST_VEHICLEFILEINTT_FIGO" por el nombre real de tu SP para DUA
                if (salesListDUA.Count > 0)
                {
                    Parameter parameterDua = new Parameter();
                    parameterDua.AddSqlParameter("@DATA", jsonSalesDUA);
                    DataTable tableDua = await _data.GetDataTable("USP_POST_VEHICLEFILE_FIGO", parameterDua);
                    _response.Data = _data.GetItem<Models.Result>(_mapping, tableDua);
                }

                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
                Util.Log.Error("_INTTBackgroundService: " + ex.Message);
            }
            return _response;
        }

        public async Task<Response<Result>> ExtractAndInsertMasterSales(string list_id)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ExtractAndInsertMasterSales(list_id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _ExtractAndInsertMasterSales(string list_id)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                int userId = 1;
                string reportName = "ExtractMasterSales";

                // 1. Traer query de la BD
                var queryResponse = await _GetReportQuery(userId, null, reportName, 0);

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                string rawQuery = queryResponse.Data.Query;
                string rawType = queryResponse.Data.Type;

                if (rawQuery.Contains(":LIST_ID"))
                {
                    if (list_id == "")
                    {
                        list_id = "'-1'";
                    }

                    rawQuery = rawQuery.Replace(":LIST_ID", list_id);
                }

                DataTable extractedData_MDV = await _oracleDB.GetDataTable(rawQuery, 4069, rawType, null);
                DataTable extractedData_CIM = await _oracleDB.GetDataTable(rawQuery, 4076, rawType, null);

                if (extractedData_MDV.Rows.Count == 0 && extractedData_CIM.Rows.Count == 0)
                {
                    _response.Message = "No se encontraron facturas en el master de ventas";
                    _response.Processed = true;
                    _response.Status = 200;
                    return _response;
                }

                // 2. Convertir DataTable a List<FIGO_MasterSales>
                List<Models.FIGO_MasterSales> salesList = new List<Models.FIGO_MasterSales>();

                if (extractedData_MDV.Rows.Count != 0)
                {
                    foreach (DataRow row in extractedData_MDV.Rows)
                    {
                        salesList.Add(new Models.FIGO_MasterSales
                        {
                            Id = row["ID"].ToString(),
                            CompanyId = row["COMPANY_ID"].ToString(),
                            CompanyTaxId = row["COMPANY_TAX_ID"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            DocumentType = row["DOCUMENT_TYPE"].ToString(),
                            InvoiceNumber = row["INVOICE_NUMBER"].ToString(),
                            NoteNumber = row["NOTE_NUMBER"].ToString(),
                            IssueDate = Convert.ToDateTime(row["ISSUE_DATE"]),
                            ClientTaxId = row["CLIENT_TAX_ID"]?.ToString(),
                            ClientName = row["CLIENT_NAME"]?.ToString(),
                            ProductName = row["PRODUCT_NAME"]?.ToString(),
                            ProductId = row["PRODUCT_ID"]?.ToString(),
                            Year = row["YEAR"]?.ToString(),
                            Vin = row["VIN"]?.ToString(),
                            EngineNumber = row["ENGINE_NUMBER"]?.ToString(),
                            LicensePlate = row["LICENSE_PLATE"]?.ToString(),
                            Color = row["COLOR"]?.ToString(),
                            UnitPrice = Convert.ToDecimal(row["UNIT_PRICE"]),
                            FinalPrice = Convert.ToDecimal(row["FINAL_PRICE"]),
                            PlatePrice = Convert.ToDecimal(row["PLATE_PRICE"]),
                            UnitPlatePrice = Convert.ToDecimal(row["UNIT_PLATE_PRICE"]),
                            TaxAmount = Convert.ToDecimal(row["TAX_AMOUNT"]),
                            TotalSales = Convert.ToDecimal(row["TOTAL_SALES"]),
                            Cost = Convert.ToDecimal(row["COST"]),
                            ExchangeRate = Convert.ToDecimal(row["EXCHANGE_RATE"])
                        });
                    }
                }

                if (extractedData_MDV.Rows.Count != 0)
                {
                    foreach (DataRow row in extractedData_CIM.Rows)
                    {
                        salesList.Add(new Models.FIGO_MasterSales
                        {
                            Id = row["ID"].ToString(),
                            CompanyId = row["COMPANY_ID"].ToString(),
                            CompanyTaxId = row["COMPANY_TAX_ID"].ToString(),
                            CompanyName = row["COMPANY_NAME"].ToString(),
                            DocumentType = row["DOCUMENT_TYPE"].ToString(),
                            InvoiceNumber = row["INVOICE_NUMBER"].ToString(),
                            NoteNumber = row["NOTE_NUMBER"].ToString(),
                            IssueDate = Convert.ToDateTime(row["ISSUE_DATE"]),
                            ClientTaxId = row["CLIENT_TAX_ID"]?.ToString(),
                            ClientName = row["CLIENT_NAME"]?.ToString(),
                            ProductName = row["PRODUCT_NAME"]?.ToString(),
                            ProductId = row["PRODUCT_ID"]?.ToString(),
                            Year = row["YEAR"]?.ToString(),
                            Vin = row["VIN"]?.ToString(),
                            EngineNumber = row["ENGINE_NUMBER"]?.ToString(),
                            LicensePlate = row["LICENSE_PLATE"]?.ToString(),
                            Color = row["COLOR"]?.ToString(),
                            UnitPrice = Convert.ToDecimal(row["UNIT_PRICE"]),
                            FinalPrice = Convert.ToDecimal(row["FINAL_PRICE"]),
                            PlatePrice = Convert.ToDecimal(row["PLATE_PRICE"]),
                            UnitPlatePrice = Convert.ToDecimal(row["UNIT_PLATE_PRICE"]),
                            TaxAmount = Convert.ToDecimal(row["TAX_AMOUNT"]),
                            TotalSales = Convert.ToDecimal(row["TOTAL_SALES"]),
                            Cost = Convert.ToDecimal(row["COST"]),
                            ExchangeRate = Convert.ToDecimal(row["EXCHANGE_RATE"])
                        });
                    }
                }

                // 3. Convertir lista a JSON
                string jsonSales = Util.Json.ConvertToJsonString(salesList);

                // 4. Ejecutar SP en SIPCON
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", jsonSales);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_MASTERSALES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
                Util.Log.Error("_ExtractAndInsertMasterSales: " + ex.Message);
            }
            return _response;
        }

        public async Task<Response<Result>> ExtractAndInsertParts()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ExtractAndInsertParts();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _ExtractAndInsertParts()
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                int userId = 1;
                string reportName = "ExtractAndInsertParts";

                // 1. Traer query de la BD
                var queryResponse = await _GetReportQuery(userId, null, reportName, 0);

                if (!queryResponse.Processed || queryResponse.Data == null)
                {
                    _response.SetError(new Exception($"No se pudo obtener el query desde la BD (VNAME: {reportName})"));
                    return _response;
                }

                string rawQuery = queryResponse.Data.Query;
                string rawType = queryResponse.Data.Type;

                DataTable extractedData = await _oracleDB.GetDataTable(rawQuery, 4069, rawType, null);

                if (extractedData.Rows.Count == 0)
                {
                    _response.Message = "No se encontraron facturas en el master de ventas";
                    _response.Processed = true;
                    _response.Status = 200;
                    return _response;
                }

                // 2. Convertir DataTable a List<FIGO_MasterSales>
                List<Models.FIGO_Parts> partList = new List<Models.FIGO_Parts>();

                if (extractedData.Rows.Count != 0)
                {
                    foreach (DataRow row in extractedData.Rows)
                    {
                        partList.Add(new Models.FIGO_Parts
                        {
                            Id = Convert.ToInt32(row["ID"]),
                            CompanyId = row["COMPANY_ID"].ToString(),
                            CompanyName = row["COMPANY"].ToString(),
                            ProductId = row["PRODUCT_ID"].ToString(),
                            ProductName = row["PRODUCT"].ToString(),
                            Stock = Convert.ToInt32(row["STOCK"]),
                            Um = row["UM"].ToString()
                        });
                    }
                }

                // 3. Convertir lista a JSON
                string jsonSales = Util.Json.ConvertToJsonString(partList);

                // 4. Ejecutar SP en SIPCON
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", jsonSales);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FIGOINVENTORY", _parameter);
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