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
using Parameter = Util.Parameter;

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

        public async Task<Models.Response<List<T>>> GetAll<T>(string? filter, int? rowFrom, int userId, int serviceTypeId, int dealerId, int? supplierId,  int? serviceId = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll<T>(filter, rowFrom, userId, serviceTypeId, dealerId, supplierId, serviceId);
            }
            finally
            {
                _semaphore.Release();
            }
        }




        private async Task<Models.Response<List<T>>> _GetAll<T>(string? filter,int? rowFrom,int userId,int serviceTypeId, int dealerId, int? supplierId, int? serviceId = null)
        {
            var _response = new Response<List<T>>();

            try
            {
                // Parámetros SQL
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@ID", serviceId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSERVICETYPE", serviceTypeId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                // Mapeo por tipo
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
                _mapping.AddItem("AssistanceTypeId", "IDASSISTANCETYPE");
                _mapping.AddItem("AssistanceType", "VASSISTANCETYPE");
                _mapping.AddItem("PossibleFaultId", "IDPOSSIBLEFAULT");
                _mapping.AddItem("PossibleFault", "VPOSSIBLEFAULT");
                _mapping.AddItem("StartDate", "DSTARTDATE");
                _mapping.AddItem("Assesment", "IQUANTITY");
                _mapping.AddItem("EndDate", "DENDDATE");


                // Ejecución del SP
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MAINTENANCES", _parameter);

                // Conversión dinámica
                _response.Data = _data.GetList<T>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }





        public async Task<Models.Response<List<Models.ServiceDetails>>> GetDetails( Int32? serviceId, String? type ,String? filter, Int32? supplierId, Int32? dealerId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDetails(serviceId, type, filter, supplierId,dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.ServiceDetails>>> _GetDetails(Int32? serviceId, String? type , String? filter,Int32? supplierId, Int32? dealerId)
        {

            Response<List<Models.ServiceDetails>> _response = new Response<List<Models.ServiceDetails>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDMAINTENANCE", serviceId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@VTYPE", type);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("ServiceId", "IDMAINTENANCE");
                _mapping.AddItem("ItemId", "IDITEM");
                _mapping.AddItem("ItemName", "VITEMNAME");
                _mapping.AddItem("ItemDescription", "VITEMDESCRIPTION");
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


        public async Task<Models.Response<List<Models.Dms>>> GetDms(Int32? userId, Int32? supplierId, String? filter, int row, DateTime? fromDate, DateTime? upToDate)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDms( userId, supplierId, filter, row, fromDate, upToDate);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.Dms>>> _GetDms(Int32? userId, Int32? supplierId, String? filter, int? row, DateTime? fromDate, DateTime? upToDate)
        {

            Response<List<Models.Dms>> _response = new Response<List<Models.Dms>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", row);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Srg", "SRG");
                _mapping.AddItem("BaseAmount", "NBASEAMOUNT");
                _mapping.AddItem("CodDms", "VCODITEM");
                _mapping.AddItem("DmsDate", "DDATEDMS");
                _mapping.AddItem("CodItem", "VCODITEM");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("PreApproval", "VPREAPPROVAL");
                _mapping.AddItem("PreApprovalDate", "DDATEPREAPPROVAL");
                _mapping.AddItem("PaidAmount", "NPAIDAMOUNT");
                _mapping.AddItem("finalApprovalDate", "DDATEFINALAPPROVAL");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("EstatusId", "IESTATUS");
                _mapping.AddItem("Estatus", "VESTATUS");
                _mapping.AddItem("Completed", "BCOMPLETED");
                _mapping.AddItem("Created", "DCREATED");



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DMS", _parameter);
                _response.Data = _data.GetList<Models.Dms>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        public async Task<Models.Response<List<Models.ReportType>>> GetImportType( Int32? supplierId, int? row )

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetImportType(supplierId,row);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.ReportType>>> _GetImportType( Int32? supplierId, int? row)
        {

            Response<List<Models.ReportType>> _response = new Response<List<Models.ReportType>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", row);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DMS", _parameter);
                _response.Data = _data.GetList<Models.ReportType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }





        public async Task<Models.Response<Models.PartItem>> GetOneItemParts(int userId, String type, int itemId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOneItemParts(userId, type, itemId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Models.PartItem>> _GetOneItemParts(int userId, String type, int itemId)
        {

            Response<Models.PartItem> _response = new Response<Models.PartItem>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@VTYPE", type);
                _parameter.AddSqlParameter("@IDSUPPLIER", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@ID", itemId);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Type", "VTYPE");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ITEMS", _parameter);
                _response.Data = _data.GetItem<Models.PartItem>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        public async Task<Models.Response<List<Models.PartItem>>> GetItemsParts(int userId, String type, int supplierId, string? filter, int rowFrom)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetItemsParts(userId, type, supplierId,filter,rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<List<Models.PartItem>>> _GetItemsParts(int userId, String type, int supplierId, string? filter, int rowFrom)
        {

            Response<List<Models.PartItem>> _response = new Response<List<Models.PartItem>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@VTYPE", type);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Type", "VTYPE");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ITEMS", _parameter);
                _response.Data = _data.GetList<Models.PartItem>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        public async Task<Models.Response<List<Models.LabortimeItem>>> GetItemsLaborTime(int userId, String type, int supplierId, string? filter, int rowFrom)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetItemsLaborTime(userId, type, supplierId, filter, rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<List<Models.LabortimeItem>>> _GetItemsLaborTime(int userId, String type, int supplierId, string? filter, int rowFrom)
        {

            Response<List<Models.LabortimeItem>> _response = new Response<List<Models.LabortimeItem>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@VTYPE", type);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Type", "VTYPE");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ITEMS", _parameter);
                _response.Data = _data.GetList<Models.LabortimeItem>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Models.Response<Models.LabortimeItem>> GetOneItemLaborTime(int userId, String type, int itemId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOneItemLaborTime(userId, type, itemId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Models.LabortimeItem>> _GetOneItemLaborTime(int userId, String type, int itemId)
        {

            Response<Models.LabortimeItem> _response = new Response<Models.LabortimeItem>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@VTYPE", type);
                _parameter.AddSqlParameter("@IDSUPPLIER", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@ID", itemId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Type", "VTYPE");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ITEMS", _parameter);
                _response.Data = _data.GetItem<Models.LabortimeItem>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }






        public async Task<Response<List<Models.ReportType>>> GetReportType()
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



        private async Task<Response<List<Models.ReportType>>> _GetReportType()
        {

            Response<List<Models.ReportType>> _response = new Response<List<Models.ReportType>>();

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


        public async Task<Response<List<Models.AssistanceType>>> GetAssistanceType()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAssistanceType();
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.AssistanceType>>> _GetAssistanceType()
        {

            Response<List<Models.AssistanceType>> _response = new Response<List<Models.AssistanceType>>();

            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ASSISTANCETYPE");

                _response.Data = _data.GetList<Models.AssistanceType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }



        public async Task<Response<List<Models.User>>> GetUserAssign(String? filter, Int32? irowFrom, Int32? Id, Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetUserAssign(userId,supplierId, Id, filter, irowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.User>>> _GetUserAssign(Int32 userId, Int32 supplierId,Int32? Id, String? filter, Int32? irowFrom)
        {

            Response<List<Models.User>> _response = new Response<List<Models.User>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", Id);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", irowFrom);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_USERS_ASSIGN",_parameter);

                _response.Data = _data.GetList<Models.User>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }



        public async Task<Response<List<Models.PossibleFault>>> GetPossibleFault()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPossibleFault();
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.PossibleFault>>> _GetPossibleFault()
        {

            Response<List<Models.PossibleFault>> _response = new Response<List<Models.PossibleFault>>();

            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POSSIBLEFAULT");

                _response.Data = _data.GetList<Models.PossibleFault>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        

        public async Task<Response<List<Models.ServiceType>>> GetServiceType()
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



        private async Task<Response<List<Models.ServiceType>>> _GetServiceType()
        {

            Response<List<Models.ServiceType>> _response = new Response<List<Models.ServiceType>>();

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


        public async Task<Models.Response<List<T>>> GetOne<T>( int userId, int serviceTypeId, int dealerId, int serviceId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll<T>( filter: null,null,userId, serviceTypeId,dealerId, null,
                    serviceId: serviceId
                );
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<List<SrgPending>> GetExportSrg(Int32 userId, Int32? supplierId, Int32? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                Response<List<Models.SrgPending>> response = await _GetExportSrg(userId, supplierId, dealerId);
                if (response.Data is List<SrgPending> srgList)
                {
                    return srgList;
                }
                return new List<SrgPending>();
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<List<Models.SrgPending>>> _GetExportSrg(Int32 userId, Int32? supplierId, Int32? dealerId)
        {
            Response<List<Models.SrgPending>> _response = new Response<List<Models.SrgPending>>();

            try
            {
                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IDUSER", userId);

                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Srg", "VSRGNUMBER");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Model", "VMODEL");
                _mapping.AddItem("DescriptionFail", "VTECHNICALSOLUTION");
                _mapping.AddItem("Invoice", "VINVOICENUMBER");
                _mapping.AddItem("InvoiceDate", "DINVOICEDATE");
                _mapping.AddItem("Exent", "NEXEMPT");
                _mapping.AddItem("TaxBase", "NTAXBASE");
                _mapping.AddItem("Mount", "NINVOICEAMOUNT");
                _mapping.AddItem("Tax", "TAX");
                _mapping.AddItem("Dealer", "DEALER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("Reporttype", "REPORTTYPE");
                

                var _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_SRG_PENDING", _parameter);
                _response.Data = _data.GetList<Models.SrgPending>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response<Models.Result>> Post_Service(Models.ServiceMaintenance? maintenance,Models.ServiceAssistance? assistance, Models.ServiceFail? failReport, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                var response = maintenance is not null ? await _Post_Service(maintenance, null, null, userId)
                      : assistance is not null ? await _Post_Service(null, assistance, null, userId)
                      : failReport is not null ? await _Post_Service(null, null, failReport, userId)
                      : new Response<Models.Result> { Processed = false, Message = "No se proporcionó ningún servicio válido." }; // Manejo de error

                return response;

            }
            finally
            {
                _semaphore.Release();
            }
        }



        

        private async Task<Response<Models.Result>> _Post_Service(Models.ServiceMaintenance? maintenance, Models.ServiceAssistance? assistance, Models.ServiceFail? failReport, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

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
                DataTable _table = await _data.GetDataTable("USP_POST_MAINTENANCE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response<Models.Result>> Post_Dms(List<Models.Dms?> _list, Int32 userId, Int32 type)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Dms(_list, userId,type);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Dms(List<Models.Dms?> _list, Int32 userId,Int32 type)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@TYPE", type);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_DMS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response<Models.Result>> Post_Details(Models.ServiceDetails serviceDetails, Int32 userId)
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

        private async Task<Response<Models.Result>> _Post_Details(Models.ServiceDetails serviceDetails, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(serviceDetails);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_MAINTENANCEDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<Models.Result>> Post_Item(Models.Item item, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Item(item, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Item(Models.Item item, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(item);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ITEM", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<Models.Result>> Delete_Details(List<Models.Action> _list, Int32 userId)
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

        private async Task<Response<Models.Result>> _Delete_Details(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_DELETE_MAINTENANCEDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response<Models.Result>> Post_ActionsDetails(List<Models.Action> _list, Int32 userId)
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

        private async Task<Response<Models.Result>> _Post_ActionsDetails(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_MAINTENANCEDETAILS_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response<Models.Result>> Post_Actions(List<Models.Action> _list, Int32 userId, Int32 serviceTypeId)
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

        private async Task<Response<Models.Result>> _Post_Actions(List<Models.Action> _list, Int32 userId, Int32 serviceTypeId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
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
                DataTable _table = await _data.GetDataTable("USP_POST_MAINTENANCE_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Models.Result>> Post_Actions_Process(List<Models.ActionInvoice> _list, Int32 userId, Int32 serviceTypeId)
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

        private async Task<Response<Models.Result>> _Post_Actions_Process(List<Models.ActionInvoice> _list, Int32 userId, Int32 serviceTypeId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
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
                DataTable _table = await _data.GetDataTable("USP_POST_MAINTENANCE_PROCESS", _parameter);
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
