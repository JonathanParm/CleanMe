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
        Task<IEnumerable<SelectListItem>> GetAreaSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAssetLocationSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAssetSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAssetTypeSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAssetTypeRateSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetCleanFrequencySelectListAsync();
        Task<IEnumerable<SelectListItem>> GetClientContactSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetClientSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetRegionSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetStaffSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetStockCodeSelectListAsync();
    }
}
