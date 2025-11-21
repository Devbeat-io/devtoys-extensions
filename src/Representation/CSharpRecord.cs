
namespace Devbeat.DTE.JsonToCSharp.Represesentation;

internal class CSharpRecord(
    string name, CSharpCode root) 
    : CSharpClassBase(name, root)
{
    //private readonly List<CSharpClassProperty> _list = [];
    //internal string Name { get; } = name;
    //internal CSharpClassProperty[] Properties => _list.ToArray();
    //public CSharpCode Root { get; } = root;

    //public CSharpRecord AddProperty(
    //    string name
    //    , string type
    //    , bool isCollection = false
    //    , bool isNullable = false)
    //{
    //    if (_list.Any(_ => _.Name == name)) return this;

    //    var p = new CSharpClassProperty(name, type, Root, isCollection, isNullable);
    //    _list.Add(p);
    //    return this;
    //}
}
