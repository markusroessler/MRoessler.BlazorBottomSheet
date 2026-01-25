using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.JSInterop;

namespace MRoessler.BlazorBottomSheet;

/// <summary>
/// A BottomSheet that displays its contents in a <see cref="BottomSheetOutlet"/>
/// </summary>
public partial class BottomSheet : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// The BottomSheet's content.
    /// Place <see cref="MinimizedExpansionMarker"/> and <see cref="NormalExpansionMarker"/> within this content for automatic height computation
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Use this to overwrite the default drag handle
    /// </summary>
    [Parameter]
    public RenderFragment? Handle { get; set; }

    [Parameter]
    public bool IsVisible { get; set; }

    /// <summary>
    /// whether to prevent interaction with the overlayed content
    /// </summary>
    [Parameter]
    public bool IsModal { get; set; }

    [Parameter]
    public bool CloseOnBackgroundClick { get; set; }

    [Parameter]
    public bool AllowClosedExpansion { get; set; } = true;

    [Parameter]
    public bool AllowMinimizedExpansion { get; set; }

    [Parameter]
    public bool AllowNormalExpansion { get; set; } = true;

    [Parameter]
    public bool AllowMaximizedExpansion { get; set; }

    /// <summary>
    /// the expansion state to apply
    /// </summary>
    [Parameter]
    public BottomSheetExpansion? Expansion { get; set; }

    /// <summary>
    /// the actual expansion state (see https://learn.microsoft.com/en-us/aspnet/core/blazor/components/data-binding?view=aspnetcore-10.0#bind-across-more-than-two-components)
    /// </summary>
    private BottomSheetExpansion _expansion;

    [Parameter]
    public EventCallback<BottomSheetExpansion> ExpansionChanged { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly Guid _sectionContentId = Guid.NewGuid();

    private readonly DotNetObjectReference<BottomSheet> _thisRef;
    private IJSObjectReference? _jsModule;

    [Inject]
    private BottomSheetOutletState OutletState { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference _layoutElm;

    private bool _disposed;

    public BottomSheet()
    {
        _thisRef = DotNetObjectReference.Create(this);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        OutletState.RegisterSectionContentId(_sectionContentId);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Handle ??= CreateDefaultHandle();

        if (Expansion != null)
            _expansion = Expansion.Value;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/MRoessler.BlazorBottomSheet/{nameof(BottomSheet)}.razor.js");
            await _jsModule.InvokeVoidAsync("init", _layoutElm, _thisRef);
        }
    }

    private async Task OnBackgroundClickAsync()
    {
        if (!CloseOnBackgroundClick)
            return;
        _expansion = BottomSheetExpansion.Closed;
        StateHasChanged();
        await ExpansionChanged.InvokeAsync(_expansion);
    }

    /// <summary>
    /// INTERNAL API
    /// </summary>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task SetExpansionAsync(int expansion)
    {
        _expansion = (BottomSheetExpansion)expansion;
        StateHasChanged();
        await ExpansionChanged.InvokeAsync(_expansion);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        OutletState.DeregisterSectionContentId(_sectionContentId);
        _thisRef.Dispose();

        if (_jsModule != null)
        {
            await _jsModule.InvokeVoidAsync("dispose");
            await _jsModule.TryDisposeAsync();
        }

        _disposed = true;
    }
}

public enum BottomSheetExpansion
{
    Closed = 0, Minimized = 1, Normal = 2, Maximized = 3
}
