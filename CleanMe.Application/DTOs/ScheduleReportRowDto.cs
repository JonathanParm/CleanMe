using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.DTOs
{
    public class ScheduleReportRowDto
    {
        public int staffId { get; set; }
        public string CleanerName { get; set; }
        public string CleanerAddress { get; set; }
        public string Area { get; set; }
        public string Location { get; set; }
        public string MdId { get; set; }
        public string Bank { get; set; }
        public string Frequency { get; set; }
        public DateTime Date { get; set; }
        public string Week1 { get; set; }
        public string Week2 { get; set; }
        public string Week3 { get; set; }
        public string Week4 { get; set; }
        public string Week5 { get; set; }
    }
}
