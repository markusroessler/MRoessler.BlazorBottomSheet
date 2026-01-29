using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test;

public static class LocatorExtensions
{
    public static void Foobar(string param)
    {

    }

    public static async Task Pan(this ILocator locator, int deltaX, int deltaY, int steps = 5)
    {
        var bounds = await locator.BoundingBoxAsync();
        double centerX = bounds.X + bounds.Width / 2;
        double centerY = bounds.Y + bounds.Height / 2;

        var touches = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "identifier", 0 },
                { "clientX", centerX },
                { "clientY", centerY }
            }
        };
        await locator.DispatchEventAsync("touchstart", new { touches, changedTouches = touches, targetTouches = touches });

        for (int i = 1; i <= steps; i++)
        {
            touches = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "identifier", 0 },
                    { "clientX", centerX + deltaX * i / steps },
                    { "clientY", centerY + deltaY * i / steps }
                }
            };
            await locator.DispatchEventAsync("touchmove", new { touches, changedTouches = touches, targetTouches = touches });
        }

        await locator.DispatchEventAsync("touchend");
    }
}
