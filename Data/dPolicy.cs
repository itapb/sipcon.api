using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models;
using Util;

namespace Data
{
    public class dPolicy
    {

        private readonly SemaphoreSlim _semaphore;

        public dPolicy() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.Policy>>> GetAll(string? filter, Int32 rowFrom, Int32 userId, Int32? supplierId,Int32? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
              return await _GetAll(filter, rowFrom, userId, supplierId,dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Policy>>> _GetAll(string? filter, Int32? rowFrom, Int32 userId, Int32? supplierId,Int32? dealerId=null)
        {
            Response<List<Models.Policy>> _response = new Response<List<Models.Policy>>();

            try
            {

                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("ModelName", "MODELNAME");
                _mapping.AddItem("Color", "VCOLOR");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("Phone", "VPHONE1");
                _mapping.AddItem("ActivationDate", "DACTIVATIONDATE");
                _mapping.AddItem("DealerCod", "CODDEALER");
                _mapping.AddItem("SupplierCod", "CODSUPPLIER");
                _mapping.AddItem("InvoiceNumber", "VINVOICENUMBER");
                _mapping.AddItem("InvoiceAmount", "NINVOICEAMOUNT");
                _mapping.AddItem("InvoiceDate", "DINVOICEDATE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusName", "ESTATUSNAME");
                _mapping.AddItem("ExpirationDate", "DEXPIRATIONDATE");
                _mapping.AddItem("Seller", "VSELLER");
                _mapping.AddItem("CustomerCity", "VCITYCUSTOMER");
                _mapping.AddItem("DealerCity", "VCITYDEALER");
                _mapping.AddItem("DateCreated", "DCREATED");
                _mapping.AddItem("DealerName", "VDEALER");



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POLICIES", _parameter);
                _response.Data = _data.GetList<Models.Policy>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }



     
        public async Task<Response<Models.PolicyExtended>> GetOne(Int32 policyId,Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(policyId,userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.PolicyExtended>> _GetOne(Int32 policyId, Int32 userId)
        {
            Response<Models.PolicyExtended> _response = new Response<Models.PolicyExtended>();

            try
            {
                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@ID", policyId);
                _parameter.AddSqlParameter("@IDUSER", userId);



                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("ModelName", "MODELNAME");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Phone", "VPHONE1");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("InvoiceNumber", "VINVOICENUMBER");
                _mapping.AddItem("Direction", "VADDRESS");
                _mapping.AddItem("InvoiceAmount", "NINVOICEAMOUNT");
                _mapping.AddItem("ActivationDate", "DACTIVATIONDATE");
                _mapping.AddItem("DealerCod", "CODDEALER");
                _mapping.AddItem("SupplierCod", "CODSUPPLIER");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("DealerName", "DEALERNAME");
                _mapping.AddItem("SupplierName", "SUPPLIERNAME");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("Color", "VCOLOR");
                _mapping.AddItem("InvoiceDate", "DINVOICEDATE");
                _mapping.AddItem("EstatusId", "IDESTATUS");
                _mapping.AddItem("EstatusName", "ESTATUSNAME");
                _mapping.AddItem("LockDate", "DLOCKDATE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("ExpirationDate", "DEXPIRATIONDATE");
                _mapping.AddItem("PayMethodId", "IDPAYMETHOD");
                _mapping.AddItem("Total", "TOTAL");

      

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POLICY", _parameter);
                _response.Data = _data.GetItem<Models.PolicyExtended>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Models.PolicyExtended>> GetOneBy(Int32 userId, string filter, Int32 filterBy,Int32? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOneBy(userId,  filter, filterBy, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Models.PolicyExtended>> _GetOneBy(Int32 userId, string filter, Int32 filterBy, Int32? supplierId)
        {
            Response<Models.PolicyExtended> _response = new Response<Models.PolicyExtended>();

            try
            {
                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BY", filterBy);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);




                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("ModelName", "MODELNAME");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Phone", "VPHONE1");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("InvoiceNumber", "VINVOICENUMBER");
                _mapping.AddItem("InvoiceAmount", "NINVOICEAMOUNT");
                _mapping.AddItem("ActivationDate", "DACTIVATIONDATE");
                _mapping.AddItem("DealerCod", "CODDEALER");
                _mapping.AddItem("SupplierCod", "CODSUPPLIER");
                _mapping.AddItem("DealerName", "DEALERNAME");
                _mapping.AddItem("SupplierName", "SUPPLIERNAME");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("Color", "VCOLOR");
                _mapping.AddItem("InvoiceDate", "DINVOICEDATE");
                _mapping.AddItem("EstatusId", "IDESTATUS");
                _mapping.AddItem("EstatusName", "ESTATUSNAME");
                _mapping.AddItem("LockDate", "DLOCKDATE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("ExpirationDate", "DEXPIRATIONDATE");
                _mapping.AddItem("PayMethodId", "IDPAYMETHOD");
                _mapping.AddItem("Total", "TOTAL");

       

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POLICY_BY", _parameter);
                _response.Data = _data.GetItem<Models.PolicyExtended>(_mapping, _table);
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<List<Models.PolicyDetails>>> GetPolicyDetails(Int32 policyId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPolicyDetails(policyId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
       

        private async Task<Response<List<Models.PolicyDetails>>> _GetPolicyDetails(Int32 policyId)
        {
            Response<List<Models.PolicyDetails>> _response = new Response<List<Models.PolicyDetails>>();

            try
            {
                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDPOLICY", policyId);
                
                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Km", "IKM");
                _mapping.AddItem("FromKm", "IFROMKM");
                _mapping.AddItem("UpToKm", "IUPTOKM");
                _mapping.AddItem("Date", "DDATE");
                _mapping.AddItem("FromDate", "DFROMDATE");
                _mapping.AddItem("UpToDate", "DUPTODATE");
                _mapping.AddItem("Valid", "VVALID");

                var _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POLICYDETAILS", _parameter);
                _response.Data = _data.GetList<Models.PolicyDetails>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<List<Models.PolicyDetails>>> GetOnePolicyDetails(Int32 policyId, Int32? km, DateTime? date)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetLogPolicyDetails(policyId, km, date);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        public async Task<Response<List<Models.PolicyDetails>>> GetLogPolicyDetails(Int32 policyId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetLogPolicyDetails(policyId, null, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<List<Models.PolicyDetails>>> _GetLogPolicyDetails(Int32 policyId, Int32? km = null, DateTime? date = null)
        {
            Response<List<Models.PolicyDetails>> _response = new Response<List<Models.PolicyDetails>>();

            try
            {
                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDPOLICY", policyId);
                _parameter.AddSqlParameter("@IKM", km);
                _parameter.AddSqlParameter("@DDATE", date);

                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Km", "IKM");
                _mapping.AddItem("FromKm", "IFROMKM");
                _mapping.AddItem("UpToKm", "IUPTOKM");
                _mapping.AddItem("Date", "DDATE");
                _mapping.AddItem("FromDate", "DFROMDATE");
                _mapping.AddItem("UpToDate", "DUPTODATE");
                _mapping.AddItem("Valid", "VVALID");

                var _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GETLOG_POLICYDETAILS", _parameter);
                _response.Data = _data.GetList<Models.PolicyDetails>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<List<Policy>> GetExport(string? _filter, Int32 userId, Int32? supplierId,Int32? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                Response<List<Models.Policy>> response = await _GetAll(_filter, null, userId, supplierId,dealerId);
                if (response.Data is List<Policy> policyList)
                {
                    return policyList;
                }
                return new List<Policy>();
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response<Models.Result>> Post_Policy(Policy _policy, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Policy(_policy, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Policy(Policy _policy, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_policy);
                
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_POLICY", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<Models.Result>> Post_Actions(List<Models.Action> _list,Int32 userId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Actions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Models.Result>> _Post_Actions(List<Models.Action> _list,Int32 userId)

        {
            Response<Models.Result> _response = new Models.Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_POLICIES_ACTIONS", _parameter);
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
