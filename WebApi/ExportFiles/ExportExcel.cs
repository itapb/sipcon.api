using ClosedXML.Excel;

namespace WebApi.ExportFiles
{
    public static class ExportExcel
    {
        public static MemoryStream ConvertToExcel(List<Dictionary<string, object>> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DATA");

                if (data.Any())
                {
                    // Encabezados dinámicos
                    var headers = data.First().Keys.ToList();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                    }

                    var headerRange = worksheet.Range(1, 1, 1, headers.Count);
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Font.Bold = true;
                    headerRange.SetAutoFilter();

                    // Filas dinámicas
                    int row = 2;
                    foreach (var dict in data)
                    {
                        for (int col = 0; col < headers.Count; col++)
                        {
                            var cell = worksheet.Cell(row, col + 1);
                            var value = dict[headers[col]];

                            if (value == null)
                            {
                                cell.Value = string.Empty;
                            }
                            else if (value is DateTime dt)
                            {
                                cell.Value = dt;
                            }
                            else if (value is int i)
                            {
                                cell.Value = i;
                            }
                            else if (value is decimal d)
                            {
                                cell.Value = d;
                            }
                            else if (value is bool b)
                            {
                                cell.Value = b ? "SI" : "NO";
                            }
                            else
                            {
                                cell.Value = value.ToString();
                            }
                        }
                        row++;
                    }



                    worksheet.Columns().AdjustToContents();
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }



        public static MemoryStream ConvertToExcelFigo(List<Dictionary<string, object>> data, List<string> columnsToTotal)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DATA");

                if (data.Any())
                {
                    // Encabezados dinámicos
                    var headers = data.First().Keys.ToList();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = headers[i];
                    }

                    var headerRange = worksheet.Range(1, 1, 1, headers.Count);
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Font.Bold = true;
                    headerRange.SetAutoFilter();

                    // Filas dinámicas
                    int row = 2;
                    foreach (var dict in data)
                    {
                        for (int col = 0; col < headers.Count; col++)
                        {
                            var cell = worksheet.Cell(row, col + 1);
                            var value = dict[headers[col]];

                            if (value == null)
                            {
                                cell.Value = string.Empty;
                            }
                            else if (value is DateTime dt)
                            {
                                cell.Value = dt;
                            }
                            else if (value is int i)
                            {
                                cell.Value = i;
                            }
                            else if (value is decimal d)
                            {
                                cell.Value = d;
                            }
                            else if (value is bool b)
                            {
                                cell.Value = b ? "SI" : "NO";
                            }
                            else
                            {
                                cell.Value = value.ToString();
                            }
                        }
                        row++;
                    }

                    // --- NUEVA SECCIÓN: TOTALIZACIÓN DINÁMICA ---
                    if (columnsToTotal != null && columnsToTotal.Any())
                    {
                        int totalRow = row; // La fila siguiente al terminar el ciclo foreach

                        // Colocamos la palabra "TOTAL" en la primera celda de la fila final
                        var firstCell = worksheet.Cell(totalRow, 1);
                        firstCell.Value = "TOTAL";
                        firstCell.Style.Font.Bold = true;

                        // Estilizamos toda la fila de totales con una línea doble inferior contable
                        var totalRange = worksheet.Range(totalRow, 1, totalRow, headers.Count);
                        totalRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        totalRange.Style.Border.BottomBorder = XLBorderStyleValues.Double;

                        // Evaluamos cada columna del Excel para ver si debe ser totalizada
                        for (int col = 0; col < headers.Count; col++)
                        {
                            string currentHeader = headers[col];

                            // Si esta columna está en la lista de campos a totalizar
                            if (columnsToTotal.Contains(currentHeader))
                            {
                                var cell = worksheet.Cell(totalRow, col + 1);

                                // Obtenemos la letra de la columna en Excel (ej: "A", "G", "AA")
                                string colLetter = cell.Address.ColumnLetter;

                                // Inyectamos la fórmula SUM de Excel desde la fila 2 hasta la última fila con datos (totalRow - 1)
                                cell.FormulaA1 = $"=SUM({colLetter}2:{colLetter}{totalRow - 1})";
                                cell.Style.Font.Bold = true;
                            }
                        }
                    }
                    // ---------------------------------------------

                    worksheet.Columns().AdjustToContents();
                }

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream;
            }
        }
    }
}