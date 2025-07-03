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
                return await GetSelectListAsync("AmendmentTypeLookup", "amendment type");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Amendment type SelectList.");
                throw new ApplicationException("Error retrieving Amendment type SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetAreaSelectListAsync(AreaLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("AreaLookup", "area", filter);
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
                return await GetSelectListAsync("AssetLocationLookup", "asset location", filter);
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
                return await GetSelectListAsync("AssetLookup", "asset", filter);
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
                return await GetSelectListAsync("CleanFrequencyLookup", "clean frequency");
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
                return await GetSelectListAsync("ClientContactLookup", "client contact", filter);
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
                return await GetSelectListAsync("ClientLookup", "client");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Client SelectList.");
                throw new ApplicationException("Error retrieving Client SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetItemCodeRateSelectListAsync(ItemCodeRateLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("ItemCodeRateLookup", "item code rate", filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Item Code Rate SelectList.");
                throw new ApplicationException("Error retrieving Item Code Rate SelectList", ex);
            }
            return Enumerable.Empty<SelectListItem>();
        }

        public async Task<IEnumerable<SelectListItem>> GetItemCodeSelectListAsync(ItemCodeLookupFilter filter)
        {
            try
            {
                return await GetSelectListAsync("ItemCodeLookup","item code", filter);
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
                return await GetSelectListAsync("RegionLookup", "region", filter);
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
                return await GetSelectListAsync("StaffLookup", "staff", filter);
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

        private async Task<IEnumerable<SelectListItem>> GetSelectListAsync(string sqlProcedure, string caption, object? filter)
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

                return new[]
                {
                    new SelectListItem { Value = "0", Text = $"-- Select {caption} --" }
                }.Concat(items
                    .OrderByDescending(al => al.Name)
                    .ThenBy(al => al.Name)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.IsActive ? l.Name : $"{l.Name} (Inactive)"
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get SelectList.");
                throw new ApplicationException("Error Get SelectList", ex);
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetSelectListAsync(string sqlProcedure, string caption)
        {
            try
            {
                string query = $"Exec {sqlProcedure}";
                var items = await _unitOfWork.DapperRepository.QueryAsync<IdNameLookupViewModel>(query);

                return new[]
                {
                    new SelectListItem { Value = "0", Text = $"-- Select {caption} --" }
                }.Concat(items
                    .OrderByDescending(al => al.Name)
                    .ThenBy(al => al.Name)
                    .Select(l => new SelectListItem
                    {
                        Value = l.Id.ToString(),
                        Text = l.IsActive ? l.Name : $"{l.Name} (Inactive)"
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get SelectList (no filter).");
                throw new ApplicationException("Error Get SelectList (no filter)", ex);
            }
        }
    }
}
