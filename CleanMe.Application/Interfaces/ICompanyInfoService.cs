using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Services
{
    public interface ICompanyInfoService
    {
        Task<CompanyInfoViewModel> GetCompanyInfoViewModelAsync();
        Task<bool> UpdateCompanyInfoAsync(CompanyInfoViewModel companyInfo, string updatedById);
    }
}
