using Microsoft.AspNetCore.Routing;

namespace CleanMe.Application.Helpers.Paging
{
    public class PagerModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }

        // Optional: if null/empty, the pager will use the current action/controller
        public string? Action { get; set; }
        public string? Controller { get; set; }

        // Extra values to preserve across paging:
        // e.g. regionId, search, sortColumn, filters, etc.
        public RouteValueDictionary RouteValues { get; set; } = new RouteValueDictionary();

        public int TotalPages =>
            PageSize <= 0 ? 0 : (int)System.Math.Ceiling((double)TotalCount / PageSize);
    }
}
