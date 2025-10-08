
using System.Data;
using Models;
using Util;

namespace Data
{
    public class dVehicle
    {

        private readonly SemaphoreSlim _semaphore;
        public dVehicle()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Response<List<Models.Vehicle>>> GetAll(Int32 userId, Int32? supplierId, Int32? dealerId, Int32 rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(userId, supplierId, dealerId, rowFrom, filter, 0 );
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Vehicle>>> _GetAll(Int32 userId, Int32? supplierId, Int32? dealerId, Int32? rowFrom, string? filter,Int32 vehicleId = 0)
        {

            Response<List<Models.Vehicle>> _response = new Response<List<Models.Vehicle>>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@ID", vehicleId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("ColorId", "IDCOLOR");
                _mapping.AddItem("ColorName", "VCOLOR");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("SupplierReference", "VSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("DealerReference", "VDEALER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusName", "VESTATUS");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VEHICLES", _parameter);
                _response.Data = _data.GetList<Models.Vehicle>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }



        public async Task<Response<List<Models.VehicleInvoice>>> GetVehiclesInvoiced(Int32 userId, Int32? supplierId, Int32? dealerId, Int32 rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetVehiclesInvoiced(userId, supplierId, dealerId, rowFrom, filter, 0);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.VehicleInvoice>>> _GetVehiclesInvoiced(Int32 userId, Int32? supplierId, Int32? dealerId, Int32? rowFrom, string? filter, Int32 vehicleId = 0)
        {

            Response<List<Models.VehicleInvoice>> _response = new Response<List<Models.VehicleInvoice>>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("PolicyTypeName", "VPOLICYTYPE");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("ColorId", "IDCOLOR");
                _mapping.AddItem("ColorName", "VCOLOR");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("SupplierReference", "VSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("DealerReference", "VDEALER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("CustomerLastName", "VCUSTOMERLASTNAME");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("Phone", "VPHONE1");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("PolicyId", "IDPOLICY");
                _mapping.AddItem("PolicyNumber", "VNUMBER");
                _mapping.AddItem("EstatusPolicyId", "IESTATUSPOLICY");
                _mapping.AddItem("EstatusPolicyName", "VESTATUSPOLICY");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusId", "IESTATUS");
                _mapping.AddItem("EstatusName", "VESTATUS");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VEHICLES_INVOICE", _parameter);
                _response.Data = _data.GetList<Models.VehicleInvoice>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        public async Task<Response<Models.Vehicle>> GetOne(Int32 userId,Int32 vehicleId )
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(userId,vehicleId );
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Vehicle>> GetOneBy(Int32 userId, Int32? dealerId ,string filter, Int32 filterBy)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOneBy( userId, dealerId,filter,  filterBy);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.Vehicle>>> GetAllAvailables(Int32 userId, Int32 dealerId, Int32 rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAvailables(userId, dealerId, rowFrom, filter, true);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.Vehicle>>> GetOneAvailable(Int32 userId, Int32 dealerId, string VinOrPlate)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAvailables(userId, dealerId, 0, VinOrPlate, false );
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Vehicle>> _GetOne(Int32 userId,Int32 vehicleId)
        {
            Response<Models.Vehicle> _response = new Response<Models.Vehicle>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", null);
                _parameter.AddSqlParameter("@IROWFROM", 0);
                _parameter.AddSqlParameter("@IDUSER", 0);
                _parameter.AddSqlParameter("@ID", vehicleId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("ColorId", "IDCOLOR");
                _mapping.AddItem("ColorName", "VCOLOR");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("SupplierReference", "VSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("DealerReference", "VDEALER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusName", "VESTATUS");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VEHICLES", _parameter);
                _response.Data = _data.GetItem<Models.Vehicle>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        private async Task<Response<Models.Vehicle>> _GetOneBy(Int32 userId, Int32? dealerId , string filter, Int32 filterBy)
        {
            Response<Models.Vehicle> _response = new Response<Models.Vehicle>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BY", filterBy);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("ColorId", "IDCOLOR");
                _mapping.AddItem("ColorName", "VCOLOR");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("SupplierReference", "VSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("DealerReference", "VDEALER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusName", "VESTATUS");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("PolicyTypeName", "VPOLICYTYPE");

                
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VEHICLES_BY", _parameter);
                _response.Data = _data.GetItem<Models.Vehicle>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        private async Task<Response<List<Models.Vehicle>>> _GetAvailables(Int32 userId, Int32 dealerId,Int32 rowFrom, string? filter, bool catalog)
        {
            Response<List<Models.Vehicle>> _response = new Response<List<Models.Vehicle>>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BCATALOG", catalog);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "VMODEL");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("ColorId", "IDCOLOR");
                _mapping.AddItem("ColorName", "VCOLOR");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "VSUPPLIER");
                _mapping.AddItem("SupplierReference", "VSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("DealerReference", "VDEALER");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("CustomerName", "VCUSTOMER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("EstatusName", "VESTATUS");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("PolicyTypeName", "VPOLICYTYPE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VEHICLES_AVAILABLE", _parameter);
                _response.Data = _data.GetList<Models.Vehicle>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }


        public async Task<List<Vehicle>> GetExport(Int32 userId, Int32? supplierId, Int32? dealerId, string? _filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Vehicle>)(await _GetAll(userId,supplierId, dealerId,null, _filter, 0)).Data ;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response<Models.VehicleFull>> GetVehicleFullBy(Int32 userId, string filter, Int32 filterBy,Int32? supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetVehicleFullBy(userId, filter, filterBy,supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async Task<Response<Models.VehicleFull>> _GetVehicleFullBy(Int32 userId, string filter, Int32 filterBy, Int32? supplierId)
        {
            Response<Models.VehicleFull> _response = new Response<Models.VehicleFull>();

            try
            {
                var _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@BY", filterBy);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);




                var _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vin", "VVIN");
                _mapping.AddItem("Plate", "VPLATE");
                _mapping.AddItem("EngineSerial", "VENGINESERIAL");
                _mapping.AddItem("ModelId", "IDMODEL");
                _mapping.AddItem("ModelName", "MODELNAME");
                _mapping.AddItem("EstatusId", "IESTATUS");
                _mapping.AddItem("EstatusName", "VESTATUSVEHICLE");
                _mapping.AddItem("CustomerId", "IDCUSTOMER");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("CustomerName", "VFIRSTNAME");
                _mapping.AddItem("CustomerLastName", "VLASTNAME");
                _mapping.AddItem("Phone", "VPHONE1");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("Direction", "VADDRESS");
                _mapping.AddItem("PolicyId", "IDPOLICY");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("PolicyTypeId", "IDPOLICYTYPE");
                _mapping.AddItem("PolicyTypeName", "VDESCRIPTION");
                _mapping.AddItem("InvoiceNumber", "VINVOICENUMBER");
                _mapping.AddItem("InvoiceAmount", "NINVOICEAMOUNT");
                _mapping.AddItem("ActivationDate", "DACTIVATIONDATE");
                _mapping.AddItem("DealerReference", "CODDEALER");
                _mapping.AddItem("SupplierReference", "CODSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("DealerName", "DEALERNAME");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("SupplierName", "SUPPLIERNAME");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("Year", "IYEAR");
                _mapping.AddItem("ColorId", "IDCOLOR");
                _mapping.AddItem("ColorName", "VCOLOR");
                _mapping.AddItem("InvoiceDate", "DINVOICEDATE");
                _mapping.AddItem("EstatusPolicyId", "IDESTATUSPOLICY");
                _mapping.AddItem("EstatusPolicyName", "VESTATUSPOLICY");
                _mapping.AddItem("LockDate", "DLOCKDATE");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("ExpirationDate", "DEXPIRATIONDATE");
                _mapping.AddItem("PayMethodId", "IDPAYMETHOD");
                _mapping.AddItem("Total", "TOTAL");



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_VEHICLEFULL_BY", _parameter);
                _response.Data = _data.GetItem<Models.VehicleFull>(_mapping, _table);
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response<Models.Result>> Post_Vehicles(List<Vehicle> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Vehicles(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        public async Task<Response<Models.Result>> Import_Vehicles(List<Vehicle> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Vehicles(_list, userId, true);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Result>> _Post_Vehicles(List<Vehicle> _list, Int32 userId,Boolean? import=false)
        {
            Response<Models.Result> _response = new Response<Models.Result>();    
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);
                
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BIMPORT", import);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();



                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_VEHICLES", _parameter);
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
                DataTable _table = await _data.GetDataTable("USP_POST_VEHICLES_ACTIONS", _parameter);
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
