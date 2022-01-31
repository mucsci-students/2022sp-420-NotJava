using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace UMLEditor.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<UMLEditor.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}