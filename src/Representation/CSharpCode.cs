namespace Devbeat.DTE.JsonToCSharp.Represesentation;

internal class CSharpCode : ICSharpObject
{
    private readonly List<CSharpClass> _classes = [];
    private readonly string _namespaceName;

    internal CSharpCode(string namespaceName)
    {
        _namespaceName = namespaceName;
    }
    internal CSharpClass[] Classes => _classes.ToArray();
    public CSharpCode Root => this;
    internal string Namespace => _namespaceName;
    internal CSharpClass AddClass(string name)
    {
        if (_classes.Any(_ => _.Name == name))
        {
            return _classes.First(_ => _.Name == name);
        }
        var c = new CSharpClass(name, this);
        _classes.Add(c);
        return c;
    }
}
