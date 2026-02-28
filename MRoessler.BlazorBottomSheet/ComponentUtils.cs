using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace MRoessler.BlazorBottomSheet;

internal static class ComponentUtils
{
    internal static async ValueTask TryDisposeAsync(this IJSObjectReference? objRef, ILogger? logger = null)
    {
        try
        {
            if (objRef != null)
                await objRef.DisposeAsync();
        }
        catch (JSDisconnectedException ex)
        {
            logger?.LogDebug(ex, "can't dispose IJSObjectReference");
        }
    }

    internal static async ValueTask TryInvokeVoidAsync(this IJSObjectReference? objRef, string identifier, object?[]? args = null, ILogger? logger = null)
    {
        try
        {
            if (objRef != null)
                await objRef.InvokeVoidAsync(identifier, args);
        }
        catch (JSDisconnectedException ex)
        {
            logger?.LogDebug(ex, "can't dispose IJSObjectReference");
        }
    }

    internal static string GetJsFileNameInfix(bool useMinifiedJavaScripts) => useMinifiedJavaScripts ? ".min" : "";
}
