using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.DTOs
{
    public class AssetHierarchyDto
    {
        public int assetId { get; set; }
        public string AssetName { get; set; } = string.Empty;

        public int clientId { get; set; }
        public string Client { get; set; } = string.Empty;

        public int assetLocationId { get; set; }
        public string AssetLocation { get; set; } = string.Empty;

        public int areaId { get; set; }
        public string AreaName { get; set; } = string.Empty;

        public int itemCodeId { get; set; }
        public string ItemCodeName { get; set; } = string.Empty;
    }
}
