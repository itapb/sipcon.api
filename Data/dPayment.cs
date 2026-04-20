using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using System.Data;
using Util;
using Parameter = Util.Parameter;


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
                _mapping.AddItem("DealerName", "VDEALER");
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
        private async Task<Response<List<Models.EstatusRecord>>> _getDocumentStatus()
        {
            Response<List<Models.EstatusRecord>> _response = new Response<List<Models.EstatusRecord>>();

            try
            {
                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Display", "VDISPLAYESTATUS");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_DOCUMENTSTATUS");
                _response.Data = _data.GetList<Models.EstatusRecord>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }
        private async Task<Response<List<Models.PaymentStatus>>> _GetPaymentStatus(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, DateTime? fromDate, DateTime? upToDate)
        {
            Response<List<Models.PaymentStatus>> _response = new Response<List<Models.PaymentStatus>>();

            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("Count", "NCOUNT");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PAYMENTSTATUS", _parameter);
                _response.Data = _data.GetList<Models.PaymentStatus>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        private async Task<Response<List<Models.PaymentDetails>>> _GetPayments(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId)
        {
            Response<List<Models.PaymentDetails>> _response = new Response<List<Models.PaymentDetails>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);
                _parameter.AddSqlParameter("@IDDEALER", dealerId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@DFROMDATE", fromDate);
                _parameter.AddSqlParameter("@DUPTODATE", upToDate);
                _parameter.AddSqlParameter("@IDESTATUS", statusId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("PaymentId", "IDPAYMENT");
                _mapping.AddItem("Date", "DDATE");
                _mapping.AddItem("Amount", "NAMOUNT");
                _mapping.AddItem("Rate", "NRATE");
                _mapping.AddItem("DateRate", "DDATERATE");
                _mapping.AddItem("CurrencyName", "VCURRENCY");
                _mapping.AddItem("CurrencyId", "IDCURRENCY");
                _mapping.AddItem("TypeName", "VTYPE");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("BankName", "VBANK");
                _mapping.AddItem("BankId", "IDBANK");
                _mapping.AddItem("AccountId", "IDACCOUNT");
                _mapping.AddItem("AccountNumber", "VACCOUNTNUMBER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("StatusName", "VESTATUS");
                _mapping.AddItem("StatusId", "IDESTATUS");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PAYMENTDETAILS", _parameter);
                _response.Data = _data.GetList<Models.PaymentDetails>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<List<Models.AccountPreview>>> _GetAccountByPayment(Int32 userId, Int32? rowfrom, Int32 PaymentId)
        {
            Response<List<Models.AccountPreview>> _response = new Response<List<Models.AccountPreview>>();

            try
            {

                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@IDPAYMENT", PaymentId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Number", "VNUMBER");
                _mapping.AddItem("Amount", "NAMOUNT"); ;
                _mapping.AddItem("DocumentDate", "DCREATED");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCOUNT_BYPAYMENT", _parameter);
                _response.Data = _data.GetList<Models.AccountPreview>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<List<Models.PaymentDetails>>> _GetPaymentDetailsById(Int32 userId, Int32 rowfrom, int? PaymentDetailId)
        {
            Response<List<Models.PaymentDetails>> _response = new Response<List<Models.PaymentDetails>>();

            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@IROWFROM", rowfrom);
                _parameter.AddSqlParameter("@IDPAYMENTDETAIL", PaymentDetailId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("PaymentId", "IDPAYMENT");
                _mapping.AddItem("Date", "DDATE");
                _mapping.AddItem("Amount", "NAMOUNT");
                _mapping.AddItem("Rate", "NRATE");
                _mapping.AddItem("DateRate", "DDATERATE");
                _mapping.AddItem("CurrencyName", "VCURRENCY");
                _mapping.AddItem("CurrencyId", "IDCURRENCY");
                _mapping.AddItem("TypeName", "VTYPE");
                _mapping.AddItem("TypeId", "IDTYPE");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("BankName", "VBANK");
                _mapping.AddItem("BankId", "IDBANK");
                _mapping.AddItem("AccountId", "IDACCOUNT");
                _mapping.AddItem("AccountNumber", "VACCOUNTNUMBER");
                _mapping.AddItem("DealerId", "IDDEALER");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                _mapping.AddItem("DealerName", "VDEALER");
                _mapping.AddItem("StatusName", "VESTATUS");
                _mapping.AddItem("StatusId", "IDESTATUS");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PAYMENTDETAILS_BY_ID", _parameter);
                _response.Data = _data.GetList<Models.PaymentDetails>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }

        private async Task<Response<Models.Result>> _PostPayment(PostPaymentDetail payment, Int32 userId)
        {
            Response<Models.Result> _response = new Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(payment);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PAYMENT", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }



        private async Task<Response<Models.Result>> _Post_Actions(List<Models.Action> _list, Int32 userId)

        {
            Response<Models.Result> _response = new Models.Response<Models.Result>();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_PAYMENT_ACTIONS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

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

        public async Task<Response<List<Models.PaymentDetails>>> GetPayments(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, string? filter, DateTime? fromDate, DateTime? upToDate, int? statusId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPayments(userId, supplierId, dealerId, rowfrom, filter, fromDate, upToDate, statusId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response<List<Models.EstatusRecord>>> GetDocumentStatus()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _getDocumentStatus();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.PaymentStatus>>> GetPaymentStatus(Int32 userId, Int32 supplierId, Int32 dealerId, Int32 rowfrom, DateTime? fromDate, DateTime? upToDate)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPaymentStatus(userId, supplierId, dealerId, rowfrom, fromDate, upToDate);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.AccountPreview>>> GetAccountByPayment(Int32 userId,Int32? rowfrom,Int32 PaymentId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAccountByPayment(userId, rowfrom, PaymentId);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public async Task<Response<List<Models.PaymentDetails>>> GetPaymentDetailsById(Int32 userId, Int32 rowfrom, int? PaymentDetailId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPaymentDetailsById(userId, rowfrom, PaymentDetailId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.Result>> PostPayment(PostPaymentDetail payment, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _PostPayment(payment, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

       


        public async Task<Response<Models.Result>> Post_Actions(List<Models.Action> _list, Int32 userId)

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


        

    }
}
