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
public partial class BottomSheetOutlet : ComponentBase, IAsyncDisposable
{
    internal const string ContentSectionName = "BottomSheetContent";
    private bool _disposed;

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

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        State.OnSectionContentIdAdded -= State_OnSectionContentIdAdded;
        State.OnSectionContentIdRemoved -= State_OnSectionContentIdRemoved;

        _disposed = true;
    }
}
