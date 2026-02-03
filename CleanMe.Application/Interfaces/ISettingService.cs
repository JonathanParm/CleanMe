using CleanMe.Application.ViewModels;

namespace CleanMe.Application.Services
{
    public interface ISettingService
    {
        Task<SettingViewModel> GetSettingViewModelAsync();
        Task<string?> GetSettingValueAsync(string settingName);
        Task<bool> UpdateSettingAsync(SettingViewModel Setting, string updatedById);
    }
}
