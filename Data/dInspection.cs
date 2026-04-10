using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Data
{
    public class dInspection
    {

        private readonly SemaphoreSlim _semaphore;

        public dInspection() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        #region "AREA"
        /*---------------------------------------------------------------PDI---------------------------------------------------------------*/
         
        public async Task<Response<List<Models.Area>>> GetAreaPDI(Int32 userId, Int32 supplierId, int? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAreaPDI(userId, supplierId, dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Area>>> _GetAreaPDI(Int32 userId, Int32 supplierId, int? dealerId)
        {

            Response<List<Models.Area>> _response = new Response<List<Models.Area>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId); 
                _parameter.AddSqlParameter("@IDUSER", userId); 

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME"); 
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER"); 
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER"); 

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_AREA_ACCESSGROUPUSER", _parameter);
                _response.Data = _data.GetList<Models.Area>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*---------------------------------------------------------------GetOne---------------------------------------------------------------*/
        public async Task<Response<Models.Area>> GetArea(int userId, int areaId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetArea(userId ,0, 0, null, null, null, areaId);
            }
            finally
            {
                _semaphore.Release();
            }
        } 
        private async Task<Response<Models.Area>> _GetArea(int userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? areaId)
        {
            Response<Models.Area> _response = new Response<Models.Area>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", null);
                _parameter.AddSqlParameter("@IDDEALER", null); 
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@BACTIVE", null);
                _parameter.AddSqlParameter("@IDUSER", userId); 
                _parameter.AddSqlParameter("@ID", areaId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_AREA", _parameter);
                _response.Data = _data.GetItem<Models.Area>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-------------------------------------------------------------- Get Export   ---------------------------------------------------------------*/

        public async Task<List<Models.Area>> GetAreaExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Models.Area>)(await _GetAllAreas(userId, supplierId, dealerId, null, filter, active)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*---------------------------------------------------------------GetAll---------------------------------------------------------------*/

        public async Task<Response<List<Models.Area>>> GetAllAreas(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllAreas(userId, supplierId, dealerId, rowFrom, filter, active);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Area>>> _GetAllAreas(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? areaId = null)
        {

            Response<List<Models.Area>> _response = new Response<List<Models.Area>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BACTIVE", active);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", areaId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_AREA", _parameter);
                _response.Data = _data.GetList<Models.Area>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*---------------------------------------------------------------POST AREAS---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostAreas(List<Models.Area> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostAreas(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostAreas(List<Models.Area> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_AREAS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //------------------------------------------------------------------POST AREA ACTIONS -------------------------------------------------------------------------
        public async Task<Response<Result>> PostAreaActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostAreaActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _PostAreaActions(List<Models.Action> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_AREA_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        #endregion

        #region "FASE"
        /*---------------------------------------------------------------GetOne---------------------------------------------------------------*/
        public async Task<Response<Models.Fase>> GetFase(int userId, int faseId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetFase(userId, 0, 0, null, null, null, faseId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Models.Fase>> _GetFase(int userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? faseId)
        {
            Response<Models.Fase> _response = new Response<Models.Fase>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", null);
                _parameter.AddSqlParameter("@IDDEALER", null);
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@BACTIVE", null);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", faseId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FASE", _parameter);
                _response.Data = _data.GetItem<Models.Fase>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-------------------------------------------------------------- Get Export   ---------------------------------------------------------------*/

        public async Task<List<Models.Fase>> GetFaseExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Models.Fase>)(await _GetAllFases(userId, supplierId, dealerId, null, filter, active)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*---------------------------------------------------------------GetAll---------------------------------------------------------------*/

        public async Task<Response<List<Models.Fase>>> GetAllFases(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllFases(userId, supplierId, dealerId, rowFrom, filter, active);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Fase>>> _GetAllFases(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? faseId = null)
        {

            Response<List<Models.Fase>> _response = new Response<List<Models.Fase>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BACTIVE", active);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", faseId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE"); 
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FASE", _parameter);
                _response.Data = _data.GetList<Models.Fase>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*---------------------------------------------------------------POST FASES---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostFases(List<Models.Fase> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFases(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostFases(List<Models.Fase> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FASES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //------------------------------------------------------------------POST FASE ACTIONS -------------------------------------------------------------------------
        public async Task<Response<Result>> PostFaseActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFaseActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _PostFaseActions(List<Models.Action> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FASE_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        #endregion

        #region "FEATURETYPE"
        /*---------------------------------------------------------------GetOne---------------------------------------------------------------*/
        public async Task<Response<Models.FeatureType>> GetFeatureType(int userId, int featureTypeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetFeatureType(userId, 0, 0, null, null, null, featureTypeId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Models.FeatureType>> _GetFeatureType(int userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? featureTypeId)
        {
            Response<Models.FeatureType> _response = new Response<Models.FeatureType>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", null);
                _parameter.AddSqlParameter("@IDDEALER", null);
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@BACTIVE", null);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", featureTypeId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("FaseName", "VFASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURETYPE", _parameter);
                _response.Data = _data.GetItem<Models.FeatureType>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-------------------------------------------------------------- Get Export   ---------------------------------------------------------------*/ 

        public async Task<List<Models.FeatureType>> GetFeatureTypeExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Models.FeatureType>)(await _GetAllFeatureType(userId, supplierId, dealerId, null, filter, active)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*---------------------------------------------------------------GetAll---------------------------------------------------------------*/

        public async Task<Response<List<Models.FeatureType>>> GetAllFeatureType(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllFeatureType(userId, supplierId, dealerId, rowFrom, filter, active);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FeatureType>>> _GetAllFeatureType(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? featureTypeId = null)
        {

            Response<List<Models.FeatureType>> _response = new Response<List<Models.FeatureType>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BACTIVE", active);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", featureTypeId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("FaseName", "VFASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURETYPE", _parameter);
                _response.Data = _data.GetList<Models.FeatureType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*---------------------------------------------------------------POST FEATURETYPE---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostFeatureType(List<Models.FeatureType> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFeatureType(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostFeatureType(List<Models.FeatureType> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FEATURETYPE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //------------------------------------------------------------------POST FEATURE ACTIONS -------------------------------------------------------------------------
        public async Task<Response<Result>> PostFeatureTypeActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFeatureTypeActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _PostFeatureTypeActions(List<Models.Action> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FEATURETYPE_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        #endregion

        #region "FEATURE"
        /*---------------------------------------------------------------GetOne---------------------------------------------------------------*/
        public async Task<Response<Models.Feature>> GetFeature(int userId, int featureId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetFeature(userId, 0, 0, null, null, null, featureId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Models.Feature>> _GetFeature(int userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? featureId)
        {
            Response<Models.Feature> _response = new Response<Models.Feature>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", null);
                _parameter.AddSqlParameter("@IDDEALER", null);
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@BACTIVE", null);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", featureId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("DefaultValue", "BDEFAULT");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("FeatureTypeId", "IDFEATURETYPE");
                _mapping.AddItem("FeatureTypeName", "VFEATURETYPE");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("FaseName", "VFASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURE", _parameter);
                _response.Data = _data.GetItem<Models.Feature>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-------------------------------------------------------------- Get Export   ---------------------------------------------------------------*/

        public async Task<List<Feature>> GetFeatureExport(Int32 userId, Int32 supplierId, int? dealerId, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Feature>)(await _GetAllFeature(userId, supplierId, dealerId, null, filter, active)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*---------------------------------------------------------------GetAll---------------------------------------------------------------*/

        public async Task<Response<List<Models.Feature>>> GetAllFeature(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllFeature(userId, supplierId, dealerId, rowFrom, filter, active);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Feature>>> _GetAllFeature(Int32 userId, Int32 supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? featureId = null)
        {

            Response<List<Models.Feature>> _response = new Response<List<Models.Feature>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BACTIVE", active);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", featureId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("DefaultValue", "BDEFAULT");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("FeatureTypeId", "IDFEATURETYPE");
                _mapping.AddItem("FeatureTypeName", "VFEATURETYPE");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("FaseName", "VFASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("Updated", "DUPDATED");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATURE", _parameter);
                _response.Data = _data.GetList<Models.Feature>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*---------------------------------------------------------------POST FEATURE---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostFeature(List<Models.Feature> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFeature(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostFeature(List<Models.Feature> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FEATURES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //------------------------------------------------------------------POST FEATURE ACTIONS -------------------------------------------------------------------------
        public async Task<Response<Result>> PostFeatureActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFeatureActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _PostFeatureActions(List<Models.Action> _list, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                 
                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping(); 

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_FEATURE_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        #endregion 
    }
}
