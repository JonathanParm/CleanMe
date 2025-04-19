using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanMe.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static HtmlString GetSortIcon(ViewContext viewContext, string column)
        {
            var sortColumn = viewContext.ViewBag.SortColumn as string ?? "StaffNo";
            var sortOrder = viewContext.ViewBag.SortOrder as string ?? "ASC";

            if (column == sortColumn)
            {
                return new HtmlString(sortOrder == "ASC"
                    ? "&#9650;" // Up arrow ▲
                    : "&#9660;"); // Down arrow ▼
            }

            return HtmlString.Empty;
        }
    }
}