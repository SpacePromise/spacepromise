using System.Globalization;

namespace spnode.world.procedural.Extensions
{
    public static class StringCaseExtensions 
    {
        public static string ToTitleCase(this string source)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(source);
        }
    }
}