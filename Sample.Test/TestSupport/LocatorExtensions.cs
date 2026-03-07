using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class LocatorExtensions
{
    public static async Task PanAsync(this ILocator locator, int deltaX, int deltaY, int steps = 5, int stepDelayMs = 100)
    {
        var (touchStartX, touchStartY) = await locator.TouchStartAsync();
        await locator.TouchMoveAsync(touchStartX, touchStartY, deltaX, deltaY, steps, stepDelayMs);
        await locator.TouchEndAsync();
    }


    public static async Task<(double touchX, double touchY)> TouchStartAsync(this ILocator locator)
    {
        var bounds = await locator.BoundingBoxAsync();
        var centerX = (double)bounds.X + bounds.Width / 2;
        var centerY = (double)bounds.Y + bounds.Height / 2;

        var touches = new List<Dictionary<string, object>>
        {
            new() {
                { "identifier", 0 },
                { "clientX", centerX },
                { "clientY", centerY }
            }
        };
        await locator.DispatchEventAsync("touchstart", new { touches, changedTouches = touches, targetTouches = touches });
        return (centerX, centerY);
    }


    public static async Task<(double lastTouchX, double lastTouchY)> TouchMoveAsync(this ILocator locator, double startX, double startY, double deltaX, double deltaY, int steps = 5, int stepDelayMs = 100)
    {
        for (var i = 1; i <= steps; i++)
        {
            var touches = new List<Dictionary<string, object>>
            {
                new() {
                    { "identifier", 0 },
                    { "clientX", startX + deltaX * i / steps },
                    { "clientY", startY + deltaY * i / steps }
                }
            };
            await locator.DispatchEventAsync("touchmove", new { touches, changedTouches = touches, targetTouches = touches });
            await Task.Delay(stepDelayMs);
        }
        return (startX + deltaX, startY + deltaY);
    }

    public static async Task TouchEndAsync(this ILocator locator) => await locator.DispatchEventAsync("touchend");


    public static async Task WhenBoundsStable(this ILocator locator)
    {
        var bounds = await locator.BoundingBoxAsync();

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(200);

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

    public static Task<double> AssertScrollTopAsync(this ILocator locator, double expected) => locator.AssertScrollTopInRangeAsync(expected, expected);

    public static async Task<double> AssertScrollTopInRangeAsync(this ILocator locator, double from, double to)
    {
        var scrollTop = await locator.EvaluateAsync<double>("el => el.scrollTop");
        Assert.That(scrollTop, Is.InRange(from, to));
        return scrollTop;
    }

    public static Task<double> AssertClientYAsync(this ILocator locator, double expected) => locator.AssertClientYInRangeAsync(expected, expected);

    public static async Task<double> AssertClientYInRangeAsync(this ILocator locator, double from, double to)
    {
        var bounds = await locator.BoundingBoxAsync();
        Assert.That(bounds.Y, Is.InRange(from, to));
        return bounds.Y;
    }
}
