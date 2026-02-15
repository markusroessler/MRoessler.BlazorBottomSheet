using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet;

/// <summary>
/// Outputs the contents of <see cref="BottomSheet"/>s. 
/// Place this in your MainLayout.
/// </summary>
public partial class BottomSheetOutlet : ComponentBase
{
    internal const string ContentSectionName = "BottomSheetContent";

    [Inject]
    private BottomSheetOutletState State { get; set; } = default!;

    /// <summary>
    /// the color scheme to use (see light/dark style classes)
    /// </summary>
    [Parameter]
    public BottomSheetColorScheme ColorScheme { get; set; }


    protected override void OnInitialized()
    {
        base.OnInitialized();
        // TODO remove listeners in Dispose method
        State.OnSectionContentIdAdded += State_OnSectionContentIdAdded;
        State.OnSectionContentIdRemoved += State_OnSectionContentIdRemoved;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        State.ColorScheme = ColorScheme;
    }

    private void State_OnSectionContentIdRemoved(Guid guid) => StateHasChanged();

    private void State_OnSectionContentIdAdded(Guid guid) => StateHasChanged();
}
