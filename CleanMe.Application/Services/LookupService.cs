using CleanMe.Application.Filters;
using CleanMe.Application.Interfaces;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace CleanMe.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LookupService> _logger;

        public LookupService(IUnitOfWork unitOfWork, ILogger<LookupService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<SelectListItem>> GetAmendmentTypeSelectListAsync()
        {
            try
            {
                var types = await _unitOfWork.AmendmentTypeRepository.GetAllAmendmentTypesAsync();
                return types.Select(t => new SelectListItem
                {
                    Value = t.amendmentTypeId.ToString(),
                    Text = t.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving AmendmentType SelectList.");
                throw new ApplicationException("Error retrieving AmendmentType SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetAreaSelectListAsync(AreaLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("AreaLookup", filter);

                //var types = await _unitOfWork.AreaRepository.GetAllAreasAsync();
                //return types.Select(t => new SelectListItem
                //{
                //    Value = t.areaId.ToString(),
                //    Text = t.Name
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Area SelectList.");
                throw new ApplicationException("Error retrieving Area SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetLocationSelectListAsync(AssetLocationLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("AssetLocationLookup", filter);

                //var locations = await _unitOfWork.AssetLocationRepository.GetAllAssetLocationsAsync();
                //return locations.Select(l => new SelectListItem
                //{
                //    Value = l.assetLocationId.ToString(),
                //    Text = l.Description ?? "N/A"
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Asset Location SelectList.");
                throw new ApplicationException("Error retrieving Asset Location SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetAssetSelectListAsync(AssetLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("AssetLookup", filter);

                //var types = await _unitOfWork.AssetRepository.GetAllAssetsAsync();
                //return types.Select(t => new SelectListItem
                //{
                //    Value = t.assetId.ToString(),
                //    Text = t.Name
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Assets SelectList.");
                throw new ApplicationException("Error retrieving Assets SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetCleanFrequencySelectListAsync()
        {
            try
            {
                var types = await _unitOfWork.CleanFrequencyRepository.GetAllCleanFrequenciesAsync();
                return types.Select(t => new SelectListItem
                {
                    Value = t.cleanFrequencyId.ToString(),
                    Text = t.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Clean Frequency SelectList.");
                throw new ApplicationException("Error retrieving Clean Frequency SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetClientContactSelectListAsync(ClientContactLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("ClientContactLookup", filter);

                //var types = await _unitOfWork.ClientContactRepository.GetAllClientContactsAsync();
                //return types.Select(t => new SelectListItem
                //{
                //    Value = t.clientContactId.ToString(),
                //    Text = $"{t.FirstName} {t.FamilyName}"
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Client Contact SelectList.");
                throw new ApplicationException("Error retrieving Client Contact SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetClientSelectListAsync()
        {
            try
            {
                var clients = await _unitOfWork.ClientRepository.GetAllClientsAsync();
                return clients.Select(c => new SelectListItem
                {
                    Value = c.clientId.ToString(),
                    Text = c.Brand
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Client SelectList.");
                throw new ApplicationException("Error retrieving Client SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetItemCodeRateSelectListAsync()
        {
            try
            {
                var types = await _unitOfWork.ItemCodeRateRepository.GetAllItemCodeRatesAsync();
                return types.Select(t => new SelectListItem
                {
                    Value = t.itemCodeId.ToString(),
                    Text = t.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item Code Rate SelectList.");
                throw new ApplicationException("Error retrieving Item Code Rate SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetItemCodeSelectListAsync()
        {
            try
            {
                var types = await _unitOfWork.ItemCodeRepository.GetAllItemCodesAsync();
                return types.Select(t => new SelectListItem
                {
                    Value = t.itemCodeId.ToString(),
                    Text = t.Code
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item Code SelectList.");
                throw new ApplicationException("Error retrieving Item Code SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetRegionSelectListAsync(RegionLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("RegionLookup", filter);

                //var types = await _unitOfWork.RegionRepository.GetAllRegionsAsync();
                //return types.Select(t => new SelectListItem
                //{
                //    Value = t.regionId.ToString(),
                //    Text = t.Name
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Region SelectList.");
                throw new ApplicationException("Error retrieving Region SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetStaffSelectListAsync(StaffLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("StaffLookup", filter);

                //var types = await _unitOfWork.StaffRepository.GetAllStaffAsync();
                //return types.Select(t => new SelectListItem
                //{
                //    Value = t.staffId.ToString(),
                //    Text = $"{t.FirstName} {t.FamilyName}"
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Staff SelectList.");
                throw new ApplicationException("Error retrieving Staff SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        //public async Task<IEnumerable<SelectListItem>> GetAreasByClientAsync(int? clientId = 0)
        //{
        //    try
        //    {
        //        var query = "EXEC dbo.AreaLookupByClient @clientId";
        //        var parameters = new
        //        {
        //            clientId = clientId,
        //        };

        //        return await GetSelectListAsync(query, parameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error Get Areas By Client SelectList.");
        //        throw new ApplicationException("Error Get Areas By Client SelectList", ex);
        //    }
        //    return Enumerable.Empty<SelectListItem>();
        //}

        //public async Task<IEnumerable<SelectListItem>> GetAssetLocationsByClientAndAreaAsync(int? clientId = 0, int? areaId = 0)
        //{
        //    try
        //    {
        //        var query = "EXEC dbo.AssetLocationLookupByClientAndArea @clientId, @areaId";
        //        var parameters = new
        //        {
        //            clientId = clientId,
        //            areaId = areaId
        //        };

        //        return await GetSelectListAsync(query, parameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error Get Asset Locations By Client And Area SelectList.");
        //        throw new ApplicationException("Error Get Asset Locations By Client And Area SelectList", ex);
        //    }
        //    return Enumerable.Empty<SelectListItem>();
        //}

        //public async Task<IEnumerable<SelectListItem>> GetAssetsByClientAreaAndLocationAsync(int? clientId = 0, int? areaId = 0, int? assetLocationId = 0)
        //{
        //    try
        //    {
        //        var query = "EXEC dbo.AssetLookupByClientAreaAndLocation @clientId, @areaId, @assetLocationId";
        //        var parameters = new
        //        {
        //            clientId = clientId,
        //            areaId = areaId,
        //            assetLocationId = assetLocationId
        //        };

        //        return await GetSelectListAsync(query, parameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error Get Assets By Client Area And Location SelectList.");
        //        throw new ApplicationException("Error Get Assets By Client Area And Location SelectList", ex);
        //    }
        //}

        private async Task<IEnumerable<SelectListItem>> GetSelectListAsync(string sqlProcedure, object? filter)
        {
            try
            {
                string query = $"Exec {sqlProcedure} ";
                string parametersList = string.Join(", ", filter.GetType().GetProperties().Select(p => $"@{p.Name}"));

                var parameters = new DynamicParameters();

                foreach (PropertyInfo prop in filter.GetType().GetProperties())
                {
                    var name = prop.Name;
                    var value = prop.GetValue(filter) ?? 0;
                    parameters.Add("@" + name, value);
                }

                var items = await _unitOfWork.DapperRepository.QueryAsync<IdNameLookupViewModel>(query + parametersList, parameters);

                return items
                    .OrderByDescending(al => al.Name)
                    .ThenBy(al => al.Name)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.IsActive ? l.Name : $"{l.Name} (Inactive)"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get SelectList.");
                throw new ApplicationException("Error Get SelectList", ex);
            }
        }
    }
}
