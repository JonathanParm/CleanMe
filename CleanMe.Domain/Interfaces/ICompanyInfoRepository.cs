using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface ICompanyInfoRepository
    {
        Task<CompanyInfo> GetCompanyInfoAsync();
        Task UpdateCompanyInfoAsync(CompanyInfo CompanyInfo);
    }
}