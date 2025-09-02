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
    public class dPolicyType
    {

        private readonly SemaphoreSlim _semaphore;

        public dPolicyType() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.PolicyType>>> GetAll(string? filter, Int32 rowFrom, Int32 userId,Int32? supplierId, Int32? brandId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
              return await _GetAll(filter, rowFrom, userId,supplierId, brandId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.PolicyType>>> _GetAll(string? filter, Int32? rowFrom, Int32 userId,Int32? supplierId = null, Int32? brandId=null, Int32? policyTypeId = null )
        {

            Response<List<Models.PolicyType>> _response = new Response<List<Models.PolicyType>>();
           
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@ID", policyTypeId);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDBRAND", brandId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("Km", "IKM");
                _mapping.AddItem("GapKm", "IGAPKM");
                _mapping.AddItem("TopKm", "ITOPKM");
                _mapping.AddItem("Months", "IMONTHS");
                _mapping.AddItem("GapMonths", "IGAPMONTHS");
                _mapping.AddItem("TopMonths", "ITOPMONTHS");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRANDNAME");
                _mapping.AddItem("IsActive", "BACTIVE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POLICYTYPES", _parameter);
                _response.Data = _data.GetList<Models.PolicyType>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


         public async Task<Response<Models.PolicyType>> GetOne(Int32 policyTypeId,Int32 userId)

        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _GetOne(policyTypeId, userId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.PolicyType>> _GetOne(Int32 policyTypeId, Int32 userId)
        {

            Response<Models.PolicyType> _response = new Response<Models.PolicyType>();

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@IROWFROM", 0);
                _parameter.AddSqlParameter("@ID", policyTypeId);
                _parameter.AddSqlParameter("@IDUSER",userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Description", "VDESCRIPTION");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Km", "IKM");
                _mapping.AddItem("GapKm", "IGAPKM");
                _mapping.AddItem("TopKm", "ITOPKM");
                _mapping.AddItem("Months", "IMONTHS");
                _mapping.AddItem("GapMonths", "IGAPMONTHS");
                _mapping.AddItem("TopMonths", "ITOPMONTHS");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRANDNAME");
                _mapping.AddItem("IsActive", "BACTIVE");



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_POLICYTYPES", _parameter);
                _response.Data = _data.GetItem<Models.PolicyType>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        public async Task<List<PolicyType>> GetExport(string? _filter, Int32 userId, Int32? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<PolicyType>)(await _GetAll(_filter, null, userId,supplierId)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Result>> Post_PolicyType(List<PolicyType> _list,Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_PolicyType(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_PolicyType(List<PolicyType> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_POLICYTYPES", _parameter);
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

                return await _Post_Actions(_list,userId);

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
                DataTable _table = await _data.GetDataTable("USP_POST_POLICYTYPES_ACTIONS", _parameter);
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
