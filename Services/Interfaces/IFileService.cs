namespace CommerceApi.Services;

public interface IFileService
{
    byte[] ExportToCsv<T>(IEnumerable<T> data);
    byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName);
    List<T> ImportFromCsv<T>(Stream stream);
    List<T> ImportFromExcel<T>(Stream stream);
}
