using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet.Sample.Components.Pages;

public partial class Home : ComponentBase
{
    private bool _isBottomSheetVisible = true;
    private BottomSheetExpansion _expansion;

    private bool _allowClosedExpansion = true;

    private bool _allowMinimizedExpansion = true;

    private bool _allowNormalExpansion = true;

    private bool _allowMaximizedExpansion = true;

    private void ToggleButtonSheetVisible() => _isBottomSheetVisible = !_isBottomSheetVisible;

    private void ToggleButtonSheetOpen() => _expansion = _expansion == BottomSheetExpansion.Closed ? BottomSheetExpansion.Normal : BottomSheetExpansion.Closed;

}
