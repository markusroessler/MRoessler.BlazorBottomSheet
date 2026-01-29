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
public class BasicSampleTest : PageTest
{
    const int SlowDragStepDelayMs = 100;

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
        await bottomSheet.WhenBoundsStable();
        await Expect(bottomSheet).ToBeInViewportAsync();
        await Expect(bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(normalMarker).ToBeInViewportAsync();
        await Expect(footer).Not.ToBeInViewportAsync();

        // drag up to maximized expansion
        await handle.PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
        await bottomSheet.WhenBoundsStable();
        await Expect(bottomSheetLayout).ToContainClassAsync("maximized");
        await Expect(footer).ToBeInViewportAsync();

        // drag down to normal expansion
        await handle.PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
        await bottomSheet.WhenBoundsStable();
        await Expect(bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(footer).Not.ToBeInViewportAsync();
        await Expect(normalMarker).ToBeInViewportAsync();

        // drag down to minimized expansion
        await handle.PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
        await bottomSheet.WhenBoundsStable();
        await Expect(bottomSheetLayout).ToContainClassAsync("minimized");
        await Expect(normalMarker).Not.ToBeInViewportAsync();
        await Expect(minimizedMarker).ToBeInViewportAsync();

        // drag down to closed expansion
        await handle.PanAsync(0, Page.ViewportSize.Height / 6, stepDelayMs: SlowDragStepDelayMs);
        await bottomSheet.WhenBoundsStable();
        await Expect(bottomSheetLayout).ToContainClassAsync("closed");
        await Expect(minimizedMarker).Not.ToBeInViewportAsync();
        await Expect(bottomSheet).Not.ToBeInViewportAsync();
    }
}
