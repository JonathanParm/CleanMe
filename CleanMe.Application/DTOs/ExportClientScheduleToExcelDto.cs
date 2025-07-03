using CleanMe.Application.Interfaces;
using CleanMe.Application.Services;
using CleanMe.Application.ViewModels;
using CleanMe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.DTOs
{
    public class ExportClientScheduleToExcelDto
    {
        public int clientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string CleanFrequency { get; set; }
        public int? Wk1 { get; set; } = null;
        public int? Wk2 { get; set; } = null;
        public int? Wk3 { get; set; } = null;
        public int? Wk4 { get; set; } = null;
        public int? Wk5 { get; set; } = null;
        public string ItemCode { get; set; }
        public string ClientReference { get; set; }
        public string MdId { get; set; }
        public string RegionName { get; set; }
        public string AreaName { get; set; }
        public string AssetLocation { get; set; }
        //public string BankName { get; set; }
    }
}
