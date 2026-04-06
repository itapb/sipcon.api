using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using System.Data;
using Util;


namespace Data
{
    public class dPayment
    {
        private readonly SemaphoreSlim _semaphore;
        public dPayment()
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);
        }

        private async Task<Response<List<Models.Currency>>> _getCurrencys()
        {
            Response<List<Models.Currency>> _response = new Response<List<Models.Currency>>();

            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CURRENCYS");
                _response.Data = _data.GetList<Models.Currency>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        private async Task<Response<List<Models.PaymentType>>> _getPaymentTypes()
        {
            Response<List<Models.PaymentType>> _response = new Response<List<Models.PaymentType>>();

            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PAYMENTTYPES");
                _response.Data = _data.GetList<Models.PaymentType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        private async Task<Response<List<Models.DocumentType>>> _getDocumentTypes()
        {
            Response<List<Models.DocumentType>> _response = new Response<List<Models.DocumentType>>();

            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DOCUMENTTYPES");
                _response.Data = _data.GetList<Models.DocumentType>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        private async Task<Response<List<Models.DocumentConcept>>> _getDocumentConcepts()
        {
            Response<List<Models.DocumentConcept>> _response = new Response<List<Models.DocumentConcept>>();

            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("IsExclusive", "BEXCLUSIVE");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DOCUMENTCONCEPTS");
                _response.Data = _data.GetList<Models.DocumentConcept>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        private async Task<Response<List<Models.AccountReceivable>>> _getAccountReceivables(Int32 userId, Int32 supplierId, Int32 dealerId, string? typeCode, string? conceptCode, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId)
        {
            Response<List<Models.AccountReceivable>> _response = new Response<List<Models.AccountReceivable>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@VTYPE", typeCode);
                _parameter.AddSqlParameter("@VCONCEPT", conceptCode);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IDESTATUS", statusId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("TypeCode", "VTYPECODE");
                _mapping.AddItem("TypeName", "VTYPENAME");
                _mapping.AddItem("ConceptCode", "VCONCEPTCODE");
                _mapping.AddItem("ConceptName", "VCONCEPTNAME");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("DocumentDate", "VDATE");
                _mapping.AddItem("DocumentDueDate", "VDUEDATE");
                _mapping.AddItem("Amount", "NAMOUNT");
                _mapping.AddItem("Balance", "NBALANCE");
                _mapping.AddItem("Rate", "NRATE");
                _mapping.AddItem("StatusName", "VSTATUSNAME");
                _mapping.AddItem("StatusId", "ISTATUSID");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCOUNTRECEIVABLE", _parameter);
                _response.Data = _data.GetList<Models.AccountReceivable>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        private async Task<Response<List<Models.BankAccount>>> _getBankAccounts(Int32 supplierId)
        {
            Response<List<Models.BankAccount>> _response = new Response<List<Models.BankAccount>>();

            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Code", "VCODE");
                _mapping.AddItem("Account", "VACCOUNT");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_BANKACCOUNTS", _parameter);
                _response.Data = _data.GetList<Models.BankAccount>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        public async Task<Response<List<Models.Currency>>> GetCurrencys()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getCurrencys();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.PaymentType>>> GetPaymentTypes()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getPaymentTypes();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.DocumentType>>> GetDocumentTypes()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getDocumentTypes();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.DocumentConcept>>> GetDocumentConcepts()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getDocumentConcepts();
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.AccountReceivable>>> GetAccountReceivables(Int32 userId, Int32 supplierId, Int32 dealerId, string? typeCode, string? conceptCode, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getAccountReceivables( userId,  supplierId, dealerId, typeCode, conceptCode, rowfrom, filter, fromDate, upToDate, statusId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.BankAccount>>> GetBankAccounts(Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getBankAccounts(supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
