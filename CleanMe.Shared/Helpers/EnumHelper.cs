using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CleanMe.Shared.Helpers
{
    public static class EnumHelper
    {
        public static List<SelectListItem> ToSelectList<TEnum>(TEnum? selectedValue = null) where TEnum : struct, Enum
        {
            var enumType = typeof(TEnum);

            return Enum.GetValues(enumType)
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Text = GetDisplayName(e),
                    Value = Convert.ToInt32(e).ToString(),
                    Selected = selectedValue?.Equals(e) ?? false
                })
                .ToList();
        }

        private static string GetDisplayName<TEnum>(TEnum enumValue) where TEnum : struct, Enum
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            var displayAttr = member?.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.Name ?? enumValue.ToString();
        }
    }
}