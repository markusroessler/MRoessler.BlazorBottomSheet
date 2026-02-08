using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MRoessler.BlazorBottomSheet;

public abstract class ExpansionMarker : ComponentBase
{
    /// <summary>
    /// style class to apply on the marker
    /// </summary>
    [Parameter]
    public string Class { get; set; } = "";

    /// <summary>
    /// styles to apply on the marker
    /// </summary>
    [Parameter]
    public string Style { get; set; } = "";

    [Parameter(CaptureUnmatchedValues = true)]
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only...", Justification = "...but not Blazor Parameters")]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }
}
