using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanMe.Application.ViewModels
{
    public class AmendmentAddEditViewModel
    {
        public int? amendmentId {  get; set; }
        public bool IsEdit => amendmentId.HasValue;

        public AmendmentViewModel AmendmentCurrent { get; set; }
        public AmendmentViewModel AmendmentLastInvoiced { get; set; } = new AmendmentViewModel();

        public IEnumerable<SelectListItem>? AmendmentTypes { get; set; }
        public IEnumerable<SelectListItem>? Areas { get; set; }
        public IEnumerable<SelectListItem>? Assets { get; set; }
        public IEnumerable<SelectListItem>? AssetLocations { get; set; }
        public IEnumerable<SelectListItem>? CleanFrequencies { get; set; }
        public IEnumerable<SelectListItem>? Clients { get; set; }
        public IEnumerable<SelectListItem>? Staff { get; set; }
    }
}
