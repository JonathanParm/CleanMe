using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
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

        public async Task<IEnumerable<SelectListItem>> GetAmendmentTypeSelectListAsync()
        {
            var types = await _unitOfWork.AmendmentTypeRepository.GetAllAmendmentTypesAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.amendmentTypeId.ToString(),
                Text = t.Name
            });
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

        public async Task<IEnumerable<SelectListItem>> GetStockCodeSelectListAsync()
        {
            var types = await _unitOfWork.StockCodeRepository.GetAllStockCodesAsync();
            return types.Select(t => new SelectListItem
            {
                Value = t.stockCodeId.ToString(),
                Text = t.Name
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetAreasByClientAsync(int? clientId = 0)
        {
            try
            {
                var query = "EXEC dbo.AreaLookupByClient @clientId";
                var parameters = new
                {
                    clientId = clientId,
                };

                return await GetSelectListAsync(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error Area lookup list Id and Name from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetLocationsByClientAndAreaAsync(int? clientId = 0, int? areaId = 0)
        {
            try
            {
                var query = "EXEC dbo.AssetLocationLookupByClientAndArea @clientId, @areaId";
                var parameters = new
                {
                    clientId = clientId,
                    areaId = areaId
                };

                return await GetSelectListAsync(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error Asset location lookup list Id and Name from stored procedure", ex);
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetsByClientAreaAndLocationAsync(int? clientId = 0, int? areaId = 0, int? assetLocationId = 0)
        {
            try
            {
                var query = "EXEC dbo.AssetLookupByClientAreaAndLocation @clientId, @areaId, @assetLocationId";
                var parameters = new
                {
                    clientId = clientId,
                    areaId = areaId,
                    assetLocationId = assetLocationId
                };

                return await GetSelectListAsync(query, parameters);
            }
            catch (Exception ex)
            {
                // Log error (you can inject a logger if needed)
                throw new ApplicationException("Error Asset lookup list Id and Name from stored procedure", ex);
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetSelectListAsync(string query, object? parameters)
        {
            var items = await _unitOfWork.DapperRepository.QueryAsync<IdNameLookupViewModel>(query, parameters);

            return items
                .OrderByDescending(al => al.Name)
                .ThenBy(al => al.Name)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.IsActive ? l.Name : $"{l.Name} (Inactive)"
                });
        }
    }
}
