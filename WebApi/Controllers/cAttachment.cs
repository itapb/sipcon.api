using System.Net.Mail;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{

    [Route("api/Attachment")]
    [ApiController]
    [Authorize]
    public class cAttachment : ControllerBase
    {

        private readonly dAttachment _dAttachment;
        private readonly dModule _dModule;

        public cAttachment(dAttachment dAttachment, dModule dModule)
        {
            _dAttachment = dAttachment;
            _dModule = dModule;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 recordId, String moduleName)
        {

            try
            {
                var _response = await _dAttachment.GetAll(moduleName,recordId );
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 userId,Int32 attachmentId)
        {
            try
            {
                // Obtener la información del adjunto desde la base de datos
                var _response = await _dAttachment.GetOne(attachmentId);

                if (_response == null || _response.Data == null)
                {
                    return NotFound($"No se encontró el adjunto con ID {attachmentId}.");
                }

                // Extraer el objeto Attachment desde la respuesta
                Models.Attachment attachment = ((List<Models.Attachment>)_response.Data).First();
               
                if (attachment == null || string.IsNullOrEmpty(attachment.FileName) || attachment.ModuleId == 0)
                {
                    return NotFound($"Datos insuficientes para reconstruir la ruta del archivo.");
                }

                // Obtener la ruta base desde la variable de entorno
                string attachmentUrl = $@"\\{Environment.MachineName}{Util.Setting.AttachmentUrl}\";
                // Obtener la lista de módulos desde la base de datos
                var _modules = await _dModule.GetAll(null, userId);

                // Buscar el módulo correspondiente según el ModuleId del attachment
                string modulePath = _modules.FirstOrDefault(m => m.Id == attachment.ModuleId)?.Name ?? "Unknown";

                // Construir la ruta completa
                string filePath = Path.Combine(attachmentUrl, modulePath, attachment.RecordId.ToString(), attachment.FileName);

                // Verificar si el archivo existe
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"El archivo no existe en la ruta especificada: {filePath}");
                }

                // Leer el archivo y retornarlo
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(fileBytes, "application/octet-stream", attachment.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener el archivo: {ex.Message}");
            }
        }



        [HttpPost("PostAttachment")]
        public async Task<IActionResult> Post_Attachment(IFormFile file, int userId, String moduleName, int recordId)
        {
            try
            {
                // Obtener la variable de entorno y validar que no sea nula
                string servicesUrl = $@"\\{Environment.MachineName}{Util.Setting.AttachmentUrl}\";

                var _modules = await _dModule.GetAll(moduleName, userId);

                // Buscar el módulo correspondiente según el moduleId recibido
                var module = _modules.FirstOrDefault(m => m.Name == moduleName);
                string modulePath = module != null ? module.Name : "Unknown";
                string basePath = Path.Combine(servicesUrl, modulePath);
                var moduleId = module.Id;
                

                // Verificar y crear la carpeta del módulo si no existe
                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                // Verificar y crear la subcarpeta del recordId dentro del módulo
                string recordPath = Path.Combine(basePath, recordId.ToString());
                if (!Directory.Exists(recordPath))
                {
                    Directory.CreateDirectory(recordPath);
                }

                // Ruta completa del archivo
                string filePath = Path.Combine(recordPath, file.FileName);

                // Guardar el archivo si no existe ya
                if (!System.IO.File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, $"El archivo '{file.FileName}' ya existe en '{recordPath}'.");
                }

                // Crear el objeto Attachment con los datos recibidos
                Models.Attachment attachment = new Models.Attachment
                {
                    ModuleId = moduleId,
                    RecordId = recordId,
                    FileName = file.FileName
                };

                // Registrar la información en la base de datos
                var _response = await _dAttachment.Post_Attachment(attachment, userId);

                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error en la carga del archivo: {ex.Message}");
            }
        }

        [HttpPost("Delete_Attachment")]
        public async Task<IActionResult> Delete_Attachment(int userId, int attachmentId)
        {
            try
            {
                // Obtener la información del adjunto desde la base de datos
                var _response = await _dAttachment.GetOne(attachmentId);

                if (_response == null || _response.Data == null)
                {
                    return NotFound($"No se encontró el adjunto con ID {attachmentId}.");
                }

                // Extraer el objeto Attachment desde la respuesta
                Models.Attachment attachment = ((List<Models.Attachment>)_response.Data).FirstOrDefault();

                if (attachment == null || string.IsNullOrEmpty(attachment.FileName) || attachment.ModuleId == 0)
                {
                    return NotFound($"Datos insuficientes para reconstruir la ruta del archivo.");
                }

                // Obtener la ruta base desde la variable de entorno
                string attachmentUrl = $@"\\{Environment.MachineName}{Util.Setting.AttachmentUrl}\";

                // Obtener la lista de módulos desde la base de datos
                var _modules = await _dModule.GetAll(null, userId);

                // Buscar el módulo correspondiente según el ModuleId del attachment
                string modulePath = _modules.FirstOrDefault(m => m.Id == attachment.ModuleId)?.Name ?? "Unknown";

                // Construir la ruta completa
                string filePath = Path.Combine(attachmentUrl, modulePath, attachment.RecordId.ToString(), attachment.FileName);

                // Verificar si el archivo existe antes de eliminarlo
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"El archivo no existe en la ruta especificada: {filePath}");
                }

                // Eliminar el archivo
                System.IO.File.Delete(filePath);

                // Ahora eliminar el registro de la base de datos
                var deleteResponse = await _dAttachment.Delete_Attachment(attachmentId, userId);

                if (deleteResponse.Status != StatusCodes.Status200OK)
                {
                    return StatusCode(deleteResponse.Status, $"Error al eliminar el registro en la base de datos.");
                }

                return Ok($"El archivo '{attachment.FileName}' ha sido eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el archivo: {ex.Message}");
            }
        }





    }
}

