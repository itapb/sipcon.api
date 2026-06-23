using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WebApi.ExportFiles
{
    public static class ExportPDF
    {
        //public static byte[] ConvertToPdfReportCxC(List<Models.FIGO_ReportCxC> _reportData, DateTime Date)
        //{
        //    QuestPDF.Settings.License = LicenseType.Community;

        //    var PDF = Document.Create(container =>
        //    {
        //        container.Page(page =>
        //        {
        //            page.Size(PageSizes.A4.Portrait());
        //            page.Margin(1, Unit.Centimetre);
        //            page.PageColor(Colors.White);

        //            // --- HEADER ---
        //            page.Header().PaddingBottom(10).Row(row =>
        //            {
        //                row.RelativeItem().Column(col =>
        //                {
        //                    col.Item().AlignLeft().Text(_reportData[0].DealerName).FontSize(9).Bold();
        //                });

        //                row.RelativeItem(2).Column(col =>
        //                {
        //                    col.Item().AlignCenter().Text(x =>
        //                    {
        //                        x.Span("RELACIÓN DE CUENTAS POR COBRAR\n").FontSize(12).Bold();
        //                        x.Span($"MONEDA ORIGINAL {_reportData[0].Currency}\n").FontSize(11).Bold();
        //                        x.Span($"FECHA DE CORTE: {Date:dd/MM/yyyy}").FontSize(11).Bold();
        //                    });
        //                });

        //                row.RelativeItem().Column(col =>
        //                {
        //                    col.Item().AlignRight().Text(x =>
        //                    {
        //                        x.Span("Página ").FontSize(9);
        //                        x.CurrentPageNumber().FontSize(9);
        //                        x.Span(" de ").FontSize(9);
        //                        x.TotalPages().FontSize(9);
        //                    });

        //                    col.Item().AlignRight().Text(x =>
        //                    {
        //                        x.Span("Fecha: ").FontSize(9);
        //                        x.Span(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")).FontSize(9);
        //                    });
        //                });
        //            });

        //            // --- BODY ---
        //            page.Content().Column(col =>
        //            {
        //                var agrupacionClientes = _reportData
        //                    .GroupBy(x => x.Vat)
        //                    .ToList();

        //                col.Item().Table(table =>
        //                {
        //                    table.ColumnsDefinition(columns =>
        //                    {
        //                        columns.RelativeColumn(1.5f);
        //                        columns.RelativeColumn(1.5f);
        //                        columns.RelativeColumn(2.0f);
        //                        columns.RelativeColumn(1.2f);
        //                        columns.RelativeColumn(1.2f);
        //                        columns.RelativeColumn(1f);  
        //                        columns.RelativeColumn(1.2f);
        //                        columns.RelativeColumn(1.2f);
        //                        columns.RelativeColumn(1.2f);
        //                    });

        //                    table.Header(header =>
        //                    {
        //                        header.Cell().Element(HeaderStyle).Text("OCURRENCIA");
        //                        header.Cell().Element(HeaderStyle).Text("DOCUMENTO");
        //                        header.Cell().Element(HeaderStyle).Text("PRODUCTO");
        //                        header.Cell().Element(HeaderStyle).Text("EMISIÓN");
        //                        header.Cell().Element(HeaderStyle).Text("VENCIMIENTO");
        //                        header.Cell().Element(HeaderStyle).Text("DÍAS VENC.");
        //                        header.Cell().Element(HeaderStyle).Text("VENCIDO");
        //                        header.Cell().Element(HeaderStyle).Text("POR VENCER");
        //                        header.Cell().Element(HeaderStyle).Text("TOTAL DEUDA");
        //                    });

        //                    foreach (var grupo in agrupacionClientes)
        //                    {
        //                        var primerRegistro = grupo.First();
        //                        string nombreCliente = primerRegistro.Client ?? "S/N";
        //                        string telefonoCliente = primerRegistro.Phone ?? "S/T";

        //                        table.Cell().RowSpan(1).ColumnSpan(9).Background(Colors.Grey.Lighten4).Padding(4).Text(x =>
        //                        {
        //                            x.Span("Cliente: ").Bold().FontSize(9);
        //                            x.Span($"{nombreCliente}         ").FontSize(9);
        //                            x.Span("Teléfono: ").Bold().FontSize(9);
        //                            x.Span(telefonoCliente).FontSize(9);
        //                        });

        //                        int totalDiasVenc = 0;
        //                        decimal totalVencido = 0;
        //                        decimal totalPorVencer = 0;
        //                        decimal totalDeuda = 0;

        //                        foreach (var detalle in grupo)
        //                        {
        //                            table.Cell().Element(RowStyle).Text(detalle.Occurrence ?? "");
        //                            table.Cell().Element(RowStyle).Text(detalle.Document ?? "");
        //                            table.Cell().Element(RowStyle).Text($"{detalle.Product} - {detalle.SerialNumber}");
        //                            table.Cell().Element(RowStyle).Text(detalle.IssueDate ?? "");
        //                            table.Cell().Element(RowStyle).Text(detalle.DueDate ?? "");

        //                            table.Cell().Element(RowStyle).Text(detalle.OverdueDays.ToString());
        //                            table.Cell().Element(RowStyle).Text(detalle.OverdueAmount.ToString("N2"));
        //                            table.Cell().Element(RowStyle).Text(detalle.CurrentAmount.ToString("N2"));
        //                            table.Cell().Element(RowStyle).Text(detalle.TotalDebt.ToString("N2"));

        //                            totalDiasVenc += detalle.OverdueDays;
        //                            totalVencido += detalle.OverdueAmount;
        //                            totalPorVencer += detalle.CurrentAmount;
        //                            totalDeuda += detalle.TotalDebt;
        //                        }

        //                        table.Cell().RowSpan(1).ColumnSpan(5).Padding(3).AlignRight().Text("TOTAL CLIENTE:").Bold().FontSize(8);

        //                        table.Cell().Element(TotalStyle).Text(totalDiasVenc.ToString());
        //                        table.Cell().Element(TotalStyle).Text(totalVencido.ToString("N2"));
        //                        table.Cell().Element(TotalStyle).Text(totalPorVencer.ToString("N2"));
        //                        table.Cell().Element(TotalStyle).Text(totalDeuda.ToString("N2"));
        //                    }
        //                });
        //            });
        //        });
        //    });

        //    return PDF.GeneratePdf();
        //}

        // Estilos
        static IContainer HeaderStyle(IContainer container) =>
            container.Background(Colors.Grey.Lighten2).Padding(2).DefaultTextStyle(x => x.SemiBold().FontSize(8));

        static IContainer RowStyle(IContainer container) =>
            container.Padding(2).DefaultTextStyle(x => x.FontSize(8));

        static IContainer TotalStyle(IContainer container) =>
            container.Background(Colors.Grey.Lighten3).Padding(3).DefaultTextStyle(x => x.Bold().FontSize(8));
    }
}