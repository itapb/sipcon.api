
using ClosedXML.Excel;
using Data;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;


namespace WebApi.Controllers
{
    [Route("api/Policy")]
    [ApiController]
    [Authorize]
    public class cPolicy : ControllerBase
    {

      
        private readonly dPolicy _dPolicy;
        private readonly dAttachment _dAttachment;

        public cPolicy(dPolicy dPolicy, dAttachment dAttachment)
        {
            _dPolicy = dPolicy;
            _dAttachment = dAttachment;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string? filter, Int32 rowFrom, Int32 userId,Int32? supplierId,Int32? dealerId, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {

            try
            {
                var _response = await _dPolicy.GetAll(filter, rowFrom, userId, supplierId, dealerId,fromDate, upToDate, estatusId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOneBy")]
        public async Task<IActionResult> GetOneBy(Int32 userId, string filter, Int32 filterBy, Int32? supplierId )
        {

            try
            {
                var _response = await _dPolicy.GetOneBy(userId, filter, filterBy, supplierId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        private MemoryStream ConvertToExcel(List<Models.Policy> _policies)
        {
            // 2. Crear el libro de trabajo Excel
            using (var workbook = new XLWorkbook())
            {
                // 3. Agregar una hoja al libro
                var worksheet = workbook.Worksheets.Add("POLIZAS");

                // 4. Agregar los encabezados
                worksheet.Cell(1, 1).Value = "NUMERO";
                worksheet.Cell(1, 2).Value = "CODIGO";
                worksheet.Cell(1, 3).Value = "TIPO POLIZA";
                worksheet.Cell(1, 4).Value = "VIN";
                worksheet.Cell(1, 5).Value = "PLACA";
                worksheet.Cell(1, 6).Value = "SERIAL MOTOR";
                worksheet.Cell(1, 7).Value = "MODELO";
                worksheet.Cell(1, 8).Value = "COLOR";
                worksheet.Cell(1, 9).Value = "RIF";
                worksheet.Cell(1, 10).Value = "NOMBRE";
                worksheet.Cell(1, 11).Value = "APELLIDO";
                worksheet.Cell(1, 12).Value = "CORREO";
                worksheet.Cell(1, 13).Value = "NUMERO TELEFONICO";
                worksheet.Cell(1, 14).Value = "CIUDAD CLIENTE";
                worksheet.Cell(1, 15).Value = "FECHA ACTIVACION";
                worksheet.Cell(1, 16).Value = "FECHA CREACION POLIZA";
                worksheet.Cell(1, 17).Value = "COD CONCESIONARIO";
                worksheet.Cell(1, 18).Value = "CONCECIONARIO";
                worksheet.Cell(1, 19).Value = "CIUDAD CONCESIONARIO";
                worksheet.Cell(1, 20).Value = "PLANTA";
                worksheet.Cell(1, 21).Value = "NUM FACTURA";
                worksheet.Cell(1, 22).Value = "MONTO FACTURA";
                worksheet.Cell(1, 23).Value = "FEC FACTURA";
                worksheet.Cell(1, 24).Value = "ESTATUS";
                worksheet.Cell(1, 25).Value = "VENDEDOR";



                // 5. Estilo para los encabezados
                var headerRange = worksheet.Range("A1:Y1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;
                var colorMap = new Dictionary<string, XLColor> { { "Activado", XLColor.Green }, { "Desactivado", XLColor.Red }, { "Bloqueado", XLColor.Orange }, { "Desbloqueado", XLColor.GreenYellow } };
                worksheet.Range("A1:Y1").SetAutoFilter();
                // 6. Llenar los datos
                for (int i = 0; i < _policies.Count; i++)
                {
                    var _policy = _policies[i];
                    worksheet.Cell(i + 2, 1).Value = _policy.Id;
                    worksheet.Cell(i + 2, 2).Value = _policy.Number;
                    worksheet.Cell(i + 2, 3).Value = _policy.Description;
                    worksheet.Cell(i + 2, 4).Value = _policy.Vin;
                    worksheet.Cell(i + 2, 5).Value = _policy.Plate;
                    worksheet.Cell(i + 2, 6).Value = _policy.EngineSerial;
                    worksheet.Cell(i + 2, 7).Value = _policy.ModelName;
                    worksheet.Cell(i + 2, 8).Value = _policy.Color;
                    worksheet.Cell(i + 2, 9).Value = _policy.Vat;
                    worksheet.Cell(i + 2, 10).Value = _policy.FirstName;
                    worksheet.Cell(i + 2, 11).Value = _policy.LastName;
                    worksheet.Cell(i + 2, 12).Value = _policy.Email;
                    worksheet.Cell(i + 2, 13).Value = _policy.Phone;
                    worksheet.Cell(i + 2, 14).Value = _policy.CustomerCity;
                    worksheet.Cell(i + 2, 15).Value = _policy.ActivationDate;
                    worksheet.Cell(i + 2, 15).Style.DateFormat.Format = "dd/MM/yyyy"; // Formato fecha
                    worksheet.Cell(i + 2, 16).Value = _policy.DateCreated;
                    worksheet.Cell(i + 2, 16).Style.DateFormat.Format = "dd/MM/yyyy";
                    worksheet.Cell(i + 2, 17).Value = _policy.DealerCod;
                    worksheet.Cell(i + 2, 18).Value = _policy.DealerName;
                    worksheet.Cell(i + 2, 19).Value = _policy.DealerCity;
                    worksheet.Cell(i + 2, 20).Value = _policy.SupplierCod;
                    worksheet.Cell(i + 2, 21).Value = _policy.InvoiceNumber;
                    worksheet.Cell(i + 2, 22).Value = _policy.InvoiceAmount;
                    worksheet.Cell(i + 2, 23).Value = _policy.InvoiceDate;
                    worksheet.Cell(i + 2, 23).Style.DateFormat.Format = "dd/MM/yyyy"; // Formato fecha
                    worksheet.Cell(i + 2, 24).Value = _policy.EstatusName;
                    worksheet.Cell(i + 2, 24).Style.Fill.BackgroundColor = colorMap.TryGetValue(_policy.EstatusName, out var color) ? color : XLColor.Yellow;
                    worksheet.Cell(i + 2, 25).Value = _policy.Seller;



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
        public async Task<IActionResult> GetExport(string? filter, Int32 userId,Int32? supplierId, Int32? dealerId, DateTime? fromDate, DateTime? upToDate, int? estatusId)
        {

            try
            {

                List<Policy> _policies = await _dPolicy.GetExport(filter, userId, supplierId, dealerId, fromDate, upToDate, estatusId);
                MemoryStream _excel = ConvertToExcel(_policies);
                string _fileName = "Polizas.xlsx";

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



        [HttpGet("ExportPdf")]
        public async Task<IActionResult> GetExportPdf(Int32 userId, Int32 policyId)
        {
            try
            {
                Response<Models.PolicyExtended> response = await _dPolicy.GetOne(policyId, userId);

                // 2. Validar que la respuesta es exitosa y contiene datos
                if (response.Status != StatusCodes.Status200OK || response.Data == null)
                {
                    return NotFound("No se encontró la póliza solicitada");
                }

                // 3. Convertir los datos correctamente
                List<PolicyExtended> policies = new List<PolicyExtended>();

                if (response.Data is PolicyExtended singlePolicy)
                {
                    // Si es un solo objeto
                    policies.Add(singlePolicy);
                }
                else if (response.Data is IEnumerable<PolicyExtended> policyList)
                {
                    // Si ya es una lista/enumerable
                    policies = policyList.ToList();
                }
                else
                {
                    // Si no reconocemos el formato
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Formato de datos de póliza no reconocido");
                }

                // 4. Validar que tenemos datos
                if (!policies.Any())
                {
                    return NotFound("No se encontraron datos válidos de póliza");
                }

                if (policies.Any(ser => ser.EstatusName == "CREADO"))
                {
                    return NotFound("POLIZA NO DISPONIBLE");
                }

                // 5. Generar el PDF
                byte[] pdfBytes = await GeneratePdfReportAsync(policies); // Corrección: Se espera el resultado de la tarea con 'await'
                string fileName = $"Poliza_{policies.FirstOrDefault()?.Number ?? "reporte"}.pdf";

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

        private async Task<byte[]> GeneratePdfReportAsync(List<PolicyExtended> policies)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            int? brandId = policies.First().BrandId;
            int? supplierId = policies.First().SupplierId;

            string brandImagePath = await GetFirstAttachmentFilePath("RECURSOS-MARCAS", brandId);
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", supplierId);
            return Document.Create(container =>
            {
                foreach (var policy in policies)
                {
                    // Primera página
                    container.Page(firstPage =>
                    {
                        firstPage.Size(PageSizes.A4.Landscape());
                        firstPage.Margin(50);

                        firstPage.Content().AlignCenter().Column(column =>
                        {
                            column.Item().AlignCenter().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                // Columna 1 - Términos y condiciones (texto completo)

                                table.Cell().RowSpan(2).PaddingRight(10).Text($@"Daños causados por otras partes NO originales de la marca {policy.BrandName}. El reemplazo de partes por otras NO autorizadas por {policy.SupplierName}, anula la garantía del vehículo en general. En el caso de las llantas (conocido también como cauchos o neumáticos), esta garantía no cubre el desgaste normal de neumáticos, los daños por accidentes en carretera, tales como cortadas, perforaciones, raspaduras, abombados o rupturas por impacto contra desniveles en la calzada, huecos, brocales, aceras u otros, así como los daños posteriores a su reparación que sean consecuencia del daño reparado o de una reparación inadecuada; los daños causados por piedras, metales u otros objetos incrustados en el caucho, los daños ocasionados por el uso abusivo del caucho, el inflado, alineación o balanceo incorrectos, cadenas para cauchos, carreras, arrancadas violentas del vehículo, derrape y exceso de carga; los daños por montajes y/o desmontajes incorrectos; las modificaciones al vehículo no autorizadas por {policy.SupplierName}.{Environment.NewLine}{Environment.NewLine}Nota importante:{Environment.NewLine}Los gastos que ocasionen los mantenimientos rutinarios o preventivos, o las revisiones periódicas, son por cuenta del Comprador o Propietario.{Environment.NewLine}{Environment.NewLine}CONDICIONES{Environment.NewLine}{Environment.NewLine}{policy.SupplierName}, ÚNICO REPRESENTANTE AUTORIZADO DE LA MARCA {policy.BrandName} EN VENEZUELA, Y A TRAVÉS SU RED DE CONCESIONARIOS, no reconocerán reclamación alguna y quedará sin efecto la garantía si la falla ocurrida se encuentra bajo las siguientes condiciones:{Environment.NewLine}1. Cuando el usuario, o cualquier persona NO autorizada por {policy.SupplierName} hagan modificaciones en el vehículo o en sus componentes.{Environment.NewLine}2. Cuando sean alterados los ajustes o regulaciones de los aparatos, componentes y/o dispositivos del vehículo.{Environment.NewLine}3. Cuando en el vehículo se hayan realizado alteraciones, perforaciones, instalación de componentes, equipos o accesorios, por ejemplo: Instalaciones incorrectas de plataformas al chasis, alarmas, y sistemas de seguridad.{Environment.NewLine}4. En caso de mantenimiento inadecuado, colisiones, accidentes, daños por factores climatológicos o sismológicos, así como cuando el vehículo es operado excediendo los límites de carga y velocidad o si es operado negligentemente.{Environment.NewLine}5. Fallas ocasionadas por el uso del combustible inadecuado o contaminado, así como los aceites, lubricantes y otros fluidos no sugeridos en el Manual de Mantenimiento. Cuando no se efectúen oportunamente las revisiones y mantenimientos indicados en el PROGRAMA DE MANTENIMIENTO establecidas en el manual de propietario.{Environment.NewLine}6. Cuando NO se hayan realizado uno o varios servicios de mantenimiento periódico indicados en el manual de propietario o la red de CONCESIONARIOS AUTORIZADOS {policy.BrandName}.{Environment.NewLine}7. Cuando se empleen repuestos no especificados por {policy.SupplierName}.{Environment.NewLine}8. Cuando el vehículo sea objeto de reparaciones o adaptaciones en TALLERES NO AUTORIZADOS por {policy.SupplierName}.").FontSize(7.5f).LineHeight(1.2f).Justify();
                                table.Cell().RowSpan(2).PaddingRight(10).Text($@"9. Cuando el vehículo sea destinado a una aplicación especial, no sean observadas las recomendaciones de las áreas de Ingeniería y/o Servicio de {policy.SupplierName}, en cuanto a componentes especiales para la aplicación específica, y a las revisiones/operaciones periódicas de mantenimientos.10.Cuando el vehículo sea destinado a una aplicación especial, no sean observadas las recomendaciones de las áreas de Ingeniería y/o Servicio de {policy.SupplierName} en cuanto a componentes especiales para la aplicación específica, y a las revisiones/operaciones periódicas de mantenimientos.{Environment.NewLine}10. Cuando hayan sido alterados los números de identificación del vehículo o los datos contenidos en la tarjeta de identificación y/o en los registros de revisiones de mantenimiento.{Environment.NewLine}11. Cuando el vehículo sea sometido a maltratos, exceso de velocidad, uso inapropiado o carga que exceda de la capacidad nominal que le corresponda.{Environment.NewLine}12. Si el Comprador o Propietario realiza declaraciones falsas o inexactas acerca del cumplimiento de los deberes que le impone esta POLIZA de GARANTIA, o que falsamente hagan presumir la vigencia de la misma.{Environment.NewLine}13. Cuando el vehículo sea sometido a sobrecarga de acuerdo a las leyes respectivas.{Environment.NewLine}Notas:{Environment.NewLine}{Environment.NewLine}. El vehículo al que se refiere esta póliza de garantía es de producción en serie, en consecuencia, las piezas, conjuntos o ensamblajes que lo componen están sujetos a las especificaciones y tolerancias establecidas por el fabricante, realiza cambios o modificaciones en los diseños y/o especificaciones de tales piezas, conjuntos o ensamblajes, o mejoras en los vehículos que produce, ni ella ni {policy.SupplierName}, O SUS CONCESIONARIOS AUTORIZADOS están obligados a introducirlas o efectuarlas en los vehículos que fabrique con posterioridad a la creación de tales cambios, modificaciones o mejoras.{Environment.NewLine}{Environment.NewLine}. {policy.SupplierName} se reserva el derecho de modificar las especificaciones de sus productos sin previo aviso.{Environment.NewLine}{Environment.NewLine}. El vehículo a que se refiere esta póliza de garantía no está diseñado para remolcar carga.{Environment.NewLine}{Environment.NewLine}PROGRAMA DE MANTENIMIENTO DE UNIDADES{Environment.NewLine}{Environment.NewLine}NOTAS IMPORTANTES Los servicios de mantenimientos preventivo deben realizarse cada 5.000 kilómetros o cada 6 meses (lo que ocurra primero). DE CARÁCTER OBLIGATORIO El propietario debe realizar inspecciones diarias o de rutina para permitir la operatividad adecuada en carretera.{Environment.NewLine}").FontSize(7.5f).LineHeight(1.3f).Justify();

                                try
                                {

                                    // Columna 3 - Imágenes
                                    table.Cell().Row(rowImages =>
                                    {
                                        rowImages.RelativeItem()
                                            .Width(100)
                                            .Image(brandImagePath)
                                            .FitArea();

                                        rowImages.RelativeItem()
                                            .Width(120)
                                            .Image(supplierImagePath)
                                            .FitArea();
                                    });
                                }
                                catch (Exception ex)
                                {

                                }

                                // Columna 3 - Datos de la póliza (dinámicos)
                                table.Cell().Text($@"{Environment.NewLine}PÓLIZA Nº:{policy.Number}{Environment.NewLine}IDENTIFICACIÓN{Environment.NewLine}CONCESIONARIO:                                           CÓDIGO:{Environment.NewLine}{policy.DealerName}                                                {policy.DealerCod}{Environment.NewLine}
                                NOMBRE DEL BENEFICIARIO DE LA GARANTÍA O RAZÓN SOCIAL:{Environment.NewLine}{policy.FirstName} {policy.LastName}{Environment.NewLine}CÉDULA DE IDENTIDAD O RIF:{Environment.NewLine}{policy.Vat}{Environment.NewLine}DIRECCIÓN:{Environment.NewLine}{policy.Direction}{Environment.NewLine}TELÉFONOS:{Environment.NewLine}{policy.Phone}{Environment.NewLine}CORREO ELECTRÓNICO:{Environment.NewLine}{policy.Email}{Environment.NewLine}{Environment.NewLine}
                                __________________________________________{Environment.NewLine}                                          FIRMA DEL COMPRADOR{Environment.NewLine}
                                Con su firma el comprador declara que se ha recibido el original de esta póliza de garantía, que la misma se encuentra libre de enmiendas, tachaduras o borrones, acepta sus términos y condiciones establecidos bajo el título de garantía del vehículo, y confirma que conjuntamente con ella, ha recibido el manual de propietario del vehículo antes mencionado.{Environment.NewLine}{Environment.NewLine}DATOS DEL VEHÍCULO{Environment.NewLine}SERIAL DE CARROCERÍA (VIN): {policy.Vin}{Environment.NewLine}SERIAL DE MOTOR: {policy.EngineSerial}{Environment.NewLine}MODELO: {policy.ModelName}{Environment.NewLine}AÑO MODELO: {policy.Year}{Environment.NewLine}PLACA: {policy.Plate}{Environment.NewLine}COLOR: {policy.Color}{Environment.NewLine}FECHA DE VENTA: {policy.InvoiceDate:dd/MM/yyyy}{Environment.NewLine}FECHA DE ACTIVACION: {policy.ActivationDate:dd/MM/yyyy}{Environment.NewLine}{Environment.NewLine}                                                       ___________________________________________________________________{Environment.NewLine}                 SELLO Y FIRMA DEL CONCESIONARIO AUTORIZADO").FontSize(7f).Justify();
                            });
                        });
                    });

                    // Segunda página
                    container.Page(secondPage =>
                    {
                        secondPage.Size(PageSizes.A4.Landscape());
                        secondPage.Margin(50);

                        secondPage.Content().AlignCenter().Column(column =>
                        {
                            column.Item().AlignCenter().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                // Columna 1 - Introducción (texto completo)

                                table.Cell().PaddingRight(10).Text($@"EN EL MOMENTO QUE USTED ADQUIERE UN VEHÍCULO {policy.BrandName} VA A CONOCER Y DISFRUTAR DE NUESTROS PRODUCTOS{Environment.NewLine}{Environment.NewLine}Gracias por la confianza depositada. No importa la actividad en la que usted se desenvuelva, estamos seguros que con nuestro producto, encontrará un fiel y vital aliado para el éxito de su empresa.{Environment.NewLine}{Environment.NewLine}El objetivo del presente Manual de Garantía y Mantenimiento, es brindarle a nuestros clientes seguridad y confianza al adquirir un vehículo ensamblado, distribuido y comercializado por {policy.SupplierName}, y en donde usted podrá contar con una red de concesionarios ubicados en toda la Geografía Nacional ofreciéndole respuesta rápida y oportuna a cualquier requerimiento que pudiese tener mediante nuestro soporte técnico altamente entrenado y calificado, el más amplio y surtido inventario de repuestos, así como los equipos y herramientas necesarias para mantener su vehículo siempre listo para el trabajo. Gracias por la confianza depositada en la calidad de nuestros productos.{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}DURACIÓN{Environment.NewLine}{Environment.NewLine}La duración de la garantía que {policy.SupplierName}, único representante autorizado de la marca {policy.BrandName} en Venezuela, otorga para vehículos nuevos es de {policy.TopYear} AÑOS ({policy.TopMonths} MESES) ó {policy.Topkm} Km (Lo que ocurra primero). Dicho intervalo será contabilizado desde la fecha de entrega de la unidad al cliente final.{Environment.NewLine}{Environment.NewLine}REQUISITOS QUE DEBEN CUMPLIR LAS SOLICITUDES PARA CONSIDERARSE AMPARADAS POR GARANTÍA{Environment.NewLine}{Environment.NewLine}Para que su solicitud de reparación bajo la Política de Garantía sea atendida en un CONCESIONARIO AUTORIZADO {policy.BrandName}, es necesario que cumpla los siguientes requisitos:{Environment.NewLine}1. Presentar esta póliza de garantía conteniendo todos los datos requeridos en la tarjeta de identificación, y los cupones de comprobación de programa de mantenimiento, debidamente sellados por el CONCESIONARIO AUTORIZADO {policy.BrandName} que haya efectuado las revisiones. La omisión de cualquiera de los servicios señalados en el PROGRAMA DE MANTENIMIENTO contenido en el manual del propietario, será motivo suficiente para anular la garantía.{Environment.NewLine}2. El vehículo debe estar dentro del periodo de tiempo cubierto por la garantía emitida por {policy.SupplierName}, ya que esta no es prorrogable en tiempo u horas de operación.{Environment.NewLine}3. Las reparaciones por garantía no serán procedentes en aquellos casos en donde el vehículo haya sido alterado, modificado o reparado fuera da la red de CONCESIONARIO AUTORIZADO {policy.BrandName}. Es decir, la garantía se conserva únicamente si estas alteraciones, modificaciones o reparaciones son realizadas en un CONCESIONARIO AUTORIZADO {policy.BrandName} y bajo previa autorización de la ensambladora o comercializadora, de manera que se garanticen las condiciones de diseño y la integridad del vehículo.").FontSize(7.5f).LineHeight(1.2f).Justify();


                                // Columna 2 - Alcance de garantía (texto completo)
                                table.Cell().PaddingRight(10).Text($@"Importante: La lectura y comprensión de la información detallada en este manual de garantía es indispensable para el usuario, ya que la misma sirve de guía para el buen funcionamiento, durabilidad y seguridad del vehículo. El incumplimiento de las condiciones expuestas en este manual es causal de nulidad de la garantía de su vehículo.{Environment.NewLine}ALCANCE{Environment.NewLine}{Environment.NewLine}{policy.SupplierName} garantiza que cada uno de los vehículos comercializados bajo la marca {policy.BrandName} está libre de defectos de material, de ensamblaje y de mano de obra, durante el tiempo señalado en la presente póliza operando en condiciones adecuadas para el vehículo, con las excepciones que luego se indicarán.{Environment.NewLine}La obligación de {policy.SupplierName} consiste en efectuar a través de la red de CONCESIONARIOS AUTORIZADOS {policy.BrandName}, la reparación, reemplazo o sustitución de las piezas defectuosas del vehículo que procedan de acuerdo a los parámetros de garantía, utilizando para ello repuestos nuevos originales y mano de obra especializada, y siempre que el vehículo sea usado y mantenido conforme al manual del propietario y el manual de mantenimiento preventivo.{Environment.NewLine}Cualquier servicio adicional no cubierto por esta garantía, quedará sujeto a los términos y condiciones establecidos en el manual del propietario y en el manual de servicio. La(s) pieza(s) defectuosa(s) que sean retiradas del vehículo con motivo de reemplazo cubierto por la presente póliza de garantía pertenecen a {policy.SupplierName}. La aceptación o rechazo de la solicitud por garantía, es competencia exclusiva del personal de {policy.SupplierName}, que es atendido y tramitado por el CONCESIONARIO AUTORIZADO {policy.BrandName}, que hará sus mejores esfuerzos para reparar el vehículo descrito en la presente Póliza de Garantía en el menor plazo posible a partir de la fecha de aprobación del reclamo hecho por el Propietario.{Environment.NewLine}{Environment.NewLine}El único representante autorizado de la marca {policy.BrandName} en Venezuela, {policy.SupplierName}, garantiza la disponibilidad de Repuestos. Si usted se encuentra viajando durante el período de Garantía, cualquier CONCESIONARIO AUTORIZADO {policy.BrandName} le prestará las atenciones y servicios estipulados en la presente Póliza de Garantía.{Environment.NewLine}{Environment.NewLine}COMPONENTES NO AMPARADOS POR LA POLIZA DE GARANTIA{Environment.NewLine}Bajo ningún concepto {policy.SupplierName}, NI SU RED DE CONCESIONARIOS, aceptan reclamación alguna por daños o perjuicios en su persona (propietario) o sus bienes, daños a terceros o gastos originados por conceptos de grúas, viáticos, gastos de traslado, alojamiento, manutención, deterioro de mercancías, pérdidas comerciales o de tiempo, lucro cesante, alquiler de otro vehículo. Todos estos gastos son por cuenta del propietario o comprador.").FontSize(7).LineHeight(1.4f).Justify();
                                // Columna 3 - Elementos no cubiertos (texto completo)

                                table.Cell().PaddingRight(10).Text($@"Transcurridos noventa (90) días a partir de la entrega del vehículo, la póliza de garantía de {policy.SupplierName} no cubrirá:{Environment.NewLine}{Environment.NewLine}1. Corrección: De sujetadores flojos y ruidos inusuales. Ajustes de luces, de frenos, de embrague, ajustes del sistema de dirección, alineación y balanceo de ruedas, nivelación de fluidos, en general cualquier corrección que resulte necesaria por el uso normal del vehículo.{Environment.NewLine}{Environment.NewLine}2. Reparaciones: Aquellas relacionadas directamente con el mantenimiento o las resultantes del uso o desgaste normal, incluyendo ajustes de frenos, pasta de disco de embrague, alineación y balanceo de ruedas, gomas de limpiaparabrisas, suspensión, lubricación y otras partes o reparaciones similares requeridas para mantener el vehículo en buenas condiciones de operación.{Environment.NewLine}{Environment.NewLine}. De los desgastes que se presenten en los frenos, pastillas, bandas, discos y tambores de freno, embragues y otros componentes que se vean afectados por malos hábitos de manejo o aplicaciones severas debidas al uso.{Environment.NewLine}{Environment.NewLine}A cualquier falla derivada del abuso o mal uso, negligencia, mantenimiento inadecuado, sobrecarga, operación incorrecta o que sean resultado de algún accidente. Incluido sistema neumático y de asistencia de embrague.{Environment.NewLine}{Environment.NewLine}. Decoloración, desteñido, falta de tonalidad o daños a la pintura, o a elementos de vestidura y acabado interior (Tapicería y tablero), partes cromadas o pulidas; como resultado de causas ambientales, o pulido inadecuado, o por el uso de soluciones o detergentes inapropiados. . Los trabajos, reparaciones, servicios, y/o modificaciones efectuadas en TALLERES NO AUTORIZADOS POR {policy.SupplierName}.{Environment.NewLine}{Environment.NewLine}3- Otros: Los aceites, grasas, lubricantes, filtros, correas, rotación de llantas y su alineación y balanceo y materiales indispensables para el funcionamiento o mantenimiento del vehículo, así como la mano de obra empleada en las revisiones periódicas de mantenimiento.{Environment.NewLine}{Environment.NewLine}La instalación y componentes eléctricos, los faros, bombillos y sus cubiertas exteriores, cristales y parabrisas, así como daños ocasionados a la pintura por influencias externas, químicas o mecánicas.{Environment.NewLine}{Environment.NewLine}Vehículos o componentes que tengan alteraciones o modificaciones no autorizadas.").FontSize(7.6f).LineHeight(1.4f).Justify();

                            });
                        });
                    });
                }
            })
            .GeneratePdf();
        }



        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 userId,  Int32 policyId)
        {

            try
            {

                var _response = await _dPolicy.GetOne(policyId, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOnePolicyDetails")]
        public async Task<IActionResult> GetOnePolicyDetails(Int32 policyId, Int32 km,DateTime date)
        {
            try
            {

                var _response = await _dPolicy.GetOnePolicyDetails(policyId,km,date);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetPolicyDetails")]
        public async Task<IActionResult> GetPolicyDetails(Int32 policyId)
        {
            try
            {

                var _response = await _dPolicy.GetPolicyDetails(policyId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetLogPolicyDetails")]
        public async Task<IActionResult> GetLogPolicyDetails(Int32 policyId)
        {
            try
            {

                var _response = await _dPolicy.GetLogPolicyDetails(policyId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


        

        [HttpPost("PostPolicy")]
        public async Task<IActionResult> Post_Policy(Int32 userId, Models.Policy policy)
        {
           

            try
            {
               var _response  = await _dPolicy.Post_Policy(policy, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(Int32 userId, List<Models.Action> actions)
        {

            try
            {

                var _response = await _dPolicy.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }


    }
}
