using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Mapping = Util.Mapping;

namespace Data
{
    public class dService
    {

        private readonly SemaphoreSlim _semaphore;

        public dService()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Models.Response> GetAll(string? filter, Int32? rowFrom, Int32 userId, Int32 serviceTypeId,Int32 dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(filter, rowFrom, userId,serviceTypeId,dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetAll(string? filter, Int32? rowFrom, Int32 userId, Int32 serviceTypeId,Int32 dealerId, Int32? serviceId = null)
        {

            Models.Response _response = new Models.Response();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@ID", serviceId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSERVICETYPE", serviceTypeId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("ServiceTypeId", "IDSERVICETYPE");
                _mapping.AddItem("ServiceTypeName", "VSERVICETYPE");
                _mapping.AddItem("OrderNumber", "VSERVICENUMBER");
                _mapping.AddItem("ServiceDate", "DMAINTENANCEDATE");
                _mapping.AddItem("ReportTypeId", "IDREPORTTYPE");
                _mapping.AddItem("Paralyzed", "BPARALYZED");
                _mapping.AddItem("CustomerReport", "VREPORTCUSTOMER");
                _mapping.AddItem("DealerReport", "VTECHNICALREPORT");
                _mapping.AddItem("TechnicalSolution", "VTECHNICALSOLUTION");
                _mapping.AddItem("SupplierReport", "VSUPPLIERDECISION");
                _mapping.AddItem("ReportTypeName", "VREPORTTYPE");
                _mapping.AddItem("DealerId", "IDDEALERSERVICE");
                _mapping.AddItem("DealerServiceName", "VDEALERSERVICE");
                _mapping.AddItem("DealerAddress", "VDEALERADDRESS");
                _mapping.AddItem("DealerServiceCity", "VNAMECITY");
                _mapping.AddItem("DealerServiceState", "VNAMESTATE");
                _mapping.AddItem("DealerServiceCod", "VDEALERCOD");
                _mapping.AddItem("AuthorizedUserName", "VAUTHORIZEDUSERNAME");
                _mapping.AddItem("AuthorizedUserLastName", "VAUTHORIZEDUSERLASTNAME");
                _mapping.AddItem("SrgNumber", "VSRGNUMBER");
                _mapping.AddItem("DealerSaleId", "IDDEALERSALE");
                _mapping.AddItem("DealerSaleName", "VDEALERSALE");
                _mapping.AddItem("DealerSaleCod", "VDEALERSALECOD");
                _mapping.AddItem("NumberPolicy", "VNUMBERPOLICY");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Km", "IKM");
                _mapping.AddItem("CustomerID", "IDCUSTOMER");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODELNAME");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMERNAME");
                _mapping.AddItem("CustomerLastName", "VCUSTOMERLASTNAME");
                _mapping.AddItem("CustomerPhone", "VPHONE1");
                _mapping.AddItem("InvoiceDate", "DINVOICEDATE");
                _mapping.AddItem("InvoiceNumber", "VINVOICENUMBER");
                _mapping.AddItem("InvoiceAmount", "NINVOICEAMOUNT");
                _mapping.AddItem("TaxBase", "NTAXBASE");
                _mapping.AddItem("Tax", "TAX");
                _mapping.AddItem("AuthotizationDate", "DAUTHORIZATIONDATE");
                _mapping.AddItem("Exempt", "NEXEMPT");
                _mapping.AddItem("EstatusId", "IESTATUS");
                _mapping.AddItem("EstatusName", "VESTATUSNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("LicenseId", "IDLICENSE");
                _mapping.AddItem("LicenseDescription", "VDESCRIPTIONLICENSE");
                _mapping.AddItem("LicenseTypeId", "IDLICENSETYPE");
                _mapping.AddItem("LicenseType", "VLICENSETYPE");
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MAINTENANCES", _parameter);
                if (serviceTypeId == 1) { _response.Data = _data.GetList<Models.ServiceMaintenance>(_mapping, _table); }
                if (serviceTypeId == 2) { _response.Data = _data.GetList<Models.ServiceAssistance>(_mapping, _table); }
                if (serviceTypeId == 3) { _response.Data = _data.GetList<Models.ServiceFail>(_mapping, _table); }
     
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Models.Response> GetDetails( Int32 serviceId, String? type ,String? filter)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDetails(serviceId,type, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetDetails(Int32 serviceId, String? type , String? filter)
        {

            Models.Response _response = new Models.Response();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDMAINTENANCE", serviceId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@VTYPE", type);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("ServiceId", "IDMAINTENANCE");
                _mapping.AddItem("ItemId", "IDITEM");
                _mapping.AddItem("ItemName", "VITEMNAME");
                _mapping.AddItem("Type", "CTYPE");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("UnitPrice", "NUNITPRICE");
                _mapping.AddItem("EstatusId", "IESTATUS");
                _mapping.AddItem("EstatusName", "VESTATUSNAME");
                _mapping.AddItem("IsTax", "BTAX");
                _mapping.AddItem("IsExternal", "BEXTERNAL");
                _mapping.AddItem("Serial", "VSERIAL");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("InvoiceNumber", "VINVOICENUMBER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MAINTENANCEDETAILS", _parameter);
                _response.Data = _data.GetList<Models.ServiceDetails>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Models.Response> GetReportType()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReportType();
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetReportType()
        {

            Models.Response _response = new Models.Response();

            try
            {
             
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORTTYPE");

                _response.Data = _data.GetList<Models.ReportType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Models.Response> GetServiceType()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetServiceType();
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetServiceType()
        {

            Models.Response _response = new Models.Response();

            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SERVICETYPE");

                _response.Data = _data.GetList<Models.ServiceType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Response> GetOne(Int32 userId, Int32 serviceTypeId, Int32 dealerId, Int32 serviceId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _GetAll(null,0, userId,serviceTypeId, dealerId, serviceId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        

        public async Task<List<Service>> GetExport(string? _filter, Int32 userId, Int32 serviceTypeId,Int32 dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Service>)(await _GetAll(_filter, null, userId, serviceTypeId,dealerId)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

       


        public async Task<Response> Post_Service(Models.ServiceMaintenance? maintenance,Models.ServiceAssistance? assistance, Models.ServiceFail? failReport, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                var response = maintenance is not null ? await _Post_Service(maintenance, null, null, userId)
                      : assistance is not null ? await _Post_Service(null, assistance, null, userId)
                      : failReport is not null ? await _Post_Service(null, null, failReport, userId)
                      : new Response { Processed = false, Message = "No se proporcionó ningún servicio válido." }; // Manejo de error

                return response;

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_Service(Models.ServiceMaintenance? maintenance, Models.ServiceAssistance? assistance, Models.ServiceFail? failReport, Int32 userId)
        {
            Response _response = new Response();

            try
            {
                string _jsonstring = string.Empty; // Initialize the variable to avoid CS0165

                if (failReport is not null) { _jsonstring = Util.Json.ConvertToJsonString(failReport); }
                
                else if (maintenance is not null) { _jsonstring = Util.Json.ConvertToJsonString(maintenance); } // Corrected to use 'maintenance'  
                
                else if (assistance is not null) { _jsonstring = Util.Json.ConvertToJsonString(assistance); } // Corrected to use 'assistance'
                
                if (string.IsNullOrEmpty(_jsonstring)) {throw new InvalidOperationException("No se pudo generar el JSON para el servicio proporcionado."); }


                Models.Service service = JsonConvert.DeserializeObject<Service>(_jsonstring, Util.Json.GetJsonSerializerSettings())
                    ?? throw new InvalidOperationException("La deserialización del JSON devolvió un valor nulo."); // Handle CS8600

                string _jsonstringlobal = Util.Json.ConvertToJsonString(service);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstringlobal);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_MAINTENANCE", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }






        public async Task<Response> Post_Details(Models.ServiceDetails serviceDetails, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Details(serviceDetails,userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_Details(Models.ServiceDetails serviceDetails, Int32 userId)
        {
            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(serviceDetails);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_MAINTENANCEDETAILS", _mapping, _parameter);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        public async Task<Response> Delete_Details(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Delete_Details(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Delete_Details(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_DELETE_MAINTENANCEDETAILS", _mapping, _parameter);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response> Post_ActionsDetails(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _Post_ActionsDetails(_list, userId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_ActionsDetails(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_MAINTENANCEDETAILS_ACTIONS", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response> Post_Actions(List<Models.Action> _list, Int32 userId, Int32 serviceTypeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _Post_Actions(_list, userId, serviceTypeId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_Actions(List<Models.Action> _list, Int32 userId, Int32 serviceTypeId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSERVICETYPE", serviceTypeId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_MAINTENANCE_ACTIONS", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response> Post_Actions_Process(List<Models.ActionInvoice> _list, Int32 userId, Int32 serviceTypeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _Post_Actions_Process(_list, userId, serviceTypeId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_Actions_Process(List<Models.ActionInvoice> _list, Int32 userId, Int32 serviceTypeId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSERVICETYPE", serviceTypeId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_MAINTENANCE_PROCCESS", _mapping, _parameter);
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
