using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
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
        #region "PDI"
        /*---------------------------------------------------------------Get AccessGroup PDI---------------------------------------------------------------*/

        public async Task<Response<List<Models.AccessGroupPDI>>> GetAccessGroupPDI(Int32 userId, Int32 supplierId, int? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAccessGroupPDI(userId, supplierId, dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.AccessGroupPDI>>> _GetAccessGroupPDI(Int32 userId, Int32 supplierId, int? dealerId)
        {

            Response<List<Models.AccessGroupPDI>> _response = new Response<List<Models.AccessGroupPDI>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDFASE");
                _mapping.AddItem("FaseName", "VFASE"); 
                _mapping.AddItem("AreaId", "IDAREA"); 
                _mapping.AddItem("AreaName", "VAREA");
                _mapping.AddItem("GivesOutCar", "BGIVESOUTCAR");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCESSGROUPPDI", _parameter);
                _response.Data = _data.GetList<Models.AccessGroupPDI>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*---------------------------------------------------------------TRANSPORTER---------------------------------------------------------------*/

        public async Task<Response<List<Models.Transporter>>> GetTransportPDI()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetTransportPDI();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Transporter>>> _GetTransportPDI()
        {
            Response<List<Models.Transporter>> _response = new Response<List<Models.Transporter>>();

            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_TRANSPORTER");
                _response.Data = _data.GetList<Models.Transporter>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*---------------------------------------------------------------DEALER---------------------------------------------------------------*/

        public async Task<Response<List<Models.Dealer>>> GetAllDealer(Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllDealer(supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Dealer>>> _GetAllDealer(Int32 supplierId)
        {

            Response<List<Models.Dealer>> _response = new Response<List<Models.Dealer>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId); 

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Reference", "VREFERENCE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DEALER", _parameter);
                _response.Data = _data.GetList<Models.Dealer>(_mapping, _table);
                _response.SetGetResponse(_table); 
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        #endregion

        #region "AREA"
        /*---------------------------------------------------------------GetOne---------------------------------------------------------------*/
        public async Task<Response<Models.Area>> GetArea(int userId, int areaId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetArea(userId, 0, 0, null, null, null, areaId);
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

        public async Task<List<Models.Area>> GetAreaExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active)
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

        public async Task<Response<List<Models.Area>>> GetAllAreas(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active)
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

        private async Task<Response<List<Models.Area>>> _GetAllAreas(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active, int? areaId = null)
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
        public async Task<Response<Models.Fase>> GetFase(Int32 userId, int faseId)
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
        private async Task<Response<Models.Fase>> _GetFase(Int32 userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? faseId)
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
                _mapping.AddItem("OrderBy", "IORDERBY");
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

        public async Task<List<Models.Fase>> GetFaseExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active)
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

        public async Task<Response<List<Models.Fase>>> GetAllFases(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active)
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

        private async Task<Response<List<Models.Fase>>> _GetAllFases(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active, int? faseId = null)
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
                _mapping.AddItem("OrderBy", "IORDERBY");
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
        public async Task<Response<Models.FeatureType>> GetFeatureType(Int32 userId, int featureTypeId)
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
        private async Task<Response<Models.FeatureType>> _GetFeatureType(Int32 userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? featureTypeId)
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

        public async Task<List<Models.FeatureType>> GetFeatureTypeExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active)
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

        public async Task<Response<List<Models.FeatureType>>> GetAllFeatureType(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active)
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

        private async Task<Response<List<Models.FeatureType>>> _GetAllFeatureType(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active, int? featureTypeId = null)
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
        public async Task<Response<Models.Feature>> GetFeature(Int32 userId, int featureId)
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
        private async Task<Response<Models.Feature>> _GetFeature(Int32 userId, int? supplierId, int? dealerId, int? rowFrom, string? filter, bool? active, int? featureId)
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
                _mapping.AddItem("FeatureValueTypeId", "IDFEATUREVALUETYPE"); 
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

        public async Task<List<Feature>> GetFeatureExport(Int32 userId, Int32 supplierId, Int32 dealerId, string? filter, bool? active, int? modelId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Feature>)(await _GetAllFeature(userId, supplierId, dealerId, null, filter, active, modelId)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*---------------------------------------------------------------GetAll---------------------------------------------------------------*/

        public async Task<Response<List<Models.Feature>>> GetAllFeature(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active, int? modelId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllFeature(userId, supplierId, dealerId, rowFrom, filter, active, modelId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Feature>>> _GetAllFeature(Int32 userId, Int32 supplierId, Int32 dealerId, int? rowFrom, string? filter, bool? active, int? modelId, int? featureId = null)
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
                _parameter.AddSqlParameter("@IDMODEL", modelId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("DefaultValue", "BDEFAULT");
                _mapping.AddItem("FeatureValueTypeId", "IDFEATUREVALUETYPE"); 
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
        /*---------------------------------------------------------------FEATURE VALUE TYPE---------------------------------------------------------------*/

        public async Task<Response<List<Models.FeatureValueType>>> GetFeatureValueType(Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetFeatureValueType(userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FeatureValueType>>> _GetFeatureValueType(Int32 userId)
        {

            Response<List<Models.FeatureValueType>> _response = new Response<List<Models.FeatureValueType>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter(); 
                _parameter.AddSqlParameter("@IDUSER", userId); 

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME"); 

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATUREVALUETYPE", _parameter);
                _response.Data = _data.GetList<Models.FeatureValueType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-------------------------------------------------------------FEATURE OPTION---------------------------------------------------------------*/

        public async Task<Response<List<Models.FeatureOption>>> GetFeatureOption(Int32 userId, Int32 featureId, bool? active)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetFeatureOption(userId, featureId, active);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.FeatureOption>>> _GetFeatureOption(Int32 userId, Int32 featureId, bool? active)
        {

            Response<List<Models.FeatureOption>> _response = new Response<List<Models.FeatureOption>>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDFEATURE", featureId);
                _parameter.AddSqlParameter("@BACTIVE", active);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");  
                _mapping.AddItem("FeatureId", "IDFEATURE");
                _mapping.AddItem("OrderBy", "IORDER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_FEATUREOPTION", _parameter);
                _response.Data = _data.GetList<Models.FeatureOption>(_mapping, _table);
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
        /*---------------------------------------------------------------POST FEATURE OPTION---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostFeatureOption(List<Models.FeatureOption> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostFeatureOption(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostFeatureOption(List<Models.FeatureOption> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_FEATUREOPTION", _parameter);
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

        #region "INSPECTION"

        /* ----------------------------- GET ALL -------------------------------*/
        public async Task<Response<List<Models.TableInspection>>> TableGetAllInspections(Int32? supplierId, Int32? rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _TableGetAllInspections(supplierId, rowfrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        /* ----------------------------- GET ALL -------------------------------*/
        public async Task<Response<List<Models.Inspection>>> GetAllInspections(Int32? AreaId, bool? IsCompleted)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllInspections(AreaId, IsCompleted);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /* ----------------------------- GET ONE -------------------------------*/
        public async Task<Response<Models.Inspection>> GetOneInspection(Int32 InspectionId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOneInspection(InspectionId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /* ----------------------------- POST INSPECTION -------------------------------*/

        public async Task<Result> Post_Inspections(List<Inspection> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Inspections(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /* ----------------------------- GET DEALERS -------------------------------*/

        public async Task<Response<List<Models.DealersByInspection>>> InspectionGetDealers(string ids)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _InspectionGetDealers(ids);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.TableInspection>>> _TableGetAllInspections(Int32? supplierId, Int32? rowfrom, string? filter)
        {
            Response<List<Models.TableInspection>> _response = new Response<List<Models.TableInspection>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("InspectionId", "IDINSPECTION");
                _mapping.AddItem("CreatedDate", "DCREATED");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Model", "MODEL");
                _mapping.AddItem("Area", "AREA");
                _mapping.AddItem("User", "VUSER");
                _mapping.AddItem("Isclosed", "IS_CLOSED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INSPECTIONS_ALL", _parameter);
                _response.Data = _data.GetList<Models.TableInspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        private async Task<Response<List<Models.Inspection>>> _GetAllInspections(Int32? AreaId, bool? IsCompleted)
        {
            Response<List<Models.Inspection>> _response = new Response<List<Models.Inspection>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDAREA", AreaId);
                _parameter.AddSqlParameter("@ISCOMPLETED", IsCompleted);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("CreatedBy", "ICREATEDBY");
                _mapping.AddItem("UserName", "USERNAME");
                _mapping.AddItem("Created", "DCREATED");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("VehiclePlate", "PLATE");
                _mapping.AddItem("Lote", "LOTE");
                _mapping.AddItem("Vin", "VIN");
                _mapping.AddItem("Model", "MODEL");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("NameArea", "AREA");
                _mapping.AddItem("DInit", "DDATEINIT");
                _mapping.AddItem("InitByName", "INITBYNAME");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("DClose", "DDATECLOSE");
                _mapping.AddItem("ClosedByName", "CLOSEDBYNAME");
                _mapping.AddItem("DReception", "DRECEPTION");
                _mapping.AddItem("RecepByName", "RECEPBYNAME");
                _mapping.AddItem("TransporterName", "TRANSPORTER_NAME");
                _mapping.AddItem("IsCompleted", "ISCOMPLETED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INSPECTIONS", _parameter);
                _response.Data = _data.GetList<Models.Inspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.Inspection>> _GetOneInspection(Int32 InspectionId)
        {
            Response<Models.Inspection> _response = new Response<Models.Inspection>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", InspectionId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("CreatedBy", "ICREATEDBY");
                _mapping.AddItem("UserName", "USERNAME");
                _mapping.AddItem("Created", "DCREATED");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("VehiclePlate", "PLATE");
                _mapping.AddItem("Lote", "LOTE");
                _mapping.AddItem("Vin", "VIN");
                _mapping.AddItem("Model", "MODEL");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("NameArea", "AREA");
                _mapping.AddItem("DInit", "DDATEINIT");
                _mapping.AddItem("InitByName", "INITBYNAME");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("DClose", "DDATECLOSE");
                _mapping.AddItem("ClosedByName", "CLOSEDBYNAME");
                _mapping.AddItem("DReception", "DRECEPTION");
                _mapping.AddItem("RecepByName", "RECEPBYNAME");
                _mapping.AddItem("TransporterName", "TRANSPORTER_NAME");
                _mapping.AddItem("IsCompleted", "ISCOMPLETED");
                _mapping.AddItem("HasFiles", "BHASTTACHMENTS");
                _mapping.AddItem("Brand", "VBRAND");
                _mapping.AddItem("Color", "COLOR");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("ClosedByVVTA", "CLOSEDBYVVTA");
                _mapping.AddItem("RecepByVVTA", "RECEPBYVVTA");
                _mapping.AddItem("TransporterByVVTA", "TRANSPORTERVVTA");
                _mapping.AddItem("NameSupplier", "NAMESUPPLIER");
                _mapping.AddItem("Format", "VFORMAT");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("BrandId", "IDBRAND");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INSPECTION_BY_ID", _parameter);
                _response.Data = _data.GetItem<Models.Inspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Result> _Post_Inspections(List<Inspection> _list)
        {
            List<Result> _results = new List<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_INSPECTION", _mapping, _parameter);
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _results[0];
        }

        private async Task<Response<List<Models.DealersByInspection>>> _InspectionGetDealers(string ids)
        {
            Response<List<Models.DealersByInspection>> _response = new Response<List<Models.DealersByInspection>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@INSPECTIONSID", ids);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("InspectionId", "ID");
                _mapping.AddItem("VehicleId", "VEHICLEID");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALERNAME");

          

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DEALERSLIST_BYINSPECTION", _parameter);
                _response.Data = _data.GetList<Models.DealersByInspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        #endregion

        #region "INSPECTIONDETAILS"

        public async Task<Response<List<Models.InspectionDetail>>> GetAllInspectionsDetails(Int32? InspectionId, Int32? AreaId, Int32? FaseId, Int32? FeatureTypeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllInspectionDetail(InspectionId, AreaId, FaseId, FeatureTypeId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.InspectionDetail>> GetOneInspectionsDetails(Int32 InspectionDetailId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOneInspectionsDetail(InspectionDetailId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> Post_InspectionDetails(List<InspectionDetail> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_InspectionDetails(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.InspectionDetail>>> _GetAllInspectionDetail(Int32? InspectionId, Int32? AreaId, Int32? FaseId, Int32? FeatureTypeId)
        {
            Response<List<Models.InspectionDetail>> _response = new Response<List<Models.InspectionDetail>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDINSPECTION", InspectionId);
                _parameter.AddSqlParameter("@IDAREA", AreaId);
                _parameter.AddSqlParameter("@IDFASE", FaseId);
                _parameter.AddSqlParameter("@IDFEATURETYPE", FeatureTypeId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Value", "BVALUE");
                _mapping.AddItem("Observation", "VOBSERVATION");
                _mapping.AddItem("FileUrl", "VFILEURL");
                _mapping.AddItem("InspectionId", "IDINSPECTION");
                _mapping.AddItem("FeatureId", "IDFEATURE");
                _mapping.AddItem("Feature", "VFEATURE");
                _mapping.AddItem("FeatureTypeId", "IDFEATURETYPE");
                _mapping.AddItem("FeatureType", "VFEATURETYPE");
                _mapping.AddItem("FeatureValueTypeId", "IDFEATUREVALUETYPE");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("Fase", "VFASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("Area", "VAREA");
                _mapping.AddItem("Color", "VCOLOR");
                _mapping.AddItem("Model", "VMODEL");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("HasFiles", "BHASTTACHMENTS");
                _mapping.AddItem("OptionSelected", "OPTIONSELECT");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INSPECTIONS_DETAIL", _parameter);
                _response.Data = _data.GetList<Models.InspectionDetail>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.InspectionDetail>> _GetOneInspectionsDetail(Int32 InspectionDetailId)
        {
            Response<Models.InspectionDetail> _response = new Response<Models.InspectionDetail>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", InspectionDetailId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Value", "BVALUE");
                _mapping.AddItem("Observation", "VOBSERVATION");
                _mapping.AddItem("FileUrl", "VFILEURL");
                _mapping.AddItem("InspectionId", "IDINSPECTION");
                _mapping.AddItem("FeatureId", "IDFEATURE");
                _mapping.AddItem("Feature", "VFEATURE");
                _mapping.AddItem("FeatureTypeId", "IDFEATURETYPE");
                _mapping.AddItem("FeatureType", "VFEATURETYPE");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("Fase", "VFASE");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("Area", "VAREA");
                _mapping.AddItem("Color", "VCOLOR");
                _mapping.AddItem("Model", "VMODEL");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("HasFiles", "BHASTTACHMENTS");
                _mapping.AddItem("OptionSelected", "OPTIONSELECT");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INSPECTION_DETAIL_BY_ID", _parameter);
                _response.Data = _data.GetItem<Models.InspectionDetail>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Result> _Post_InspectionDetails(List<InspectionDetail> _list)
        {
            List<Result> _results = new List<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_INSPECTION_DETAIL", _mapping, _parameter);
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _results[0];
        }
        #endregion

        #region "INSPECTIONFASE"
        public async Task<Response<List<Models.InspectionFase>>> GetAllInspectionFase(Int32? AreaId, Int32? FaseId, Int32? InspectionId, bool? IsCompleted, bool? IsCompletedInspection)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllInspectionFase(AreaId, FaseId, InspectionId, IsCompleted, IsCompletedInspection);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result> Post_InspectionFase(List<InspectionFase> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_InspectionFase(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.InspectionFase>>> _GetAllInspectionFase(Int32? AreaId, Int32? FaseId, Int32? InspectionId, bool? IsCompleted, bool? IsCompletedInspection)
        {
            Response<List<Models.InspectionFase>> _response = new Response<List<Models.InspectionFase>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDAREA", AreaId);
                _parameter.AddSqlParameter("@IDFASE", FaseId);
                _parameter.AddSqlParameter("@IDINSPECTION", InspectionId);
                _parameter.AddSqlParameter("@ISCOMPLETED", IsCompleted);
                _parameter.AddSqlParameter("@ISCOMPLETEDINSPECTION", IsCompletedInspection);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("InspectionId", "IDINSPECTION");
                _mapping.AddItem("FaseId", "IDFASE");
                _mapping.AddItem("Fase", "FASE");
                _mapping.AddItem("CompletedDate", "DCOMPLETED");
                _mapping.AddItem("InitDate", "DDATEINIT");
                _mapping.AddItem("IsCompleted", "ISCOMPLETED");
                _mapping.AddItem("UserInitId", "IIDUSERINIT");
                _mapping.AddItem("AreaId", "IDAREA");
                _mapping.AddItem("Area", "AREA");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("Completed", "ISCOMPLETEDINSPECTION");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INPECTIONFASE", _parameter);
                _response.Data = _data.GetList<Models.InspectionFase>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Result> _Post_InspectionFase(List<InspectionFase> _list)
        {
            List<Result> _results = new List<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_INSPECTION_FASE", _mapping, _parameter);
            }
            catch (Exception ex)
            {
            }

            return _results[0];
        }
        #endregion

        #region "FULLINSPECTION"
        public async Task<Result> Post_FullInspection(List<FullInspection> _list, Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_FullInspection(_list, userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Result> _Post_FullInspection(List<FullInspection> _list, Int32 userId, Int32 supplierId)
        {
            List<Result> _results = new List<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_GENERATE_FULL_INSPECTION", _mapping, _parameter);
            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _results[0];
        }
        #endregion
    }
}
