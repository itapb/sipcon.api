using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



namespace WebApi.Controllers
{
    [Route("api/Security")]
    [ApiController]
    public class cSecurity : ControllerBase
    {


        private readonly dSecurity _dSecurity;
        private readonly IConfiguration _config;
        private readonly RefreshTokenStore _refreshTokenStore;

        public cSecurity(dSecurity dSecurity, IConfiguration config, RefreshTokenStore refreshTokenStore)
        {
            _dSecurity = dSecurity;
            _config = config;
            _refreshTokenStore = refreshTokenStore;
        }



        [HttpGet("GetAccessGroup")]
        public async Task<IActionResult> GetAccessGroup(string? filter, Int32 rowFrom)

        {

            try
            {
                Response _response = await _dSecurity.GetAccessGroup(filter, rowFrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetAccessGroupDetails")]
        public async Task<IActionResult> GetAccessGroupDetails(Int32 groupAccessId, Int32? rowFrom, String? filter)

        {

            try
            {
                Response _response = await _dSecurity.GetAccessGroupDetails(rowFrom, groupAccessId, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }



        [HttpGet("GetAccessGroupUser")]
        public async Task<IActionResult> GetAccessGroupUser(Int32 rowFrom, Int32 userId,Boolean? assign)

        {

            try
            {
                Response _response = await _dSecurity.GetAccessGroupUser(rowFrom, userId, assign);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetAccessUserbyGroup")]
        public async Task<IActionResult> GetAccessUserbyGroup(Int32 rowFrom, Int32 accessGroupId, String? filter, Boolean? assign)

        {

            try
            {
                Response _response = await _dSecurity.GetAccessUserbyGroup(rowFrom,accessGroupId,filter, assign);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }




        [HttpPost("PostAccessGroup")]
        public async Task<IActionResult> Post_AccessGroup(Int32 userId,Models.AccessGroup accessGroup)
        {


            try
            {
                Response _response = await _dSecurity.Post_AccessGroup(accessGroup, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

      


        [HttpPost("PostAccessGroupDetails")]
        public async Task<IActionResult> Post_AccessGroupDetails(Int32 userId, List<Models.AccessGroupDetail> accessGroupDetail)
        {


            try
            {
                Response _response = await _dSecurity.Post_AccessGroupDetails(accessGroupDetail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("PostAccessGroupUser")]
        public async Task<IActionResult> Post_AccessGroupUser(Int32 userId, List<Models.AccessGroupUser> accessGroupUser)
        {


            try
            {
                Response _response = await _dSecurity.Post_AccessGroupUser(accessGroupUser, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("Post_CrendentialsUser")]
        public async Task<IActionResult> Post_CrendentialsUser(Models.CredentialLogin credentials,Int32 userId)
        {


            try
            {
                Response _response = await _dSecurity.Post_CrendentialsUser(credentials,userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("Post_TemporyKey")]
        public async Task<IActionResult> Post_TemporyKey(Models.CredentialLogin login)
        {


            try
            {
                Response _response = await _dSecurity.Post_TemporyKey(login);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("Post_Password")]
        public async Task<IActionResult> Post_Password(Models.Credentials credentials)
        {


            try
            {
                Response _response = await _dSecurity.Post_Password(credentials);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("Auth_User")]
        public async Task<IActionResult> Auth_User(Models.AuthUser credentials)
        {
            Response _response = new Response();
            try
            {
                // Initialize AuthResult with required properties
                AuthResult _data = new AuthResult
                {
                    Token = string.Empty, // Placeholder value
                    RefreshToken = string.Empty // Placeholder value
                };

                // 1. Autenticación y obtención del usuario
                var authResponse = await _dSecurity.Auth_User(credentials);
                if (authResponse.Data is not List<User> users || !users.Any())
                {
                    // manejar error de autenticación
                    _response.SetError(new Exception("USUARIO O CLAVE INVALIDOS"));
                    return StatusCode(_response.Status, _response);
                }

                User user = users.First();
                _data.Users = user;

                // 2. Obtener todas las compañías relacionadas (suppliers + dealers mezclados)
                var companyResponse = await _dSecurity.Get_CompanyByUser(user.Id);
                if (companyResponse.Data is not List<Companies> allCompanies || allCompanies.Count == 0)
                {
                    _response.SetError(new Exception("No se pudieron obtener las compañías del usuario."));
                    return StatusCode(_response.Status, _response);
                }

                // 3. Separar por tipo
                _data.Suppliers = allCompanies.Where(c => c.Type == "SUPPLIER").ToList();
                _data.Dealers = allCompanies.Where(c => c.Type == "DEALER").ToList();

                var modulesResponse = await _dSecurity.Get_ModulesByUser(user.Id);
                if (modulesResponse?.Data is not List<Module> allModules || allModules.Count == 0)
                {
                    _response.SetError(new Exception("No se pudieron obtener los módulos asignados al usuario o la lista está vacía."));
                    return StatusCode(_response.Status, _response);
                }

                // Obtener acciones por usuario
                var actionsResponse = await _dSecurity.Get_ActionByUser(user.Id);
                if (actionsResponse?.Data is not List<ActionModule> allActions || allActions.Count == 0)
                {
                    _response.SetError(new Exception("No se pudieron obtener las acciones del usuario."));
                    return StatusCode(_response.Status, _response);
                }

                // Agrupar acciones por módulo
                var modulesWithActions = allModules
                    .Select(m => {
                        m.Actions = allActions
                            .Where(a => a.ModuleId == m.Id && a.ActionId.HasValue && !string.IsNullOrWhiteSpace(a.ActionName))
                            .DistinctBy(a => a.ActionId)
                            .ToList();
                        return m;
                    })
                    .ToList();

                // Asignar módulos enriquecidos
                _data.Modules = modulesWithActions;

                var jwtToken = GenerateJwtToken(user.Login, _config);
                var refreshToken = GenerateRefreshToken();

                // Guardar refreshToken por usuario si usas alguna clase tipo RefreshTokenStore
                _refreshTokenStore.Save(user.Login, refreshToken);

                // Empaquetar en la respuesta
                _data.Token = jwtToken;
                _data.RefreshToken = refreshToken;
                _response.Data = _data;
                _response.Total = 1;

                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                _response.Processed = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status409Conflict, _response);
            }
        }


        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] Models.RefreshRequest request)
        {
            Response _response = new Response();
            try
            {
                var savedRefreshToken = _refreshTokenStore.Get(request.Username);
                if (savedRefreshToken == request.RefreshToken)
                {
                    var newJwtToken = GenerateJwtToken(request.Username, _config);
                    var newRefreshToken = GenerateRefreshToken();

                    _refreshTokenStore.Save(request.Username, newRefreshToken);

                    var result = new AuthResult
                    {
                        Token = newJwtToken,
                        RefreshToken = newRefreshToken
                    };

                    _response.Data = result;
                    _response.Total = 1;
                    return StatusCode(_response.Status, _response);
                }
                else
                {
                    _response.SetError(new Exception("Refresh token inválido o expirado."));
                    return StatusCode(_response.Status, _response);
                }
            }
            catch (Exception ex)
            {
                _response.Processed = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status409Conflict, _response);
            }
        }


        private string GenerateJwtToken(string username, IConfiguration config)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, username),
        new Claim("UsuarioAutenticado", "true"),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(int.Parse(config["Jwt:TokenExpire"]));

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        [HttpPost("Post_AccessGroup_Actions")]
        public async Task<IActionResult> Post_AccessGroup_Actions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                Response _response = await _dSecurity.Post_AccessGroup_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


    }
}