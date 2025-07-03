using OfficeOpenXml;
using Novacode; // Community DocX
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CleanMe.Application.Interfaces;

namespace CleanMe.Application.Services
{
    public class zzReportOutputService : zzIReportOutputService
    {
        public async Task<byte[]> GenerateExcelAsync<T>(List<T> data, string title)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(SanitizeWorksheetName(title));

                // Title Header
                worksheet.Cells["A1"].Value = title;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;

                worksheet.Cells["A2"].Value = $"Generated on: {System.DateTime.Now:yyyy-MM-dd HH:mm}";
                worksheet.Cells["A2"].Style.Font.Italic = true;

                // Table headers
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[4, i + 1].Value = properties[i].Name;
                    worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                }

                // Fill table data
                int row = 5;
                foreach (var item in data)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(item);
                        worksheet.Cells[row, i + 1].Value = value;
                    }
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;

                return await Task.FromResult(package.GetAsByteArray());
            }
        }

        public async Task<byte[]> GenerateWordAsync<T>(List<T> data, string title)
        {
            using (var ms = new MemoryStream())
            {
                var document = DocX.Create(ms);

                document.InsertParagraph(title)
                        .FontSize(18)
                        .Bold()
                        .Alignment = Alignment.center;

                document.InsertParagraph($"Generated on: {System.DateTime.Now:yyyy-MM-dd HH:mm}")
                        .SpacingAfter(20);

                var properties = typeof(T).GetProperties();
                var table = document.AddTable(data.Count + 1, properties.Length);

                table.Design = TableDesign.TableGrid;

                for (int i = 0; i < properties.Length; i++)
                {
                    table.Rows[0].Cells[i].Paragraphs[0].Append(properties[i].Name).Bold();
                }

                for (int r = 0; r < data.Count; r++)
                {
                    for (int c = 0; c < properties.Length; c++)
                    {
                        var value = properties[c].GetValue(data[r])?.ToString() ?? "";
                        table.Rows[r + 1].Cells[c].Paragraphs[0].Append(value);
                    }
                }

                document.InsertTable(table);
                document.Save();
                return await Task.FromResult(ms.ToArray());
            }
        }

        public async Task<byte[]> GeneratePdfAsync<T>(List<T> data, string title)
        {
            var properties = typeof(T).GetProperties();
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text(title).FontSize(20).Bold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            for (int i = 0; i < properties.Length; i++)
                                columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            foreach (var prop in properties)
                            {
                                header.Cell().Element(CellStyle).Text(prop.Name);
                            }
                        });

                        foreach (var item in data)
                        {
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(item)?.ToString() ?? "";
                                table.Cell().Element(CellStyle).Text(value);
                            }
                        }
                    });
                });
            });

            return await Task.FromResult(document.GeneratePdf());
        }

        private string SanitizeWorksheetName(string name)
        {
            var invalid = new string(Path.GetInvalidFileNameChars()) + new string(new[] { '[', ']', '*', '?' });
            foreach (var c in invalid)
            {
                name = name.Replace(c.ToString(), "");
            }

            return name.Length > 31 ? name.Substring(0, 31) : name;
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container.Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
        }
    }
}