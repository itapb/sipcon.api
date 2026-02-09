using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
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


        public async Task<Response<List<Inventory>>> GetAll(Int32 userId, Int32 supplierId, Int32? rowfrom, string? filter, bool? withStock = true, string? locationType = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId, rowfrom, filter, withStock, locationType);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<Inventory>> GetExport(Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Inventory>)(await _GetAll(userId, supplierId, null, null, null, null)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Movement>> getLast(Int32? userId, Int32? supplierId, string typeId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getlast(userId, supplierId, typeId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Adjustment>> getLastAdjustment(Int32? userId, Int32? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getlastAdjustment(userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Movement>> _getlast(Int32? userId, Int32? supplierId, string typeId)
        {
            Response<Movement> _response = new Response<Movement>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VTYPE", typeId);

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
                DataTable _table = await _data.GetDataTable("USP_GET_LASTMOVEMENT", _parameter);
                _response.Data = _data.GetItem<Models.Movement>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<Adjustment>> _getlastAdjustment(Int32? userId, Int32? supplierId)
        {
            Response<Adjustment> _response = new Response<Adjustment>();
            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("DCreated", "DCREATED");
                _mapping.AddItem("DUpdated", "DUPDATED");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("StatusId", "IDSTATUS");
                _mapping.AddItem("StatusName", "VSTATUS");
                _mapping.AddItem("UserLogin", "VLOGIN");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LASTADJUSTMENT", _parameter);
                _response.Data = _data.GetItem<Models.Adjustment>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<List<Inventory>>> _GetAll(Int32 userId, Int32 supplierId, Int32? rowfrom, string? filter, bool? withStock = true, string? locationType = null)
        {
            Response<List<Inventory>> _response = new Response<List<Inventory>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@WITHSTOCK", withStock);
                _parameter.AddSqlParameter("@VTYPE", locationType);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");

                _mapping.AddItem("LocationId", "IDLOCATION");
                _mapping.AddItem("LocationName", "VLOCATION");

                _mapping.AddItem("ZoneId", "IDZONE");
                _mapping.AddItem("ZoneName", "VZONE");
                _mapping.AddItem("ZoneSize", "VZONESIZE");

                _mapping.AddItem("WarehouseId", "IDWAREHOUSE");
                _mapping.AddItem("WarehouseName", "VWAREHOUSE");

                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartSize", "VPARTSIZE");
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

        public async Task<Response<List<Movement>>> GetMovements(Int32 userId, Int32 supplierId, string typeId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovements(userId, supplierId, typeId, rowfrom, filter, fromDate, upToDate, estatusId, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /*-------------------------------------------------------------- Get Export Reception   ---------------------------------------------------------------*/

        public async Task<List<Movement>> GetExportMovementsReception(Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Movement>)(await _getMovements(userId, supplierId, "R", null, null,null,null,null)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*-------------------------------------------------------------- Get Export relocation   ---------------------------------------------------------------*/

        public async Task<List<Movement>> GetExportMovementsRelocation(Int32 userId, Int32 supplierId, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Movement>)(await _getMovements(userId, supplierId, "T", null,null, fromDate, upToDate, estatusId, null)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*------------------------------------------------------------------------------------------------------------------------------------*/

        public async Task<Response<Movement>> GetMovement(Int32 userId, Int32? movementId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovement(userId, null, null, null, null, movementId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<MovementDetails>>> GetMovementDetails(Int32 userId, Int32 movementId, Int32? detailId)
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

        public async Task<Response<List<MovementDetails>>> GetMovementDetailsByUser(Int32 userId, Int32? supplierId ,string movementType, string mode)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovementDetailsByUser(userId,supplierId, movementType, mode);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Location>> GetValidLocation(Int32 movementId, string scannedText)
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

        public async Task<Response<List<MovementDetails>>> GetMovementDetailsPicking(Int32 userId, Int32? supplierId, Int32? dealerId, Int32? rowfrom, string? filter, bool? pending, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovementDetailsPicking(userId, supplierId, dealerId, rowfrom, filter, pending, fromDate, upToDate, estatusId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Movement>>> _getMovements(Int32 userId, Int32? supplierId, string? typeId, Int32? rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId, Int32? movementId = null)
        {
            Response<List<Movement>> _response = new Response<List<Movement>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VTYPE", typeId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", movementId);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IESTATUS", estatusId);

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
                _response.Data = _data.GetList<Models.Movement>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<Movement>> _getMovement(Int32 userId, Int32? supplierId, string? typeId, Int32? rowfrom, string? filter, Int32? movementId = null)
        {
            Response<Movement> _response = new Response<Movement>();
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
                _response.Data = _data.GetItem<Models.Movement>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<List<MovementDetails>>> _getMovementDetailsPicking(Int32 userId, Int32? supplierId, Int32? dealerId, Int32? rowfrom, string? filter, bool? pending, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            Response<List<MovementDetails>> _response = new Response<List<MovementDetails>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VTYPE", "P");
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BPENDING", pending);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IESTATUS", estatusId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("MovementId", "IDMOVEMENT");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartDescription", "VPART");
                _mapping.AddItem("LocationName", "VLOCATION");
                _mapping.AddItem("RequiredQty", "IREQUIRED");
                _mapping.AddItem("RealQty", "IREAL");
                _mapping.AddItem("TypeName", "VMOVEMENTTYPEDETAIL");
                _mapping.AddItem("Processed", "BPROCESSED");
                _mapping.AddItem("UserName", "VLOGIN");
                _mapping.AddItem("Stock", "ISTOCK");
                _mapping.AddItem("SaleOrderId", "VIDSALEORDER"); 
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("GeneratedDate", "DGENERATED");
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");
                



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MOVEMENTDETAILSMAIN", _parameter);
                _response.Data = _data.GetList<Models.MovementDetails>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        private async Task<Response<List<MovementDetails>>> _getMovementDetails(Int32 userId, Int32 movementId, Int32? detailId)
        {
            Response<List<MovementDetails>> _response = new Response<List<MovementDetails>>();
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
                _mapping.AddItem("PartSize", "VPARTSIZE");
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
                 
                _mapping.AddItem("PartialTypeId", "IDPARTIALTYPE");
                _mapping.AddItem("PartialTypeName", "VPARTIALTYPE");

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

        private async Task<Response<List<MovementDetails>>> _getMovementDetailsByUser(Int32 userId, Int32? supplierId ,string movementType, string mode)
        {
            Response<List<MovementDetails>> _response = new Response<List<MovementDetails>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
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
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        //---------------------------------------------------------------------------------------------------

        public async Task<Response<List<MovementDetails>>> GetMovementDetailsTransfer(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, bool? pending)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getMovementDetailsTransfer(userId, supplierId, rowfrom, filter, pending);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<List<MovementDetails>>> _getMovementDetailsTransfer(Int32 userId, Int32? supplierId, Int32? rowfrom, string? filter, bool? pending)
        {
            Response<List<MovementDetails>> _response = new Response<List<MovementDetails>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VTYPE", "T");
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BPENDING", pending);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("MovementId", "IDMOVEMENT");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartDescription", "VPART");
                _mapping.AddItem("LocationName", "VLOCATION");
                _mapping.AddItem("DestinationName", "VDESTINATION");
                _mapping.AddItem("RequiredQty", "IREQUIRED");
                _mapping.AddItem("RealQty", "IREAL");
                _mapping.AddItem("TypeName", "VMOVEMENTTYPEDETAIL");
                _mapping.AddItem("Processed", "BPROCESSED");
                _mapping.AddItem("UserName", "VLOGIN");
                _mapping.AddItem("Stock", "ISTOCK");
                _mapping.AddItem("GeneratedDate", "DGENERATED");
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");




                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_TRANSFERMOVEMENTDETAILS", _parameter);
                _response.Data = _data.GetList<Models.MovementDetails>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<Result>> PostMovementDetailsTransferActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postMovementDetailsTransferActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _postMovementDetailsTransferActions(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_MOVEMENTDETAIL_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //---------------------------------------------------------------------------------------------------
        private async Task<Response<Models.Location>> _getValidLocation(Int32 movementId, string scannedText)
        {
            Response<Models.Location> _response = new Response<Models.Location>();
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
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*   public async Task<List<Inventory>> GetExport(string? _filter, Int32 userId)
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
           }*/

        public async Task<Response<Result>> PostMovements(List<Models.Movement> _list, Int32 userId)
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

        public async Task<Response<Result>> PostMovementDetail(List<Models.NewMovementDetail> _list, Int32 userId, bool isImport = false)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostMovementDetail(_list, userId, isImport);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Result>> PostMovementDetailMobile(Models.MovementDetails movementDetail, Int32 userId)
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


        private async Task<Response<Result>> _PostMovements(List<Models.Movement> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_MOVEMENTS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Result>> _PostMovementDetail(List<Models.NewMovementDetail> _list, Int32 userId, bool isImport = false)
        {
            Response<Result> _response = new Response<Result>();
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
                DataTable _table = await _data.GetDataTable("USP_POST_MOVEMENTDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Result>> _PostMovementDetailMobile(Models.MovementDetails movementDetail, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(movementDetail);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_MOVEMENTDETAILS_GROUPED", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response<Result>> Post_Actions(List<Models.Action> _list, Int32 userId)
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

        public async Task<Response<Result>> DeleteMovementDetail(List<Models.Action> _list, Int32 userId)
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


        private async Task<Response<Result>> _Post_Actions(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_MOVEMENTS_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Result>> PostMovementDetailsPickingActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postMovementDetailsPickingActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _postMovementDetailsPickingActions(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_MOVEMENTDETAIL_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        private async Task<Response<Result>> _deleteMovementDetail(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_DELETE_MOVEMENTDETAIL", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        private async Task<Response<Result>> _postGuide(Models.Guide guide, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(guide);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_GUIDES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //public async Task<Response<Result>> PostClaim(Int32 guideId, Int32 userId)
        //{
        //    await _semaphore.WaitAsync(Util.Setting.TimeOut);
        //    try
        //    {
        //        return await _postClaim(guideId, userId);
        //    }
        //    finally
        //    {
        //        _semaphore.Release();
        //    }
        //}

        //private async Task<Response<Result>> _postClaim(Int32 guideId, Int32 userId)
        //{
        //    Response<Result> _response = new Response<Result>();
        //    try
        //    {
        //        Util.Parameter _parameter = new Util.Parameter();
        //        _parameter.AddSqlParameter("@IDGUIDE", guideId);   
        //        _parameter.AddSqlParameter("@IDUSER", userId);

        //        Mapping _mapping = new Mapping();
        //        _mapping.SetDefaultPostMapping();

        //        Util.Data _data = Util.Data.GetInstance();
        //        DataTable _table = await _data.GetDataTable("USP_POST_CLAIM", _parameter);   
        //        _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
        //        _response.SetPostResponse();
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.SetError(ex);
        //    }
        //    return _response;
        //}
        //-------------------------------------------------------------------------CLAIM
        private async Task<Response<Result>> _postPackage(Models.Package package)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(package);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PACKAGES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        private async Task<Response<Result>> _addPackge(string packageCode, Int32 guideId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VCODE", packageCode);
                _parameter.AddSqlParameter("@IDGUIDE", guideId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_GUIDE_ADDPACKGE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        private async Task<Response<Result>> _openPackage(int packageId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDPACKAGE", packageId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PACKAGE_OPEN", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        private async Task<Response<Result>> _deletePackge(string packageCode, Int32 guideId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@VCODE", packageCode);
                _parameter.AddSqlParameter("@IDGUIDE", guideId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_GUIDE_DELETEPACKGE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Result>> _postPackageDetail(Models.PackageDetail packageDetail)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(packageDetail);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PACKAGEDETAIL", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Result>> PostPackageDetail(Models.PackageDetail packageDetail)
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
        /*------------------------------------------------------------------------------------------------------------------------------------------*/
         
        private async Task<Response<Result>> _postReplicatePackage(int packageId, int replicationCount)
        {
            Response<Result> _response = new Response<Result>();
            try
            { 
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDPACKAGE", packageId);
                _parameter.AddSqlParameter("@REPLICATION_COUNT", replicationCount);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_REPLICATE_PACKAGE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response<Result>> PostReplicatePackage(int packageId, int replicationCount)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postReplicatePackage(packageId, replicationCount);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*----------------------------------------------------------POST PACKAGEPRINT-------------------------------------------------------------*/

        public async Task<Response<Result>> PostPackagePrint(Models.Printqueue printqueue, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostPackagePrint(printqueue, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostPackagePrint(Models.Printqueue printqueue, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(printqueue);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PRINTQUEUE", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        /*-------------------------------------------------------------------------------------------------------------------*/

        public async Task<Response<Result>> AddPackge(string packageCode, Int32 guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _addPackge(packageCode, guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Result>> DeletePackge(string packageCode, Int32 guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _deletePackge(packageCode, guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response<Result>> OpenPackage(int packageId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _openPackage(packageId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        public async Task<Response<Result>> PostGuide(Models.Guide guide, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postGuide(guide, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Result>> PostPackage(Models.Package package)
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


        private async Task<Response<List<Customer>>> _getCustomersForPacking(Int32? supplierId)
        {
            Response<List<Customer>> _response = new Response<List<Customer>>();
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

        private async Task<Response<List<Provider>>> _getProviderToDeliver()
        {
            Response<List<Provider>> _response = new Response<List<Provider>>();
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



        private async Task<Response<List<Models.Guide>>> _getGuides(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            Response<List<Models.Guide>> _response = new Response<List<Models.Guide>>();
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
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<List<Models.Package>>> _getPackages(Int32 supplierId, Int32 customerId, Int32 userId)
        {
            Response<List<Models.Package>> _response = new Response<List<Models.Package>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDCUSTOMER", customerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("GuideId", "IDGUIDE");
                _mapping.AddItem("Number", "INUMBER");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("Weight", "NWEIGHT");
                _mapping.AddItem("Closed", "BCLOSED");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGES", _parameter);
                _response.Data = _data.GetList<Models.Package>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<List<Models.Package>>> _getPackagesFromGuide(Int32 guideId)
        {
            Response<List<Models.Package>> _response = new Response<List<Models.Package>>();
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
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<List<PackageDetail>>> _getPackageDetails(Int32 packageId)
        {
            Response<List<PackageDetail>> _response = new Response<List<PackageDetail>>();
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
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<List<PackageDetail>>> _getPackageDetailsPending(Int32 packageId)
        {
            Response<List<PackageDetail>> _response = new Response<List<PackageDetail>>();
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
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<List<PackageDetail>>> _getPackageDetailByPart(Int32 packageId, string scanCode)
        {
            Response<List<PackageDetail>> _response = new Response<List<PackageDetail>>();
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
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*----------------------------------------------------------PACKAGELIST------------------------------------------------------*/

        private async Task<Response<List<PackageList>>> _GetPackagesList(Int32 supplierId, string packageCode)
        {
            Response<List<PackageList>> _response = new Response<List<PackageList>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VPACKAGECODE", packageCode);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("PartName", "VDESCRIPTION");
                _mapping.AddItem("Quantity", "IQUANTITY");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKAGESLIST", _parameter);
                _response.Data = _data.GetList<Models.PackageList>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<List<PackageList>>> _GetPackingList(Int32 supplierId)
        {
            Response<List<PackageList>> _response = new Response<List<PackageList>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("PartName", "VDESCRIPTION");
                _mapping.AddItem("Quantity", "IQUANTITY");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PACKINGLIST", _parameter);
                _response.Data = _data.GetList<Models.PackageList>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        /*-----------------------------------------------------------------------------------------------------------------------*/


        public async Task<Response<List<Customer>>> GetCustomersForPacking(Int32? supplierId)
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

        public async Task<Response<List<Provider>>> GetProviderToDeliver()
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

        public async Task<Response<List<Models.Guide>>> GetGuides(Int32 supplierId, Int32 customerId, Int32 userId)
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

        public async Task<Response<List<Package>>> GetPackages(Int32 supplierId, Int32 customerId, Int32 userId)
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

        public async Task<Response<List<Package>>> GetPackagesFromGuide(Int32 guideId)
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



        public async Task<Response<List<PackageDetail>>> GetPackageDetails(Int32 packageId)
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

        public async Task<Response<List<PackageDetail>>> GetPackageDetailByPart(Int32 packageId, string scanCode)
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

        public async Task<Response<List<PackageDetail>>> GetPackageDetailsPending(Int32 packageId)
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

        /*----------------------------------------------------------PACKAGELIST------------------------------------------------------*/
        public async Task<Response<List<PackageList>>> GetPackagesList(Int32 supplierId, string packageCode)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPackagesList(supplierId, packageCode);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<PackageList>>> GetPackingList(Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPackingList(supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /*-----------------------------------------------------------------------------------------------------------------------*/
        public async Task<Response<List<Models.Guide>>> GetAllGuides(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllGuides(userId, supplierId, dealerId, rowfrom, filter, fromDate, upToDate, estatusId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Guide>> GetOneGuide(int guideId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getOneGuide(0, 0, 0, "", guideId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Guide>> _getOneGuide(Int32 userId, Int32 supplierId, Int32 rowfrom, string? filter = "", int? guideId = null)
        {
            Response<Models.Guide> _response = new Response<Models.Guide>();
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

                _mapping.AddItem("CreatedDate", "DCREATEDDATE");
                _mapping.AddItem("DeliveredDate", "DDELIVERYDATE");
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");

                _mapping.AddItem("Weith", "NWEITH");




                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_GUIDES", _parameter);
                _response.Data = _data.GetItem<Models.Guide>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<List<Models.Guide>>> _GetAllGuides(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId, int? guideId = null)
        {
            Response<List<Models.Guide>> _response = new Response<List<Models.Guide>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IDGUIDE", guideId); //guideId es null
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IESTATUS", estatusId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");

                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");

                _mapping.AddItem("ProviderId", "IDPROVIDER");
                _mapping.AddItem("ProviderName", "VPROVIDER");

                _mapping.AddItem("Number", "VNUMBER");

                _mapping.AddItem("CreatedDate", "DCREATEDDATE");
                _mapping.AddItem("DeliveredDate", "DDELIVERYDATE");
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");

                _mapping.AddItem("Weith", "NWEITH");




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

        public async Task<Response<Result>> PostDeliverGuides(List<Models.DeliveredGuides> guides)
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


        private async Task<Response<Result>> _PostDeliverGuides(List<DeliveredGuides> guides)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(guides);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_DELIVERED_GUIDES", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<List<Models.GuideDetails>>> _GetGuideDetails(int guideId)
        {
            Response<List<Models.GuideDetails>> _response = new Response<List<Models.GuideDetails>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDGUIDE", guideId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("GuideId", "IDGUIDE");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("PartInnercode", "VINNERCODE");
                _mapping.AddItem("PartDescription", "VDESCRIPTION");
                _mapping.AddItem("ReasonId", "IDREASON");
                _mapping.AddItem("ReasonDescription", "VREASON");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("PackageId", "IDPACKAGE");
                _mapping.AddItem("PackageNumber", "IPACKAGENUMBER");
                _mapping.AddItem("PackageCode", "VPACKAGECODE");
                _mapping.AddItem("Received", "IRECEIVED");
                _mapping.AddItem("Observation", "VOBSERVATION");
                _mapping.AddItem("SaleOrderId", "IDSALEORDER");
                _mapping.AddItem("Confirmed", "BCONFIRMED");


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


        public async Task<Response<List<Models.GuideDetails>>> GetGuideDetails(int guideid)
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

        private async Task<Response<List<Models.PartialType>>> _GetPartialTypes()
        {
            Response<List<Models.PartialType>> _response = new Response<List<Models.PartialType>>();
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


        public async Task<Response<List<Models.PartialType>>> GetPartialTypes()
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

        #region "BACKORDEN"

        /*--------------------------------------------------------------GET IMPORT-------------------------------------------------------------*/

        public async Task<List<BackOrder>> GetExportBackOrders(int userId, int supplierId, int dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<BackOrder>)(await _GetBackOrders(userId, supplierId, dealerId, null, null, null, null)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /*--------------------------------------------------------------GET ALL-------------------------------------------------------------*/

        public async Task<Response<List<Models.BackOrder>>> GetBackOrders(int userId, int supplierId, int? dealerId, int? rowfrom, string? filter, DateTime? startdate, DateTime? enddate, bool stockOnly = false)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetBackOrders(userId, supplierId, dealerId, rowfrom, filter, startdate, enddate, stockOnly);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<BackOrder>>> _GetBackOrders(int userId, int supplierId, int? dealerId, int? rowfrom, string? filter, DateTime? startdate, DateTime? enddate, bool stockOnly =false)
        {
            Response<List<BackOrder>> _response = new Response<List<BackOrder>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@STARTDATE", startdate);
                _parameter.AddSqlParameter("@ENDDATE", enddate);
                _parameter.AddSqlParameter("@BSTOCKONLY", stockOnly);
                

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDBACKORDER");
                _mapping.AddItem("PartDescription", "VDESCRIPTION");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Quantity", "IBACKORDER");
                _mapping.AddItem("StatusId", "IDSTATUS");
                _mapping.AddItem("Arrival", "DARRIVAL");
                _mapping.AddItem("CreatedDate", "DCREATED");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALER");
                _mapping.AddItem("SaleOrderNumber", "IDSALEORDER");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("TypeName", "VTYPE");
                _mapping.AddItem("SupplierRef", "VREFERENCESUPPLIER");
                _mapping.AddItem("DealerRef", "VREFERENCEDEALER");
                _mapping.AddItem("Stock", "ISTOCK");
                _mapping.AddItem("Cost", "NCOST");
                _mapping.AddItem("Price", "NPRICE");


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
        /*--------------------------------------------------------------POST IMPORT-------------------------------------------------------------*/

        public async Task<Response<Result>> PostImportBackOrders(List<Models.BackOrder> backOrder, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postImportBacKOrder(backOrder, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /*--------------------------------------------------------------POST-------------------------------------------------------------*/

        public async Task<Response<Result>> PostBacKOrder(Models.BackOrder backOrder, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postBacKOrder(backOrder, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _postBacKOrder(Models.BackOrder backOrder, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(backOrder);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BFULL", true);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_BACKORDER", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Result>> _postImportBacKOrder(List<Models.BackOrder> backOrder, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(backOrder);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BFULL", false);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_BACKORDER", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        /*--------------------------------------------------------------POST ACTION-------------------------------------------------------------*/


        public async Task<Response<Result>> PostBackorder_Actions(List<Models.Action> _list, Int32 userId)
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



        private async Task<Response<Result>> _PostBackorder_Actions(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_BACKORDER_ACTIONS", _parameter);
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


        public async Task<Response<Result>> PostGuideDetails(List<GuideDetails> _list, int? userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostGuideDetails(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostGuideDetails(List<GuideDetails> details, int? userid)
        {
            Response<Result> _response = new Response<Result>();
            try
            {

                string _jsonstring = Util.Json.ConvertToJsonString(details);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userid);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_GUIDES_DETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }




        public async Task<Response<Result>> PostGuidesActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postGuidesActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _postGuidesActions(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_GUIDES_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response<Result>> PostGuideNumber(Int32 userId, Int32 guideId, string guideNumber)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postGuideNumber(userId, guideId, guideNumber);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _postGuideNumber(Int32 userId, Int32 guideId, string guideNumber)
        {
            Response<Result> _response = new Response<Result>();
            try
            {


                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDGUIDE", guideId);
                _parameter.AddSqlParameter("@VNUMBER", guideNumber);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_GUIDE_ACTNUMBER", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        #region ADJUSTMENT
        /*--------------------------------------------------------------GetAll---------------------------------------------------------------*/
        public async Task<Response<List<Models.Adjustment>>> GetAdjustments(Int32 userId, Int32 supplierId, Int32 rowFrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAdjustments(userId, supplierId, rowFrom, filter, fromDate, upToDate, estatusId, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Adjustment>>> _GetAdjustments(Int32 userId, Int32 supplierId, Int32? rowFrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? estatusId, Int32? adjustmentId = null)
        {
            Response<List<Models.Adjustment>> _response = new Response<List<Models.Adjustment>>();

            try
            {
                var _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", adjustmentId);
                 _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IESTATUS", estatusId);


                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("DCreated", "DCREATED");
                _mapping.AddItem("DUpdated", "DUPDATED");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("StatusId", "IDSTATUS");
                _mapping.AddItem("StatusName", "VSTATUS");
                _mapping.AddItem("UserLogin", "VLOGIN");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ADJUSTMENT", _parameter);

                _response.Data = _data.GetList<Models.Adjustment>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        /*--------------------------------------------------------------GetOne---------------------------------------------------------------*/

        public async Task<Response<Adjustment>> GetAdjustment(Int32 userId, Int32? adjustmentId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAdjustment(userId, 0, 0, "", adjustmentId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Adjustment>> _GetAdjustment(Int32 userId, Int32 supplierId, Int32 rowFrom, string? filter, Int32? adjustmentId = null)
        {
            Response<Adjustment> _response = new Response<Adjustment>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", adjustmentId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("DCreated", "DCREATED");
                _mapping.AddItem("DUpdated", "DUPDATED");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("StatusId", "IDSTATUS");
                _mapping.AddItem("StatusName", "VSTATUS");
                _mapping.AddItem("UserLogin", "VLOGIN");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ADJUSTMENT", _parameter);
                _response.Data = _data.GetItem<Models.Adjustment>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-------------------------------------------------------------- Get Export   ---------------------------------------------------------------*/

        public async Task<List<Adjustment>> GetExportAdjustment(Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Adjustment>)(await _GetAdjustments(userId, supplierId, null, null, null,null,null,null)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /*-------------------------------------------------------------- GetDetails ---------------------------------------------------------------*/


        public async Task<Response<List<AdjustmentDetails>>> GetAdjustmentDetails(Int32 adjustmentId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAdjustmentDetails(adjustmentId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<AdjustmentDetails>>> _GetAdjustmentDetails(Int32 adjustmentId)
        {
            Response<List<AdjustmentDetails>> _response = new Response<List<AdjustmentDetails>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDADJUSTMENT", adjustmentId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("AdjustmentId", "IDADJUSTMENT");
                _mapping.AddItem("LocationId", "IDLOCATION");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("Stock", "ISTOCK");
                _mapping.AddItem("ReasonId", "IDREASON");
                _mapping.AddItem("AdjustmentType", "CTYPE");
                _mapping.AddItem("ReasonType", "CTYPE");
                _mapping.AddItem("ReasonDescription", "VREASON");
                _mapping.AddItem("Inncercode", "VINNERCODE");
                _mapping.AddItem("PartDescription", "VPART");
                _mapping.AddItem("PartPrice", "NPRICE");
                _mapping.AddItem("Location", "VLOCATION");
                _mapping.AddItem("Zone", "VZONE");
                _mapping.AddItem("Warehouse", "VWAREHOUSE");
                _mapping.AddItem("Cost", "NCOST");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ADJUSTMENTDETAILS", _parameter);
                _response.Data = _data.GetList<Models.AdjustmentDetails>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        /*-------------------------------------------------------------- GetReasons ---------------------------------------------------------------*/


        public async Task<Response<List<Models.Reason>>> GetReasons()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReasons();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Reason>>> _GetReasons()
        {
            Response<List<Models.Reason>> _response = new Response<List<Models.Reason>>();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Type", "CTYPE");
                _mapping.AddItem("Description", "DESC");

                var _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REASONS");
                _response.Data = _data.GetList<Models.Reason>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*-------------------------------------------------------------- POST ADJUSTMENT ---------------------------------------------------------------*/
        public async Task<Response<Result>> PostAdjustment(Models.Adjustment _item, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostAdjustment(_item, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Result>> _PostAdjustment(Adjustment _item, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_item);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ADJUSTMENT", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        /*----------------------------------------------------------- POST ADJUSTMENTDETAILS -------------------------------------------------------------*/

        public async Task<Response<Result>> PostAdjustmentDetails(Models.AdjustmentDetails adjustmentDetails, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostAdjustmentDetails(adjustmentDetails, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostAdjustmentDetails(AdjustmentDetails adjustmentDetails, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(adjustmentDetails);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ADJUSTMENTDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        /*----------------------------------------------------------- POST ADJUSTMENT ACTION -------------------------------------------------------------*/


        public async Task<Response<Result>> PostAdjustmentActions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_AdjustmentActions(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Result>> _Post_AdjustmentActions(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_ADJUSTMENT_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        /*----------------------------------------------------------- DELETE ADJUSTMENTDETAILS -------------------------------------------------------------*/

        public async Task<Response<Result>> DeleteAdjustmentDetail(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _DeleteAdjustmentDetail(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _DeleteAdjustmentDetail(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_DELETE_ADJUSTMENTDETAIL", _parameter);
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


        #region "INVOICECONTROL"


        /*--------------------------------------------------------------GET EXPORT-------------------------------------------------------------*/

        public async Task<Response<List<Models.Invoicecontrol>>> GetDispatchedControlTxtExport(int userId, int supplierId, int idcontrol)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDispatchedControlTxtExport(userId, supplierId, idcontrol);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Invoicecontrol>>> _GetDispatchedControlTxtExport(int userId, int supplierId, int idcontrol)
        {
            Response<List<Invoicecontrol>> _response = new Response<List<Invoicecontrol>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@IDCONTROL", idcontrol);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("Dispatched", "IDISPATCHED");  
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("ControlId", "IDCONTROL"); 

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DISPATCHEDCONTROL_GROUP", _parameter);

                _response.Data = _data.GetList<Invoicecontrol>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

       
        /*--------------------------------------------------------------GET DISPATCHED-------------------------------------------------------------*/

        public async Task<Response<List<Models.Invoicecontrol>>> GetDispatchedControl(int userId, int supplierId, int? rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDispatchedControl(userId, supplierId, rowfrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<List<Invoicecontrol>>> _GetDispatchedControl(int userId, int supplierId, int? rowfrom, string? filter)
        {
            Response<List<Invoicecontrol>> _response = new Response<List<Invoicecontrol>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter); 


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SaleOrderNumber", "IDSALEORDER");
                _mapping.AddItem("MovementDetailId", "IDMOVEMENTDETAIL");
                _mapping.AddItem("BackOrderId", "IDBACKORDER");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("CustomerId", "IDCUSTOMER"); //id cliente
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("Dispatched", "IDISPATCHED"); //cantidad despachada
                _mapping.AddItem("StockInfo", "STOCKINFO");

                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FiscalName", "VFISCALNAME");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("SupplierName", "VSUPPLIER"); 
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("Mark", "BMARK");
                _mapping.AddItem("SaleOrderType", "VSALEORDERTYPE");
                



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DISPATCHEDCONTROL", _parameter);
                _response.Data = _data.GetList<Models.Invoicecontrol>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        /*--------------------------------------------------------------GET DISPATCHED TXT-------------------------------------------------------------*/

        public async Task<Response<List<Models.Invoicecontrol>>> GetDispatchedControlTxt(int userId, int supplierId, int? rowfrom, string? filter, int? pendant)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDispatchedControlTxt(userId, supplierId, rowfrom, filter, pendant);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Invoicecontrol>>> _GetDispatchedControlTxt(int userId, int supplierId, int? rowfrom, string? filter, int? pendant)
        {
            Response<List<Invoicecontrol>> _response = new Response<List<Invoicecontrol>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BPENDANT", pendant);
                _parameter.AddSqlParameter("@IDCONTROL", 0);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SaleOrderNumber", "IDSALEORDER");
                _mapping.AddItem("MovementDetailId", "IDMOVEMENTDETAIL");
                _mapping.AddItem("BackOrderId", "IDBACKORDER");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("CustomerId", "IDCUSTOMER"); //id cliente
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("InvoiceId", "IDINVOICE"); //numero de factura
                _mapping.AddItem("ControlId", "IDCONTROL"); //numero de control
                _mapping.AddItem("Invoiced", "IINVOICED");  //numero de cantidad facturada
                _mapping.AddItem("Dispatched", "IDISPATCHED"); //cantidad despachada
                _mapping.AddItem("Mark", "BMARK");
                _mapping.AddItem("UserSinc", "IDUSERSINC");
                _mapping.AddItem("ControlDate", "DCONTROLDATE");
                _mapping.AddItem("SincDate", "DSINCDATE");
                _mapping.AddItem("Pending", "IPENDING");

                _mapping.AddItem("StatusId", "IDESTATUS");
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");

                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FiscalName", "VFISCALNAME");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("LocationName", "VLOCATION");
                _mapping.AddItem("Required", "IREQUIRED");
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("SaleOrderType", "VSALEORDERTYPE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DISPATCHEDCONTROL_TXT", _parameter);

                _response.Data = _data.GetList<Invoicecontrol>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        /*--------------------------------------------------------------GET INVOICE-------------------------------------------------------------*/

        public async Task<Response<List<Models.Invoicecontrol>>> GetInvoice(int userId, int supplierId, int? rowfrom, string? filter, int? pendant)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetInvoice(userId, supplierId, rowfrom, filter, pendant);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Invoicecontrol>>> _GetInvoice(int userId, int supplierId, int? rowfrom, string? filter, int? pendant)
        {
            Response<List<Invoicecontrol>> _response = new Response<List<Invoicecontrol>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BSURPLUS", pendant);
                
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID"); 
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("CustomerId", "IDCUSTOMER"); //id cliente
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("InvoiceId", "IDINVOICE"); //numero de factura
                _mapping.AddItem("ControlId", "IDCONTROL"); //numero de control
                _mapping.AddItem("Invoiced", "IINVOICED");  //numero de cantidad facturada
                _mapping.AddItem("Dispatched", "IDISPATCHED"); //cantidad despachada 
                _mapping.AddItem("UserSinc", "IDUSERSINC");
                _mapping.AddItem("ControlDate", "DCREATED");
                _mapping.AddItem("SincDate", "DSINCDATE");
                _mapping.AddItem("Pending", "ISURPLUS");

                _mapping.AddItem("StatusId", "IESTATUS");
                _mapping.AddItem("StatusName", "STATUS_DISPLAY"); 

                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FiscalName", "VFISCALNAME");
                _mapping.AddItem("PartInnerCode", "VCODEPART");
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("SupplierName", "VSUPPLIER"); 
                _mapping.AddItem("Price", "NPRICE");
              

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INVOICE", _parameter);

                _response.Data = _data.GetList<Invoicecontrol>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        //*--------------------------------------------------------------POST-------------------------------------------------------------*/
        public async Task<Response<Result>> PostInvoiceControl(List<Models.Invoicecontrol> invoiceControls, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostInvoiceControl(invoiceControls, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostInvoiceControl(List<Models.Invoicecontrol> invoiceControls, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                var updateData = invoiceControls.Select(ic => new
                {
                    Id = ic.Id,
                    Mark = ic.Mark
                }).ToList();

                string _jsonstring = Util.Json.ConvertToJsonString(updateData);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_INVOICECONTROL", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*--------------------------------------------------------------POST ACTION-------------------------------------------------------------*/
        public async Task<Response<Result>> PostInvoiceControl_Actions(List<Models.Action> actions, Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostInvoiceControl_Actions(actions, userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostInvoiceControl_Actions(List<Models.Action> actions, Int32 userId, Int32 supplierId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(actions);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_DISPATCHEDCONTROL_ACTION", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        public async Task<Response<Result>> ImportInvoiceReport(List<Models.InvoiceReport> _list, Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _importInvoiceReport(_list, userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _importInvoiceReport(List<Models.InvoiceReport> _list, Int32 userId, Int32 supplierId)
        {
            Response<Result> _response = new Response<Result>();
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
                DataTable _table = await _data.GetDataTable("USP_POST_INVOICEREPORT", _parameter);
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

        #region "CLAIM"
        /*--------------------------------------------------------------GetOne---------------------------------------------------------------*/
         
        public async Task<Response<Models.ClaimPart>> GetClaim(int userId, int claimId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetClaim(userId, claimId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.ClaimPart>> _GetClaim(int userId, int claimId)
        {
            Response<Models.ClaimPart> _response = new Response<Models.ClaimPart>();
            try
            {
                var _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", null);  
                _parameter.AddSqlParameter("@IDDEALER", null);    
                _parameter.AddSqlParameter("@IROWFROM", null);
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@ID", claimId);  

                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Note", "VNOTE");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("Invoice", "VINVOICE");
                _mapping.AddItem("Guide", "VGUIDE");
                _mapping.AddItem("DCreated", "DCREATED");
                _mapping.AddItem("DUpdated", "DUPDATED");
                _mapping.AddItem("StatusId", "IDSTATUS");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALER");

                // User/Creator
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("FiscalName", "VFISCALNAME");
                // Status
                _mapping.AddItem("StatusName", "DISPLAY_STATUS");
                

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CLAIMS", _parameter);
                 
                _response.Data = _data.GetItem<Models.ClaimPart>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*--------------------------------------------------------------GetAll---------------------------------------------------------------*/
        public async Task<Response<List<Models.ClaimPart>>> GetAllClaims(int userId, int? supplierId, int? dealerId, int? rowFrom, DateTime? fromDate, DateTime? upToDate, int? estatusId, string? filter, int? claimId = null)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAllClaims(userId, supplierId, dealerId, rowFrom, fromDate, upToDate, estatusId, filter, claimId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.ClaimPart>>> _GetAllClaims(int userId, int? supplierId, int? dealerId, int? rowFrom, DateTime? fromDate, DateTime? upToDate, int? estatusId, string? filter, int? claimId = null)
        {  
            Response<List<Models.ClaimPart>> _response = new Response<List<Models.ClaimPart>>();

            try
            { 
                var _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IESTATUS", estatusId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", claimId);

                var _mapping = new Mapping();  
                  
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Note", "VNOTE");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("Invoice", "VINVOICE");
                _mapping.AddItem("Guide", "VGUIDE");
                _mapping.AddItem("DCreated", "DCREATED");
                _mapping.AddItem("DUpdated", "DUPDATED");
                _mapping.AddItem("StatusId", "IDSTATUS");

                //CAMPOS DE DETALLES
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("ReasonDescription", "VDESCRIPTION"); 

                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALER");

                // User/Creator
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("FiscalName", "VFISCALNAME"); 

                // Status 
                _mapping.AddItem("StatusName", "DISPLAY_STATUS");
                 

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CLAIMS", _parameter);

                _response.Data = _data.GetList<Models.ClaimPart>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        /*--------------------------------------------------------------Details---------------------------------------------------------------*/

        public async Task<Response<List<Models.ClaimDetails>>> GetClaimDetails(Int32 claimId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetClaimDetails(claimId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.ClaimDetails>>> _GetClaimDetails(Int32 claimId)
        {
            Response<List<Models.ClaimDetails>> _response = new Response<List<Models.ClaimDetails>>();

            try
            {
                var _parameter = new Util.Parameter();
                // Usar @IDCLAIM como parámetro (igual que en el stored procedure)
                _parameter.AddSqlParameter("@IDCLAIM", claimId);

                var _mapping = new Mapping();
                // Mapeo de columnas requeridas
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("ClaimId", "IDCLAIM");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("StatusId", "IDSTATUS");
                _mapping.AddItem("Quantity", "IQUANTITY");
                _mapping.AddItem("ReasonId", "IDREASON");

                //mapeo para la aprobacion del item
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("ReplacementPartId", "IDREPLACEMENT_PART");
                _mapping.AddItem("IApproved", "IAPPROVEDQUANTITY"); 
                _mapping.AddItem("ITransaction", "ITRANSACTIONQUANTITY");


                // Mapeo de información de partes
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("PartInnerCode", "PARTCODE");
                _mapping.AddItem("Price", "PRICE");

                _mapping.AddItem("ReplacemetName", "VREPLACEMENT");
                _mapping.AddItem("RemplacemetInnerCode", "REPLACEMENTCODE");
                _mapping.AddItem("RemplacemetPrice", "REPLACEMENTPRICE");

                 
                // Mapeo de razón y estado
                _mapping.AddItem("ReasonDescription", "VDESCRIPTION");   
                _mapping.AddItem("StatusName", "VDISPLAYESTATUS");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CLAIMDETAILS", _parameter);

                _response.Data = _data.GetList<Models.ClaimDetails>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        private async Task<Response<ClaimPart>> _getlastClaim(Int32? userId, Int32? dealerId)
        {
            Response<ClaimPart> _response = new Response<ClaimPart>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Note", "VNOTE");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("Invoice", "VINVOICE");
                _mapping.AddItem("Guide", "VGUIDE");
                _mapping.AddItem("DCreated", "DCREATED");
                _mapping.AddItem("DUpdated", "DUPDATED");
                _mapping.AddItem("StatusId", "IDSTATUS");
                // Supplier
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");

                // Dealer
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALER");

                // User/Creator
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("FiscalName", "VFISCALNAME");
                // Status
                _mapping.AddItem("StatusName", "DISPLAY_STATUS");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LASTSCLAIM", _parameter);
                _response.Data = _data.GetItem<Models.ClaimPart>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
         

        public async Task<Response<ClaimPart>> GetlastClaim(Int32? userId, Int32? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getlastClaim(userId, dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /*---------------------------------------------------------------POST CLAIM---------------------------------------------------------------------------*/
        public async Task<Response<Result>> PostClaim(List<Models.ClaimPart> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostClaim(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostClaim(List<Models.ClaimPart> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_CLAIM", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        //------------------------------------------------------------------POST CLAIMDETAILS-------------------------------------------------------------------------
        public async Task<Response<Result>> PostClaimDetails(List<Models.ClaimDetails> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostClaimDetails(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _PostClaimDetails(List<Models.ClaimDetails> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_CLAIMDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

       //------------------------------------------------------------------POST ACTIONS DETAILS-------------------------------------------------------------------------
        public async Task<Response<Models.Result>> Post_ActionsDetails(List<Models.ActionClaim> _list, Int32 userId)
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

        private async Task<Response<Models.Result>> _Post_ActionsDetails(List<Models.ActionClaim> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_CLAIMDETAILS_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //------------------------------------------------------------------POST ACTIONS CLAIM-------------------------------------------------------------------------
        public async Task<Response<Models.Result>> Post_ActionsClaim(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _Post_ActionsClaim(_list, userId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_ActionsClaim(List<Models.Action> _list, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDGUIDE", 0);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_CLAIM_ACTION", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        //---------------------------------------------------------------DELETE CLAIM---------------------------------------------------------------*/



        public async Task<Response<Result>> DeleteClaimDetail(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _DeleteClaimDetail(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _DeleteClaimDetail(List<Models.Action> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_DELETE_CLAIMDETAILS", _parameter);
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
