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
    public class ExportStaffScheduleToExcelDto
    {
        public int staffId { get; set; }
        public string CleanerName { get; set; }
        public string CleanerAddress { get; set; }
        public string AreaName { get; set; }
        public string AssetLocation { get; set; }
        public string MdId { get; set; }
        public string BankName { get; set; }
        public string CleanFrequency { get; set; }
        public int? Week1 { get; set; } = null;
        public int? Week2 { get; set; } = null;
        public int? Week3 { get; set; } = null;
        public int? Week4 { get; set; } = null;
        public int? Week5 { get; set; } = null;
    }
}
