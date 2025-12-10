using Azure;
using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using Data;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using Colors = QuestPDF.Helpers.Colors;

namespace WebApi.Controllers
{
    [Route("api/Service")]
    [ApiController]
    public class cService : ControllerBase
    {


        private readonly dService _dService;
        private readonly dAttachment _dAttachment;

        public cService(dService dService, dAttachment dAttachment)
        {
            _dService = dService;
            _dAttachment = dAttachment;

        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? filter, int rowFrom, int userId, int serviceTypeId, int dealerId,int? supplierId)
        {
            try
            {
                switch (serviceTypeId)
                {
                    case 1:
                        Models.Response<List<Models.ServiceMaintenance>> maintenanceResponse =
                            await _dService.GetAll<Models.ServiceMaintenance>(filter, rowFrom, userId, serviceTypeId, dealerId, supplierId);
                        return StatusCode(maintenanceResponse.Status, maintenanceResponse);

                    case 2:
                        Models.Response<List<Models.ServiceAssistance>> assistanceResponse =
                            await _dService.GetAll<Models.ServiceAssistance>(filter, rowFrom, userId, serviceTypeId, dealerId, supplierId);
                        return StatusCode(assistanceResponse.Status, assistanceResponse);

                    case 3:
                        Models.Response<List<Models.ServiceFail>> failResponse =
                            await _dService.GetAll<Models.ServiceFail>(filter, rowFrom, userId, serviceTypeId, dealerId, supplierId);
                        return StatusCode(failResponse.Status, failResponse);

                    default:
                        return StatusCode(StatusCodes.Status400BadRequest, $"Tipo de servicio no soportado: {serviceTypeId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(Int32 serviceId,String? type ,String? filter)
        {

            try
            {
                var _response = await _dService.GetDetails(serviceId, type,filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }


        [HttpGet("GetItems")]
        public async Task<IActionResult> GetItems(int userId, String type, int supplierId, string? filter, int rowFrom,int modelId)
        {
            try
            {
                if (type == "L")
                {
                    Models.Response<List<Models.LabortimeItem>> _response = await _dService.GetItemsLaborTime(userId, type, supplierId, filter, rowFrom);
                    return StatusCode(_response.Status, _response);
                }
                else if (type == "P")
                {
                    Models.Response<List<Models.PartItem>> _response = await _dService.GetItemsParts(userId, type, supplierId, filter, rowFrom);
                    return StatusCode(_response.Status, _response);
                }
                else
                {
                    throw new ArgumentException("Tipo de items no válido");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }




        [HttpGet("GetOneItem")]
        public async Task<IActionResult> GetOneItem(int userId, String type, int itemId)
        {
            try
            {
                if (type == "L")
                {
                    Models.Response<Models.LabortimeItem> _response = await _dService.GetOneItemLaborTime(userId, type, itemId);
                    return StatusCode(_response.Status, _response);
                }
                else if (type == "P")
                {
                    Models.Response<Models.PartItem> _response = await _dService.GetOneItemParts(userId, type, itemId);
                    return StatusCode(_response.Status, _response);
                }
                else
                {
                    throw new ArgumentException("Tipo de items no válido");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        private MemoryStream ConvertToExcel(List<Models.Service> _services, Int32? serviceTypeId)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                IXLWorksheet worksheet;
                List<string> headers = new();

                // 1. Crear hoja y encabezados según tipo de servicio
                switch (serviceTypeId)
                {
                    case 1:
                        worksheet = workbook.Worksheets.Add("MANTENIMIENTOS");
                        headers = new()
                {
                    "NUM ", "NUMERO DE ORDEN", "CONCESIONARIO DEL MANTENIMIENTO", "CODIGO CONCESIONARIO",
                    "VIN DE VEHICULO", "PLACA", "KM", "AÑO", "MODELO", "CONCESIONARIO DE VENTA",
                    "CODIGO CONCESIONARIO", "RIF CLIENTE", "NOMBRE DE CLIENTE", "APELLIDO DE CLIENTE","REPORTE DE CONCESIONARIO",
                    "NUM FACTURA", "FECHA FACTURACION", "ESTATUS"
                };
                        break;

                    case 2:
                        worksheet = workbook.Worksheets.Add("ASISTENCIAS TECNICAS");
                        headers = new()
                {
                    "NUM ", "NUMERO DE REPORTE DE FALLA",  "CONCESIONARIO DEL SERVICIO", "CODIGO CONCESIONARIO",
                    "VIN DE VEHICULO", "PLACA","REPORTE DE CLIENTE","REPORTE DE CONCESIONARIO","REPORTE DE PLANTA",
                     "SIST. RELACIONADO CON POSIBLE FALLA","TIPO DE ASISTENCIA", "KM","AÑO", "MODELO","RIF CLIENTE", "NOMBRE DE CLIENTE", "APELLIDO DE CLIENTE", "NOMBRE DE AUTORIZADO","FECHA DE INICIO",
                     "FECHA DE FINALIZACION","CALIFICACION" ,"ESTATUS"
                };
                        break;

                    case 3:
                        worksheet = workbook.Worksheets.Add("REPORTE DE FALLAS");
                        headers = new()
                {
                    "NUM ", "NUMERO DE ORDEN", "CONCESIONARIO DEL SERVICIO", "CODIGO CONCESIONARIO",
                    "VIN DE VEHICULO", "PLACA", "KM", "AÑO", "MODELO", "CONCESIONARIO DE VENTA", "CODIGO CONCESIONARIO",
                    "RIF CLIENTE", "NOMBRE DE CLIENTE", "APELLIDO DE CLIENTE","REPORTE DE CLIENTE","REPORTE DE CONDICIONES Y POSIBLES CAUSAS","DIAGNOSTICO DE CONCESIONARIO","REPORTE DE PLANTA",
                     "NOMBRE DE AUTORIZADO","SRG NUM", "NUM FACTURA", "MONTO FACTURACION", "FECHA FACTURACION",
                    "ESTATUS"
                };
                        break;

                    default:
                        throw new ArgumentException("Tipo de servicio no válido");
                }

                // 2. Agregar encabezados
                for (int col = 0; col < headers.Count; col++)
                {
                    if (!string.IsNullOrWhiteSpace(headers[col]))
                        worksheet.Cell(1, col + 1).Value = headers[col];
                }

                // 3. Estilo de encabezados
                var headerRange = worksheet.Range(1, 1, 1, headers.Count);
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;
                headerRange.SetAutoFilter();

                var colorMap = new Dictionary<string, XLColor>
        {
            { "Validado", XLColor.GreenYellow },
            { "Rechazado", XLColor.Red },
            { "Para Validar", XLColor.Orange },
            { "Para Aprobar", XLColor.GreenYellow },
            { "Aprobado", XLColor.Green },
            { "Procesado", XLColor.Blue }
        };

                // 4. Filtrar datos
                var filteredServices = _services.Where(s => s.ServiceTypeId == serviceTypeId).ToList();

                // 5. Insertar datos
                for (int i = 0; i < filteredServices.Count; i++)
                {
                    var s = filteredServices[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = s.Id;

                    if (serviceTypeId == 1)
                    {
                        worksheet.Cell(row, 2).Value = s.OrderNumber;
                        worksheet.Cell(row, 3).Value = s.DealerServiceName;
                        worksheet.Cell(row, 4).Value = s.DealerSaleCod;
                        worksheet.Cell(row, 5).Value = s.Vin;
                        worksheet.Cell(row, 6).Value = s.Plate;
                        worksheet.Cell(row, 7).Value = s.Km;
                        worksheet.Cell(row, 8).Value = s.Year;
                        worksheet.Cell(row, 9).Value = s.ModelName;
                        worksheet.Cell(row, 10).Value = s.DealerSaleName;
                        worksheet.Cell(row, 11).Value = s.DealerSaleCod;
                        worksheet.Cell(row, 12).Value = s.Vat;
                        worksheet.Cell(row, 13).Value = s.CustomerName;
                        worksheet.Cell(row, 14).Value = s.CustomerLastName;
                        worksheet.Cell(row, 15).Value = s.DealerReport;
                        worksheet.Cell(row, 16).Value = s.InvoiceNumber;
                        worksheet.Cell(row, 17).Value = s.InvoiceDate;
                        worksheet.Cell(row, 17).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(row, 18).Value = s.EstatusName;
                    }
                    else if (serviceTypeId == 2)
                    {
                        worksheet.Cell(row, 2).Value = s.OrderNumber;
                        worksheet.Cell(row, 3).Value = s.DealerServiceName;
                        worksheet.Cell(row, 4).Value = s.DealerServiceCod;
                        worksheet.Cell(row, 5).Value = s.Vin;
                        worksheet.Cell(row, 6).Value = s.Plate;
                        worksheet.Cell(row, 7).Value = s.CustomerReport;
                        worksheet.Cell(row, 8).Value = s.DealerReport;
                        worksheet.Cell(row, 9).Value = s.TechnicalSolution;
                        worksheet.Cell(row, 10).Value = s.PossibleFault;
                        worksheet.Cell(row, 11).Value = s.AssistanceType;
                        worksheet.Cell(row, 12).Value = s.Km;
                        worksheet.Cell(row, 13).Value = s.Year;
                        worksheet.Cell(row, 14).Value = s.ModelName;
                        worksheet.Cell(row, 15).Value = s.Vat;
                        worksheet.Cell(row, 16).Value = s.CustomerName;
                        worksheet.Cell(row, 17).Value = s.CustomerLastName;
                        worksheet.Cell(row, 18).Value = s.AuthorizedUserName;
                        worksheet.Cell(row, 19).Value = s.StartDate;
                        worksheet.Cell(row, 19).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(row, 20).Value = s.EndDate;
                        worksheet.Cell(row, 20).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(row, 21).Value = s.Assesment;
                        worksheet.Cell(row, 22).Value = s.EstatusName;

                    }
                    else if (serviceTypeId == 3)
                    {
                        worksheet.Cell(row, 2).Value = s.OrderNumber;
                        worksheet.Cell(row, 3).Value = s.DealerServiceName;
                        worksheet.Cell(row, 4).Value = s.DealerSaleCod;
                        worksheet.Cell(row, 5).Value = s.Vin;
                        worksheet.Cell(row, 6).Value = s.Plate;
                        worksheet.Cell(row, 7).Value = s.Km;
                        worksheet.Cell(row, 8).Value = s.Year;
                        worksheet.Cell(row, 9).Value = s.ModelName;
                        worksheet.Cell(row, 10).Value = s.DealerSaleName;
                        worksheet.Cell(row, 11).Value = s.DealerSaleCod;
                        worksheet.Cell(row, 12).Value = s.Vat;
                        worksheet.Cell(row, 13).Value = s.CustomerName;
                        worksheet.Cell(row, 14).Value = s.CustomerLastName;
                        worksheet.Cell(row, 15).Value = s.CustomerReport;
                        worksheet.Cell(row, 16).Value = s.DealerReport;
                        worksheet.Cell(row, 17).Value = s.TechnicalSolution;
                        worksheet.Cell(row, 18).Value = s.SupplierReport;
                        worksheet.Cell(row, 19).Value = s.AuthorizedUserName;
                        worksheet.Cell(row, 20).Value = s.SrgNumber;
                        worksheet.Cell(row, 21).Value = s.InvoiceNumber;
                        worksheet.Cell(row, 22).Value = s.InvoiceAmount;
                        worksheet.Cell(row, 23).Value = s.InvoiceDate;
                        worksheet.Cell(row, 23).Style.DateFormat.Format = "dd/MM/yyyy";
                        worksheet.Cell(row, 24).Value = s.EstatusName;
                    }
                }
                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Centra contenido de las columnas 
                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;



            }

        }

        [HttpGet("Export")]
        public async Task<IActionResult> GetExport(string? filter, int serviceTypeId, int userId, int dealerId, int supplierId)
        {
            try
            {
                object rawData = null;

                switch (serviceTypeId)
                {
                    case 1:
                        var maintenanceResponse = await _dService.GetAll<ServiceMaintenance>(filter, null, userId, serviceTypeId, dealerId, supplierId);
                        if (maintenanceResponse.Status != 200 || maintenanceResponse.Data == null)
                            return StatusCode(maintenanceResponse.Status, maintenanceResponse.Message);
                        rawData = maintenanceResponse.Data;
                        break;

                    case 2:
                        var assistanceResponse = await _dService.GetAll<ServiceAssistance>(filter, null, userId, serviceTypeId, dealerId, supplierId);
                        if (assistanceResponse.Status != 200 || assistanceResponse.Data == null)
                            return StatusCode(assistanceResponse.Status, assistanceResponse.Message);
                        rawData = assistanceResponse.Data;
                        break;

                    case 3:
                        var failResponse = await _dService.GetAll<ServiceFail>(filter, null, userId, serviceTypeId, dealerId, supplierId);
                        if (failResponse.Status != 200 || failResponse.Data == null)
                            return StatusCode(failResponse.Status, failResponse.Message);
                        rawData = failResponse.Data;
                        break;

                    default:
                        return StatusCode(StatusCodes.Status400BadRequest, $"Tipo de servicio no soportado: {serviceTypeId}");
                }

                // Convertir a List<Service>
                var services = serviceTypeId switch
                {
                    1 => (rawData as IEnumerable<ServiceMaintenance>)?.Select(ConvertToService).ToList(),
                    2 => (rawData as IEnumerable<ServiceAssistance>)?.Select(ConvertToService).ToList(),
                    3 => (rawData as IEnumerable<ServiceFail>)?.Select(ConvertToService).ToList(),
                    _ => null
                };

                if (services == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo convertir la data a List<Service>");

                // Exportar a Excel
                var excel = ConvertToExcel(services, serviceTypeId);
                var fileName = $"SERVICIOS_{serviceTypeId}.xlsx";

                return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        //Método auxiliar para convertir cualquier servicio específico en Service
        private Models.Service ConvertToService(object serviceData)
        {
            if (serviceData is null)
                throw new InvalidOperationException("No se proporcionó un servicio válido.");

            // Convertir a JSON
            string jsonString = Util.Json.ConvertToJsonString(serviceData);

            if (string.IsNullOrEmpty(jsonString))
                throw new InvalidOperationException("No se pudo generar el JSON para el servicio proporcionado.");

            // Deserializar a Service
            return JsonConvert.DeserializeObject<Models.Service>(jsonString, Util.Json.GetJsonSerializerSettings())
                   ?? throw new InvalidOperationException("La deserialización del JSON devolvió un valor nulo.");
        }


        [HttpGet("ExportSRG")]
        public async Task<IActionResult> GetExportSRG(Int32 userId, Int32 dealerId, Int32 serviceId)
        {
            try
            {
                
                var response = await _dService.GetOne<ServiceFail>(userId, 3, dealerId,serviceId);

                // 2. Validar que la respuesta es exitosa y contiene datos
                if (response.Status != StatusCodes.Status200OK || response.Data == null)
                {
                    return NotFound("No se encontró Solicitud de Reembolso de Garantia solicitado");
                }

                // 3. Convertir los datos correctamente
                List<ServiceFail> services = new List<ServiceFail>();

              
                if (response.Data is IEnumerable<ServiceFail> serviceList)
                {
                    // Si ya es una lista/enumerable
                    services = serviceList.ToList();
                }
                else
                {
                    // Si no reconocemos el formato
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Formato de datos de Servicio no reconocido");
                }

                // 4. Validar que tenemos datos
                if (!services.Any())
                {
                    return NotFound("No se encontraron datos válidos de servicio");
                }



                var estadosPermitidos = new[] { "APROBADO", "GENERADO", "PROCESADO" };

                if (!services.All(ser => estadosPermitidos.Contains(ser.EstatusName)))
                {
                    return NotFound("El reporte no puede imprimirse");
                }


                // 5. Generar el PDF
                byte[] pdfBytes = await GeneratePdfReportAsync(services, serviceId);
                string reportTypeName = services.FirstOrDefault()?.ReportTypeName ?? "SinTipo";
                string fileName = $"S.R.{reportTypeName}_{services.FirstOrDefault()?.SrgNumber ?? "reporte"}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        private async Task<string> GetFirstAttachmentFilePath(string moduleName, int? recordId)
        {
            var _response = await _dAttachment.GetAll(moduleName, recordId);
            if (_response?.Data == null) return null;

            var attachment = ((List<Models.Attachment>)_response.Data).FirstOrDefault();
            if (attachment == null || string.IsNullOrEmpty(attachment.FileName)) return null;

            string attachmentUrl = $@"\\{Environment.MachineName}{Util.Setting.AttachmentUrl}\";
            var _modules = moduleName;

            return Path.Combine(attachmentUrl, _modules, attachment.RecordId.ToString(), attachment.FileName);
        }

        [Obsolete]
        private async Task<byte[]> GeneratePdfReportAsync(List<ServiceFail> services, Int32 serviceId)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var service = services.First();
            string reportTypeName = services.FirstOrDefault()?.ReportTypeName ?? "SinTipo";
            string fileTitle = $"SOLICITUD DE REEMBOLSO {reportTypeName}";


            int? brandId = services.First().BrandId;
            int? supplierId = services.First().SupplierId;

            string brandImagePath = await GetFirstAttachmentFilePath("RECURSOS-MARCAS", brandId);
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", supplierId);


            // 🔄 Cargar los datos antes del documento

            var response = _dService.GetDetails(serviceId, null, null).GetAwaiter().GetResult();

            var serviceDetailsList = new List<ServiceDetails>();

            if (response.Data is IEnumerable<ServiceDetails> detailList)
                serviceDetailsList = detailList
                    .Where(detalle => detalle.EstatusId != 19)
                    .ToList();


            decimal? totalManoObra = serviceDetailsList
           .Where(x => x.Type == "L")
           .Sum(x => x.Price);

            decimal? totalRepuestos = serviceDetailsList
                .Where(x => x.Type == "P")
                .Sum(x => x.Price);

            decimal? subTotal = totalManoObra + totalRepuestos;
            decimal? totalGeneral = service.InvoiceAmount;


            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.Ledger.Landscape());
                    page.Content().Column(col =>
                    {
                        try
                        {


                            col.Item().PaddingBottom(2).Row(row =>
                            {
                                row.RelativeItem()
                                    .Width(200)
                                    .Image(brandImagePath)
                                    .FitArea();  // Imagen pegada a la esquina superior izquierda

                                row.RelativeItem()
                                    .Text(fileTitle)
                                    .Bold()
                                    .FontSize(15)
                                    .AlignCenter();

                                row.RelativeItem()
                                    .PaddingLeft(80)
                                    .Width(150)
                                    .Image(supplierImagePath)
                                    .FitArea(); // Imagen pegada a la esquina superior derecha
                            });

                        }
                        catch (Exception ex)
                        {

                        }
                        // Datos del cliente y vehículo en recuadros con título arriba y valor abajo

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });
                            void AddTitleRow(params string[] titles)
                            {
                                foreach (var title in titles)
                                {
                                    table.Cell().Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text(title).Bold().FontSize(10);
                                }
                            }

                            void AddValueRow(params string[] values)
                            {
                                foreach (var value in values)
                                {
                                    table.Cell().Border(1).Padding(1.5f).Text(value).FontSize(11);
                                }
                            }

                            // Fila 1
                            AddTitleRow("Placa", "N° Póliza", "Fecha Venta", "Cliente", "RIF", "Nro");
                            AddValueRow(service.Plate, service.NumberPolicy, $"{service.InvoiceDate:yyyy-MM-dd}", $"{service.CustomerName} {service.CustomerLastName}", service.Vat, service.SrgNumber);

                            // Fila 2
                            AddTitleRow("Modelo Vehiculo", "Año", "Fecha Creacion", "Kilometraje", "Direccion", "Autoriza");
                            AddValueRow(service.ModelName, $"{service.Year}", $"{service.ServiceDate:yyyy-MM-d}", $"{service.Km} km", service.DealerAddress, $"{service.AuthorizedUserName}");

                            // Fila 3
                            AddTitleRow("VIN-Serial Completo", "Fecha de Salida", "Ciudad", "Estado", "Telefonos", "Fecha Autorizacion");
                            AddValueRow(service.Vin, $"{service.ServiceDate:yyyy-MM-dd}", service.DealerServiceCity, service.DealerServiceState, service.CustomerPhone, $"{service.AuthotizationDate:yyyy-MM-dd}");


                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Concesionario ocupará 3 columnas
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Fila independiente para "Concesionario"
                            table.Cell()
                                .Border(1)
                                .Background(Colors.Grey.Lighten1)
                                .Padding(1.5f)
                                .Text("Concesionario")
                                .Bold()
                                .FontSize(10); // Une 3 columnas para el título

                            table.Cell()
                                .Border(1)
                                .Background(Colors.Grey.Lighten1)
                                .Padding(1.5f)
                                .Text("Código")
                                .Bold()
                                .FontSize(10);

                            table.Cell()
                                .Border(1)
                                .Background(Colors.Grey.Lighten1)
                                .Padding(1.5f)
                                .Text("Ciudad")
                                .Bold()
                                .FontSize(10);
                           
                            table.Cell()
                                .Border(1)
                                .Background(Colors.Grey.Lighten1)
                                .Padding(1.5f)
                                .Text("Factura")
                                .Bold()
                                .FontSize(10);

                            table.Cell()
                               .Border(1)
                               .Background(Colors.Grey.Lighten1)
                               .Padding(1.5f)
                               .Text("Fecha Facturacion")
                               .Bold()
                               .FontSize(10);


                            // Fila independiente para los valores
                            table.Cell()
                                .Border(1)
                                .Padding(1.5f)
                                .Text(service.DealerSaleName)
                                .FontSize(10)
                                ; // Une 3 columnas para el valor de Concesionario

                            table.Cell()
                                .Border(1)
                                .Padding(1.5f)
                                .Text(service.DealerSaleCod)
                                .FontSize(10);

                            table.Cell()
                                .Border(1)
                                .Padding(1.5f)
                                .Text(service.DealerServiceCity)
                                .FontSize(10);

                            table.Cell()
                               .Border(1)
                               .Padding(1.5f)
                               .Text(service.InvoiceNumber)
                               .FontSize(10);
                            
                            table.Cell()
                               .Border(1)
                               .Padding(1.5f)
                               .Text($"{service.InvoiceDate:yyyy-MM-dd}")
                               .FontSize(10);
                        });



                        // Descripción de la falla
                        col.Item().Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Descripción de la Falla:").Bold().FontSize(10);
                        col.Item().Border(1).Padding(1.5f).Text(service.TechnicalSolution).FontSize(11);
                        col.Item().Table(async table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(0.5f); // Ítem
                                columns.RelativeColumn(2); // Referencia
                                columns.RelativeColumn(2); // Serial
                                columns.RelativeColumn(0.4f); // Tipo
                                columns.RelativeColumn(1); // Compra Externa
                                columns.RelativeColumn(4); // Descripción
                                columns.RelativeColumn(0.9f); // Cantidad
                                columns.RelativeColumn(1); // Precio
                                columns.RelativeColumn(1.5f); // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Ítem").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Referencia").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Num Factura").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Tipo").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Compra Externa").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Descripción").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Cantidad").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Precio").Bold().AlignCenter().FontSize(10);
                                header.Cell().PaddingTop(4).Border(1).Background(Colors.Grey.Lighten1).Padding(1.5f).Text("Total").Bold().AlignCenter().FontSize(10);
                            });


                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(0.5f); // Ítem
                                    columns.RelativeColumn(2); // Referencia
                                    columns.RelativeColumn(2); // Serial
                                    columns.RelativeColumn(0.4f); // Tipo
                                    columns.RelativeColumn(1); // EXTERNO
                                    columns.RelativeColumn(4); // Descripción
                                    columns.RelativeColumn(0.9f); // Cantidad
                                    columns.RelativeColumn(1); // Precio
                                    columns.RelativeColumn(1.5f); // Total
                                });

                                if (serviceDetailsList.Count() == 0)
                                {
                                    table.Cell().ColumnSpan(8).Padding(5).Text("No hay datos disponibles.").Italic().FontSize(10);
                                    return;
                                }

                                int index = 1;
                                foreach (var part in serviceDetailsList)
                                {
                                    table.Cell().Border(1).Padding(1.5f).Text(index++.ToString()).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text(part.Reference).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text(part.Serial).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text(part.Type).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text((bool)part.IsExternal ? "Sí" : "No").FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text(part.ItemDescription).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text(part.Quantity.ToString()).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text(part.UnitPrice.ToString()).FontSize(10);
                                    table.Cell().Border(1).Padding(1.5f).Text((part.Price).ToString()).FontSize(10);
                                }
                            });

                        });

                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Celda 1: Autorización del Cliente
                            table.Cell().Border(1).Padding(1)
                                .Text("SRG                                                                     500000002876\nAUTORIZACIÓN DEL CLIENTE:\n* Para efectuar las reparaciones arriba mencionadas.\n* Para probar este vehículo fuera del taller si fuere necesario.\n               Aceptado: ___________________________\n                       FIRMAS AUTORIZADAS DEL CONCESIONARIO:\n____________________________               ____________________________\n     Gerente de Servicio                                        Gerente Repuestos\n_____________________________                                2024-06-18\n     Gte. de Administración                                   Fecha de Registro ")
                                .Justify().FontSize(10).LineHeight(1).Bold();

                            // Celda 2: Certificación del Servicio Efectuado
                            table.Cell().Border(1).Padding(3)
                                .Text("LLENAR SOLO DESPUÉS DE HABERSE REALIZADO LA REPARACIÓN\nCERTIFICACIÓN DEL SERVICIO EFECTUADO\nCertifico que el vehículo arriba descrito fue reparado por este concesionario y que no se hizo cargo alguno por concepto de mano de obra y/o repuestos\nsegún lo establecido en la Póliza de Garantía para tal fin.\n\nACEPTADO Y FIRMADO: ___________________________\n\nFECHA DE SALIDA: ___________________________")
                                .Justify().FontSize(10).LineHeight(1).Bold();
                            foreach (var service in services)
                            {
                                table.Cell().Border(1).Padding(3).Column(column =>
                                {
                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Total Mano de Obra:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{totalManoObra:N2}").Bold();
                                    });

                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Total Repuestos:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{totalRepuestos:N2}").Bold();
                                    });

                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Sub-Total:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{subTotal:N2}").Bold();
                                    });

                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Exento:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{service.Exempt:N2}").Bold();
                                    });

                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Base Imponible:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{service.TaxBase:N2}").Bold();
                                    });

                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Impuesto:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{service.Tax:N2}").Bold();
                                    });

                                    column.Item().Row(row =>
                                    {
                                        row.RelativeColumn().Text("Total General:").Bold();
                                        row.ConstantColumn(100).AlignRight().Text($"{service.InvoiceAmount:N2}").Bold();
                                    });
                                });
                            }


                        });
                    });
                });
            });
            return document.GeneratePdf();
        }


        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int userId, int serviceTypeId, int dealerId, int serviceId)
        {
            try
            {
                switch (serviceTypeId)
                {
                    case 1:
                        Models.Response<List<Models.ServiceMaintenance>> maintenanceResponse =
                            await _dService.GetOne<Models.ServiceMaintenance>(userId, serviceTypeId, dealerId, serviceId);
                        return StatusCode(maintenanceResponse.Status, maintenanceResponse);

                    case 2:
                        Models.Response<List<Models.ServiceAssistance>> assistanceResponse =
                            await _dService.GetOne<Models.ServiceAssistance>(userId, serviceTypeId, dealerId, serviceId);
                        return StatusCode(assistanceResponse.Status, assistanceResponse);

                    case 3:
                        Models.Response<List<Models.ServiceFail>> failResponse =
                            await _dService.GetOne<Models.ServiceFail>(userId, serviceTypeId, dealerId, serviceId);
                        return StatusCode(failResponse.Status, failResponse);

                    default:
                        return StatusCode(StatusCodes.Status400BadRequest, $"Tipo de servicio no soportado: {serviceTypeId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpGet("GetReportType")]
        public async Task<IActionResult> GetReportType()
        {

            try
            {
                 var _response = await _dService.GetReportType();
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetAssistanceType")]
        public async Task<IActionResult> GetAssistanceType()
        {

            try
            {
                var _response = await _dService.GetAssistanceType();
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetPossibleFault")]
        public async Task<IActionResult> GetPossibleFault()
        {

            try
            {
                var _response = await _dService.GetPossibleFault();
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetServiceType")]
        public async Task<IActionResult> GetServiceType()
        {

            try
            {

                var _response = await _dService.GetServiceType();
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpGet("GetUserAssign")]
        public async Task<IActionResult> GetUserAssign(String? filter, Int32? irowFrom, Int32? Id, Int32 userId, Int32 supplierId)
        {

            try
            {

                var _response = await _dService.GetUserAssign(filter,irowFrom,Id,userId,supplierId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private async Task<MemoryStream> ConvertToExcelSrgAsync(List<Models.SrgPending> _srg)
        {
             var dealer = _srg.FirstOrDefault()?.Dealer ?? "DESCONOCIDO";
            int? brandId = _srg.First().BrandId;
            int? supplierId = _srg.First().SupplierId;

            string brandImagePath = await GetFirstAttachmentFilePath("RECURSOS-MARCAS", brandId);
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", supplierId);

            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {


                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("SOLICTUD PAGO DE GARANTIA");
                worksheet.ShowGridLines = false;

                var image = worksheet.AddPicture(supplierImagePath)
               .WithPlacement(XLPicturePlacement.FreeFloating)
               .MoveTo(worksheet.Cell("C1"), 10, 5);
                image.Width = 180;   // Ancho en píxeles
                image.Height = 80;


                var image2 = worksheet.AddPicture(brandImagePath)
                .WithPlacement(XLPicturePlacement.FreeFloating)
                .MoveTo(worksheet.Cell("I1"), 10, 5); // 10 px a la derecha, 5 px hacia abajo
                image2.Width = 180;
                image2.Height = 80;

                worksheet.Cell(2, 4).Value = "SOLICITUD PAGO DE GARANTIA A CONCESIONARIO";
                worksheet.Cell(2, 4).Style.Fill.BackgroundColor = XLColor.LightGray;

                worksheet.Range(2, 4, 2, 9).Merge();
                worksheet.Cell(2, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(2, 4).Style.Font.Bold = true;

                worksheet.Cell(5, 1).Value = "DIRIGIDO A :";
                worksheet.Cell(5, 2).Value = "JEFA DE CONTABILIDAD";
                ApplyFullBorder(worksheet, 5, 2);
                worksheet.Cell(6, 1).Value = "CONCESIONARIO: ";
                worksheet.Cell(6, 2).Value = dealer;
                ApplyFullBorder(worksheet, 6, 2);

                worksheet.Cell(5, 12).Value = "FECHA:";
                ApplyFullBorder(worksheet, 5, 12);
                worksheet.Cell(5, 13).Value = DateTime.Now.ToString("dd/MM/yyyy");
                ApplyFullBorder(worksheet, 5, 13);

                worksheet.Cell(6, 12).Value = "RELACION N° ";
                ApplyFullBorder(worksheet, 6, 12);
                worksheet.Cell(6, 13).Value = "  ";
                ApplyFullBorder(worksheet, 6, 13);

                worksheet.Cell(8, 2).Value = "La presente es para solicitarle, realizar cruce de cuenta a favor del Concesionario";
                worksheet.Range(8, 2, 8, 5).Merge();
                worksheet.Cell(8, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Cell(8, 2).Style.Font.Bold = true;

                worksheet.Cell(8, 6).Value =dealer;
                worksheet.Range(8, 6, 8, 8).Merge();
                worksheet.Cell(8, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(8, 6).Style.Font.Bold = true;

                worksheet.Cell(8, 9).Value = "por Concepto de Solicitud de Reembolso de Garantias y Facturas.";
                worksheet.Range(8, 9, 8, 13).Merge();
                worksheet.Cell(8, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell(8, 9).Style.Font.Bold = true;


                // 4. Agregar los encabezados
                worksheet.Cell(11, 1).Value = "VIN";
                worksheet.Cell(11, 2).Value = "N° SRG ";
                worksheet.Cell(11, 3).Value = "N° RELACION";
                worksheet.Cell(11, 4).Value = "TIPO REPORTE";
                worksheet.Cell(11, 5).Value = "MODELO";
                worksheet.Cell(11, 6).Value = "DESCRIPCION DEL TRABAJO REALIZADO";
                worksheet.Cell(11, 7).Value = "FACTURA N°";
                worksheet.Cell(11, 8).Value = "F/FACTURA";
                worksheet.Cell(11, 9).Value = "TASA BCV";
                worksheet.Cell(11, 10).Value = "TOTAL NETO";
                worksheet.Cell(11, 11).Value = "IMPUESTO (IVA)";
                worksheet.Cell(11, 12).Value = "TOTAL GENERAL";
                worksheet.Cell(11, 13).Value = "TOTAL $$";
                
                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A11:M11");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;
                worksheet.Range("A11:M11").SetAutoFilter();
                // 6. Llenar los datos
                int i;
                decimal total = 0;
                for (i = 0; i < _srg.Count; i++)
                {
                    var _s = _srg[i];
                    int row = i + 12;

                    worksheet.Cell(row, 1).Value = _s.Vin;
                    worksheet.Cell(row, 2).Value = _s.Srg;
                    worksheet.Cell(row, 3).Value = _s.Id;
                    worksheet.Cell(row, 4).Value = _s.Reporttype;
                    worksheet.Cell(row, 5).Value = _s.Model;
                    worksheet.Cell(row, 6).Value = _s.DescriptionFail;
                    worksheet.Cell(row, 7).Value = _s.Invoice;
                    worksheet.Cell(row, 8).Value = _s.InvoiceDate;
                    worksheet.Cell(row, 9).Style.DateFormat.Format = "dd/MM/yyyy";
                    worksheet.Cell(row, 10).Value = _s.TaxBase+_s.Exent;
                    worksheet.Cell(row, 11).Value = _s.Tax;
                    worksheet.Cell(row, 12).Value = "";
                    worksheet.Cell(row, 13).Value = _s.Mount;
                    total += (decimal)_s.Mount;
                    // Aplicar bordes a todas las columnas de esta fila
                    for (int col = 1; col <= 13; col++)
                    {
                        ApplyFullBorder(worksheet, row, col);
                    }
                }

                worksheet.Cell(i + 12, 11).Value = "TOTAL";
                ApplyFullBorder(worksheet, i + 12, 11);
                worksheet.Cell(i + 12, 12).Value = "0.00";
                ApplyFullBorder(worksheet, i + 12, 12);
                worksheet.Cell(i + 12, 13).Value = total;
                ApplyFullBorder(worksheet, i + 12, 13);

                worksheet.Cell(i + 13, 1).Value = "Asimismo, hacemos entrega de soporte original: Factura Fiscal original o copia, el expediente original queda bajo resguardo de garantia.";
                worksheet.Range(i + 13, 1, i + 13, 11).Merge();
                worksheet.Cell(i + 13, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell(i + 13, 1).Style.Font.Bold = true;

                worksheet.Cell(i + 15, 1).Value = "ENTREGADO";
                worksheet.Range(i + 15, 1, i + 15, 3).Merge();
                worksheet.Cell(i + 15, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(i + 15, 1).Style.Font.Bold = true;
                ApplyFullBorderToRange(worksheet, i + 15, 1, 3);

                worksheet.Cell(i + 15, 9).Value = "RECIBIDO";
                worksheet.Range(i + 15, 9, i + 15, 13).Merge();
                worksheet.Cell(i + 15, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(i + 15, 9).Style.Font.Bold = true;
                ApplyFullBorderToRange(worksheet, i + 15, 9, 13);

                worksheet.Cell(i + 18, 1).Value = "NOMBRE: ";
                worksheet.Range(i + 18, 1, i + 18, 3).Merge();
                worksheet.Cell(i + 18, 9).Value = "NOMBRE: ";
                worksheet.Range(i + 18, 9, i + 18, 13).Merge();

                worksheet.Cell(i + 22, 1).Value = "NOTA: ";

                worksheet.Cell(i + 23, 1).Value = "* Los montos expresados en Bolívares  varian según la tasa del dia del BCV : ";
                worksheet.Range(i + 23, 1, i + 23, 2).Merge();

                // 7. Ajustar el ancho de las columnas al contenido 
                worksheet.Columns().AdjustToContents();

                // 8. Preparar el stream para la respuesta
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0; // Importante: rebobinar el stream
                return stream;

            }

        }


       

        private void ApplyFullBorder(IXLWorksheet sheet, int row, int column, XLBorderStyleValues style = XLBorderStyleValues.Thin)
        {
            var cell = sheet.Cell(row, column);

            cell.Style.Border.TopBorder = style;
            cell.Style.Border.BottomBorder = style;
            cell.Style.Border.LeftBorder = style;
            cell.Style.Border.RightBorder = style;

            cell.Style.Border.TopBorderColor = XLColor.Black;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
            cell.Style.Border.LeftBorderColor = XLColor.Black;
            cell.Style.Border.RightBorderColor = XLColor.Black;
        }

        private void ApplyFullBorderToRange(IXLWorksheet sheet, int row, int startColumn, int endColumn, XLBorderStyleValues style = XLBorderStyleValues.Thin)
        {
            var range = sheet.Range(row, startColumn, row, endColumn);
            range.Merge();

            range.Style.Border.TopBorder = style;
            range.Style.Border.BottomBorder = style;
            range.Style.Border.LeftBorder = style;
            range.Style.Border.RightBorder = style;

            range.Style.Border.TopBorderColor = XLColor.Black;
            range.Style.Border.BottomBorderColor = XLColor.Black;
            range.Style.Border.LeftBorderColor = XLColor.Black;
            range.Style.Border.RightBorderColor = XLColor.Black;
        }

        [HttpGet("ExportSrgPending")]
        public async Task<IActionResult> ExportSrgPending(Int32 userId, Int32? supplierId, Int32? dealerId)
        {

            try
            {

                List<SrgPending> _srg = await _dService.GetExportSrg(userId, supplierId, dealerId);
                MemoryStream _excel = await ConvertToExcelSrgAsync(_srg);
                string _fileName = "ListadoSrg.xlsx";

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



        [HttpPost("PostMaintenance")]
        public async Task<IActionResult> Post_Maintenance(Models.ServiceMaintenance maintenance, Int32 userId)
        {


            try
            {
                var _response = await _dService.Post_Service(maintenance,null,null, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostDetails")]
        public async Task<IActionResult> Post_Details(Models.ServiceDetails serviceDetails, Int32 userId)
        {


            try
            {
                var _response = await _dService.Post_Details(serviceDetails,userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("PostItem")]
        public async Task<IActionResult> Post_Item(Models.Item item, Int32 userId)
        {


            try
            {
                var _response = await _dService.Post_Item(item, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostFailReport")]
        public async Task<IActionResult> Post_FailReport(Models.ServiceFail failReport, Int32 userId)
        {


            try
            {
                var _response = await _dService.Post_Service(null,null,failReport, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostAssistence")]
        public async Task<IActionResult> Post_Assitence(Models.ServiceAssistance assistance, Int32 userId)
        {


            try
            {
                var _response = await _dService.Post_Service(null,assistance,null, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }


        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions, Int32 userId, Int32 serviceTypeId)
        {

            try
            {

                var _response = await _dService.Post_Actions(actions, userId, serviceTypeId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        [HttpPost("PostProcess")]
        public async Task<IActionResult> Post_Actions_Process(List<Models.ActionInvoice> actions, Int32 userId, Int32 serviceTypeId)
        {

            try
            {

                var _response = await _dService.Post_Actions_Process(actions, userId, serviceTypeId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("PostActionsDetails")]
        public async Task<IActionResult> Post_ActionsDetails(List<Models.Action> actions, Int32 userId)
        {

            try
            {

                var _response = await _dService.Post_ActionsDetails(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpPost("Delete_Details")]
        public async Task<IActionResult> Delete_Details(List<Models.Action> _list, int userId)
        {
            try
            {

                var _response = await _dService.Delete_Details(_list, userId);
                return StatusCode(_response.Status, _response);


            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al eliminar el archivo: {ex.Message}");
            }
        }



    }
}