
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Dynamic;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Models;
using Util;

namespace Data
{
    public class dContact
    {
        private readonly dBrand _dBrand;
        private readonly SemaphoreSlim _semaphore;

        public dContact(dBrand dBrand)
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
            _dBrand = dBrand;
        }


        public async Task<List<Models.Contact>> GetAll(string? _filter, int _rowFrom, Int32 idUser, string? moduleName )
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(_filter, _rowFrom, idUser,null,moduleName);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<Contact>> _GetAll(string? _filter, int? _rowFrom, Int32 idUser, Int32? idContact = null, string? moduleName = null)
        {
            List<Contact> _list = new List<Contact>();
            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", _filter);
                _parameter.AddSqlParameter("@IROWFROM", _rowFrom);
                _parameter.AddSqlParameter("@IDUSER", idUser);
                _parameter.AddSqlParameter("@ID", idContact);
                _parameter.AddSqlParameter("@VMODULE", moduleName);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Total", "TOTAL");
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("FiscalName", "VFISCALNAME");
                _mapping.AddItem("Address", "VADDRESS");
                _mapping.AddItem("Phone1", "VPHONE1");
                _mapping.AddItem("Phone2", "VPHONE2");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("login", "VLOGIN");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("IsDealer", "BDEALER");
                _mapping.AddItem("IsCustomer", "BCUSTOMER");
                _mapping.AddItem("IsSupplier", "BSUPPLIER");
                _mapping.AddItem("IsUser", "BUSER");
                _mapping.AddItem("IsProvider", "BPROVIDER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("CityId", "IDCITY");
                _mapping.AddItem("CityName", "VCITY");
                _mapping.AddItem("State", "VSTATE");
                _mapping.AddItem("Male", "BMALE");
                _mapping.AddItem("Birthday", "DBIRTHDATE");
                _mapping.AddItem("Type", "VTYPE");
                _mapping.AddItem("PayMethodId", "IDPAYMETHOD");
                _mapping.AddItem("GroupId", "IDGROUP");
                _mapping.AddItem("AgentId", "IDAGENT");
                _mapping.AddItem("AgentName", "VAGENT");
                _mapping.AddItem("Blocked", "BLOCKED");
                

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.Contact>("USP_GET_CONTACTS", _mapping, _parameter);


            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }
            return _list;
        }

