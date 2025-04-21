using CleanMe.Application.Interfaces;
using CleanMe.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace CleanMe.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LookupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SelectListItem>> GetAreaSelectListAsync()
        {
            var types = await _unitOfWork.AreaRepository.GetAllAreasAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.areaId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetLocationSelectListAsync()
        {
            var locations = await _unitOfWork.AssetLocationRepository.GetAllAssetLocationsAsync();
            return locations.Select(l => new SelectListItem
            {
                Value = l.assetLocationId.ToString(),
                Text = l.Description
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetSelectListAsync()
        {
            var types = await _unitOfWork.AssetRepository.GetAllAssetsAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.assetId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetTypeSelectListAsync()
        {
            var types = await _unitOfWork.AssetTypeRepository.GetAllAssetTypesAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.assetTypeId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetTypeRateSelectListAsync()
        {
            var types = await _unitOfWork.AssetTypeRateRepository.GetAllAssetTypeRatesAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.assetTypeId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetCleanFrequencySelectListAsync()
        {
            var types = await _unitOfWork.CleanFrequencyRepository.GetAllCleanFrequenciesAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.cleanFrequencyId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetClientContactSelectListAsync()
        {
            var types = await _unitOfWork.ClientContactRepository.GetAllClientContactsAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.clientContactId.ToString(),
                Text = $"{t.FirstName} {t.FamilyName}"
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetClientSelectListAsync()
        {
            var clients = await _unitOfWork.ClientRepository.GetAllClientsAsync();
            return clients.Select(c => new SelectListItem
            {
                Value = c.clientId.ToString(),
                Text = c.Brand
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetRegionSelectListAsync()
        {
            var types = await _unitOfWork.RegionRepository.GetAllRegionsAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.regionId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetStaffSelectListAsync()
        {
            var types = await _unitOfWork.StaffRepository.GetAllStaffAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.staffId.ToString(),
                Text = $"{t.FirstName} {t.FamilyName}"
            });
        }
    }
}
