using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet;

public partial class BottomSheetSectionEnd
{
    [Parameter]
    public BottomSheetSection Section { get; set; }
}

public enum BottomSheetSection
{
    Minimized = 1, Normal = 2
}