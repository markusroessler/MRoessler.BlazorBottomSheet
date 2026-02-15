using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// whether to prevent interaction with the overlayed content
    /// </summary>
    [Parameter]
    public bool IsModal { get; set; }

    /// <summary>
    /// close the sheet when the background overlay is clicked?
    /// </summary>
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


    private const string AutoColorSchemeClass = "auto-color";
    private const string LightColorSchemeClass = "light";
    private const string DarkColorSchemeClass = "dark";
    private string GetColorSchemeStyleClass() => OutletState.ColorScheme switch
    {
        BottomSheetColorScheme.Auto => AutoColorSchemeClass,
        BottomSheetColorScheme.Light => LightColorSchemeClass,
        BottomSheetColorScheme.Dark => DarkColorSchemeClass,
        _ => AutoColorSchemeClass,
    };

    private BottomSheetExpansion _expansion;
    /// <summary>
    /// the expansion state to apply
    /// </summary>
    [Parameter]
    public BottomSheetExpansion Expansion { get; set; }
    [Parameter]
    public EventCallback<BottomSheetExpansion> ExpansionChanged { get; set; }

    private bool _isOpen;
    /// <summary>
    /// Alternative to <see cref="Expansion"/> for easier ViewModel bindings.
    /// When set to true <see cref="DefaultExpansion"/> will be applied.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// <see cref="Expansion"/> to be applied when <see cref="IsOpen"/> is set to true
    /// </summary>
    [Parameter]
    public BottomSheetExpansion DefaultExpansion { get; set; } = BottomSheetExpansion.Normal;

    /// <summary>
    /// the actual expansion state (see https://learn.microsoft.com/en-us/aspnet/core/blazor/components/data-binding?view=aspnetcore-10.0#bind-across-more-than-two-components)
    /// </summary>
    private BottomSheetExpansion _expansionToRender;

    /// <summary>
    /// style class to apply on the BottomSheet element
    /// </summary>
    [Parameter]
    public string Class { get; set; } = "";

    /// <summary>
    /// style class to apply on the BottomSheet background (visible when <see cref="IsModal"/> is true)
    /// </summary>
    [Parameter]
    public string BackgroundClass { get; set; } = "";

    [Parameter(CaptureUnmatchedValues = true)]
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only...", Justification = "...but not Blazor Parameters")]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly Guid _sectionContentId = Guid.NewGuid();

    private readonly DotNetObjectReference<BottomSheet> _thisRef;
    private IJSObjectReference? _jsModule;

    /// <summary>
    /// Reference to the javascript object.
    /// You may use this to add event listeners.
    /// </summary>
    public IJSObjectReference? JavaScriptObjRef { get; private set; }

    [Inject]
    private BottomSheetOutletState OutletState { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private ElementReference _layoutElm;

    private TaskCompletionSource? _whenRenderedOnce;

    private bool _disposed;


    public BottomSheet()
    {
        _thisRef = DotNetObjectReference.Create(this);
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();
        OutletState.RegisterSectionContentId(_sectionContentId);
        OutletState.PropertyChanged += OutletState_PropertyChanged;
    }

    private void OutletState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BottomSheetOutletState.ColorScheme))
            StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        Handle ??= CreateDefaultHandle();

        if (Expansion != _expansion)
        {
            var isOpenChanged = _expansion == BottomSheetExpansion.Closed || Expansion == BottomSheetExpansion.Closed;
            _expansionToRender = _expansion = Expansion;
            if (isOpenChanged)
                await IsOpenChanged.InvokeAsync(_expansionToRender != BottomSheetExpansion.Closed);
        }
        else if (IsOpen != _isOpen)
        {
            _isOpen = IsOpen;
            _expansionToRender = IsOpen ? DefaultExpansion : BottomSheetExpansion.Closed;
            await ExpansionChanged.InvokeAsync(_expansionToRender);
        }
    }

    /// <summary>
    /// Use this to wait for the sheet render at least once.
    /// <see cref="JavaScriptObjRef"/> should be set after the first render.
    /// </summary>
    public Task WhenRenderedOnce()
    {
        _whenRenderedOnce ??= new();
        return _whenRenderedOnce.Task;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/MRoessler.BlazorBottomSheet/{nameof(BottomSheet)}.razor.js");
            JavaScriptObjRef = await _jsModule.InvokeAsync<IJSObjectReference>("createBottomSheet", _layoutElm, _thisRef);
            _whenRenderedOnce?.SetResult();
        }
    }

    private Task OnBackgroundClickAsync()
    {
        if (CloseOnBackgroundClick)
            return UpdateExpansionAsync(BottomSheetExpansion.Closed);
        else
            return Task.CompletedTask;
    }

    /// <summary>
    /// INTERNAL API
    /// </summary>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task SetExpansionAsync(int expansion) => UpdateExpansionAsync((BottomSheetExpansion)expansion);

    private async Task UpdateExpansionAsync(BottomSheetExpansion expansion)
    {
        if (_expansionToRender == expansion)
            return;

        var isOpenChanged = _expansionToRender == BottomSheetExpansion.Closed || expansion == BottomSheetExpansion.Closed;
        _expansionToRender = expansion;
        StateHasChanged();

        await ExpansionChanged.InvokeAsync(_expansionToRender);

        if (isOpenChanged)
            await IsOpenChanged.InvokeAsync(_expansionToRender != BottomSheetExpansion.Closed);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        _whenRenderedOnce?.TrySetCanceled();

        OutletState.DeregisterSectionContentId(_sectionContentId);
        OutletState.PropertyChanged -= OutletState_PropertyChanged;

        _thisRef.Dispose();

        if (JavaScriptObjRef != null)
        {
            await JavaScriptObjRef.TryInvokeVoidAsync("dispose");
            await JavaScriptObjRef.TryDisposeAsync();
        }

        if (_jsModule != null)
            await _jsModule.TryDisposeAsync();

        _disposed = true;
    }
}

public enum BottomSheetExpansion
{
    Closed = 0, Minimized = 1, Normal = 2, Maximized = 3
}

/// <summary>
/// supported color schemes
/// </summary>
public enum BottomSheetColorScheme
{
    /// <summary>
    /// Applies colors using light-dark css function (see https://developer.mozilla.org/en-US/docs/Web/CSS/Reference/Values/color_value/light-dark). <br/>
    /// For this to work you must set <c>color-scheme: light dark;</c> on <c>:root</c>
    /// </summary>
    Auto,
    Light,
    Dark
}
