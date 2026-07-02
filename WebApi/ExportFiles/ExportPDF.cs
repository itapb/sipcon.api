using Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace WebApi.ExportFiles
{
    public static class ExportPDF
    {
       
        public static byte[] PdfFactory(  int reportId, List<Dictionary<string, object>> data,  string jsonParameters)
        {

           
            switch (reportId)
            {
                case 1:
                    {
                        var model = Map<FIGO_ReportCxC>(data);

                        DateTime fechaCorte = JsonDocument.Parse(jsonParameters).RootElement.GetProperty("FECHA_CORTE")
                                                 .GetString() is string fecha ? DateTime.Parse(fecha) : DateTime.Today;

                        return ReportCxCPdf(model, fechaCorte);
                    }

                case 2:
                    {
                        var model = Map<FIGO_ReportCxC>(data);

                        DateTime fechaCorte = JsonDocument.Parse(jsonParameters).RootElement.GetProperty("FECHA_CORTE")
                                                 .GetString() is string fecha ? DateTime.Parse(fecha) : DateTime.Today;

                        return ReportCxCPdf(model, fechaCorte);
                    }

                case 3:
                    {
                        var model = Map<FIGO_ReportCxC>(data);

                        DateTime fechaCorte = JsonDocument.Parse(jsonParameters).RootElement.GetProperty("FECHA_CORTE")
                                                 .GetString() is string fecha ? DateTime.Parse(fecha) : DateTime.Today;

                        return ReportCxCPdf(model, fechaCorte);
                    }

                default:
                    throw new NotImplementedException($"No existe un PDF para el ReportId {reportId}");
            }
        }

        public static List<T> Map<T>(List<Dictionary<string, object>> data) where T : new()
        {
            var result = new List<T>();

            if (data == null)
                return result;

            var properties = typeof(T).GetProperties();

            foreach (var row in data)
            {
                T item = new();

                foreach (var property in properties)
                {
                    // 1. Obtener nombre de columna desde atributo
                    var columnName = property
                        .GetCustomAttributes(typeof(ColumnAttribute), false)
                        .Cast<ColumnAttribute>()
                        .FirstOrDefault()?.Name
                        ?? property.Name;

                    // 2. Buscar key en dictionary (case insensitive)
                    var key = row.Keys.FirstOrDefault(x =>
                        x.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                    if (key == null)
                        continue;

                    var value = row[key];

                    if (value == null || value == DBNull.Value)
                        continue;

                    try
                    {
                        var propertyType = Nullable.GetUnderlyingType(property.PropertyType)
                                           ?? property.PropertyType;

                        // Manejo seguro de conversiones comunes
                        object safeValue = ConvertValue(value, propertyType);

                        property.SetValue(item, safeValue);
                    }
                    catch
                    {
                        // ignorar errores de conversión
                    }
                }

                result.Add(item);
            }

            return result;
        }

        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null!;

            if (targetType == typeof(string))
                return value.ToString() ?? string.Empty;

            if (targetType == typeof(int))
                return Convert.ToInt32(value);

            if (targetType == typeof(long))
                return Convert.ToInt64(value);

            if (targetType == typeof(decimal))
                return Convert.ToDecimal(value);

            if (targetType == typeof(DateTime))
                return Convert.ToDateTime(value);

            if (targetType.IsEnum)
                return Enum.Parse(targetType, value.ToString()!);

            return Convert.ChangeType(value, targetType);
        }

        public static byte[] ReportCxCPdf(List<Models.FIGO_ReportCxC> _reportData, DateTime Date)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var PDF = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Portrait());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    // --- HEADER ---
                    page.Header().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignLeft().Text(_reportData[0].DealerName).FontSize(9).Bold();
                        });

                        row.RelativeItem(2).Column(col =>
                        {
                            col.Item().AlignCenter().Text(x =>
                            {
                                x.Span("RELACIÓN DE CUENTAS POR COBRAR\n").FontSize(12).Bold();
                                x.Span($"MONEDA ORIGINAL {_reportData[0].Currency}\n").FontSize(11).Bold();
                                x.Span($"FECHA DE CORTE: {Date:dd/MM/yyyy}").FontSize(11).Bold();
                            });
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignRight().Text(x =>
                            {
                                x.Span("Página ").FontSize(9);
                                x.CurrentPageNumber().FontSize(9);
                                x.Span(" de ").FontSize(9);
                                x.TotalPages().FontSize(9);
                            });

                            col.Item().AlignRight().Text(x =>
                            {
                                x.Span("Fecha: ").FontSize(9);
                                x.Span(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")).FontSize(9);
                            });
                        });
                    });

                    // --- BODY ---
                    page.Content().Column(col =>
                    {
                        var agrupacionClientes = _reportData
                            .GroupBy(x => x.Vat)
                            .ToList();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(2.0f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderStyle).Text("OCURRENCIA");
                                header.Cell().Element(HeaderStyle).Text("DOCUMENTO");
                                header.Cell().Element(HeaderStyle).Text("PRODUCTO");
                                header.Cell().Element(HeaderStyle).Text("EMISIÓN");
                                header.Cell().Element(HeaderStyle).Text("VENCIMIENTO");
                                header.Cell().Element(HeaderStyle).Text("DÍAS VENC.");
                                header.Cell().Element(HeaderStyle).Text("VENCIDO");
                                header.Cell().Element(HeaderStyle).Text("POR VENCER");
                                header.Cell().Element(HeaderStyle).Text("TOTAL DEUDA");
                            });

                            foreach (var grupo in agrupacionClientes)
                            {
                                var primerRegistro = grupo.First();
                                string nombreCliente = primerRegistro.Client ?? "S/N";
                                string telefonoCliente = primerRegistro.Phone ?? "S/T";

                                table.Cell().RowSpan(1).ColumnSpan(9).Background(Colors.Grey.Lighten4).Padding(4).Text(x =>
                                {
                                    x.Span("Cliente: ").Bold().FontSize(9);
                                    x.Span($"{nombreCliente}         ").FontSize(9);
                                    x.Span("Teléfono: ").Bold().FontSize(9);
                                    x.Span(telefonoCliente).FontSize(9);
                                });

                                int totalDiasVenc = 0;
                                decimal totalVencido = 0;
                                decimal totalPorVencer = 0;
                                decimal totalDeuda = 0;

                                foreach (var detalle in grupo)
                                {
                                    table.Cell().Element(RowStyle).Text(detalle.Occurrence ?? "");
                                    table.Cell().Element(RowStyle).Text(detalle.Document ?? "");
                                    table.Cell().Element(RowStyle).Text($"{detalle.Product} - {detalle.SerialNumber}");
                                    table.Cell().Element(RowStyle).Text(detalle.IssueDate ?? "");
                                    table.Cell().Element(RowStyle).Text(detalle.DueDate ?? "");

                                    table.Cell().Element(RowStyle).Text(detalle.OverdueDays.ToString());
                                    table.Cell().Element(RowStyle).Text(detalle.OverdueAmount.ToString("N2"));
                                    table.Cell().Element(RowStyle).Text(detalle.CurrentAmount.ToString("N2"));
                                    table.Cell().Element(RowStyle).Text(detalle.TotalDebt.ToString("N2"));

                                    totalDiasVenc += detalle.OverdueDays;
                                    totalVencido += detalle.OverdueAmount;
                                    totalPorVencer += detalle.CurrentAmount;
                                    totalDeuda += detalle.TotalDebt;
                                }

                                table.Cell().RowSpan(1).ColumnSpan(5).Padding(3).AlignRight().Text("TOTAL CLIENTE:").Bold().FontSize(8);

                                table.Cell().Element(TotalStyle).Text(totalDiasVenc.ToString());
                                table.Cell().Element(TotalStyle).Text(totalVencido.ToString("N2"));
                                table.Cell().Element(TotalStyle).Text(totalPorVencer.ToString("N2"));
                                table.Cell().Element(TotalStyle).Text(totalDeuda.ToString("N2"));
                            }
                        });
                    });
                });
            });

            return PDF.GeneratePdf();
        }

        // Estilos
        static IContainer HeaderStyle(IContainer container) =>
            container.Background(Colors.Grey.Lighten2).Padding(2).DefaultTextStyle(x => x.SemiBold().FontSize(8));

        static IContainer RowStyle(IContainer container) =>
            container.Padding(2).DefaultTextStyle(x => x.FontSize(8));

        static IContainer TotalStyle(IContainer container) =>
            container.Background(Colors.Grey.Lighten3).Padding(3).DefaultTextStyle(x => x.Bold().FontSize(8));


   //     public static byte[] ConvertToPdfReport(
   //List<Dictionary<string, object>> reportData,
   //string jsonParameters)
   //     {
   //         QuestPDF.Settings.License = LicenseType.Community;

   //         if (reportData == null || reportData.Count == 0)
   //             throw new Exception("No existen datos para generar el PDF.");

   //         //==========================
   //         // FECHA DE CORTE
   //         //==========================
   //         DateTime? fechaCorte = null;

   //         if (!string.IsNullOrWhiteSpace(jsonParameters))
   //         {
   //             using var json = JsonDocument.Parse(jsonParameters);

   //             if (json.RootElement.TryGetProperty("FECHA_CORTE", out var fechaElement))
   //             {
   //                 DateTime fecha;

   //                 if (DateTime.TryParse(fechaElement.GetString(), out fecha))
   //                     fechaCorte = fecha;
   //             }
   //         }

   //         //==========================
   //         // DATOS CABECERA
   //         //==========================
   //         var first = reportData.First();

   //         string dealer = first.ContainsKey("DealerName")
   //             ? first["DealerName"]?.ToString() ?? ""
   //             : "";

   //         string currency = first.ContainsKey("Currency")
   //             ? first["Currency"]?.ToString() ?? ""
   //             : "";

   //         //==========================
   //         // COLUMNAS
   //         //==========================
   //         var columns = reportData.First().Keys.ToList();

   //         var pdf = Document.Create(container =>
   //         {
   //             container.Page(page =>
   //             {
   //                 page.Size(PageSizes.A4.Landscape());

   //                 page.Margin(1, Unit.Centimetre);

   //                 page.PageColor(Colors.White);

   //                 //---------------------------------------
   //                 // HEADER
   //                 //---------------------------------------
   //                 page.Header().PaddingBottom(10).Row(row =>
   //                 {
   //                     row.RelativeItem().Column(col =>
   //                     {
   //                         col.Item().AlignLeft().Text(dealer).Bold().FontSize(9);
   //                     });

   //                     row.RelativeItem(2).Column(col =>
   //                     {
   //                         col.Item().AlignCenter().Text(x =>
   //                         {
   //                             x.Span("REPORTE\n").FontSize(12).Bold();

   //                             if (!string.IsNullOrWhiteSpace(currency))
   //                                 x.Span($"MONEDA {currency}\n").FontSize(10).Bold();

   //                             if (fechaCorte.HasValue)
   //                                 x.Span($"FECHA DE CORTE: {fechaCorte:dd/MM/yyyy}")
   //                                     .FontSize(10)
   //                                     .Bold();
   //                         });
   //                     });

   //                     row.RelativeItem().Column(col =>
   //                     {
   //                         col.Item().AlignRight().Text(text =>
   //                         {
   //                             text.DefaultTextStyle(x => x.FontSize(9));

   //                             text.Span("Página ");
   //                             text.CurrentPageNumber();
   //                             text.Span(" de ");
   //                             text.TotalPages();
   //                         });

   //                         col.Item().AlignRight().Text(
   //                             DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))
   //                             .FontSize(9);
   //                     });
   //                 });

   //                 //---------------------------------------
   //                 // TABLA
   //                 //---------------------------------------
   //                 page.Content().Table(table =>
   //                 {
   //                     table.ColumnsDefinition(c =>
   //                     {
   //                         foreach (var item in columns)
   //                             c.RelativeColumn();
   //                     });

   //                     //-----------------------------------
   //                     // CABECERA
   //                     //-----------------------------------
   //                     table.Header(header =>
   //                     {
   //                         foreach (var column in columns)
   //                         {
   //                             header.Cell()
   //                                 .Element(HeaderStyle)
   //                                 .Text(column.Replace("_", " "));
   //                         }
   //                     });

   //                     //-----------------------------------
   //                     // FILAS
   //                     //-----------------------------------
   //                     foreach (var row in reportData)
   //                     {
   //                         foreach (var column in columns)
   //                         {
   //                             row.TryGetValue(column, out var value);

   //                             table.Cell()
   //                                  .Element(RowStyle)
   //                                  .Text(value?.ToString() ?? "");
   //                         }
   //                     }
   //                 });
   //             });
   //         });

   //         return pdf.GeneratePdf();
   //     }

   //     static IContainer HeaderStyle(IContainer container)
   //     {
   //         return container
   //             .Background(Colors.Grey.Lighten2)
   //             .Border(1)
   //             .Padding(3)
   //             .DefaultTextStyle(x => x.SemiBold().FontSize(8));
   //     }

   //     static IContainer RowStyle(IContainer container)
   //     {
   //         return container
   //             .BorderBottom(0.5f)
   //             .Padding(3)
   //             .DefaultTextStyle(x => x.FontSize(8));
   //     }
    }
}