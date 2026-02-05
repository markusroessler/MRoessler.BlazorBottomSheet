using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MRoessler.BlazorBottomSheet.Sample.Utils;
using MRoessler.BlazorBottomSheet.Sample.ViewModels;

namespace MRoessler.BlazorBottomSheet.Sample.Components.Pages;

public sealed partial class BasicSample : ComponentBase, IDisposable
{
    const string AllowCloseOption = "Allow close";
    const string AllowMinimizeOption = "Allow minimize";
    const string AllowNormalOption = "Allow normal";
    const string AllowMaximizeOption = "Allow maximize";
    const string IsModalOption = "Modal";
    const string CloseOnBackgroundClickOption = "Close on background click";
    const string ApplyMudBlazorStylingOption = "Apply MudBlazor styling";

    [Inject]
    public BasicSampleViewModel ViewModel { get; set; } = default!;

    [Inject]
    public SynchronizationContextDispatcher SyncContextDispatcher { get; set; } = default!;

    [Inject]
    private TestHelper TestHelper { get; set; } = default!;

    private bool _isBottomSheetVisible = true;

    private BottomSheetExpansion _expansion;


    IReadOnlyCollection<string> _selectedOptions = [AllowCloseOption, AllowMinimizeOption, AllowNormalOption, AllowMaximizeOption, IsModalOption, CloseOnBackgroundClickOption];

    readonly Guid _instanceId = Guid.NewGuid();


    protected override void OnInitialized()
    {

        base.OnInitialized();
        ViewModel.StateChanged += ViewModel_StateChanged;
        SyncContextDispatcher.InitFromCurrentSyncContext();
        TestHelper.ActiveBasicSamplePages[_instanceId] = this;
    }

    private void ViewModel_StateChanged() => StateHasChanged();


    private void ToggleButtonSheetVisible() => _isBottomSheetVisible = !_isBottomSheetVisible;

    private void ToggleButtonSheetOpen() => _expansion = _expansion == BottomSheetExpansion.Closed ? BottomSheetExpansion.Normal : BottomSheetExpansion.Closed;

    private string GetBottomSheetClass() => _selectedOptions.Contains(ApplyMudBlazorStylingOption) ? "mud-bottom-sheet-layout" : "";


    public void Dispose()
    {
        ViewModel.StateChanged -= ViewModel_StateChanged;
        // TestHelper.ActiveBasicSamplePages.Remove(_instanceId, out _);
    }
}
