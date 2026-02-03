using CleanMe.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Application.ViewModels
{
    public class AreaWithAssetLocationsViewModel
    {
        public Area Area { get; set; } = null!;
        public IEnumerable<SelectListItem> Regions { get; set; } = new List<SelectListItem>();
        public IReadOnlyList<AssetLocationIndexViewModel> AssetLocations { get; set; } = Array.Empty<AssetLocationIndexViewModel>();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
