using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.Filters
{
    public class ItemCodeRateLookupFilter
    {
        public int? ClientId { get; set; } = 0;
        public int? ItemCodeId { get; set; } = 0;
    }
}
