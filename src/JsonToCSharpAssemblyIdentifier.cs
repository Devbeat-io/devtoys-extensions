using DevToys.Api;
using System.ComponentModel.Composition;

namespace Devbeat.DTE.JsonToCSharp;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(JsonToCSharpAssemblyIdentifier))]
internal sealed class JsonToCSharpAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}