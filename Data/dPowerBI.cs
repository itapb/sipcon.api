using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Models;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Util;

namespace Data
{
    public class dPowerBI
    {
        private readonly SemaphoreSlim _semaphore;

        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string PBI_grant_type;
        private readonly string PBI_resource;
        private readonly string PBI_client_id;
        private readonly string PBI_username;
        private readonly string PBI_password;
        private readonly string PBI_client_secret;
        private readonly string PBI_scope;

        public dPowerBI()
        {

            Util.Setting.GetSettings(true);
            _semaphore = new SemaphoreSlim(300, 500);

            PBI_grant_type = Util.Setting.grant_type;
            PBI_resource = Util.Setting.resource;
            PBI_client_id = Util.Setting.client_id;
            PBI_username = Util.Setting.username;
            PBI_password = Util.Setting.password;
            PBI_client_secret = Util.Setting.client_secret;
            PBI_scope = Util.Setting.scope;
        }

        public async Task<Response<List<Models.PowerBI_Reports>>> GetReports(string userId)
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetReports(userId);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Response<Models.PowerBI_Token>> GetTokenAPI()
        {
            await _semaphore.WaitAsync(Util.Setting.TimeOut);
            try
            {
                return await _GetTokenAPI();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Response<List<Models.PowerBI_Reports>>> _GetReports(string userId)
        {
            Response<List<Models.PowerBI_Reports>> _response = new Response<List<Models.PowerBI_Reports>>();
            try
            {
                Parameter _parameter = new Parameter();
                _parameter.AddSqlParameter("@IDUSER", userId);

                Mapping _mapping = new Mapping();
                _mapping.AddItem("Id", "ID");
                _mapping.AddItem("IsActive", "BACTIVE");
                _mapping.AddItem("Report", "VNAME");
                _mapping.AddItem("ReportId", "VREPORT");
                _mapping.AddItem("WorkSpace", "VWORKSPACE");
                _mapping.AddItem("WorkSpaceId", "IDWORKSPACE");


                Util.Data _data = Util.Data.GetInstance();
                DataTable _table = await _data.GetDataTable("USP_GET_REPORTS_POWERBI", _parameter);
                _response.Data = _data.GetList<Models.PowerBI_Reports>(_mapping, _table);
                _response.SetGetResponse(_table);

            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }

        private async Task<Response<Models.PowerBI_Token>> _GetTokenAPI()
        {
            Response<Models.PowerBI_Token> _response = new Response<Models.PowerBI_Token>();
            try
            {
                string url = "https://login.microsoftonline.com/common/oauth2/token";

                var postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", PBI_grant_type),
                    new KeyValuePair<string, string>("client_id", PBI_client_id),
                    new KeyValuePair<string, string>("client_secret", PBI_client_secret),
                    new KeyValuePair<string, string>("resource", PBI_resource),
                    new KeyValuePair<string, string>("username", PBI_username),
                    new KeyValuePair<string, string>("password", PBI_password),
                    new KeyValuePair<string, string>("scope", PBI_scope)
                };

                var content = new FormUrlEncodedContent(postData);

                HttpResponseMessage httpResponse = await _httpClient.PostAsync(url, content);
                string jsonResponse = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        string token = doc.RootElement.GetProperty("access_token").GetString();
                        var tokenObject = new Models.PowerBI_Token();
                        tokenObject.Token = token;
                        _response.Data = tokenObject;
                    }
                }
                else
                {
                    _response.SetError(new Exception($"Error API: {jsonResponse}"));
                }
            }
            catch (Exception ex)
            {
                _response.SetError(ex);
            }

            return _response;
        }
    }
}