        public async Task<Response<Models.Contact>> GetByVat(string vat, string contactType)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetByVat(vat, contactType);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.Contact>>> GetByContactType(string contactType, Int32 idUser, int rowFrom, string? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetByContactType(contactType, idUser, rowFrom, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<Models.Contact>> _GetByVat(string vat, string contactType)
        {
            Response<Models.Contact> _response = new Response<Models.Contact>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VVAT", vat);
                _parameter.AddSqlParameter("@VTYPE", contactType);


                Mapping _mapping = new Mapping();
              
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Address", "VADDRESS");
                _mapping.AddItem("Phone1", "VPHONE1");
                _mapping.AddItem("Phone2", "VPHONE2");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("login", "VLOGIN");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("IsDealer", "BDEALER");
                _mapping.AddItem("IsCustomer", "BCUSTOMER");
                _mapping.AddItem("IsSupplier", "BSUPPLIER");
                _mapping.AddItem("IsUser", "BUSER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("CityId", "IDCITY");
                _mapping.AddItem("CityName", "VCITY");
                _mapping.AddItem("State", "VSTATE");
                _mapping.AddItem("Male", "BMALE");
                _mapping.AddItem("Birthday", "DBIRTHDATE");
                _mapping.AddItem("Type", "VTYPE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CONTACTS_BYVAT", _parameter);
                _response.Data = _data.GetItem<Models.Contact>(_mapping, _table);
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        private async Task<Response<List<Models.Contact>>> _GetByContactType(string contactType, Int32 idUser,int rowFrom, string? filter)
        {
            Response<List<Models.Contact>> _response = new Response<List<Models.Contact>>();

            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VTYPE", contactType);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);


                Mapping _mapping = new Mapping();

                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Address", "VADDRESS");
                _mapping.AddItem("Phone1", "VPHONE1");
                _mapping.AddItem("Phone2", "VPHONE2");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("login", "VLOGIN");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("IsDealer", "BDEALER");
                _mapping.AddItem("IsCustomer", "BCUSTOMER");
                _mapping.AddItem("IsSupplier", "BSUPPLIER");
                _mapping.AddItem("IsUser", "BUSER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("CityId", "IDCITY");
                _mapping.AddItem("CityName", "VCITY");
                _mapping.AddItem("State", "VSTATE");
                _mapping.AddItem("Male", "BMALE");
                _mapping.AddItem("Birthday", "DBIRTHDATE");
                _mapping.AddItem("Type", "VTYPE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_CONTACTS_BY_TYPE", _parameter);
                _response.Data = _data.GetList<Models.Contact>(_mapping, _table);
                _response.SetGetResponse(_table);



            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }
        public async Task<List<Models.Contact>> GetOne(int idContact)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll(string.Empty, 0, 0, idContact);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<Contact>> GetAll_by( bool dealers, bool suppliers)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll_by(dealers, suppliers);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<Contact>> GetDealers(Int32 idSupplier)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll_by(true, false, idSupplier);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<Contact>> GetAll_bySupplier(Int32 idSupplier, bool dealers, bool suppliers)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAll_by( dealers, suppliers, idSupplier);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.Contact>>> GetUsersByModule(string moduleName, Int32 supplierId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetUsersByModule(moduleName, supplierId);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<List<Contact>> _GetAll_by( bool dealers, bool suppliers, Int32? idSupplier = null)
        {
            List<Contact> _list = new List<Contact>();
            try
            {


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDSUPPLIER", idSupplier);
                _parameter.AddSqlParameter("@BDEALERS", dealers);
                _parameter.AddSqlParameter("@BSUPPLIERS", suppliers);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Total", "TOTAL");
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Address", "VADDRESS");
                _mapping.AddItem("Phone1", "VPHONE1");
                _mapping.AddItem("Phone2", "VPHONE2");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("login", "VLOGIN");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("IsDealer", "BDEALER");
                _mapping.AddItem("IsCustomer", "BCUSTOMER");
                _mapping.AddItem("IsSupplier", "BSUPPLIER");
                _mapping.AddItem("IsUser", "BUSER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("CityId", "IDCITY");
                _mapping.AddItem("CityName", "VCITY");
                _mapping.AddItem("State", "VSTATE");
                _mapping.AddItem("Male", "BMALE");
                _mapping.AddItem("Birthday", "DBIRTHDATE");
                _mapping.AddItem("Type", "VTYPE");

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.Contact>("USP_GET_CONTACTS_BY", _mapping, _parameter);


            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }
            return _list;
        }

        private async Task<Response<List<Models.Contact>>> _GetUsersByModule(string moduleName, Int32 supplierId )
        {
            Response<List<Models.Contact>> _response = new Response<List<Models.Contact>>();
            try
            {


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VMODULE", moduleName);
                _parameter.AddSqlParameter("@IDSUPPLIER", supplierId);

                Mapping _mapping = new Mapping();
               
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Vat", "VVAT");
                _mapping.AddItem("FirstName", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Address", "VADDRESS");
                _mapping.AddItem("Phone1", "VPHONE1");
                _mapping.AddItem("Phone2", "VPHONE2");
                _mapping.AddItem("BrandId", "IDBRAND");
                _mapping.AddItem("BrandName", "VBRAND");
                _mapping.AddItem("Email", "VEMAIL");
                _mapping.AddItem("login", "VLOGIN");
                _mapping.AddItem("Reference", "VREFERENCE");
                _mapping.AddItem("IsDealer", "BDEALER");
                _mapping.AddItem("IsCustomer", "BCUSTOMER");
                _mapping.AddItem("IsSupplier", "BSUPPLIER");
                _mapping.AddItem("IsUser", "BUSER");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("CityId", "IDCITY");
                _mapping.AddItem("CityName", "VCITY");
                _mapping.AddItem("State", "VSTATE");
                _mapping.AddItem("Male", "BMALE");
                _mapping.AddItem("Birthday", "DBIRTHDATE");
                _mapping.AddItem("Type", "VTYPE");

              
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MODULECONTACTS", _parameter);
                _response.Data = _data.GetList<Models.Contact>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }
            return _response;
        }


        public async Task<List<Related>> GetRelated(Int32 idUser)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetRelated(idUser);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<Related>> _GetRelated(Int32 idUser)
        {
            List<Related> _list = new List<Related>();
            try
            {

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", idUser);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("RecordId", "IDUSER");
                _mapping.AddItem("RecordName", "VUSER");
                _mapping.AddItem("IsDealer", "BDEALER");
                _mapping.AddItem("IsSupplier", "BSUPPLIER");
                _mapping.AddItem("IsRelated", "BRELATED");
                _mapping.AddItem("RelatedId", "IDRELATED");
                _mapping.AddItem("RelatedName", "VRELATED");
                _mapping.AddItem("Reference", "VBRAND");
                _mapping.AddItem("SupplierId", "IDSUPPLIER");
                

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.Related>("USP_GET_RELATED", _mapping, _parameter);


            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }
            return _list;
        }


        public async Task<ContactWithContext> GetOne_WithContext(Int32 idContact)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetOne_WithContext(idContact);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<List<Models.PayMethod>>> GetPayMethods(bool? isDealer)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetPayMethods(isDealer);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.PayMethod>>> _GetPayMethods(bool? isDealer)
        {
            Response<List<Models.PayMethod>> _response = new Response<List<Models.PayMethod>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@BDEALER", isDealer);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_PAYMETHODS", _parameter);
                _response.Data = _data.GetList<Models.PayMethod>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        private async Task<ContactWithContext> _GetOne_WithContext(Int32 idContact)
        {

            ContactWithContext _data = new ContactWithContext();

            //if (idContact == 0)
            //{
            //    _data.Contact = new Contact();
            //}
            if (idContact > 0) 
            {
                List<Contact> _list = await GetOne(idContact);
                if (_list.Count > 0)
                {
                    _data.Contact = _list[0];
                }
                else
                {
                    _data.Contact = new Contact();
                    _data.Contact.BrandId = 1;
                }
            }

            _data.Brands = await _dBrand.GetAll(null);
            _data.Relateds = await GetRelated(idContact);
            _data.Cities = await GetCitys();
            _data.Groups = (List<Group>)(await GetGroups(true)).Data ;
            _data.PayMethods = (List<PayMethod>)(await GetPayMethods(true)).Data;

            return _data;

        }

        public async Task<List<Contact>> GetExport(string? _filter, Int32 userId,string? moduleName)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return (List<Contact>)(await _GetAll(_filter,null, userId,null, moduleName));
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<Response<List<Models.Group>>> GetGroups(bool? isDealer)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetGroups(isDealer);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.Group>>> _GetGroups(bool? isDealer)
        {
            Response<List<Models.Group>> _response = new Response<List<Models.Group>>();
            try
            {
                Util.Parameter _parameter = new Util.Parameter();
                _parameter.AddSqlParameter("@BDEALER", isDealer);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_GROUP", _parameter);
                _response.Data = _data.GetList<Models.Group>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

        public async Task<Result> Post_Contacts(List<Contact> _list, int _idUser)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Contacts(_list, _idUser);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Result> _Post_Contacts(List<Contact> _list, int _idUser)
        {
            List<Result> _results = new List<Result>();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", _idUser);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Value", "IRESULT");

                Util.Data _data = Util.Data.GetInstance();
                _results = await _data.ExecuteReaderAsync<Models.Result>("USP_POST_CONTACTS", _mapping, _parameter);

            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _results[0];
        }


        public async Task<bool> Post_Relateds(List<Related> _list)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Relateds(_list);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<City>> _GetCitys()
        {
            List<City> _list = new List<City>();
            try
            {


                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("State", "VSTATE");

                Util.Data _data = Util.Data.GetInstance();
                _list = await _data.ExecuteReaderAsync<Models.City>("USP_GET_CITYS", _mapping);


            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }
            return _list;
        }

        public async Task<List<City>> GetCitys()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetCitys();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<bool> _Post_Relateds(List<Related> _list)
        {
            bool _result = false;
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Util.Data _data = Util.Data.GetInstance();
                _result = await _data.ExecuteNonQueryAsync("USP_POST_RELATEDS", _parameter);

            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _result;
        }


        public async Task<bool> Post_Actions(List<Models.Action> _list, Int32 userId)
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

        private async Task<bool> _Post_Actions(List<Models.Action> _list,Int32 userId)
        {
            bool _result = false;
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Util.Data _data = Util.Data.GetInstance();
                _result = await _data.ExecuteNonQueryAsync("USP_POST_CONTACTS_ACTIONS", _parameter);

            }
            catch (Exception ex)
            {
                Util.Log.Error(ex);
                throw;
            }

            return _result;
        }




    }
}
