using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Common;
using CleanMe.Domain.Entities;
using CleanMe.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using QuestPDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SettingService> _logger;

        public SettingService(IUnitOfWork unitOfWork, ILogger<SettingService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SettingViewModel> GetSettingViewModelAsync()
        {
            var Setting = await _unitOfWork.SettingRepository.GetSettingAsync();

            var viewModel = new SettingViewModel
            {
                settingId = Setting.settingId,
                ExcelExportPath = Setting.ExcelExportPath
            };
            return viewModel;
        }

        public async Task<string?> GetSettingValueAsync(string settingName)
        {
            var settings = await _unitOfWork.SettingRepository.GetSettingAsync();

            if (settings == null)
                return null;

            var setting = typeof(Settings).GetProperty(settingName);
            if (setting == null)
                return null;

            var value = setting.GetValue(settings);
            return value?.ToString();
        }

        public async Task<bool> UpdateSettingAsync(SettingViewModel settingVM, string updatedById)
        {
            var setting = await _unitOfWork.SettingRepository.GetSettingAsync();
            if (setting == null)
            {
                _logger.LogWarning($"System setting not found.");
                throw new Exception("System setting not found.");
            }

            setting.settingId = settingVM.settingId;
            setting.ExcelExportPath = settingVM.ExcelExportPath;
            setting.UpdatedAt = DateTime.Now;
            setting.UpdatedById = updatedById;

            _unitOfWork.SettingRepository.UpdateSettingAsync(setting);
            return true;
        }
    }
}
