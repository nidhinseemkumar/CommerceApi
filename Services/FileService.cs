using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace CommerceApi.Services;

public class FileService : IFileService
{
    public byte[] ExportToCsv<T>(IEnumerable<T> data)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        csv.WriteRecords(data);
        writer.Flush();
        return memoryStream.ToArray();
    }

    public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);
        
        // Populate headers and data using reflection-based helper OR simple AddRange
        worksheet.Cell(1, 1).InsertTable(data);
        worksheet.Columns().AdjustToContents();

        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    public List<T> ImportFromCsv<T>(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csv.GetRecords<T>().ToList();
    }

    public List<T> ImportFromExcel<T>(Stream stream)
    {
        var list = new List<T>();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var table = worksheet.Table(0); // Assumes data was inserted as table or first range

        if (table == null)
        {
             // Fallback if no table: use range
             var range = worksheet.RangeUsed();
             if (range == null) return list;
             return MapRangeToObjects<T>(range);
        }

        return MapTableToObjects<T>(table);
    }

    private List<T> MapTableToObjects<T>(IXLTable table)
    {
        var list = new List<T>();
        var properties = typeof(T).GetProperties();
        var headers = table.Fields.Select(f => f.Name).ToList();

        foreach (var row in table.DataRange.Rows())
        {
            var obj = Activator.CreateInstance<T>();
            for (int i = 0; i < headers.Count; i++)
            {
                var prop = properties.FirstOrDefault(p => p.Name.Equals(headers[i], StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    var val = row.Cell(i + 1).Value;
                    prop.SetValue(obj, ConvertValue(val, prop.PropertyType));
                }
            }
            list.Add(obj);
        }
        return list;
    }

    private List<T> MapRangeToObjects<T>(IXLRange range)
    {
        var list = new List<T>();
        var properties = typeof(T).GetProperties();
        var rows = range.Rows().ToList();
        if (rows.Count < 2) return list;

        var headers = rows[0].Cells().Select(c => c.Value.ToString()).ToList();

        foreach (var row in rows.Skip(1))
        {
            var obj = Activator.CreateInstance<T>();
            for (int i = 0; i < headers.Count; i++)
            {
                var prop = properties.FirstOrDefault(p => p.Name.Equals(headers[i], StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    var val = row.Cell(i + 1).Value;
                    prop.SetValue(obj, ConvertValue(val, prop.PropertyType));
                }
            }
            list.Add(obj);
        }
        return list;
    }

    private object? ConvertValue(XLCellValue value, Type targetType)
    {
        if (value.IsBlank) return null;
        
        string stringVal = value.ToString();
        if (targetType == typeof(int)) return int.Parse(stringVal);
        if (targetType == typeof(decimal)) return decimal.Parse(stringVal);
        if (targetType == typeof(double)) return double.Parse(stringVal);
        if (targetType == typeof(DateTime)) return value.GetDateTime();
        
        return Convert.ChangeType(stringVal, targetType);
    }
}
