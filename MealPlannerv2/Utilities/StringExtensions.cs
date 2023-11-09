using System.Globalization;

namespace MealPlannerv2.Utilities;

public static class StringExtensions
{
    public static string ToNormal(this string str)
    {
        return string.Concat(str.Where(c => !char.IsWhiteSpace(c)))
            .ToUpper(CultureInfo.InvariantCulture);
    }
}