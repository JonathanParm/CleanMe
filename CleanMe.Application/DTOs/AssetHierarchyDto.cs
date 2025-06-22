using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.DTOs
{
    public class AssetHierarchyDto
    {
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;

        public int ClientId { get; set; }
        public string Client { get; set; } = string.Empty;

        public int AssetLocationId { get; set; }
        public string AssetLocation { get; set; } = string.Empty;

        public int AreaId { get; set; }
        public string AreaName { get; set; } = string.Empty;
    }
}
