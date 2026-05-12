using Models;
using System.Data;
using Util;

namespace Data
{
    public class dBranchOffice
    {
        private readonly SemaphoreSlim _semaphore;
        public dBranchOffice()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);
        }

        public async Task<Response<List<Models.BranchOffice>>> GetAll(Int32? ContactId, bool? IsActive)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);

            try
            {
                return await _GetAll(ContactId, IsActive);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Result>> Post_BranchOffice(List<Models.BranchOffice> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);

            try
            {
                return await _Post_BranchOffice(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        
        private async Task<Response<List<Models.BranchOffice>>> _GetAll(Int32? ContactId, bool? IsActive)
        {
            Response<List<Models.BranchOffice>> _response = new Response<List<Models.BranchOffice>>();

            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDCONTACT", ContactId);
                _parameter.AddSqlParameter("@BACTIVE", IsActive);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("ContactId", "IDCONTACT");
                _mapping.AddItem("Address", "VADDRESS");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("CreatedBy", "VCREATEDBY");
                _mapping.AddItem("UpdatedBy", "VUPDATEDBY");
                _mapping.AddItem("DateCreated", "DCREATED");
                _mapping.AddItem("DateUpdated", "DUPDATE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_BRANCHOFFICE", _parameter);
                _response.Data = _data.GetList<Models.BranchOffice>(_mapping, _table);
                _response.SetGetResponse(_table);
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.Result>> _Post_BranchOffice(List<Models.BranchOffice> _list)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_BRANCHOFFICE", _parameter);
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
