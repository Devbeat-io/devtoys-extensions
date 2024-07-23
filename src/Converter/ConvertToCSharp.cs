using Devbeat.DTE.JsonToCSharp.Represesentation;
using System.Text.Json;

namespace Devbeat.DTE.JsonToCSharp.Converter;

internal sealed class ConvertToCSharp
{
    private readonly string _json;
    private readonly string? _namespaceName;

    internal static ConvertToCSharp LoadFromText(string json, string? namespaceName = null)
        => new (json, namespaceName);
    
    private ConvertToCSharp(string json, string? namespaceName)
    {
        _json = json;
        _namespaceName = namespaceName;
    }

    internal CSharpCode Convert()
    {
        CSharpCode root = new (_namespaceName ?? "Untitled");
        if (string.IsNullOrEmpty(_json)) return root;

        try
        {
            using var doc = JsonDocument.Parse(_json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true
                , CommentHandling = JsonCommentHandling.Skip
            });
            ParseElement(doc.RootElement, "", "Root", root);

            return root;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private static void ParseElement(
        JsonElement element
        , string path
        , string name
        , ICSharpObject root
        , bool parentIsArray = false)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var classItem = root.Root.AddClass(FixCasing(name));
                if (root is CSharpClass sharpClass && parentIsArray == false)
                {
                    sharpClass.AddProperty(FixCasing(name), classItem.Name);
                }
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    ParseElement(property.Value, $"{path}/{property.Name}", property.Name, classItem);
                }
                break;
            case JsonValueKind.Array:
                if (element.GetArrayType() == JsonValueKind.Object)
                {
                    if (root is CSharpClass sharpClassForArray)
                    {
                        sharpClassForArray.AddProperty(
                            FixCasing(name)
                            , FixCasing(name) + "Item"
                            , true);
                    }
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        ParseElement(item, "", FixCasing(name) + "Item", root, true);
                    }
                }
                else if (element.GetArrayType() != JsonValueKind.Object)
                {
                    if (root is CSharpClass sharpClassForArray)
                    {
                        sharpClassForArray.AddProperty(
                            FixCasing(name)
                            , DetermineType(element.PeakArrayElement())
                            , true);
                    }
                }
                break;
            case JsonValueKind.String:
                AddProperty(root, name, element, null);
                break;
            case JsonValueKind.Number:
                AddProperty(root, name, element, "long");
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                AddProperty(root, name, element, "bool");
                break;
            case JsonValueKind.Null:
                AddProperty(root, name, element, "string", true);
                break;
            default:
                throw new InvalidOperationException($"Unsupported JsonValueKind: {element.ValueKind}");
        }
    }

    private static void AddProperty(
        ICSharpObject root
        , string name
        , JsonElement element
        , string? typeName
        , bool? isNullable = false)
    {
        if (root is CSharpClass sharpClass)
        {
            sharpClass.AddProperty(
                FixCasing(name)
                , typeName ?? DetermineType(element)
                , isCollection: false
                , isNullable ?? false);
        }
    }

    private static string DetermineType(JsonElement? e, string fallbackType = "string")
    {
        if (e is null) return fallbackType;

        JsonValueKind valueKind = e.Value.ValueKind;
        if (valueKind == JsonValueKind.True || valueKind == JsonValueKind.False)
        {
            return "bool";
        }
        if (valueKind == JsonValueKind.Number)
        {
            return "long";
        }
        if (valueKind == JsonValueKind.Undefined)
        {
            return "string";
        }

        var value = e.Value.ToString();
        if (string.IsNullOrEmpty(value)) return "string";
        if (DateTime.TryParse(value, out var _))
        {
            return "DateTime";
        }
        if (Guid.TryParse(value, out var _))
        {
            return "Guid";
        }
        if (long.TryParse(value, out var _))
        {
            return "long";
        }
        if (int.TryParse(value, out var _))
        {
            return "int";
        }
        if (bool.TryParse(value, out var _))
        {
            return "bool";
        }
        return "string";
    }

    private static string FixCasing(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        Span<char> span = input.ToCharArray();
        span[0] = char.ToUpper(span[0]);

        return new string(span);
    }
}
