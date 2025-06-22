using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.DTOs
{
    public class zzExportScheduleToExcelDto
    {
        public int StaffId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public string Address
        {
            get
            {
                return _companyInfo.Address.ToMultiLine();
            }
        }

        public string CompanyAndContactDetails
        {
            get
            {
                return string.Format("{0}{3}{1}{3}{2}", _companyInfo.Name, _companyInfo.Phone, _companyInfo.Email, Environment.NewLine);
            }
        }

        private readonly ICompanyInfoService _companyInfoService;
        private readonly IStaffService _staffService;
        private readonly CompanyInfoViewModel _companyInfo;

        public zzExportScheduleToExcelDto(ICompanyInfoService companyInfoService, IStaffService staffService)
        {
            _staffService = staffService;
            _companyInfoService = companyInfoService;

            // Fixed the conversion issue by mapping CompanyInfoViewModel to CompanyInfo
            var companyInfoViewModel = _companyInfoService.GetCompanyInfoViewModelAsync().GetAwaiter().GetResult();
        }

    }
}
