using Azure;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Models;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WebApi.Controllers
{

    [Route("api/Attachment")]
    [ApiController]
    [Authorize]
    public class cAttachment : ControllerBase
    {

        private readonly dAttachment _dAttachment;

        public cAttachment(dAttachment dAttachment)
        {
            _dAttachment = dAttachment;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 recordId, string moduleName)
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
            var response = new Models.Response<List<Models.Result>>();
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

                string baseUrl = Util.Setting.AttachmentUrl;
                if (string.IsNullOrEmpty(baseUrl))
                {
                    response.SetError(new Exception("Ruta base de adjuntos no configurada."));
                    return StatusCode(response.Status, response);
                }

                // Obtener la ruta base desde la variable de entorno
                string attachmentUrl = Path.Combine($"\\\\{Environment.MachineName}", baseUrl);
                // Obtener la lista de módulos desde la base de datos
                var _modules = await _dAttachment.GetModule(null, userId);

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



        //[HttpPost("PostAttachment")]
        //public async Task<IActionResult> Post_Attachment(IFormFile file, int userId, string moduleName, int recordId)
        //{
        //    var response = new Response<Models.Result>();

        //    try
        //    {
        //        // Validar archivo recibido
        //        if (file == null || file.Length == 0)
        //        {
        //            response.SetError(new Exception("No se recibió ningún archivo o el archivo está vacío."));
        //            return BadRequest(response);
        //        }

        //        // Validar tamaño máximo (15 MB = 15 * 1024 * 1024 bytes)
        //        const long maxFileSize = 8 * 1024 * 1024;
        //        if (file.Length > maxFileSize)
        //        {
        //            response.SetError(new Exception("El archivo excede el tamaño máximo permitido de 8 MB."));
        //            return StatusCode(response.Status, response);
        //        }

        //        // Obtener la ruta base desde la configuración
        //        string baseUrl = Util.Setting.AttachmentUrl;
        //        if (string.IsNullOrEmpty(baseUrl))
        //        {
        //            response.SetError(new Exception("Ruta base de adjuntos no configurada."));
        //            return StatusCode(response.Status, response);
        //        }

        //        string servicesUrl = Path.Combine($"\\\\{Environment.MachineName}", baseUrl);

        //        // Obtener el módulo desde la base de datos
        //        var modules = await _dModule.GetAll(moduleName, userId);
        //        var module = modules?.FirstOrDefault(m => m.Name == moduleName);

        //        if (module == null)
        //        {
        //            response.SetError(new Exception($"No se encontró el módulo '{moduleName}' para el usuario {userId}."));
        //            return StatusCode(response.Status, response);
        //        }

        //        string modulePath = module.Name;
        //        int moduleId = module.Id;

        //        // Construir ruta completa
        //        string basePath = Path.Combine(servicesUrl, modulePath);
        //        string recordPath = Path.Combine(basePath, recordId.ToString());

        //        // Crear carpetas si no existen
        //        Directory.CreateDirectory(recordPath);

        //        // Ruta final del archivo
        //        string filePath = Path.Combine(recordPath, file.FileName);

        //        // Verificar si el archivo ya existe
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            response.SetError(new Exception($"El archivo '{file.FileName}' ya existe."));
        //            return StatusCode(response.Status, response);
        //        }

        //        // Guardar el archivo
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Registrar en base de datos
        //        var attachment = new Models.Attachment
        //        {
        //            ModuleId = moduleId,
        //            RecordId = recordId,
        //            FileName = file.FileName
        //        };

        //        var dbResponse = await _dAttachment.Post_Attachment(attachment, userId);
        //        return StatusCode(dbResponse.Status, dbResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.SetError(ex);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}



        [HttpPost("PostAttachments")]
        public async Task<IActionResult> Post_Attachments(List<IFormFile> files, int userId, string moduleName, int recordId)
        {
            var response = new Models.Response<List<Models.Result>>();
            var results = new List<Models.Result>();

            try
            {
                // Validar archivos recibidos
                if (files == null || files.Count == 0)
                {
                    response.SetError(new Exception("No se recibió ningún archivo."));
                    return StatusCode(response.Status, response);
                }

                if (files.Count > 6)
                {
                    response.SetError(new Exception("Solo se permiten hasta 3 archivos por carga."));
                    return StatusCode(response.Status, response);
                }

                const long maxFileSize = 8 * 1024 * 1024;

                // Obtener ruta base
                string baseUrl = Util.Setting.AttachmentUrl;
                if (string.IsNullOrEmpty(baseUrl))
                {
                    response.SetError(new Exception("Ruta base de adjuntos no configurada."));
                    return StatusCode(response.Status, response);
                }

                string servicesUrl = Path.Combine($"\\\\{Environment.MachineName}", baseUrl);

                // Obtener módulo
                var modules = await _dAttachment.GetModule(moduleName, userId);
                var module = modules?.FirstOrDefault(m => m.Name == moduleName);
                if (module == null)
                {
                    response.SetError(new Exception($"El Usuario no posee permisos para cargar archivos"));
                    return StatusCode(response.Status, response);
                }

                string modulePath = module.Name;
                int moduleId = module.Id;
                string recordPath = Path.Combine(servicesUrl, modulePath, recordId.ToString());

                Directory.CreateDirectory(recordPath);

                foreach (var file in files)
                {
                    var result = new Models.Result();

                    try
                    {
                        if (file == null || file.Length == 0)
                            throw new Exception("Archivo vacío o no válido.");

                        if (file.Length > maxFileSize)
                            throw new Exception($"El archivo '{file.FileName}' excede el tamaño máximo permitido de 8 MB.");

                        string filePath = Path.Combine(recordPath, file.FileName);

                        if (System.IO.File.Exists(filePath))
                            throw new Exception($"El archivo '{file.FileName}' ya existe.");

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var attachment = new Models.Attachment
                        {
                            ModuleId = moduleId,
                            RecordId = recordId,
                            FileName = file.FileName
                        };

                        var dbResponse = await _dAttachment.Post_Attachment(attachment, userId);
                        // Puedes agregar el resultado a la lista si lo deseas
                        results.Add(dbResponse.Data);
                    }
                    catch (Exception ex)
                    {
                        // Puedes agregar un resultado de error a la lista si lo deseas
                        response.SetError(ex);
                    }
                }

                response.Data = results;
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                response.SetError(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        [HttpPost("Delete_Attachment")]
        public async Task<IActionResult> Delete_Attachment(int userId, int attachmentId)
        {
            try
            {
                Models.Response<Models.Result> response = new Models.Response<Models.Result>();
                // Obtener la información del adjunto desde la base de datos
                var _response = await _dAttachment.GetOne(attachmentId);

                if (_response == null || _response.Data == null)
                {
                    response.SetError(new Exception($"No se encontró el adjunto con ID {attachmentId}."));
                    return StatusCode(response.Status, response);
                }

                // Extraer el objeto Attachment desde la respuesta
                Models.Attachment attachment = ((List<Models.Attachment>)_response.Data).FirstOrDefault();

                if (attachment == null || string.IsNullOrEmpty(attachment.FileName) || attachment.ModuleId == 0)
                {

                    response.SetError(new Exception($"Datos insuficientes para reconstruir la ruta del archivo."));
                    return StatusCode(response.Status, response);
                }

                string baseUrl = Util.Setting.AttachmentUrl;
                if (string.IsNullOrEmpty(baseUrl))
                {
                    response.SetError(new Exception("Ruta base de adjuntos no configurada."));
                    return StatusCode(response.Status, response);
                }

                // Obtener la ruta base desde la variable de entorno
                string attachmentUrl = Path.Combine($"\\\\{Environment.MachineName}", baseUrl);

                // Obtener la lista de módulos desde la base de datos
                var _modules = await _dAttachment.GetModule(null, userId);
               
                if (_modules == null)
                {
                    response.SetError(new Exception($"El Usuario no posee permisos para eliminar archivos"));
                    return StatusCode(response.Status, response);
                }

                // Buscar el módulo correspondiente según el ModuleId del attachment
                string modulePath = _modules.FirstOrDefault(m => m.Id == attachment.ModuleId)?.Name ?? "Unknown";

                // Construir la ruta completa
                string filePath = Path.Combine(attachmentUrl, modulePath, attachment.RecordId.ToString(), attachment.FileName);

                var deleteResponse = await _dAttachment.Delete_Attachment(attachmentId, userId);

                if (deleteResponse.Status != StatusCodes.Status200OK)
                {

                    response.SetError(new Exception(deleteResponse.Message));
                    return StatusCode(response.Status, response);
                }

                // Verificar si el archivo existe antes de eliminarlo
                if (!System.IO.File.Exists(filePath))
                {
                    response.SetError(new Exception("El archivo no existe en la ruta especificada."));
                    return StatusCode(response.Status, response);
                }

                // Eliminar el archivo
                System.IO.File.Delete(filePath);

                // Ahora eliminar el registro de la base de datos
               
               
                return StatusCode(response.Status, deleteResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el archivo: {ex.Message}");
            }
        }





    }
}

