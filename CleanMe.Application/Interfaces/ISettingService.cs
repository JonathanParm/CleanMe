using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Services
{
    public interface ISettingService
    {
        Task<SettingViewModel> GetSettingViewModelAsync();
        Task<string?> GetSettingValueAsync(string settingName);
        Task<bool> UpdateSettingAsync(SettingViewModel Setting, string updatedById);
    }
}
