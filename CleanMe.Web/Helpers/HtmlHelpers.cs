using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

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

        public static IHtmlContent LabeledTextBoxFor<TModel, TValue>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression,
            int labelCols = 3,
            int inputCols = 9)
        {
            var propertyInfo = GetPropertyInfo(expression);
            if (propertyInfo == null)
                throw new ArgumentException("Unable to extract property info from expression.", nameof(expression));

            var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var labelText = displayAttr?.Name ?? propertyInfo.Name;

            var label = htmlHelper.LabelFor(expression, labelText, new { @class = $"col-md-{labelCols} col-form-label" });
            var textbox = htmlHelper.TextBoxFor(expression, new { @class = "form-control" });

            var div = new TagBuilder("div");
            div.AddCssClass("mb-3 row");

            var labelDiv = new TagBuilder("div");
            labelDiv.AddCssClass($"col-md-{labelCols}");
            labelDiv.InnerHtml.AppendHtml(label);

            var inputDiv = new TagBuilder("div");
            inputDiv.AddCssClass($"col-md-{inputCols}");
            inputDiv.InnerHtml.AppendHtml(textbox);

            div.InnerHtml.AppendHtml(labelDiv);
            div.InnerHtml.AppendHtml(inputDiv);

            return div;
        }

        public static IHtmlContent YesNoButtonsFor<TModel>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression,
            int labelCols = 4,
            int inputCols = 8)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var propertyName = memberExpression.Member.Name;
            var propertyInfo = typeof(TModel).GetProperty(propertyName);
            var displayName = propertyInfo?.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyName;

            var fieldId = htmlHelper.IdFor(expression);
            var fieldName = htmlHelper.NameFor(expression);
            var value = htmlHelper.ViewData.Model != null
                ? expression.Compile().Invoke(htmlHelper.ViewData.Model)
                : false;

            // Label
            var labelTag = new TagBuilder("label");
            labelTag.Attributes["for"] = fieldId;
            labelTag.AddCssClass($"col-sm-{labelCols} col-form-label text-sm-end text-start");
            labelTag.Attributes["style"] = "color: #003366; font-weight: 600; white-space: nowrap;";
            labelTag.InnerHtml.Append(displayName);

            // YES
            string yesId = fieldId + "_yes";
            var yesInput = new TagBuilder("input");
            yesInput.Attributes["type"] = "radio";
            yesInput.Attributes["id"] = yesId;
            yesInput.Attributes["name"] = fieldName;
            yesInput.Attributes["value"] = "true";
            yesInput.Attributes["class"] = "btn-check";
            if (value) yesInput.Attributes["checked"] = "checked";

            var yesLabel = new TagBuilder("label");
            yesLabel.AddCssClass("btn btn-outline-success border border-1 border-secondary");
            yesLabel.Attributes["for"] = yesId;
            yesLabel.InnerHtml.Append("Yes");

            // NO
            string noId = fieldId + "_no";
            var noInput = new TagBuilder("input");
            noInput.Attributes["type"] = "radio";
            noInput.Attributes["id"] = noId;
            noInput.Attributes["name"] = fieldName;
            noInput.Attributes["value"] = "false";
            noInput.Attributes["class"] = "btn-check";
            if (!value) noInput.Attributes["checked"] = "checked";

            var noLabel = new TagBuilder("label");
            noLabel.AddCssClass("btn btn-outline-danger border border-1 border-secondary");
            noLabel.Attributes["for"] = noId;
            noLabel.InnerHtml.Append("No");

            var btnGroup = new TagBuilder("div");
            btnGroup.AddCssClass("btn-group");
            btnGroup.Attributes["role"] = "group";
            btnGroup.InnerHtml.AppendHtml(yesInput);
            btnGroup.InnerHtml.AppendHtml(yesLabel);
            btnGroup.InnerHtml.AppendHtml(noInput);
            btnGroup.InnerHtml.AppendHtml(noLabel);

            var validation = htmlHelper.ValidationMessageFor(expression, "", new { @class = "text-danger" });

            var inputDiv = new TagBuilder("div");
            inputDiv.AddCssClass($"col-sm-{inputCols}");
            inputDiv.InnerHtml.AppendHtml(btnGroup);
            inputDiv.InnerHtml.AppendHtml(validation);

            var rowDiv = new TagBuilder("div");
            rowDiv.AddCssClass("mb-3 row");
            rowDiv.InnerHtml.AppendHtml(labelTag);
            rowDiv.InnerHtml.AppendHtml(inputDiv);

            return rowDiv;
        }

        public static IHtmlContent LabeledDropDownFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            string optionLabel = "",
            int labelCols = 3,
            int inputCols = 9)
        {
            // Get the property info from the expression
            var propertyInfo = GetPropertyInfo(expression);
            if (propertyInfo == null)
                throw new ArgumentException("Unable to determine property info from the expression", nameof(expression));

            // Try to get the DisplayAttribute for label text
            var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var labelText = displayAttr?.Name ?? propertyInfo.Name;

            // Generate label
            var label = htmlHelper.LabelFor(expression, labelText, new { @class = $"col-md-{labelCols} col-form-label" });

            // Generate dropdown
            var dropdown = htmlHelper.DropDownListFor(expression, selectList, optionLabel, new { @class = "form-control" });

            // Combine label and dropdown in Bootstrap layout
            var div = new TagBuilder("div");
            div.AddCssClass("mb-3 row");

            var labelDiv = new TagBuilder("div");
            labelDiv.AddCssClass($"col-md-{labelCols}");
            labelDiv.InnerHtml.AppendHtml(label);

            var inputDiv = new TagBuilder("div");
            inputDiv.AddCssClass($"col-md-{inputCols}");
            inputDiv.InnerHtml.AppendHtml(dropdown);

            div.InnerHtml.AppendHtml(labelDiv);
            div.InnerHtml.AppendHtml(inputDiv);

            return div;
        }

        public static IHtmlContent LabeledTextAreaFor<TModel>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, string>> expression,
            int rows = 4,
            int labelCols = 2,
            int inputCols = 10)
        {
            var propertyInfo = GetPropertyInfo(expression);
            if (propertyInfo == null)
                throw new ArgumentException("Unable to extract property info from expression", nameof(expression));

            var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            var labelText = displayAttr?.Name ?? propertyInfo.Name;

            var label = htmlHelper.LabelFor(expression, labelText, new { @class = $"col-md-{labelCols} col-form-label" });

            var textarea = htmlHelper.TextAreaFor(expression, rows, 0, new { @class = "form-control" });

            var inputDiv = new TagBuilder("div");
            inputDiv.AddCssClass($"col-md-{inputCols}");
            inputDiv.InnerHtml.AppendHtml(textarea);

            var labelDiv = new TagBuilder("div");
            labelDiv.AddCssClass($"col-md-{labelCols}");
            labelDiv.InnerHtml.AppendHtml(label);

            var row = new TagBuilder("div");
            row.AddCssClass("mb-3 row");
            row.InnerHtml.AppendHtml(labelDiv);
            row.InnerHtml.AppendHtml(inputDiv);

            return row;
        }

        //readonly textbox
        public static IHtmlContent LabeledDisplayFor<TModel, TValue>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression,
            int labelCols = 3,
            int inputCols = 9)
        {
            var memberExpression = (MemberExpression)expression.Body;
            var propertyName = memberExpression.Member.Name;
            var propertyInfo = typeof(TModel).GetProperty(propertyName);
            var displayName = propertyInfo?.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyName;

            var fieldId = htmlHelper.IdFor(expression);

            // === Label ===
            var labelTag = new TagBuilder("label");
            labelTag.Attributes["for"] = fieldId;
            labelTag.AddCssClass($"col-sm-{labelCols} col-form-label text-sm-end text-start");
            labelTag.Attributes["style"] = "color: #003366; font-weight: 600;";
            labelTag.InnerHtml.Append(displayName);

            // === Display Value ===
            var value = htmlHelper.DisplayTextFor(expression).ToString();

            var valueTag = new TagBuilder("p");
            valueTag.AddCssClass("form-control-plaintext mb-0"); // Bootstrap style for read-only
            valueTag.InnerHtml.Append(value);

            var inputDiv = new TagBuilder("div");
            inputDiv.AddCssClass($"col-sm-{inputCols}");
            inputDiv.InnerHtml.AppendHtml(valueTag);

            // === Row Wrapper ===
            var rowDiv = new TagBuilder("div");
            rowDiv.AddCssClass("mb-3 row");
            rowDiv.InnerHtml.AppendHtml(labelTag);
            rowDiv.InnerHtml.AppendHtml(inputDiv);

            return rowDiv;
        }

        public static PropertyInfo? GetPropertyInfo<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            if (expression.Body is MemberExpression member)
                return member.Member as PropertyInfo;

            if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression unaryMember)
                return unaryMember.Member as PropertyInfo;

            return null;
        }

    }
}