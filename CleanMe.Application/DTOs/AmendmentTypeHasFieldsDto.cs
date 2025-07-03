using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.DTOs
{
    public class AmendmentTypeHasFieldsDto
    {
        public bool HasStaffId { get; set; }

        public bool HasClientId { get; set; }

        //public bool HasRegionId { get; set; }

        public bool HasAreaId { get; set; }

        public bool HasAssetLocationId { get; set; }

        public bool HasItemCodeId { get; set; }

        public bool HasAssetId { get; set; }

        public bool HasCleanFrequencyId { get; set; }

        public bool HasRate { get; set; }

        //public bool HasAccess { get; set; }

        public bool HasIsAccessable { get; set; }
    }
}
