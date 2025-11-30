using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet.SampleV2.Components.Pages;

public partial class Home : ComponentBase
{
    private bool _isBottomSheetVisible = true;

    private bool _isBottomSheetOpen;

    private void ToggleButtonSheetVisible() => _isBottomSheetVisible = !_isBottomSheetVisible;

    private void ToggleButtonSheetOpen() => _isBottomSheetOpen = !_isBottomSheetOpen;
}
