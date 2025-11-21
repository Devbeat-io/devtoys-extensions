namespace Devbeat.DTE.JsonToCSharp.Writers;

internal sealed class CSharpWriteOptions
{
    internal bool NullableProperties { get; set; } = false;
    internal bool NullableClasses { get; set; } = false;
    internal bool WriteNumbersAsInt { get; set; } = false;
    internal int WriteCollectionAs { get; set; } = 0; // 0 = Array, 1 = List, 2 = IEnumerable
    internal bool WriteClassAsRecord { get; set; } = false;

    internal static CSharpWriteOptions Default => new();
}