using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Microsoft.Playwright.TestAdapter;
using MRoessler.BlazorBottomSheet.Sample.Components;
using MudBlazor.Services;

namespace MRoessler.BlazorBottomSheet.Sample.Test;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    WebApplicationFactory<Program> _webAppFactory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _webAppFactory = new WebApplicationFactory<Program>();
        _webAppFactory.UseKestrel(5001);
        _webAppFactory.StartServer();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardownAsync()
    {
        await _webAppFactory.DisposeAsync();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return Playwright.Devices[Environment.GetEnvironmentVariable("DEVICE_NAME")];
    }

    [Test]
    public async Task Test_SlowDragInDirection()
    {
        await Page.GotoAsync("http://localhost:5001");

        var bottomSheet = Page.GetByTestId("bottom-sheet");
        var bottomSheetLayout = Page.GetByTestId("bottom-sheet-layout");
        var minimizedMarker = Page.GetByTestId("minimized-marker");
        var normalMarker = Page.GetByTestId("normal-marker");
        var footer = Page.GetByTestId("footer");
        var handle = Page.GetByTestId("bottom-sheet-handle");

        await Expect(bottomSheet).Not.ToBeInViewportAsync();
        await Expect(bottomSheet).ToContainClassAsync("closed");

        await Page.GetByTestId("open-close-button").ClickAsync();

        // check default expansion (normal)
        await Expect(bottomSheet).ToBeInViewportAsync();
        await Expect(bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(normalMarker).ToBeInViewportAsync();
        await Expect(footer).Not.ToBeInViewportAsync();

        // drag up to maximized expansion
        await Pan(handle, 0, -Page.ViewportSize.Height / 2);
        await Expect(bottomSheetLayout).ToContainClassAsync("maximized");
        await Expect(footer).ToBeInViewportAsync();

        // drag down to normal expansion
        await Pan(handle, 0, Page.ViewportSize.Height / 2);
        await Expect(bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(footer).Not.ToBeInViewportAsync();
        await Expect(normalMarker).ToBeInViewportAsync();

        // drag down to minimized expansion
        await Pan(handle, 0, Page.ViewportSize.Height / 4);
        await Expect(bottomSheetLayout).ToContainClassAsync("minimized");
        await Expect(normalMarker).Not.ToBeInViewportAsync();
        await Expect(minimizedMarker).ToBeInViewportAsync();

        // drag down to closed expansion
        await Pan(handle, 0, Page.ViewportSize.Height / 8);
        await Expect(bottomSheetLayout).ToContainClassAsync("closed");
        await Expect(minimizedMarker).Not.ToBeInViewportAsync();
        await Expect(bottomSheet).Not.ToBeInViewportAsync();
    }

    public static async Task Pan(ILocator locator, int deltaX, int deltaY, int steps = 5)
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
