using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using System.Data;
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

        public async Task<Response<List<Models.Inspection>>> GetAll()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Inspection>> GetOne(Int32 InspectionId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne(InspectionId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

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

        private async Task<Response<List<Models.Inspection>>> _GetAll()
        {
            Response<List<Models.Inspection>> _response = new Response<List<Models.Inspection>>();
            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDINSPECTION");
                _mapping.AddItem("CreatedBy", "ICREATEDBY");
                _mapping.AddItem("NameUser", "USER_NAME");
                _mapping.AddItem("IdVehicle", "IDVEHICLE");
                _mapping.AddItem("VehiclePlate", "VEHICLE_PLATE");
                _mapping.AddItem("IdArea", "IDAREA");
                _mapping.AddItem("NameArea", "AREA_NAME");
                _mapping.AddItem("DateReceipt", "DDATERECEIPT");
                _mapping.AddItem("DateInspection", "DDATEINSPECTION");
                _mapping.AddItem("Created", "DCREATED");
                _mapping.AddItem("Updated", "DUPDATED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_INSPECTIONS");
                _response.Data = _data.GetList<Models.Inspection>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.Inspection>> _GetOne(Int32 InspectionId)
        {
            Response<Models.Inspection> _response = new Response<Models.Inspection>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@ID", InspectionId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDINSPECTION");
                _mapping.AddItem("CreatedBy", "ICREATEDBY");
                _mapping.AddItem("NameUser", "USER_NAME");
                _mapping.AddItem("IdVehicle", "IDVEHICLE");
                _mapping.AddItem("VehiclePlate", "VEHICLE_PLATE");
                _mapping.AddItem("IdArea", "IDAREA");
                _mapping.AddItem("NameArea", "AREA_NAME");
                _mapping.AddItem("DateReceipt", "DDATERECEIPT");
                _mapping.AddItem("DateInspection", "DDATEINSPECTION");
                _mapping.AddItem("Created", "DCREATED");
                _mapping.AddItem("Updated", "DUPDATED");

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
    }
}