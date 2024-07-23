namespace Devbeat.DTE.JsonToCSharp.Represesentation;

internal class CSharpClassProperty
    (string name, string type, CSharpCode root, bool isCollection, bool isNullable = false) : ICSharpObject
{
    internal string Name { get; } = name;
    internal string Type { get; } = type;
    internal bool IsCollection { get; } = isCollection;
    internal bool IsNullable { get; } = isNullable;

    public CSharpCode Root { get; } = root;
}