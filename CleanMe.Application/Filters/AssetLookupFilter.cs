using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Filters
{
    public class AssetLookupFilter
    {
        public int? StaffId { get; set; } = 0;
        public int? ClientId { get; set; }
        public int? RegionId { get; set; } = 0;
        public int? AreaId { get; set; } = 0;
        public int? AssetLocationId { get; set; } = 0;
        public int? ItemCodeId { get; set; } = 0;
    }
}
