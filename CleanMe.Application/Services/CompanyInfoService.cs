using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Services
{
    public class CompanyInfoService : ICompanyInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CompanyInfoService> _logger;

        public CompanyInfoService(IUnitOfWork unitOfWork, ILogger<CompanyInfoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CompanyInfoViewModel> GetCompanyInfoViewModelAsync()
        {
            var companyInfo = await _unitOfWork.CompanyInfoRepository.GetCompanyInfoAsync();

            var viewModel = new CompanyInfoViewModel
            {
                companyInfoId = companyInfo.companyInfoId,
                Name = companyInfo.Name,
                Phone = companyInfo.Phone,
                Email = companyInfo.Email,
                Address = new AddressViewModel
                {
                    Line1 = companyInfo.Address.Line1,
                    Line2 = companyInfo.Address.Line2,
                    Suburb = companyInfo.Address.Suburb,
                    TownOrCity = companyInfo.Address.TownOrCity,
                    Postcode = companyInfo.Address.Postcode
                }
            };
            return viewModel;
        }

        public async Task<bool> UpdateCompanyInfoAsync(CompanyInfoViewModel companyInfoVM, string updatedById)
        {
            var companyInfo = await _unitOfWork.CompanyInfoRepository.GetCompanyInfoAsync();
            if (companyInfo == null)
            {
                _logger.LogWarning($"Company information not found.");
                throw new Exception("Company information not found.");
            }

            companyInfo.companyInfoId = companyInfoVM.companyInfoId;
            companyInfo.Name = companyInfoVM.Name;
            companyInfo.Phone = companyInfoVM.Phone;
            companyInfo.Email = companyInfoVM.Email;
            companyInfo.Address = new Address
            {
                Line1 = companyInfoVM.Address.Line1,
                Line2 = companyInfoVM.Address.Line2,
                Suburb = companyInfoVM.Address.Suburb,
                TownOrCity = companyInfoVM.Address.TownOrCity,
                Postcode = companyInfoVM.Address.Postcode,
            };
            companyInfo.UpdatedAt = DateTime.Now;
            companyInfo.UpdatedById = updatedById;

            _unitOfWork.CompanyInfoRepository.UpdateCompanyInfoAsync(companyInfo);
            return true;
        }
    }
}
