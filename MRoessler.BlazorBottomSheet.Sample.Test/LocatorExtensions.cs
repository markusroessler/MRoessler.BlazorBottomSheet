using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test;

public static class LocatorExtensions
{
    public static async Task PanAsync(this ILocator locator, int deltaX, int deltaY, int steps = 5, int stepDelayMs = 100)
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
            await Task.Delay(stepDelayMs);
        }

        await locator.DispatchEventAsync("touchend");
    }


    public static async Task WhenBoundsStable(this ILocator locator)
    {
        var bounds = await locator.BoundingBoxAsync();

        for (int i = 0; i < 2; i++)
        {
            await Task.Delay(500);

            var newBounds = await locator.BoundingBoxAsync();
            if (Equals(bounds, newBounds))
                return;

            bounds = newBounds;
        }

        throw new TimeoutException("Element is still unstable");
    }


    private static bool Equals(LocatorBoundingBoxResult bounds1, LocatorBoundingBoxResult bounds2)
    {
        return bounds1 == bounds2
            || (bounds1.X == bounds2.X
                && bounds1.Y == bounds2.Y
                && bounds1.Width == bounds2.Width
                && bounds1.Height == bounds2.Height);
    }
}
