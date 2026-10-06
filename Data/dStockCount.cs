using System.Data;
using Models;
using Util;

namespace Data
{
    public class dStockCount
    {
        private readonly SemaphoreSlim _semaphore;

        public dStockCount()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);
        }

        //--------------------------------------------------------- GET INVENTORY TYPES-----------------------------------------------------------------------------
        public async Task<Response<List<InventoryTypeOption>>> GetInventoryTypes(
            int userId,
            int supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetInventoryTypes(userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<InventoryTypeOption>>> _GetInventoryTypes(
            int userId,
            int supplierId)
        {
            Response<List<InventoryTypeOption>> _response = new Response<List<InventoryTypeOption>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VACTION", "GETTYPES");

                Mapping _mapping = new Mapping();
                _mapping.AddItem("IdCountType", "IDCOUNTTYPE");
                _mapping.AddItem("VName", "VNAME");
                _mapping.AddItem("IsActive", "ISACTIVE");
                _mapping.AddItem("AssignmentType", "ASSIGNMENTTYPE");
                _mapping.AddItem("AssignmentName", "ASSIGNMENTNAME");
                _mapping.AddItem("IdInventoryCount", "IDINVENTORYCOUNT");
                _mapping.AddItem("HasAssignment", "HASASSIGNMENT");
                _mapping.AddItem("IdStatus", "IDESTATUS");
                _mapping.AddItem("VDescription", "VDESCRIPTION");
                _mapping.AddItem("IdCount", "IDCOUNT");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_STOCK_COUNT", _parameter);
                _response.Data = _data.GetList<InventoryTypeOption>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        //--------------------------------------------------------- GET DETAILS-----------------------------------------------------------------------------
        public async Task<Response<List<InventoryDetailResponse>>> GetDetails(
            int userId,
            int supplierId,
            int inventoryId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDetails(userId, supplierId, inventoryId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<InventoryDetailResponse>>> _GetDetails(
            int userId,
            int supplierId,
            int inventoryId)
        {
            Response<List<InventoryDetailResponse>> _response = new Response<List<InventoryDetailResponse>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDINVENTORYCOUNT", inventoryId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("IdLocation", "IDLOCATION");
                _mapping.AddItem("LocationName", "LOCATIONNAME");
                _mapping.AddItem("IdPart", "IDPART");
                _mapping.AddItem("PartCode", "PARTCODE");
                _mapping.AddItem("PartDescription", "PARTDESCRIPTION");
                _mapping.AddItem("IdStatus", "IDSTATUS");
                _mapping.AddItem("IdCount", "IDCOUNT");
                _mapping.AddItem("IsPending", "ISPENDING");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DETAIL_STOCK_COUNT", _parameter);
                _response.Data = _data.GetList<InventoryDetailResponse>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        

        //--------------------------------------------------------- CHECK ACTIVE INVENTORY-----------------------------------------------------------------------------
        public async Task<Response<ActiveInventoryResponse>> CheckActiveInventory(
            int userId,
            int supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _CheckActiveInventory(userId, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<ActiveInventoryResponse>> _CheckActiveInventory(
            int userId,
            int supplierId)
        {
            Response<ActiveInventoryResponse> _response = new Response<ActiveInventoryResponse>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@VACTION", "CHECKACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_STOCK_COUNT", _parameter);

                _response.Data = new ActiveInventoryResponse();
                if (_table.Rows.Count > 0)
                {
                    _response.Data.HasActiveInventory = Convert.ToBoolean(_table.Rows[0]["HASACTIVEINVENTORY"]);
                }
                else
                {
                    _response.Data.HasActiveInventory = false;
                }

                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        //--------------------------------------------------------- VALIDATE LOCATION-----------------------------------------------------------------------------
        public async Task<Response<LocationValidationResponse>> ValidateLocation(
            int userId,
            int supplierId,
            int inventoryId,
            string locationCode)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ValidateLocation(userId, supplierId, inventoryId, locationCode);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<LocationValidationResponse>> _ValidateLocation(
    int userId,
    int supplierId,
    int inventoryId,
    string locationCode)
        {
            Response<LocationValidationResponse> _response = new Response<LocationValidationResponse>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDINVENTORYCOUNT", inventoryId);
                _parameter.AddSqlParameter("@LOCATIONCODE", locationCode);

                Mapping _mapping = new Mapping();
                
                _mapping.AddItem("LocationId", "LOCATIONID");
                _mapping.AddItem("LocationName", "LOCATIONNAME");
                _mapping.AddItem("ZoneName", "ZONENAME");
               

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VALIDATELOCATION_STOCKCOUNT", _parameter);

                if (_table.Rows.Count > 0)
                {
                    _response.Data = _data.GetItem<LocationValidationResponse>(_mapping, _table);
                    _response.Data.IsValid = true;
                    _response.Data.Message = "Ubicación válida";
                }
                else
                {
                    _response.Data = new LocationValidationResponse
                    {
                        IsValid = false,
                        Message = "Ubicación no válida o sin acceso"
                    };
                }

                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
                _response.Data = new LocationValidationResponse
                {
                    IsValid = false,
                    Message = ex.Message
                };
            }

            return _response;
        }

        //--------------------------------------------------------- VALIDATE PART-----------------------------------------------------------------------------
        public async Task<Response<PartValidationResponse>> ValidatePart(
            int userId,
            int supplierId,
            int locationId,
            string partCode)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _ValidatePart(userId, supplierId, locationId, partCode);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<PartValidationResponse>> _ValidatePart(
            int userId,
            int supplierId,
            int locationId,
            string partCode)
        {
            Response<PartValidationResponse> _response = new Response<PartValidationResponse>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDLOCATION", locationId);
                _parameter.AddSqlParameter("@PARTCODE", partCode);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("PartId", "PARTID");
                _mapping.AddItem("PartCode", "PARTCODE");
                _mapping.AddItem("PartDescription", "PARTDESCRIPTION");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VALIDATEPART_STOCKCOUNT", _parameter);

                if (_table.Rows.Count > 0)
                {
                    _response.Data = _data.GetItem<PartValidationResponse>(_mapping, _table);
                    _response.Data.IsValid = true;
                    _response.Data.Message = "Repuesto válido";
                }
                else
                {
                    _response.Data = new PartValidationResponse
                    {
                        IsValid = false,
                        Message = "Repuesto no encontrado en esta ubicación"
                    };
                }

                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
                _response.Data = new PartValidationResponse
                {
                    IsValid = false,
                    Message = ex.Message
                };
            }

            return _response;
        }

        //--------------------------------------------------------- ADD UNASSIGNED PART-----------------------------------------------------------------------------
        public async Task<Response<StockCountResponse>> AddUnassignedPart(
            int userId,
            int supplierId,
            UnassignedPartRequest data)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _AddUnassignedPart(userId, supplierId, data);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<StockCountResponse>> _AddUnassignedPart(
            int userId,
            int supplierId,
            UnassignedPartRequest data)
        {
            Response<StockCountResponse> _response = new Response<StockCountResponse>();

            try
            {
                string jsonData = Util.Json.ConvertToJsonString(data);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@DATA", jsonData);
                _parameter.AddSqlParameter("@ISUNASSIGNED", true);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Iid", "IID");
                _mapping.AddItem("Iinserted", "IINSERTED");
                _mapping.AddItem("Iupdated", "IUPDATED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_STOCK_COUNT", _parameter);
                _response.Data = _data.GetItem<StockCountResponse>(_mapping, _table);
                _response.SetPostResponse();
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        //----------------------------------------------------------POST STOCK COUNT-----------------------------------------------------------------------------
        public async Task<Response<StockCountResponse>> PostStockCount(int userId, int supplierId, StockCountDetail data)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostStockCount(userId, supplierId, data);
            }
            finally
            {
                _semaphore.Release();
            }
        } 

        private async Task<Response<StockCountResponse>> _PostStockCount(int userId, int supplierId, StockCountDetail data)
        {
            Response<StockCountResponse> _response = new Response<StockCountResponse>();

            try
            {
                string jsonData = Util.Json.ConvertToJsonString(data);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@DATA", jsonData);
                _parameter.AddSqlParameter("@ISUNASSIGNED", false);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Iid", "IID");
                _mapping.AddItem("Iinserted", "IINSERTED");
                _mapping.AddItem("Iupdated", "IUPDATED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_STOCK_COUNT", _parameter);
                _response.Data = _data.GetItem<StockCountResponse>(_mapping, _table);
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