using System.Text.Json;

namespace Devbeat.DTE.JsonToCSharp;

internal static class JsonElementExtensions
{
    internal static JsonValueKind GetArrayType(this JsonElement element)
    {
        if (element.GetArrayLength() == 0) return JsonValueKind.Undefined;

        return element.EnumerateArray().First().ValueKind;
    }
    internal static JsonElement? PeakArrayElement(this JsonElement element)
    {
        if (element.GetArrayLength() == 0) return null;

        return element.EnumerateArray().First();
    }
}
