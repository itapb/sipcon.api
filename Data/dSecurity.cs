using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models;
using Util;

namespace Data
{
    public class dSecurity
    {

        private readonly SemaphoreSlim _semaphore;

        public dSecurity() 
        {
            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(100, 150);
        }

        public async Task<Models.Response> GetAccessGroup(string? filter, Int32 rowFrom)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
              return await _GetAccessGroup(filter, rowFrom);
            }
            finally
            {
                _semaphore.Release();
            }
        }
       
        

        private async Task<Models.Response> _GetAccessGroup(string? filter, Int32 rowFrom)
        {

            Models.Response _response = new Models.Response(true);
           
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@VFILTER", filter);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Name", "VNAME");
                _mapping.AddItem("IsActive", "BACTIVE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCESSGROUP", _parameter);
                _response.Data = _data.GetList<Models.AccessGroup>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }

       

        public async Task<Response> Post_AccessGroup(AccessGroup _accessGroup, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_AccessGroup(_accessGroup, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_AccessGroup(AccessGroup _accessGroup, Int32 userId)
        {
            
            
            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_accessGroup);
                

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();
                

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ACCESSGROUP", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> Post_AccessGroupDetails(List<AccessGroupDetail> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_AccessGroupDetails(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_AccessGroupDetails(List<AccessGroupDetail> _list, Int32 userId)
        {


            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ACCESSGROUPDETAILS", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Response> Post_AccessGroupUser(List<AccessGroupUser> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_AccessGroupUser(_list, userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_AccessGroupUser(List<AccessGroupUser> _list, Int32 userId)
        {


            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ACCESSGROUPUSER", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
        public async Task<Response> Post_CrendentialsUser(Models.CredentialLogin credentials,Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_CrendentialsUser(credentials,userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_CrendentialsUser(Models.CredentialLogin credentials, Int32 userId)
        {


            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(credentials);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);


                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_CREDENTIALSUSER", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> Post_TemporyKey(Models.CredentialLogin login)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_TemporyKey(login);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_TemporyKey(Models.CredentialLogin login)
        {


            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(login);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_TEMPORARYKEY", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response> Post_Password(Models.Credentials credentials)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Post_Password(credentials);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        private async Task<Response> _Post_Password(Models.Credentials credentials)
        {


            Response _response = new Response();

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(credentials);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_SET_PASSWORD", _parameter);
                _response.Data = _data.GetItem<Models.Result>(_mapping, _table);
                _response.SetPostResponse();

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        public async Task<Models.Response> GetAccessGroupDetails(Int32? rowFrom, Int32 groupAccessId, String? filter)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAccessGroupDetails(rowFrom,groupAccessId, filter);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetAccessGroupDetails( Int32? rowFrom, Int32 groupAccessId, String? filter)
        {

            Models.Response _response = new Models.Response(true);

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDACCESSGROUP", groupAccessId);
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@VFILTER", filter);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("AccessGroupId", "IDACCESGROUP");
                _mapping.AddItem("ActionId", "IDACTION");
                _mapping.AddItem("Action", "VACTION");
                _mapping.AddItem("ModuleId", "IDMODULE");
                _mapping.AddItem("Module", "VMODULE");
                _mapping.AddItem("ActionDisplay", "VACTIONDISPLAY");
                _mapping.AddItem("Assign", "BASSIGN");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCESSGROUPDETAILS", _parameter);
                _response.Data = _data.GetList<Models.AccessGroupDetail>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }




        public async Task<Models.Response> GetAccessGroupUser(Int32 rowFrom, Int32 userId,Boolean? assign)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAccessGroupUser(rowFrom,userId, assign);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetAccessGroupUser(Int32? rowFrom,Int32 userId, Boolean? assign)
        {

            Models.Response _response = new Models.Response(true);

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@IDUSER", userId);
                _parameter.AddSqlParameter("@BASSIGN", assign);


                Mapping _mapping = new Mapping();
                _mapping.AddItem("AccessGroupId", "IDACCESSGROUP");
                _mapping.AddItem("AccessGroup", "VACCESSGROUP");
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("UserName", "VFIRSTNAME");
                _mapping.AddItem("UserLastName", "VLASTNAME");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("Assign", "BASSIGN");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCESSGROUPBYUSER", _parameter);
                _response.Data = _data.GetList<Models.AccessGroupUser>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }





        public async Task<Models.Response> GetAccessUserbyGroup(Int32 rowFrom, Int32 accessGroupId, String? filter, Boolean? assign)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetAccessUserbyGroup(rowFrom, accessGroupId, filter, assign);
            }
            finally
            {
                _semaphore.Release();
            }
        }



        private async Task<Models.Response> _GetAccessUserbyGroup(Int32? rowFrom, Int32 accessGroupId, String? filter, Boolean? assign)
        {

            Models.Response _response = new Models.Response(true);

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IROWFROM", rowFrom);
                _parameter.AddSqlParameter("@IDACCESSGROUP", accessGroupId);
                _parameter.AddSqlParameter("@BASSIGN", assign);
                _parameter.AddSqlParameter("@VFILTER", filter);



                Mapping _mapping = new Mapping();
                _mapping.AddItem("AccessGroupId", "IDACCESSGROUP");
                _mapping.AddItem("AccessGroup", "VACCESSGROUP");
                _mapping.AddItem("UserId", "IDUSER");
                _mapping.AddItem("UserName", "VFIRSTNAME");
                _mapping.AddItem("UserLastName", "VLASTNAME");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("Assign", "BASSIGN");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACCESSUSERBYGROUP", _parameter);
                _response.Data = _data.GetList<Models.AccessGroupUser>(_mapping, _table);
                _response.SetGetResponse(_table);


            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;

        }



        public async Task<Response> Auth_User(Models.AuthUser credentials)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Auth_User(credentials);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Auth_User(Models.AuthUser _credentials)
        {


            Response _response = new Response(true);

            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_credentials);


                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("Login", "VLOGIN");
                _mapping.AddItem("Name", "VFIRSTNAME");
                _mapping.AddItem("LastName", "VLASTNAME");
                _mapping.AddItem("Vat", "VVAT");
                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_AUTH_ACCESSUSER", _parameter);
                _response.Data = _data.GetItem<Models.User>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


         public async Task<Response> Get_CompanyByUser(Int32? userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Get_CompanyByUser(userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Get_CompanyByUser(Int32? userId)
        {


            Response _response = new Response(true);

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDCOMPANY");
                _mapping.AddItem("Name", "VCOMPANY");
                _mapping.AddItem("Type", "VTYPE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_COMPANYBYUSER", _parameter);
                _response.Data = _data.GetList<Models.Companies>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response> Get_ModulesByUser(Int32? userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Get_ModulesByUser(userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Get_ModulesByUser(Int32? userId)
        {


            Response _response = new Response(true);

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "IDMODULE");
                _mapping.AddItem("Name", "VMODULE");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_MODULESBYUSER", _parameter);
                _response.Data = _data.GetList<Models.Module>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }



        public async Task<Response> Get_ActionByUser(Int32? userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _Get_ActionByUser(userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Get_ActionByUser(Int32? userId)
        {


            Response _response = new Response();

            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("ModuleId", "IDMODULE");
                _mapping.AddItem("ActionId", "IDACTION");
                _mapping.AddItem("ActionName", "VACTION");

                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_ACTIONSBYUSER", _parameter);
                _response.Data = _data.GetList<Models.ActionModule>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }


        public async Task<Response> Post_AccessGroup_Actions(List<Models.Action> _list, Int32 userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {

                return await _Post_AccessGroup_Actions(_list, userId);

            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response> _Post_AccessGroup_Actions(List<Models.Action> _list, Int32 userId)
        {
            Response _response = new Response();
            try
            {
                string _jsonstring = Util.Json.ConvertToJsonString(_list);

                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@DATA", _jsonstring);
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.SetDefaultPostMapping();


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_POST_ACCESSGROUP_ACTIONS", _parameter);
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
