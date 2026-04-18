using CleanMe.Domain.Entities;

namespace CleanMe.Application.ViewModels
{
    public class RegionWithAreasViewModel
    {
        public RegionViewModel RegionViewModel { get; set; } = null!;
        public IReadOnlyList<Area> Areas { get; set; } = Array.Empty<Area>();

        //public int PageNumber { get; set; }
        //public int PageSize { get; set; }
        //public int TotalCount { get; set; }
    }
}

