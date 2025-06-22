using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Services
{
    public interface ICompanyInfoService
    {
        Task<CompanyInfoViewModel> GetCompanyInfoViewModelAsync();
        Task<bool> UpdateCompanyInfoAsync(CompanyInfoViewModel companyInfo, string updatedById);
    }
}
