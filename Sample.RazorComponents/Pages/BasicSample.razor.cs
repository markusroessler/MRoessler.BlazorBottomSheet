using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.ViewModels;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Pages;

public sealed partial class BasicSample : ComponentBase, IDisposable
{
    const string AllowCloseOption = "Allow Close";
    const string AllowMinimizeOption = "Allow Minimize";
    const string AllowNormalOption = "Allow Normal";
    const string AllowMaximizeOption = "Allow Maximize";
    const string IsModalOption = "Modal";
    const string CloseOnBackgroundClickOption = "Auto-Close";
    const string ApplyMudBlazorStylingOption = "MudBlazor Styling";

    [Inject]
    public BasicSampleViewModel ViewModel { get; set; } = default!;

    [Inject]
    public SynchronizationContextDispatcher SyncContextDispatcher { get; set; } = default!;

    [Inject]
    private TestHelper TestHelper { get; set; } = default!;

    private bool _isBottomSheetVisible = true;

    private BottomSheetExpansion _expansion;


    IReadOnlyCollection<string> _selectedOptions = [AllowCloseOption, AllowMinimizeOption, AllowNormalOption, AllowMaximizeOption, IsModalOption, CloseOnBackgroundClickOption, ApplyMudBlazorStylingOption];

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

    private string GetBottomSheetClass() => _selectedOptions.Contains(ApplyMudBlazorStylingOption) ? "mud-bottom-sheet" : "";
    private string GetBottomSheetBackgroundClass() => _selectedOptions.Contains(ApplyMudBlazorStylingOption) ? "mud-bottom-sheet-background" : "";


    public void Dispose()
    {
        ViewModel.StateChanged -= ViewModel_StateChanged;
        // TestHelper.ActiveBasicSamplePages.Remove(_instanceId, out _);
    }
}
