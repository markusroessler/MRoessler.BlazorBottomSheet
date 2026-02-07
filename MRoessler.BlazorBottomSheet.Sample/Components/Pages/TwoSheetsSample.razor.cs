using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet.Sample.Components.Pages;

public partial class TwoSheetsSample
{
    readonly Guid _instanceId = Guid.NewGuid();

    private BottomSheetExpansion _expansion1;
    private BottomSheetExpansion _expansion2;


    private void ToggleIsOpen1() => _expansion1 = _expansion1 == BottomSheetExpansion.Closed ? BottomSheetExpansion.Minimized : BottomSheetExpansion.Closed;
    private void ToggleIsOpen2() => _expansion2 = _expansion2 == BottomSheetExpansion.Closed ? BottomSheetExpansion.Minimized : BottomSheetExpansion.Closed;

}
