using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MRoessler.BlazorBottomSheet;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Pages;

public sealed partial class DynamicContentSample : IAsyncDisposable
{
    readonly Guid _instanceId = Guid.NewGuid();

    private BottomSheetExpansion _expansion;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private BottomSheet? _sheet;
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _jsObjRef;
    private ElementReference _rootElm;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var sheet = _sheet ?? throw new InvalidOperationException("_sheet is null");
            await _sheet.WhenRenderedOnce();
            var jsSheetObjRef = sheet.JavaScriptObjRef ?? throw new InvalidOperationException("_sheet?.JavaScriptObjRef is null");

            _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./_content/Sample.Lib/Pages/{nameof(DynamicContentSample)}.razor.js");
            _jsObjRef = await _jsModule.InvokeAsync<IJSObjectReference>("createDynamicContentSample", _rootElm, jsSheetObjRef);
        }
    }

    private void ToggleButtonSheetOpen() => _expansion = _expansion == BottomSheetExpansion.Closed ? BottomSheetExpansion.Minimized : BottomSheetExpansion.Closed;


    public async ValueTask DisposeAsync()
    {
        if (_jsObjRef != null)
        {
            await _jsObjRef.InvokeVoidAsync("dispose");
            await _jsObjRef.TryDisposeAsync();
        }

        if (_jsModule != null)
            await _jsModule.TryDisposeAsync();
    }
}
