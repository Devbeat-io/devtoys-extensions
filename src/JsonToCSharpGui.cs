using Devbeat.DTE.JsonToCSharp.Converter;
using Devbeat.DTE.JsonToCSharp.Writers;
using DevToys.Api;
using System.ComponentModel.Composition;
using static DevToys.Api.GUI;

namespace Devbeat.DTE.JsonToCSharp;

[Export(typeof(IGuiTool))]
[Name("JsonToCSharpExtension")]
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",
    IconGlyph = '\uEA0F',
    GroupName = PredefinedCommonToolGroupNames.Converters,
    ResourceManagerAssemblyIdentifier = nameof(JsonToCSharpAssemblyIdentifier), 
    ResourceManagerBaseName = "Devbeat.DTE.JsonToCSharp.JsonToCSharpExtension",
    ShortDisplayTitleResourceName = nameof(JsonToCSharpExtension.ShortDisplayTitle),
    LongDisplayTitleResourceName = nameof(JsonToCSharpExtension.LongDisplayTitle),
    DescriptionResourceName = nameof(JsonToCSharpExtension.Description),
    AccessibleNameResourceName = nameof(JsonToCSharpExtension.AccessibleName))]
internal sealed class JsonToCSharpGui : IGuiTool
{
    private readonly IUIMultiLineTextInput _inputTextArea = MultiLineTextInput("json-to-yaml-input-text-area");
    private readonly IUIMultiLineTextInput _outputTextArea = MultiLineTextInput("json-to-yaml-output-text-area");

    private string _namespaceName = "DefaultNamespace";

    private enum GridColumn
    {
        Content
    }

    private enum GridRow
    {
        Header,
        Content,
        Footer
    }

    public UIToolView View
        => new(
            isScrollable: true,
            Grid()
                .ColumnLargeSpacing()
                .RowLargeSpacing()
                .Rows(
                    (GridRow.Header, Auto),
                    (GridRow.Content, new UIGridLength(1, UIGridUnitType.Fraction))
                )
                .Columns(
                    (GridColumn.Content, new UIGridLength(1, UIGridUnitType.Fraction))
                )
            .Cells(
                Cell(
                    GridRow.Header,
                    GridColumn.Content,
                    Stack().Vertical().WithChildren(
                        Label().Text(JsonToCSharpExtension.ConvertJsonToCSharpConfigurationTitle)
                        //, Setting("json-to-yaml-text-conversion-setting")
                        //    .Icon("FluentSystemIcons", '\uF18D')
                        //    .Title("bla")
                        //    .Description("bla")
                        //    //.Handle(
                        //    //    _settingsProvider,
                        //    //    conversionMode,
                        //    //    OnConversionModeChanged,
                        //    //    Item(JsonYamlConverter.JsonToYaml, JsonToYamlConversion.JsonToYaml),
                        //    //    Item(JsonYamlConverter.YamlToJson, JsonToYamlConversion.YamlToJson)
                        //    //)
                        , SingleLineTextInput("json-to-csharp-namespace-name")
                            .Title("Namespace")
                            .Text(_namespaceName)
                            .OnTextChanged((s) => { _namespaceName = s; })
                            //.Icon("FluentSystemIcons", '\uF6F8')
                            //.Title("bla")
                            //.Handle(
                            //    _settingsProvider,
                            //    indentationMode,
                            //    OnIndentationModelChanged,
                            //    Item(JsonYamlConverter.TwoSpaces, Indentation.TwoSpaces),
                            //    Item(JsonYamlConverter.FourSpaces, Indentation.FourSpaces)
                            //)
                    )
                ),
                Cell(
                    GridRow.Content,
                    GridColumn.Content,
                    SplitGrid()
                        .Vertical()
                        .WithLeftPaneChild(
                            _inputTextArea
                                .Title(JsonToCSharpExtension.ConvertJsonToCSharpInputTitle)
                                .Language("json")
                                .OnTextChanged(OnInputTextChanged))
                        .WithRightPaneChild(
                            _outputTextArea
                                .Title(JsonToCSharpExtension.ConvertJsonToCSharpOutputTitle)
                                .Language("csharp")
                                .ReadOnly()
                                .Extendable())
                )
            )
        );

    public void OnDataReceived(string dataTypeName, object? parsedData)
    {
        throw new NotImplementedException();
    }
    private void OnInputTextChanged(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _outputTextArea.Text(string.Empty);
            return;
        }

        try
        {
            var csharpRepresentation = ConvertToCSharp
                .LoadFromText(text, _namespaceName)
                .Convert();

            _outputTextArea.Text(
                CSharpWriter.Write(csharpRepresentation));
        }
        catch
        {
            _outputTextArea.Text("Please provide a valid JSON");
        }
    }
}