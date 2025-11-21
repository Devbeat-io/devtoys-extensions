namespace Devbeat.DTE.JsonToCSharp.Represesentation;

internal abstract class CSharpClassBase(
    string name, CSharpCode root)
    : ICSharpObject
{
    internal string Name { get; } = name;
    public CSharpCode Root { get; } = root;

    private readonly List<CSharpClassProperty> _list = [];
    //internal string Name { get; } = name;
    internal CSharpClassProperty[] Properties => [.. _list];

    internal void AddProperty(CSharpClassProperty p)
    {
        _list.Add(p);
    }
    internal bool HasProperty(string name)
    {
        return _list.Any(_ => _.Name == name);
    }
}

internal static class CSharpClassBaseExtensions
{
    public static CSharpRecord AddProperty(
        this CSharpRecord record
        , string name
        , string type
        , bool isCollection = false
        , bool isNullable = false)
    {
        if (record.HasProperty(name)) return record;

        var p = new CSharpClassProperty(name, type, record.Root, isCollection, isNullable);
        record.AddProperty(p);
        return record;
    }
    public static CSharpClass AddProperty(
        this CSharpClass record
        , string name
        , string type
        , bool isCollection = false
        , bool isNullable = false)
    {
        if (record.HasProperty(name)) return record;

        var p = new CSharpClassProperty(name, type, record.Root, isCollection, isNullable);
        record.AddProperty(p);
        return record;
    }
}