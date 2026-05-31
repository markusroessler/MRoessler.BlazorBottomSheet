using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet;

/// <summary>
/// Optional config options
/// </summary>
public sealed class BottomSheetServicesConfiguration
{
    /// <summary>
    /// Whether to load minified versions of JavaScript files (Default: true)
    /// </summary>
    public bool UseMinifiedJavaScripts { get; init; } = true;
}
