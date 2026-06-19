using ClosedXML.Excel;

namespace WebApi.ExportFiles
{
    public static class ExportExcel
    {
        //public static MemoryStream ConvertToExcelReportCxC(List<Models.FIGO_ReportCxC> _reportData, DateTime Date)
        //{
        //    using (var workbook = new XLWorkbook())
        //    {
        //        string FormatDate = Date.ToString("dd/MM/yyyy");

        //        var worksheet = workbook.Worksheets.Add("RELACIÓN DE CUENTAS POR COBRAR");

        //        worksheet.Cell(1, 1).Value = $"RELACIÓN DE CUENTAS POR COBRAR - MONEDA ORIGINAL {_reportData[0].Currency} - Fecha de corte: {FormatDate}\"";
        //        worksheet.Cell(1, 1).Style.Font.Bold = true;
        //        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        //        worksheet.Range("A1:M1").Merge();
        //        worksheet.Row(1).Height = 25;

        //        int headerRow = 3;
        //        worksheet.Cell(headerRow, 1).Value = "EMPRESA";
        //        worksheet.Cell(headerRow, 2).Value = "CLIENTE";
        //        worksheet.Cell(headerRow, 3).Value = "RIF";
        //        worksheet.Cell(headerRow, 4).Value = "TELEFONO";
        //        worksheet.Cell(headerRow, 5).Value = "OCURRENCIA";
        //        worksheet.Cell(headerRow, 6).Value = "DOCUMENTO";
        //        worksheet.Cell(headerRow, 7).Value = "PRODUCTO";
        //        worksheet.Cell(headerRow, 8).Value = "EMISIÓN";
        //        worksheet.Cell(headerRow, 9).Value = "VENCIMIENTO";
        //        worksheet.Cell(headerRow, 10).Value = "DÍAS VENCIDO";
        //        worksheet.Cell(headerRow, 11).Value = "VENCIDO";
        //        worksheet.Cell(headerRow, 12).Value = "POR VENCER";
        //        worksheet.Cell(headerRow, 13).Value = "TOTAL DEUDA";

        //        int startDataRow = 4;
        //        for (int i = 0; i < _reportData.Count; i++)
        //        {
        //            var _data = _reportData[i];
        //            int row = startDataRow + i;

        //            worksheet.Cell(row, 1).Value = _data.DealerName;
        //            worksheet.Cell(row, 2).Value = _data.Client;
        //            worksheet.Cell(row, 3).Value = _data.Vat;
        //            worksheet.Cell(row, 4).Value = _data.Phone;
        //            worksheet.Cell(row, 5).Value = _data.Occurrence;
        //            worksheet.Cell(row, 6).Value = _data.Document;
        //            worksheet.Cell(row, 7).Value = _data.Product + " - " + _data.SerialNumber;
        //            worksheet.Cell(row, 8).Value = _data.IssueDate;
        //            worksheet.Cell(row, 9).Value = _data.DueDate;
        //            worksheet.Cell(row, 10).Value = _data.OverdueDays;
        //            worksheet.Cell(row, 11).Value = _data.OverdueAmount;
        //            worksheet.Cell(row, 12).Value = _data.CurrentAmount;
        //            worksheet.Cell(row, 13).Value = _data.TotalDebt;
        //            worksheet.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            worksheet.Cell(row, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            worksheet.Cell(row, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            worksheet.Cell(row, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //        }

        //        int lastDataRow = startDataRow + _reportData.Count - 1;
        //        if (_reportData.Count == 0) lastDataRow = headerRow;

        //        var tableRange = worksheet.Range($"A{headerRow}:M{lastDataRow}");
        //        var excelTable = tableRange.CreateTable();

        //        var headerRange = worksheet.Range($"A{headerRow}:M{headerRow}");
        //        headerRange.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
        //        headerRange.Style.Font.Bold = true;
        //        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //        if (_reportData.Count > 0)
        //        {
        //            excelTable.ShowTotalsRow = true;
        //            excelTable.Field(0).TotalsRowLabel = "TOTALES";

        //            excelTable.Field(9).TotalsRowFunction = XLTotalsRowFunction.Sum;
        //            excelTable.Field(10).TotalsRowFunction = XLTotalsRowFunction.Sum;
        //            excelTable.Field(11).TotalsRowFunction = XLTotalsRowFunction.Sum;
        //            excelTable.Field(12).TotalsRowFunction = XLTotalsRowFunction.Sum;

        //            // Forzar alineación a la derecha
        //            int totalsRowIndex = lastDataRow + 1;
        //            worksheet.Cell(totalsRowIndex, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            worksheet.Cell(totalsRowIndex, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //            worksheet.Cell(totalsRowIndex, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //        }

        //        int finalRowInSheet = _reportData.Count > 0 ? lastDataRow + 1 : headerRow;
        //        var dataRange = worksheet.Range($"A3:M{finalRowInSheet}");
        //        dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        //        worksheet.Columns().AdjustToContents();

        //        var stream = new MemoryStream();
        //        workbook.SaveAs(stream);
        //        stream.Position = 0;
        //        return stream;
        //    }
        //}
    }
}