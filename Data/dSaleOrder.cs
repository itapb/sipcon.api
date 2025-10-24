using System;
using System.Data;
using Models;
using Util;

namespace Data
{
    public class dSaleOrder
    {
        private readonly SemaphoreSlim _semaphore;


        public dSaleOrder()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<SaleOrder>>> GetAll(Int32? userId, Int32? supplierId, Int32? dealerId, Int32? rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId, dealerId,  rowfrom, filter, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<List<SaleOrder>> GetExport(Int32 userId, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<SaleOrder>)(await _GetAll(userId, supplierId, null, null, null)).Data;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<List<SaleOrder>>> _GetAll(Int32? userId, Int32? supplierId, Int32? dealerId, Int32? rowfrom, string? filter, Int32? saleOrderId = null)
        {
            Response<List<SaleOrder>> _response =  new Response<List<SaleOrder>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", saleOrderId);



                
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("StatusId", "IDESTATUS");
                _mapping.AddItem("StatusName", "VESTATUS");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("TypeName", "VTYPE");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("Created", "VCREATED");
                _mapping.AddItem("CreatedBy", "VCREATEDBY");
                _mapping.AddItem("IsClaim", "BCLAIM");
                _mapping.AddItem("Paralyzed", "BPARALYZED");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("VehicleVin", "VVIN");
                _mapping.AddItem("VehicleCustomer", "VCUSTOMER");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SALEORDERS", _parameter);
                _response.Data =  _data.GetList<Models.SaleOrder>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<List<ServiceFailList>>> GetReportFail(Int32? userId,Int32? dealerId,String? type, Int32? rowfrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReportFail(userId,dealerId, type,rowfrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<ServiceFailList>>> _GetReportFail(Int32? userId, Int32? dealerId,String? type, Int32? rowfrom)
        {
            Response<List<ServiceFailList>> _response = new Response<List<ServiceFailList>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VTYPE", type);




                Mapping _mapping = new Mapping();
                _mapping.AddItem("ReportId", "ID");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("Customer", "VCUSTOMER");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Paralyzed", "BPARALYZED");
                _mapping.AddItem("VehicleId", "IDVEHICLE");





                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORT_FAIL", _parameter);
                _response.Data = _data.GetList<Models.ServiceFailList>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }





        private async Task<Response<SaleOrder>> _getOne(Int32? userId, Int32? dealerId, Int32? rowfrom, string? filter, Int32? saleOrderId = null)
        {
            Response<SaleOrder> _response = new Response<SaleOrder>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
      
                _parameter.AddSqlParameter("@ID", saleOrderId);




                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("StatusId", "IDESTATUS");
                _mapping.AddItem("StatusName", "VESTATUS");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("TypeName", "VTYPE");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("Created", "VCREATED");
                _mapping.AddItem("CreatedBy", "VCREATEDBY");
                _mapping.AddItem("IsClaim", "BCLAIM");
                _mapping.AddItem("Paralyzed", "BPARALYZED");
                _mapping.AddItem("VehicleId", "IDVEHICLE");
                _mapping.AddItem("VehicleVin", "VVIN");
                _mapping.AddItem("VehicleCustomer", "VCUSTOMER");
                _mapping.AddItem("NameCreatedBy", "VNAMECREATEDBY");
                



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SALEORDERS", _parameter);
                _response.Data =  _data.GetItem<Models.SaleOrder>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<SaleOrder>> _getlast(Int32? userId, Int32? dealerId)
        {
            Response<SaleOrder> _response = new Response<SaleOrder>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
         

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("StatusId", "IDESTATUS");
                _mapping.AddItem("StatusName", "VESTATUS");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("TypeName", "VTYPE");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("Comment", "VCOMMENT");
                _mapping.AddItem("Created", "VCREATED");
                _mapping.AddItem("CreatedBy", "VCREATEDBY");
                _mapping.AddItem("IsClaim", "BCLAIM");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_LASTSALEORDER", _parameter);
                _response.Data = _data.GetItem<Models.SaleOrder>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response<SaleOrder>> GetOne(Int32? saleOrderId, Int32? userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getOne(userId, null, null, null, saleOrderId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<SaleOrder>> getLast(Int32? userId, Int32? dealerId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getlast(userId, dealerId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        //------------------------------------- Claim Reasons ------------------------------------------
        public async Task<Response<List<Models.Reason>>> GetClaimReasons()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetClaimReasons();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Reason>>> _GetClaimReasons()
        {
            Response<List<Models.Reason>> _response = new Response<List<Models.Reason>>();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Type", "CCLAIM");
                _mapping.AddItem("Description", "DESC");

                var _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CLAIM_REASONS");
                _response.Data = _data.GetList<Models.Reason>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        //-----------------------------------------------------------------------------------------------

        public async Task<Response<Result>> PostSaleOrder(List<Models.SaleOrder> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postSaleOrder(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response<Result>> _postSaleOrder(List<SaleOrder> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_POST_SALEORDERS", _parameter);
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
                DataTable _table = await _data.GetDataTable("USP_POST_SALEORDERS_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        

        public async Task<Response<List<SaleOrderDetail>>> GetDetails( Int32 saleOrderId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetDetails(saleOrderId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<SaleOrderDetail>>> _GetDetails(Int32 saleOrderId)
        {
            Response<List<SaleOrderDetail>> _response = new Response<List<SaleOrderDetail>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSALEORDER", saleOrderId);
 

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SaleOrdeId", "IDSALEORDER");
                _mapping.AddItem("PartId", "IDPART");
                _mapping.AddItem("ReasonId", "IDREASON");
                //  _mapping.AddItem("ReasonType", "CCLAIM");
                _mapping.AddItem("ReasonDescription", "VREASON"); 
                _mapping.AddItem("PartName", "VPART");
                _mapping.AddItem("PartInnerCode", "VINNERCODE");
                _mapping.AddItem("Quantity", "IREQUIRED");
                _mapping.AddItem("Cost", "NCOST");
                _mapping.AddItem("Price", "NPRICE");
                _mapping.AddItem("Claim", "ICLAIM");
                _mapping.AddItem("TaxAmount", "NTAX");
                _mapping.AddItem("SubTotal", "NSUBTOTAL");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SALEORDERDETAILS", _parameter);
                _response.Data = _data.GetList<Models.SaleOrderDetail>(_mapping, _table);
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<Response<Result>> PostDetail(Models.SaleOrderDetail saleOrderDetail, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _postDetail(saleOrderDetail, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _postDetail(SaleOrderDetail saleOrderDetail, Int32 userId)
        {
            Response<Result> _response = new Response<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(saleOrderDetail);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_SALEORDERDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response<Result>> DeleteDetails(List<Models.Detail> list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _deleteDetails(list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Result>> _deleteDetails(List<Models.Detail> _list, Int32 userId)
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
                DataTable _table = await _data.GetDataTable("USP_DELETE_SALEORDERDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response<List<SaleOrderType>>> GetSaleOrderTypes()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getSaleOrderTypes();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<SaleOrderType>>> _getSaleOrderTypes()
        {
            Response<List<SaleOrderType>> _response = new Response<List<SaleOrderType>>();
            try
            {

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SALEORDERTYPES");
                _response.Data = _data.GetList<Models.SaleOrderType>(_mapping, _table);
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
