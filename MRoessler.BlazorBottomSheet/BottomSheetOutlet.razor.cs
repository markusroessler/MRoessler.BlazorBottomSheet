using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet;

public partial class BottomSheetOutlet : ComponentBase
{
    internal const string ContentSectionName = "BottomSheetContent";

    [Inject]
    private BottomSheetOutletState State { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        State.OnSectionContentIdAdded += State_OnSectionContentIdAdded;
        State.OnSectionContentIdRemoved += State_OnSectionContentIdRemoved;
    }

    private void State_OnSectionContentIdRemoved(Guid guid) => StateHasChanged();

    private void State_OnSectionContentIdAdded(Guid guid) => StateHasChanged();
}
