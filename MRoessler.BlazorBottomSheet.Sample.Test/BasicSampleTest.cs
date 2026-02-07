using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
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
using MRoessler.BlazorBottomSheet.Sample.Utils;
using MudBlazor.Services;
using NUnit.Framework.Interfaces;

namespace MRoessler.BlazorBottomSheet.Sample.Test;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class BasicSampleTest : CustomPageTest
{
    const int SlowDragStepDelayMs = 100;
    const int FastDragStepDelayMs = 0;

    ILocator _bottomSheet;
    ILocator _bottomSheetLayout;
    ILocator _minimizedMarker;
    ILocator _normalMarker;
    ILocator _footer;
    ILocator _handle;
    ILocator _openCloseButton;


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

    private Task<IResponse> GotoBasicSamplePageAsync() => Page.GotoAsync(WebAppFactory.ClientOptions.BaseAddress.ToString());


    [Test]
    public Task Test_SlowDragInDirection()
    {
        return TestAsync(async () =>
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

        });
    }


    [Test]
    public Task Test_FastDragInDirection()
    {
        return TestAsync(async () =>
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
        });
    }

    [Test]
    public Task Test_SlowDragToPosition()
    {
        return TestAsync(async () =>
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
            await _handle.PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
            await Expect(_footer).ToBeInViewportAsync();

            // drag down to minimized expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height - 100, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
            await Expect(_normalMarker).Not.ToBeInViewportAsync();
            await Expect(_minimizedMarker).ToBeInViewportAsync();

            // drag up to maximized expansion
            await _handle.PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
            await Expect(_footer).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_ToggleIsOpen()
    {
        return TestAsync(async () =>
        {
            await GotoBasicSamplePageAsync();

            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");

            var samplePageInstanceIdElm = await Page.GetByTestId("sample-instance-id").ElementHandleAsync();
            var samplePageInstanceId = Guid.Parse(await samplePageInstanceIdElm.TextContentAsync());

            var samplePage = TestHelper.ActiveBasicSamplePages[samplePageInstanceId];
            var basicSampleViewModel = samplePage.ViewModel;
            var syncContextDispatcher = samplePage.SyncContextDispatcher;

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(true));
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).ToBeInViewportAsync();
            await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
            await Expect(_normalMarker).ToBeInViewportAsync();
            await Expect(_footer).Not.ToBeInViewportAsync();

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(false));
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");
        });
    }
}
