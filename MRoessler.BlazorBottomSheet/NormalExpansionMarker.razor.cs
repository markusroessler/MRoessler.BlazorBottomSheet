using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet;

/// <summary>
/// Marker to be placed within the <see cref="BottomSheet"/> content for height-autosizing
/// </summary>
public partial class NormalExpansionMarker : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }
}
