using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.JSInterop;

namespace MRoessler.BlazorBottomSheet;

public partial class BottomSheet : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Handle { get; set; }

    [Parameter]
    public bool IsVisible { get; set; }

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

    [Parameter]
    public BottomSheetExpansion Expansion { get; set; }

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
        Expansion = BottomSheetExpansion.Closed;
        StateHasChanged();
        await ExpansionChanged.InvokeAsync(Expansion);
    }

    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task SetExpansionAsync(int expansion)
    {
        Expansion = (BottomSheetExpansion)expansion;
        StateHasChanged();
        await ExpansionChanged.InvokeAsync(Expansion);
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
