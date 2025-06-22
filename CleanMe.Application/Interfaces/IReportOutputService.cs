using System.Threading.Tasks;

namespace CleanMe.Application.Interfaces
{
    public interface IReportOutputService
    {
        Task<byte[]> GenerateExcelAsync<T>(List<T> data, string title);
        Task<byte[]> GenerateWordAsync<T>(List<T> data, string title);
        Task<byte[]> GeneratePdfAsync<T>(List<T> data, string title);
    }
}
