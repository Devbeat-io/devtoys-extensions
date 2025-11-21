namespace Devbeat.DTE.JsonToCSharp.Models;

internal enum OutputType
{
    Class,
    Record
}

internal static class OutputTypeExtensions
{
    public static string ToLowerString(this OutputType outputType)
    {
        return outputType.ToString().ToLower();
    }
}
