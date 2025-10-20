using Azure;
using ClosedXML.Excel;
using ClosedXML.Graphics;
using Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Configuration.Provider;
using System.IO.Packaging;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using Colors = QuestPDF.Helpers.Colors;


namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/SaleOrder")]
    [ApiController]

    [Authorize]
    public class cSaleOrder : ControllerBase
    {
        private readonly dSaleOrder _dSaleOrder;
        private readonly dAttachment _dAttachment;
        public cSaleOrder(dSaleOrder dSaleOrder, dAttachment dAttachment)
        {
            _dSaleOrder = dSaleOrder;
            _dAttachment = dAttachment;
         
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Int32 userId, Int32? supplierId,Int32? dealerId, Int32 rowfrom, string? filter)
        {

            try
            {
                var _response = await _dSaleOrder.GetAll(userId, supplierId,dealerId, rowfrom, filter);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(Int32 saleOrderId, Int32 userId)
        {

            try
            {
                var _response = await _dSaleOrder.GetOne(saleOrderId, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetOneWithContext")]
        public async Task<IActionResult> GetOneWithContext(Int32 userId, Int32 saleOrderId)
        {
            try
            {

                var _get = await _dSaleOrder.GetOne(saleOrderId, userId);
                var _details = await _dSaleOrder.GetDetails(saleOrderId);

                SaleOrderWithContext _req = new SaleOrderWithContext();
                _req.SaleOrder = (SaleOrder)(_get.Data);
                _req.Details = (List<SaleOrderDetail>)(_details.Data);

                Models.Response<SaleOrderWithContext> _response = new Models.Response<SaleOrderWithContext>();
                _response.Data = _req;
                _response.Total = _get.Total;
                _response.Processed = _get.Processed;
                _response.Message = _get.Message;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

/*
        [HttpGet("GetReasons")]
        public async Task<IActionResult> GetReasons()
        {

            try
            {
                var _response = await _dSaleOrder.GetReasons();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        */

        [HttpGet("GetSaleOrderTypes")]
        public async Task<IActionResult> GetSaleOrderTypes()
        {

            try
            {
                var _response = await _dSaleOrder.GetSaleOrderTypes();
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }

        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails(Int32 saleOrderId)
        {

            try
            {
                var _response = await _dSaleOrder.GetDetails(saleOrderId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }

        [HttpGet("GetReportFail")]
        public async Task<IActionResult> GetReportFail(Int32 userId, Int32? dealerId,String type, Int32 rowfrom)
        {

            try
            {
                var _response = await _dSaleOrder.GetReportFail(userId, dealerId,type, rowfrom);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }


        }



        [HttpGet("NewSaleOrderWithContext")]
        public async Task<IActionResult> NewSaleOrderWithContext(Int32 userId, Int32 dealerId, Int32 typeId)
        {
            try
            {

                SaleOrder _new = new SaleOrder();
                _new.Id = 0;
                _new.DealerId = dealerId;
                _new.SupplierId = 0;
                _new.TypeId = typeId;

                if (dealerId == 0)
                {
                    throw new Exception("Concesionario Invalido!.");
                }


                List<SaleOrder> _list = new List<SaleOrder>();
                _list.Add(_new);

                var _get2 = (SaleOrder)(await _dSaleOrder.getLast(userId, dealerId)).Data;

                if (_get2.Id is null || _get2.Id == 0)
                {

                    Models.Response <Result> _resp = await _dSaleOrder.PostSaleOrder(_list, userId);
                    if (_resp.Processed == false)
                    {
                        throw new Exception(_resp.Message);
                    }

                    Result _resul = new Result();
                    _resul = (Result)(_resp.Data);

                    var _get = await _dSaleOrder.GetOne(_resul.LastId, userId);
                    _new = (SaleOrder)(_get.Data);


                }
                else
                {
                    _new = _get2;
                }



                List<SaleOrderDetail> _details = new List<SaleOrderDetail>();

                SaleOrderWithContext _req = new SaleOrderWithContext();
                _req.SaleOrder = _new;
                _req.Details = _details;

                Models.Response<SaleOrderWithContext> _response = new Models.Response<SaleOrderWithContext>();
                _response.Data = _req;
                _response.Total = 1;


                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                Models.Response<SaleOrderWithContext> _response = new Models.Response<SaleOrderWithContext>();
                _response.SetError(ex);

                return StatusCode(StatusCodes.Status409Conflict, _response);
            }
        }



        [HttpGet("ExportSaleOrder")]
        public async Task<IActionResult> ExportSaleOrder(int saleOrderId, int userId)
        {
            try
            {
                Models.Response<Models.SaleOrder> response = await _dSaleOrder.GetOne(saleOrderId, userId);

                // 2. Validar que la respuesta es exitosa y contiene datos
                if (response.Status != StatusCodes.Status200OK || response.Data == null)
                {
                    response.SetError(new Exception("DATOS DE PEDIDO NO ENCONTRADOS"));
                    return StatusCode(response.Status, response);
                }

                // 3. Convertir los datos correctamente
                SaleOrder saleOrder;

                if (response.Data is SaleOrder saleOrderSingle)
                {
                    // Asignar directamente el modelo
                    saleOrder = saleOrderSingle;
                }
                else
                {
                    // Si no reconocemos el formato
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Formato de datos de Servicio no reconocido");
                }

                // 4. Validar que tenemos datos
                if (saleOrder == null)
                {
                    return NotFound("No se encontraron datos válidos de servicio");
                }

                // 5. Generar el PDF
                byte[] pdfBytes = await GeneratePdfReport(saleOrder, saleOrderId, userId);
                string fileName = $"Pedido_{saleOrderId.ToString() ?? "reporte"}.pdf";

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
        private async Task<byte[]> GeneratePdfReport(Models.SaleOrder saleOrder,int saleOrderId, int userId)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var response = _dSaleOrder.GetDetails(saleOrderId).GetAwaiter().GetResult();
            var SaleOrderDetailsList = new List<SaleOrderDetail>();

            SaleOrderDetailsList = response.Data.ToList();

            
            int? supplierId = saleOrder.SupplierId;
            string supplierImagePath = await GetFirstAttachmentFilePath("RECURSOS-EMPRESAS", supplierId);

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.Ledger.Portrait());

                    page.Header().Element(header =>
                    {
                        header.Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                // LOGO (izquierda)
                                row.ConstantItem(110).Image(supplierImagePath, ImageScaling.FitWidth);

                                // TÍTULO (centro)
                               
                                row.RelativeItem()
                                   .AlignCenter()
                                   .Text($"COMPROBANTE DE PEDIDO")
                                   .FontSize(20)
                                   .Bold();


                                // ID y fecha (derecha)
                                row.ConstantItem(130).Column(right =>
                                {
                                    right.Item().AlignRight().Text($"NUMERO DE PEDIDO: {saleOrderId}");
                                    right.Item().AlignRight().Text($"Fecha: {saleOrder.Created:dd/MM/yyyy}");
                                });
                            });

                            // BLOQUE DE INFORMACIÓN debajo del título
                            col.Item().PaddingTop(10).PaddingBottom(5).Border(1).Padding(3).Column(info =>
                            {

                                info.Item().Row(r =>
                                {
                                    r.ConstantItem(90).Text("PROVEEDOR:").Bold();
                                    r.RelativeItem().Text(saleOrder?.SupplierName ?? "N/A").WrapAnywhere();
                                });

                                info.Item().Row(r =>
                                {
                                    r.ConstantItem(90).Text("CLIENTE:").Bold();
                                    r.RelativeItem().Text(saleOrder?.DealerName ?? "N/A").WrapAnywhere();
                                });

                                info.Item().Row(r =>
                                {
                                    r.ConstantItem(90).Text("RESPONSABLE:").Bold();
                                    r.RelativeItem().Text($"{saleOrder?.NameCreatedBy ?? "N/A"}").WrapAnywhere();

                                });


                            });
                        });
                    });

                    page.Content().Column(column =>
                    {
                        column.Item().LineHorizontal(1);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Código
                                columns.RelativeColumn(3); // Descripción
                                columns.RelativeColumn(1); // Cantidad
                                columns.RelativeColumn(1); // PRECIOUNITARIO
                                columns.RelativeColumn(1); // SUBTOTAL
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("CODIGO");
                                header.Cell().Element(CellStyle).Text("DESCRIPCION");
                                header.Cell().Element(CellStyle).Text("CANTIDAD");
                                header.Cell().Element(CellStyle).Text("PRECIO UNITARIO");
                                header.Cell().Element(CellStyle).Text("SUBTOTAL");

                                IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.Bold()).Padding(5).Background("#EEE");
                            });
                            decimal total=0.00m;
                            int totalunidades = 0;
                            foreach (var detail in SaleOrderDetailsList)
                            {
                                table.Cell().Element(CellStyle).Text(detail.PartInnerCode);
                                table.Cell().Element(CellStyle).Text(detail.PartName);
                                table.Cell().Element(CellStyle).Text(detail.Quantity.ToString());
                                table.Cell().Element(CellStyle).Text($"{detail.Price.ToString()}$");
                                table.Cell().Element(CellStyle).Text($"{detail.SubTotal.ToString()}$");

                                IContainer CellStyle(IContainer container) =>
                                    container.Padding(5);
                                total += (decimal)detail.SubTotal;
                                totalunidades += (int)detail.Quantity;
                            }

                            table.Footer(footer =>
                            {

                                footer.Cell().ColumnSpan(5).BorderTop(1).AlignRight().Element(CellStyle).Text($"TOTAL UNIDADES: {totalunidades}        MONTO TOTAL: {total}$");
                              

                                IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.Bold()).Padding(5).Background("#EEE"); ;
                            });
                            column.Item().LineHorizontal(1);
                        });

                        column.Item().PaddingTop(25).Text("Observaciones: ______________________________________________________________________________________________________________");
                        column.Item().PaddingTop(25).AlignCenter().Text("DOCUMENTO NO FISCAL");
                    });


                });
            });

            return document.GeneratePdf();
        }
        [HttpGet("ExportExcelsaleOrder")]
        public async Task<IActionResult> GetExport(Int32 userId, Int32 supplierId)
        {
            try
            {
                List<SaleOrder> _response = await _dSaleOrder.GetExport(userId, supplierId);
                MemoryStream _excel = ConvertToExcel(_response);
                string _fileName = "Pedidos.xlsx";

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

        private MemoryStream ConvertToExcel(List<Models.SaleOrder> _saleOrders)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("PEDIDOS");

                // Encabezados (fila 1)  

                worksheet.Cell(1, 1).Value = "NRO";
                worksheet.Cell(1, 2).Value = "FECHA";
                worksheet.Cell(1, 3).Value = "CONCESIONARIO";
                worksheet.Cell(1, 4).Value = "TIPO";
                worksheet.Cell(1, 5).Value = "ESTATUS";
                worksheet.Cell(1, 6).Value = "USUARIO";

                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Font.Bold = true;

                worksheet.Range("A1:F1").SetAutoFilter();

                for (int i = 0; i < _saleOrders.Count; i++)
                {
                    var _saleOrder = _saleOrders[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = _saleOrder.Id;
                    worksheet.Cell(row, 2).Value = _saleOrder.Created;
                    worksheet.Cell(row, 3).Value = _saleOrder.DealerName;
                    worksheet.Cell(row, 4).Value = _saleOrder.TypeName;
                    worksheet.Cell(row, 5).Value = _saleOrder.StatusName;
                    worksheet.Cell(row, 6).Value = _saleOrder.CreatedBy;
                }

                worksheet.Columns().AdjustToContents();

                var centerStyle = worksheet.Style;
                centerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                centerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }


        [HttpPost("PostSaleOrder")]
        public async Task<IActionResult> PostSaleOrder(List<Models.SaleOrder> saleOrders, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.PostSaleOrder(saleOrders, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        [HttpPost("PostActions")]
        public async Task<IActionResult> Post_Actions(List<Models.Action> actions, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.Post_Actions(actions, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }

        }




        [HttpPost("PostDetail")]
        public async Task<IActionResult> PostDetail(Models.SaleOrderDetail detail, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.PostDetail(detail, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }



        [HttpPost("DeleteDetails")]
        public async Task<IActionResult> DeleteDetails(List<Models.Detail> details, Int32 userId)
        {
            try
            {
                var _response = await _dSaleOrder.DeleteDetails(details, userId);
                return StatusCode(_response.Status, _response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, ex.Message);
            }
        }

    }


}
