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
        Task<IEnumerable<SelectListItem>> GetClientSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAssetLocationSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetAssetTypeSelectListAsync();
    }
}
