using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet;

/// <summary>
/// Marker to be placed within the <see cref="BottomSheet"/> content for height-autosizing
/// </summary>
public partial class MinimizedExpansionMarker : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only...", Justification = "...but not Blazor Parameters")]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }
}
