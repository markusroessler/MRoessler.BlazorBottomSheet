using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class LocatorExtensions
{
    public static async Task PanAsync(this ILocator locator, double deltaX, double deltaY, int steps = 5, int stepDelayMs = 100, bool useMouseEvents = false)
    {
        if (useMouseEvents)
        {
            var (clientX, clientY) = await locator.MouseDownAsync();
            await locator.MouseMoveAsync(clientX, clientY, deltaX, deltaY, steps, stepDelayMs);
            await Task.Delay(stepDelayMs);
            await locator.MouseUpAsync();
        }
        else
        {
            var (touchStartX, touchStartY) = await locator.TouchStartAsync();
            await locator.TouchMoveAsync(touchStartX, touchStartY, deltaX, deltaY, steps, stepDelayMs);
            await Task.Delay(stepDelayMs);
            await locator.TouchEndAsync();
        }
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

    public static async Task<(double clientX, double clientY)> MouseDownAsync(this ILocator locator)
    {
        var bounds = await locator.BoundingBoxAsync();
        var centerX = (double)bounds.X + bounds.Width / 2;
        var centerY = (double)bounds.Y + bounds.Height / 2;

        await locator.DispatchEventAsync("mousedown", new { clientX = centerX, clientY = centerY });
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
            await Task.Delay(stepDelayMs);
            await locator.DispatchEventAsync("touchmove", new { touches, changedTouches = touches, targetTouches = touches });
        }
        return (startX + deltaX, startY + deltaY);
    }

    public static async Task<(double lastTouchX, double lastTouchY)> MouseMoveAsync(this ILocator locator, double startX, double startY, double deltaX, double deltaY, int steps = 5, int stepDelayMs = 100)
    {
        for (var i = 1; i <= steps; i++)
        {
            await Task.Delay(stepDelayMs);
            await locator.DispatchEventAsync("mousemove", new
            {
                clientX = startX + deltaX * i / steps,
                clientY = startY + deltaY * i / steps
            });
        }
        return (startX + deltaX, startY + deltaY);
    }

    public static async Task TouchEndAsync(this ILocator locator) => await locator.DispatchEventAsync("touchend");

    public static async Task MouseUpAsync(this ILocator locator) => await locator.DispatchEventAsync("mouseup");


    public static async Task WhenBoundsStable(this ILocator locator)
    {
        var bounds = await locator.BoundingBoxAsync();
        await Task.Delay(200);
        await ExpectAsync(async () =>
        {
            var newBounds = await locator.BoundingBoxAsync();
            if (Equals(bounds, newBounds))
                return (true, "stable");

            bounds = newBounds;
            return (false, "unstable");
        });
    }


    private static bool Equals(LocatorBoundingBoxResult bounds1, LocatorBoundingBoxResult bounds2)
    {
        return bounds1 == bounds2
            || (bounds1.X == bounds2.X
                && bounds1.Y == bounds2.Y
                && bounds1.Width == bounds2.Width
                && bounds1.Height == bounds2.Height);
    }

    public static Task<double> ExpectScrollTopToBeInRangeAsync(this ILocator locator, double from, double to)
    {
        return ExpectAsync(async () =>
        {
            var scrollTop = await locator.EvaluateAsync<double>("el => el.scrollTop");
            if (scrollTop >= from && scrollTop <= to)
                return (true, scrollTop);
            else
                return (false, scrollTop);
        });
    }

    public static Task<double> ExpectClientYToBeAsync(this ILocator locator, double expected) => locator.ExpectClientYToBeInRangeAsync(expected, expected);

    public static Task<double> ExpectClientYToBeInRangeAsync(this ILocator locator, double from, double to)
    {
        return ExpectAsync(async () =>
        {
            var bounds = await locator.BoundingBoxAsync();
            if (bounds.Y >= from && bounds.Y <= to)
                return (true, (double)bounds.Y);
            else
                return (false, (double)bounds.Y);
        });
    }

    private static async Task<TResult> ExpectAsync<TResult>(Func<Task<(bool, TResult)>> func)
    {
        var stopwatch = Stopwatch.StartNew();
        TResult lastResult = default;

        while (stopwatch.Elapsed.TotalSeconds < 5)
        {
            var (succeeded, result) = await func();
            lastResult = result;
            if (succeeded)
                return result;
            else
                await Task.Delay(200);
        }

        throw new PlaywrightException($"Retry timeout (last value: {lastResult})");
    }
}
