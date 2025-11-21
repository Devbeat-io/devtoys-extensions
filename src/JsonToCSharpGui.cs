using Devbeat.DTE.JsonToCSharp.Converter;
using Devbeat.DTE.JsonToCSharp.Models;
using Devbeat.DTE.JsonToCSharp.Writers;
using DevToys.Api;
using Devbeat.DTE.JsonToCSharp;
using System.ComponentModel.Composition;
using static DevToys.Api.GUI;
using static System.Net.Mime.MediaTypeNames;

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
    private static readonly SettingDefinition<NumberType> _numberTypeDefinition
        = new(name: $"{nameof(JsonToCSharpGui)}.{nameof(_numberTypeDefinition)}", defaultValue: NumberType.Long);

    private static readonly SettingDefinition<bool> _forceNullPropertiesDefinition
        = new(name: $"{nameof(JsonToCSharpGui)}.{nameof(_forceNullPropertiesDefinition)}", defaultValue: false);

    private static readonly SettingDefinition<OutputType> _outputTypeDefinition
        = new(name: $"{nameof(JsonToCSharpGui)}.{nameof(_outputTypeDefinition)}", defaultValue: OutputType.Class);

    [Import] // Import the settings provider.
    private ISettingsProvider _settingsProvider = null!;

    private readonly IUIMultiLineTextInput _inputTextArea = MultiLineTextInput("json-to-csharp-input-text-area");
    private readonly IUIMultiLineTextInput _outputTextArea = MultiLineTextInput("json-to-csharp-output-text-area");

    private string _namespaceName = "DefaultNamespace";
    private bool _forceNullProperties = false;
    private OutputType _outputType = OutputType.Class;

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
                    Stack()
                        .Vertical()
                        .WithChildren(
                            Label()
                                .Text(JsonToCSharpExtension.ConvertJsonToCSharpConfigurationTitle), SingleLineTextInput("json-to-csharp-namespace-name")
                                    .Title("Namespace")
                                    .Text(_namespaceName)
                                    .OnTextChanged((s) => { _namespaceName = s; }),
                            SettingGroup()
                                .Title("Advanced")
                                .Icon("FluentSystemIcons", '\uF6A8')
                                .WithSettings(
                                    Setting()
                                        .Icon("FluentSystemIcons", '\uF57C')
                                        .Title(JsonToCSharpExtension.NumberType)
                                        .Handle(
                                            _settingsProvider,
                                            _numberTypeDefinition,
                                            OnNumberTypeModeChanged,
                                            Item("int", NumberType.Int),
                                            Item("long", NumberType.Long),
                                            Item("float", NumberType.Float),
                                            Item("double", NumberType.Double),
                                            Item("decimal", NumberType.Decimal)
                                        ),
                                    Setting()
                                        .Icon("FluentSystemIcons", '\uF637')
                                        .Title("Force Null Properties")
                                        .Description("Make all properties nullable regardless of their value in the JSON.")
                                        .Handle(
                                            _settingsProvider,
                                            _forceNullPropertiesDefinition,
                                            stateDescriptionWhenOn: "Force null",
                                            stateDescriptionWhenOff: "No force",
                                            OnForceNullProperties
                                        ),
                                    Setting()
                                        .Icon("FluentSystemIcons", '\uEBC8')
                                        .Title("Output Type")
                                        .Description("Currently only 'class' output type is supported.")
                                        .Handle(
                                            _settingsProvider,
                                            _outputTypeDefinition,
                                            OnOutputTypeChanged
                                                , Item("Class", OutputType.Class)
                                                , Item("Record", OutputType.Record)
                                        )
                                )
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
        //throw new NotImplementedException();
        if (dataTypeName == PredefinedCommonDataTypeNames.Json)
        {
            StartConvert(_inputTextArea.Text);
        }
    }

    private void OnForceNullProperties(bool isOn)
    {
        _forceNullProperties = isOn;
        StartConvert(_inputTextArea.Text);
    }
    private void OnOutputTypeChanged(OutputType outputType)
    {
        _outputType = outputType;
        StartConvert(_inputTextArea.Text);
    }

    private void OnNumberTypeModeChanged(NumberType numType)
    {
        StartConvert(_inputTextArea.Text);
    }

    private void OnInputTextChanged(string text)
    {
        StartConvert(text);
    }

    private void StartConvert(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _outputTextArea.Text(string.Empty);
            return;
        }

        try
        {
            var csharpRepresentation = ConvertToCSharp
                .LoadFromText(
                    text, _namespaceName
                    , _settingsProvider.GetSetting(_numberTypeDefinition).ToLowerString()
                    , _outputType
                    , _forceNullProperties)
                .Convert();

            var output = CSharpWriter.CreateNew();
            _outputTextArea.Text(
                output.Write(csharpRepresentation));
        }
        catch
        {
            _outputTextArea.Text("Please provide a valid JSON");
        }
    }
}