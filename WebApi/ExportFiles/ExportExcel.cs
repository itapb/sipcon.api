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
    }
}