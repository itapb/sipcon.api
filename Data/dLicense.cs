using System;
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
    public class dLicense
    {

        private readonly SemaphoreSlim _semaphore;

        public dLicense() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.License>>> GetAll(string? filter, Int32 rowFrom, Int32 userId,Int32? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
              return await _GetAll(filter, rowFrom, userId,supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.License>>> GetOne(Int32 licenseId, Int32 userId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _GetAll(null, null, userId,null, licenseId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.License>>> _GetAll(string? filter, Int32? rowFrom, Int32 userId,Int32? supplierId = null, Int32? licenseId = null)
        {

            Response<List<Models.License>> _response = new Response<List<Models.License>>();
           
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@ID", licenseId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("TypeId", "IDLICENSETYPE");
                _mapping.AddItem("Type", "VLICENSETYPE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("ExpirationDate", "DEXPIRATIONDATE");
                _mapping.AddItem("EstatusId", "IESTATUS");
                _mapping.AddItem("EstatusName", "VESTATUSNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LICENSE", _parameter);
                _response.Data = _data.GetList<Models.License>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        


        public async Task<Response<List<Models.LicenseDetails>>> GetDetails(string? filter, Int32? rowFrom,Int32 licenseId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDetails(filter, rowFrom, userId, licenseId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.LicenseDetails>>> _GetDetails(string? filter, Int32? rowFrom, Int32 userId, Int32? licenseId = null, Boolean? available = false)
        {

            Response<List<Models.LicenseDetails>> _response = new Response<List<Models.LicenseDetails>>();

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@IDLICENSE", licenseId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BAVAILABLE", available);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("ReportId", "IDMAINTENANCE");
                _mapping.AddItem("LicenseId", "IDLICENSE");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("LicenseName", "VLICENSENAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusDetailId", "IESTATUS");
                _mapping.AddItem("EstatusDetail", "VESTATUSDETAIL");
                _mapping.AddItem("EstatusVehicle", "VESTATUSVEHICLE");
                _mapping.AddItem("Model", "VMODEL");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VFIRSTNAME");
                _mapping.AddItem("CustomerLastName", "VLASTNAME");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LICENSEDETAILS", _parameter);
                _response.Data = _data.GetList<Models.LicenseDetails>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<Response<List<Models.LicenseDetails>>> GetDetailBy(String filter, Int32 userId,Boolean? available=true)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _GetDetails(filter, null, userId,null, available);

            }
            finally
            {
                _semaphore.Release();
            }
        }



        public async Task<List<LicenseDetails>> GetExportDetails(string? filter,Int32 userId, Int32 licenseId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                Response<List<Models.LicenseDetails>> _response = new Response<List<Models.LicenseDetails>>();
                _response = await _GetDetails(filter,null,userId, licenseId );
                return (List<LicenseDetails>)_response.Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }





        public async Task<Response<Models.Result>> Post_License(List<License> _list,Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_License(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_License(List<License> _list, Int32 userId)
        {
            
            
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);
                

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();
                


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_LICENSE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        
        public async Task<Response<Models.Result>> Post_LicenseDetails(List<LicenseDetails> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_LicenseDetails(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Models.Result>> _Post_LicenseDetails(List<LicenseDetails> _list, Int32 userId)
        {


            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

        

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_LICENSEDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Models.Result>> Post_Actions(List<Models.Action> _list, Int32 userId)
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

        private async Task<Response<Models.Result>> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_LICENSE_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Models.Result>> Post_DetailsActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _Post_DetailsActions(_list, userId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_DetailsActions(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_DELETE_LICENSEDETAIL", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        


        public async Task<Response<List<Models.LicenseTypes>>> GetLicenseType()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetLicenseType();
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Response<List<Models.LicenseTypes>>> _GetLicenseType()
        {

            Response<List<Models.LicenseTypes>> _response = new Response<List<Models.LicenseTypes>>();

            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LICENSETYPE");
                _response.Data = _data.GetList<Models.LicenseTypes>(_mapping, _table);
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
