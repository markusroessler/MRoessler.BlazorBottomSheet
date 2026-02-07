using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet.Sample.Components.Pages;

public partial class DynamicContentSample
{
    readonly Guid _instanceId = Guid.NewGuid();

    private BottomSheetExpansion _expansion;


    private void ToggleButtonSheetOpen() => _expansion = _expansion == BottomSheetExpansion.Closed ? BottomSheetExpansion.Minimized : BottomSheetExpansion.Closed;

}
