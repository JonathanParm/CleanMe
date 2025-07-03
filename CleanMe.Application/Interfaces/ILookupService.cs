using CleanMe.Application.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<SelectListItem>> GetAmendmentTypeSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAreaSelectListAsync(AreaLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetAssetLocationSelectListAsync(AssetLocationLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetAssetSelectListAsync(AssetLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetCleanFrequencySelectListAsync();
        Task<IEnumerable<SelectListItem>> GetClientContactSelectListAsync(ClientContactLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetClientSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetItemCodeRateSelectListAsync(ItemCodeRateLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetItemCodeSelectListAsync(ItemCodeLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetRegionSelectListAsync(RegionLookupFilter filter);
        Task<IEnumerable<SelectListItem>> GetStaffSelectListAsync(StaffLookupFilter filter);

        //Task<IEnumerable<SelectListItem>> GetAreasByClientAsync(int? clientId);
        //Task<IEnumerable<SelectListItem>> GetAssetLocationsByClientAndAreaAsync(int? clientId, int? areaId);
        //Task<IEnumerable<SelectListItem>> GetAssetsByClientAreaAndLocationAsync(int? clientId, int? areaId, int? assetLocationId);
    }
}
