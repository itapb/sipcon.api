using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        public async Task<Response> GetAll(Int32? userId, Int32? dealerId, Int32? rowfrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, dealerId, rowfrom, filter, null);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _GetAll(Int32? userId, Int32? dealerId, Int32? rowfrom, string? filter, Int32? saleOrderId = null)
        {
            Response _response = (saleOrderId == null) ? new Response(true) : new Response();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BCLAIM", false);
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


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_SALEORDERS", _parameter);
                _response.Data = (saleOrderId == null) ? _data.GetList<Models.SaleOrder>(_mapping, _table) : _data.GetItem<Models.SaleOrder>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response> GetOne(Int32? saleOrderId, Int32? userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, null, null, null, saleOrderId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response> GetReasons()
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

        private async Task<Response> _GetReasons()
        {
            Response _response = new Response(true);
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
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


        public async Task<Response> PostSaleOrder(List<Models.SaleOrder> _list, Int32 userId)
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


        private async Task<Response> _postSaleOrder(List<SaleOrder> _list, Int32 userId)
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
        private async Task<Response> _Post_Actions(List<Models.Action> _list, Int32 userId)
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
        

        public async Task<Response> GetDetails( Int32 saleOrderId)
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

        private async Task<Response> _GetDetails(Int32 saleOrderId)
        {
            Response _response = new Response(true);
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSALEORDER", saleOrderId);
 

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SaleOrdeId", "IDSALEORDER");
                _mapping.AddItem("PartId", "IDPART");
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


        public async Task<Response> PostDetail(Models.SaleOrderDetail saleOrderDetail, Int32 userId)
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

        private async Task<Response> _postDetail(SaleOrderDetail saleOrderDetail, Int32 userId)
        {
            Response _response = new Response();
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


        public async Task<Response> DeleteDetails(List<Models.Detail> list, Int32 userId)
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

        private async Task<Response> _deleteDetails(List<Models.Detail> _list, Int32 userId)
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


        public async Task<Response> GetSaleOrderTypes()
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

        private async Task<Response> _getSaleOrderTypes()
        {
            Response _response = new Response(true);
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
