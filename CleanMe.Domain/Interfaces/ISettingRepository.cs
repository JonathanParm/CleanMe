using CleanMe.Domain.Entities;

namespace CleanMe.Domain.Interfaces
{
    public interface ISettingRepository
    {
        Task<Setting> GetSettingAsync();
        Task UpdateSettingAsync(Setting Setting);
    }
}