using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Filters
{
    public class AssetLocationLookupFilter
    {
        public int? StaffId { get; set; } = 0;
        public int? ClientId { get; set; } = 0;
        public int? RegionId { get; set; } = 0;
        public int? AreaId { get; set; } = 0;
    }
}
