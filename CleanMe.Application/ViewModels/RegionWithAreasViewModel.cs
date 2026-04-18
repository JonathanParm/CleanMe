using CleanMe.Domain.Entities;

namespace CleanMe.Application.ViewModels
{
    public class RegionWithAreasViewModel
    {
        public RegionViewModel RegionViewModel { get; set; } = null!;
        public IReadOnlyList<Area> Areas { get; set; } = Array.Empty<Area>();
    }
}

