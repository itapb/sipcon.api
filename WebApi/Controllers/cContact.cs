using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;
using System.Drawing;
using System.Threading;
using Microsoft.IdentityModel.Tokens;
using ClosedXML.Excel;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{

    [Route("api/Contact")]
    [ApiController]

    [Authorize]
    public class cContact : ControllerBase
    {

        private readonly dContact _dContact;

        public cContact(dContact dContact)
        {
            _dContact = dContact;

        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? filter , Int32 rowFrom, Int32 idUser, string? moduleName = null)
        {

            try
            {
                List<Contact> _contacts = await _dContact.GetAll(filter, rowFrom, idUser, moduleName);
                return StatusCode(StatusCodes.Status200OK, _contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 contactId)
        {
    
            try
            {
                List<Contact> _contacts = await _dContact.GetOne(contactId);
                return StatusCode(StatusCodes.Status200OK, _contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetSuppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
      
            try
            {
                List<Models.Contact> _contacts = await _dContact.GetAll_by(false, true);
                return StatusCode(StatusCodes.Status200OK, _contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetDealers")]
        public async Task<IActionResult> GetDealers(Int32 idSupplier)
        {

            try
            {
                List<Models.Contact> _contacts = await _dContact.GetAll_bySupplier(idSupplier,true, false);
                return StatusCode(StatusCodes.Status200OK, _contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }


        [HttpGet("GetUsersByModule")]
        public async Task<IActionResult> GetUsersByModule(string moduleName, Int32 supplierId)
        {
            try
            {
                var _response = await _dContact.GetUsersByModule(moduleName, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroups(bool? isDealer)
        {
            try
            {
                var _response = await _dContact.GetGroups(isDealer);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetByContactType")]
        public async Task<IActionResult> GetByContactType(string contactType, Int32 idUser, int rowFrom, string? filter)
        {
            try
            {
                var _response = await _dContact.GetByContactType( contactType,  idUser,  rowFrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetRelated")]
        public async Task<IActionResult> GetRelated(Int32 IdUser)
        {
    
            try
            {
                List<Models.Related> _contacts = await _dContact.GetRelated(IdUser);
                return StatusCode(StatusCodes.Status200OK, _contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }


        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities()
        {

            try
            {
                List<Models.City> _contacts = await _dContact.GetCitys();
                return StatusCode(StatusCodes.Status200OK, _contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetOne_WithContext")]
        public async Task<IActionResult> GetOne_WithContext(Int32 contactId)
        {

            try
            {
                ContactWithContext _data = await _dContact.GetOne_WithContext(contactId);
                return StatusCode(StatusCodes.Status200OK, _data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("GetByVat")]
        public async Task<IActionResult> GetByVat(string vat, string contactType)
        {

            try
            {
                var _response = await _dContact.GetByVat(vat, contactType);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private MemoryStream ConvertToExcel(List<Models.Contact> _contacts)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("CONTACTOS");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "RIF";
                worksheet.Cell(1, 3).Value = "NOMBRE";
                worksheet.Cell(1, 4).Value = "APELLIDO";
                worksheet.Cell(1, 5).Value = "DIRECCION";
                worksheet.Cell(1, 6).Value = "CIUDAD";
                worksheet.Cell(1, 7).Value = "ESTADO";
                worksheet.Cell(1, 8).Value = "TELEFONO1";
                worksheet.Cell(1, 9).Value = "TELEFONO2";
                worksheet.Cell(1, 10).Value = "CORREO";
                worksheet.Cell(1, 11).Value = "MARCA";
                worksheet.Cell(1, 12).Value = "REFERENCIA";
                worksheet.Cell(1, 13).Value = "GENERO";
                worksheet.Cell(1, 14).Value = "NACIMIENTO";
                worksheet.Cell(1, 15).Value = "TIPO";

                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:O1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                // 6. Llenar los datos
                for (int i = 0; i < _contacts.Count; i++)
                {
                    var _contacto = _contacts[i];
                    worksheet.Cell(i + 2, 1).Value = _contacto.Id;
                    worksheet.Cell(i + 2, 2).Value = _contacto.Vat;
                    worksheet.Cell(i + 2, 3).Value = _contacto.FirstName;
                    worksheet.Cell(i + 2, 4).Value = _contacto.LastName;
                    worksheet.Cell(i + 2, 5).Value = _contacto.Address;
                    worksheet.Cell(i + 2, 6).Value = _contacto.CityName;
                    worksheet.Cell(i + 2, 7).Value = _contacto.State;
                    worksheet.Cell(i + 2, 8).Value = _contacto.Phone1;
                    worksheet.Cell(i + 2, 9).Value = _contacto.Phone2;
                    worksheet.Cell(i + 2, 10).Value = _contacto.Email;
                    worksheet.Cell(i + 2, 11).Value = _contacto.BrandName;
                    worksheet.Cell(i + 2, 12).Value = _contacto.Reference;
                    worksheet.Cell(i + 2, 13).Value = _contacto.Male == true ? "M" : "F" ;
                    worksheet.Cell(i + 2, 14).Value = _contacto.Birthday;
                    worksheet.Cell(i + 2, 14).Style.DateFormat.Format = "dd/MM/yyyy"; // Formato fecha
                    worksheet.Cell(i + 2, 15).Value = _contacto.Type;
                }

                // 7. Ajustar el ancho de las columnas al contenido
                worksheet.Columns().AdjustToContents();

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;


                //// 9. Retornar el archivo Excel
                //return System.IO.File(
                //    content,
                //    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                //    "PersonasExportadas.xlsx");


            }

        }

        [HttpGet("GetExport")]
        public async Task<IActionResult> GetExport(string? _filter, Int32 userId, string moduleName)
        {

            try
            {
                
                List<Contact> _contacts = await _dContact.GetExport(_filter, userId, moduleName);
                MemoryStream _excel = ConvertToExcel(_contacts);
                string _fileName = "Contactos.xlsx";

                return File(
                 _excel,
                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 _fileName);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("Post_Contacts")]
        public async Task<IActionResult> Post_Contacts(List<Models.Contact> _contacts,Int32 userId)
        {

            try
            {
                var error = "";
                foreach (var contact in _contacts) { 
                if ((contact.Vat.Substring(0,1).ToUpper() == "V") || (contact.Vat.Substring(0, 1).ToUpper() == "E"))
                    {
                        if (string.IsNullOrEmpty(contact.LastName))
                        {
                            error = "Dato Apellido, es requerido para: " + contact.FirstName;
                            throw new Exception(error);
                        }
                    }
                    else
                    {
                        contact.LastName = string.Empty;
                    }

                }

                Result _resul = await _dContact.Post_Contacts(_contacts, userId);
                return StatusCode(StatusCodes.Status200OK, _resul);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }       

        }

        [HttpPost("Post_Relateds")]
        public async Task<IActionResult> Post_Relateds(List<Models.Related> _relateds)
        {

            try
            {
                bool resul = await _dContact.Post_Relateds(_relateds);
                return StatusCode(StatusCodes.Status200OK, resul);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPost("Post_Actions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> _actions, Int32 userId)
        {

            try
            {
                bool resul = await _dContact.Post_Actions(_actions, userId);
                return StatusCode(StatusCodes.Status200OK, resul);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }


    }
}
