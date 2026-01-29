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
    const int FastDragStepDelayMs = 0;

    WebApplicationFactory<Program> _webAppFactory;

    ILocator _bottomSheet;
    ILocator _bottomSheetLayout;
    ILocator _minimizedMarker;
    ILocator _normalMarker;
    ILocator _footer;
    ILocator _handle;
    ILocator _openCloseButton;


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

    [SetUp]
    public void Setup()
    {
        _bottomSheet = Page.GetByTestId("bottom-sheet");
        _bottomSheetLayout = Page.GetByTestId("bottom-sheet-layout");
        _minimizedMarker = Page.GetByTestId("minimized-marker");
        _normalMarker = Page.GetByTestId("normal-marker");
        _footer = Page.GetByTestId("footer");
        _handle = Page.GetByTestId("bottom-sheet-handle");
        _openCloseButton = Page.GetByTestId("open-close-button");
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return Playwright.Devices[Environment.GetEnvironmentVariable("DEVICE_NAME")];
    }

    private Task<IResponse> GotoBasicSamplePageAsync() => Page.GotoAsync("http://localhost:5001");

    [Test]
    public async Task Test_SlowDragInDirection()
    {
        await GotoBasicSamplePageAsync();

        await Expect(_bottomSheet).Not.ToBeInViewportAsync();
        await Expect(_bottomSheet).ToContainClassAsync("closed");

        await _openCloseButton.ClickAsync();

        // check default expansion (normal)
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheet).ToBeInViewportAsync();
        await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(_normalMarker).ToBeInViewportAsync();
        await Expect(_footer).Not.ToBeInViewportAsync();

        // drag up to maximized expansion
        await _handle.PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
        await Expect(_footer).ToBeInViewportAsync();

        // drag down to normal expansion
        await _handle.PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(_footer).Not.ToBeInViewportAsync();
        await Expect(_normalMarker).ToBeInViewportAsync();

        // drag down to minimized expansion
        await _handle.PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
        await Expect(_normalMarker).Not.ToBeInViewportAsync();
        await Expect(_minimizedMarker).ToBeInViewportAsync();

        // drag down to closed expansion
        await _handle.PanAsync(0, Page.ViewportSize.Height / 6, stepDelayMs: SlowDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("closed");
        await Expect(_minimizedMarker).Not.ToBeInViewportAsync();
        await Expect(_bottomSheet).Not.ToBeInViewportAsync();
    }


    [Test]
    public async Task Test_FastDragInDirection()
    {
        await GotoBasicSamplePageAsync();

        await Expect(_bottomSheet).Not.ToBeInViewportAsync();
        await Expect(_bottomSheet).ToContainClassAsync("closed");

        await _openCloseButton.ClickAsync();

        // check default expansion (normal)
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheet).ToBeInViewportAsync();
        await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
        await Expect(_normalMarker).ToBeInViewportAsync();
        await Expect(_footer).Not.ToBeInViewportAsync();

        // slow drag to minimized expansion
        await _handle.PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
        await Expect(_normalMarker).Not.ToBeInViewportAsync();
        await Expect(_minimizedMarker).ToBeInViewportAsync();

        // fast drag to maximized expansion
        await _handle.PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
        await Expect(_footer).ToBeInViewportAsync();

        // fast drag to minimized expansion
        await _handle.PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
        await _bottomSheet.WhenBoundsStable();
        await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
        await Expect(_normalMarker).Not.ToBeInViewportAsync();
        await Expect(_minimizedMarker).ToBeInViewportAsync();
    }
}
