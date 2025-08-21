using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Models;
using Util;
using static Models.StaticEnum;

namespace Data
{
    public class dInventory
    {
        private readonly dContact _dContact;

        private readonly SemaphoreSlim _semaphore;
        public dInventory(dContact dContact)
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
            _dContact = dContact;
        }

        public async Task<Response> GetAll(Int32 userId, Int32 supplierId, Int32 rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId,rowfrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetAll(Int32 userId, Int32 supplierId,  Int32 rowfrom, string? filter )
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                
                _mapping.AddItem("LocationId", "IDLOCATION");
                _mapping.AddItem("LocationName", "VLOCATION");

                _mapping.AddItem("ZoneId", "IDZONE");
                _mapping.AddItem("ZoneName", "VZONE");

                _mapping.AddItem("WarehouseId", "IDWAREHOUSE");
                _mapping.AddItem("WarehouseName", "VWAREHOUSE");

                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("Stock", "ISTOCK");
                _mapping.AddItem("Price", "NPRICE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INVENTORIES", _parameter);
                _response.Data = _data.GetList<Models.Inventory>(_mapping, _table);
                _response.SetGetResponse(_table);

             

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> GetMovements(Int32 userId, Int32 supplierId, string typeId, Int32 rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovements(userId, supplierId,typeId, rowfrom, filter, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetMovement(Int32 userId, Int32? movementId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovements(userId,null,null, null, null, movementId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetMovementDetails(Int32 userId, Int32 movementId, Int32? detailId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovementDetails(userId, movementId, detailId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetMovementDetailsByUser(Int32 userId, string movementType, string mode)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovementDetailsByUser(userId,  movementType,mode);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetValidLocation(Int32 movementId, string scannedText)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getValidLocation(movementId, scannedText);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _getMovements(Int32 userId, Int32? supplierId, string? typeId, Int32? rowfrom, string? filter, Int32? movementId = null)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VTYPE", typeId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", movementId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("GuideNumber", "VGUIDENUMBER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierReference", "VSUPPLIER");
                _mapping.AddItem("SupplierName", "VNAMESUPPLIER");
                _mapping.AddItem("ContactId", "IDCONTACT");
                _mapping.AddItem("ContactName", "VCONTACT");
                _mapping.AddItem("StatusId", "IESTATUS");
                _mapping.AddItem("StatusName", "VESTATUS");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("TypeId", "VTYPE");
                _mapping.AddItem("TypeName", "VMOVEMENTTYPE");
                _mapping.AddItem("Created", "VCREATED");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("CreatedBy", "VCREATEDBY");
                _mapping.AddItem("ManagerName", "VNAMEMANAGER");
                _mapping.AddItem("ManagerLastName", "VLASTNAMEMANAGER");
                _mapping.AddItem("AssignTo", "VASSIGNED");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MOVEMENTS", _parameter);
                _response.Data = (movementId == null) ? _data.GetList<Models.Movement>(_mapping, _table) : _data.GetItem<Models.Movement>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getMovementDetails(Int32 userId, Int32 movementId, Int32? detailId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
               // _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDMOVEMENT", movementId);
                _parameter.AddSqlParameter("@ID", detailId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("MovementId", "IDMOVEMENT");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("SerialCode", "VSERIAL");
                _mapping.AddItem("LocationType", "VLOCATIONTYPE");
                _mapping.AddItem("LocationId", "IDLOCATION");
                _mapping.AddItem("LocationName", "VLOCATION");
                _mapping.AddItem("DestinationId", "IDDESTINATION");
                _mapping.AddItem("DestinationName", "VDESTINATION");
                _mapping.AddItem("RequiredQty", "IREQUIRED");
                _mapping.AddItem("RealQty", "IREAL");

                _mapping.AddItem("TypeId", "VTYPE");
                _mapping.AddItem("TypeName", "VTYPEMOVEMENT");

                _mapping.AddItem("Processed", "BPROCESSED");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartDescription", "VPART");
              
              
                _mapping.AddItem("UpdatedDate", "VUPDATED");


                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("UserName", "VLOGIN");
                _mapping.AddItem("Mapping", "IMAPPING");

                _mapping.AddItem("Cost", "NCOST");
                _mapping.AddItem("SubTotal", "NSUBTOTAL");
                _mapping.AddItem("Stock", "ISTOCK");
                


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MOVEMENTDETAILS", _parameter);
                _response.Data = _data.GetList<Models.MovementDetails>(_mapping, _table);
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getMovementDetailsByUser(Int32 userId, string movementType, string mode)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@VTYPE", movementType);
                _parameter.AddSqlParameter("@VMODE", mode);
                

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("MovementId", "IDMOVEMENT");
                _mapping.AddItem("TypeId", "VTYPE");
                _mapping.AddItem("TypeName", "VTYPEMOVEMENT");

                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("SerialCode", "VSERIAL");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartDescription", "VPART");
                _mapping.AddItem("PartBarcode", "VBARCODE");
                _mapping.AddItem("Serializable", "BSERIALIZABLE");

                _mapping.AddItem("LocationId", "IDLOCATION");
                _mapping.AddItem("LocationType", "VLOCATIONTYPE");
                _mapping.AddItem("LocationName", "VLOCATION");
                _mapping.AddItem("Mapping", "IMAPPING");
                
                _mapping.AddItem("RequiredQty", "IREQUIRED");
                _mapping.AddItem("RealQty", "IREAL");
   
                _mapping.AddItem("Processed", "BPROCESSED");
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("UserName", "VLOGIN");
                _mapping.AddItem("Stock", "ISTOCK");



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MOVEMENTDETAILS_GROUPED", _parameter);
                _response.Data = _data.GetList<Models.MovementDetails>(_mapping, _table);
                _response.SetGetResponse(_table,true);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getValidLocation(Int32 movementId, string scannedText)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                // _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", movementId);
                _parameter.AddSqlParameter("@SCANNED", scannedText);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "LOCATIONID");
                _mapping.AddItem("Name", "LOCATIONAME");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VALIDLOCATION", _parameter);
                _response.Data = _data.GetItem<Models.Location>(_mapping, _table);
                _response.SetGetResponse(_table, true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<List<Inventory>> GetExport(string? _filter, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetExport(_filter, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<Inventory>> _GetExport(string? _filter, Int32 userId)
        {
            List<Inventory> _list = new List<Inventory>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VFILTER", _filter);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@ID", null);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");

                _mapping.AddItem("LocationId", "LOCATION_ID");
                _mapping.AddItem("LocationName", "LOCATION_NAME");

                _mapping.AddItem("ZoneId", "ZONE_ID");
                _mapping.AddItem("ZoneName", "ZONE_NAME");

                _mapping.AddItem("WarehouseId", "WAREHOUSE_ID");
                _mapping.AddItem("WarehouseName", "WAREHOUSE_NAME");

                _mapping.AddItem("SupplierId", "IDPLANTA");

                _mapping.AddItem("PartId", "PART_ID");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("PartDesc", "PART_DESC");
                _mapping.AddItem("Stock", "ISTOCK");

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.Inventory>("USP_GET_INVENTORIES_EXPORT", _mapping, _parameter);



            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _list;
        }

        public async Task<Response> PostMovements(List<Models.Movement> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostMovements(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> PostMovementDetail(List<Models.NewMovementDetail> _list, Int32 userId, bool isImport = false)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostMovementDetail(_list, userId,isImport);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> PostMovementDetailMobile(Models.MovementDetails movementDetail, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostMovementDetailMobile(movementDetail, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _PostMovements(List<Models.Movement> _list, Int32 userId)
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
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_MOVEMENTS", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _PostMovementDetail(List<Models.NewMovementDetail> _list, Int32 userId, bool  isImport = false)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BIMPORT", isImport);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_MOVEMENTDETAILS", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _PostMovementDetailMobile(Models.MovementDetails movementDetail, Int32 userId)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(movementDetail);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_MOVEMENTDETAILS_GROUPED", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> Post_Actions(List<Models.Action> _list, Int32 userId)
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

        public async Task<Response> DeleteMovementDetail(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _deleteMovementDetail(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _Post_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_MOVEMENTS_ACTIONS", _mapping, _parameter);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        private async Task<Response> _deleteMovementDetail(List<Models.Action> _list, Int32 userId)
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
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_DELETE_MOVEMENTDETAIL", _mapping, _parameter);
                _response.SetPostResponse();

                
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        private async Task<Response> _postGuide(Models.Guide guide)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(guide);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
 
                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_GUIDES", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _postPackage(Models.Package package)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(package);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_PACKAGES", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _addPackge(string packageCode , Int32 guideId)
        {
            Response _response = new Response();
            try
            {


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VCODE", packageCode);
                _parameter.AddSqlParameter("@IDGUIDE", guideId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_GUIDE_ADDPACKGE", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _deletePackge(string packageCode, Int32 guideId)
        {
            Response _response = new Response();
            try
            {


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VCODE", packageCode);
                _parameter.AddSqlParameter("@IDGUIDE", guideId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_GUIDE_DELETEPACKGE", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _postPackageDetail(Models.PackageDetail packageDetail)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(packageDetail);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_PACKAGEDETAIL", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response> PostPackageDetail(Models.PackageDetail packageDetail)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postPackageDetail(packageDetail);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> AddPackge(string packageCode, Int32 guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _addPackge(packageCode,guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> DeletePackge(string packageCode, Int32 guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _deletePackge(packageCode,guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        public async Task<Response> PostGuide(Models.Guide guide)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postGuide(guide);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> PostPackage(Models.Package package)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postPackage(package);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _getCustomersForPacking(Int32? supplierId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VFIRSTNAME");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CUSTOMERS_FORPACKING", _parameter);
                _response.Data = _data.GetList<Models.Customer>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getProviderToDeliver()
        {
            Response _response = new Response();
            try
            {

 
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VFIRSTNAME");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PROVIDERS_TODELIVERY");
                _response.Data = _data.GetList<Models.Provider>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        private async Task<Response> _getGuides(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDCONTACT", customerId);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("ProviderId", "IDPROVIDER");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("Delivered", "BDELIVERED");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_GUIDES_PENDING", _parameter);
                _response.Data = _data.GetList<Models.Guide>(_mapping, _table);
                _response.SetGetResponse(_table,true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getPackages(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDCUSTOMER", customerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Number", "INUMBER");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("Weight", "NWEIGHT");
                _mapping.AddItem("Closed", "BCLOSED");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGES", _parameter);
                _response.Data = _data.GetList<Models.Package>(_mapping, _table);
                _response.SetGetResponse(_table,true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getPackagesFromGuide(Int32 guideId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDGUIDE", guideId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Number", "INUMBER");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("Weight", "NWEIGHT");
                _mapping.AddItem("Closed", "BCLOSED");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGES_FROMGUIDE", _parameter);
                _response.Data = _data.GetList<Models.Package>(_mapping, _table);
                _response.SetGetResponse(_table, true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response> _getPackageDetails(Int32 packageId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDPACKAGE", packageId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("PackageId", "IDPACKAGE");
                _mapping.AddItem("PackageCode", "VPACKAGECODE");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartName", "VDESCRIPTION");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("PartBarCode", "VBARCODE");
                _mapping.AddItem("Quantity", "IQUANTITY");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGEDETAIL", _parameter);
                _response.Data = _data.GetList<Models.PackageDetail>(_mapping, _table);
                _response.SetGetResponse(_table, true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response> _getPackageDetailsPending(Int32 packageId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDPACKAGE", packageId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("PackageId", "IDPACKAGE");
                _mapping.AddItem("PackageCode", "VPACKAGECODE");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartName", "VDESCRIPTION");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("PartBarCode", "VBARCODE");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("Pending", "IPENDING");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGEDETAILPENDING", _parameter);
                _response.Data = _data.GetList<Models.PackageDetail>(_mapping, _table);
                _response.SetGetResponse(_table, true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response> _getPackageDetailByPart(Int32 packageId, string scanCode)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDPACKAGE", packageId);
                _parameter.AddSqlParameter("@VPART", scanCode);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("PackageId", "IDPACKAGE");
                _mapping.AddItem("PackageCode", "VPACKAGECODE");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartName", "VDESCRIPTION");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("PartBarCode", "VBARCODE");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("Pending", "IPENDING");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGEDETAIL_BYPART", _parameter);
                _response.Data = _data.GetList<Models.PackageDetail>(_mapping, _table);
                _response.SetGetResponse(_table, true);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        public async Task<Response> GetCustomersForPacking(Int32? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getCustomersForPacking(supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetProviderToDeliver()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getProviderToDeliver();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetGuides(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getGuides(supplierId, customerId, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetPackages(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getPackages(supplierId, customerId, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetPackagesFromGuide(Int32 guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getPackagesFromGuide(guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        

        public async Task<Response> GetPackageDetails(Int32 packageId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getPackageDetails(packageId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetPackageDetailByPart(Int32 packageId,string scanCode)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getPackageDetailByPart(packageId, scanCode);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetPackageDetailsPending(Int32 packageId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getPackageDetailsPending(packageId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetAllGuides(Int32 userId, Int32 supplierId, Int32 rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllGuides(userId, supplierId, rowfrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response> GetOneGuide(Int32 userId, Int32 supplierId ,int guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllGuides(userId, supplierId, 0, "", guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetAllGuides(Int32 userId, Int32 supplierId, Int32 rowfrom, string? filter = "", int? guideId=null)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IDGUIDE", guideId); //guideId es null

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");

                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");

                _mapping.AddItem("ProviderId", "IDPROVIDER");
                _mapping.AddItem("ProviderName", "VPROVIDER");

                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("Number", "VNUMBER");

                _mapping.AddItem("CreatedDate", "DCREATEDDATE");
                _mapping.AddItem("DeliveredDate", "DDELIVERYDATE");
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");




                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_GUIDES", _parameter);
                _response.Data = _data.GetList<Models.Guide>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> PostDeliverGuides(List<Models.DeliveredGuides> guides)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostDeliverGuides(guides);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _PostDeliverGuides(List<DeliveredGuides> guides)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(guides);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_DELIVERED_GUIDES", _mapping, _parameter);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response> _GetGuideDetails(int guideId)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDGUIDE", guideId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartInnercode", "VINNERCODE");
                _mapping.AddItem("PartDescription", "VDESCRIPTION");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("PackageId", "IDPACKAGE");
                _mapping.AddItem("PackageNumber", "INUMBER");
                _mapping.AddItem("PackageCode", "VCODE");



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_GUIDES_DETAILS", _parameter);
                _response.Data = _data.GetList<Models.GuideDetails>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> GetGuideDetails(int guideid)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetGuideDetails(guideid);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetPartialTypes()
        {
            Response _response = new Response();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PARTIALTYPES");
                _response.Data = _data.GetList<Models.PartialType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> GetPartialTypes()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPartialTypes();
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _GetBackOrders(int userId, int supplierId, int? rowfrom, string? filter)
        {
            Response _response = new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDBACKORDER");
                _mapping.AddItem("PartDescription", "VDESCRIPTION");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Quantity", "IBACKORDER");
                _mapping.AddItem("StatusId", "IDSTATUS");
                _mapping.AddItem("CreatedDate", "DCREATED");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALER");
                _mapping.AddItem("SaleOrderNumber", "IDSALEORDER");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("TypeName", "VTYPE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_BACKORDER", _parameter);
                _response.Data = _data.GetList<Models.BackOrder>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response> GetBackOrders(int userId, int supplierId, int? rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetBackOrders(userId, supplierId, rowfrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response> PostBackorder_Actions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostBackorder_Actions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _PostBackorder_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();

                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_BACKORDER_ACTIONS", _mapping, _parameter);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> PostBacKOrder(Models.PostBackOrder backOrder, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostBacKOrder(backOrder, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response> _PostBacKOrder(Models.PostBackOrder backOrder, Int32 userId)
        {
            Response _response = new Response();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(backOrder);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                _response.Data = await _data.ExecuteReaderAsync<Result>("USP_POST_BACKORDER", _mapping, _parameter);
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
