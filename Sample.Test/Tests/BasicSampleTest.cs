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
using MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;
using NUnit.Framework.Interfaces;

namespace MRoessler.BlazorBottomSheet.Sample.Test.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class BasicSampleTest : CustomPageTest
{
    const int SlowDragStepDelayMs = 100;
    const int FastDragStepDelayMs = 10;

    private Task<IResponse> GotoBasicSamplePageAsync() => Page.GotoAsync(WebAppFactory.ClientOptions.BaseAddress.ToString());

    ILocator GetSheetLayout() => Page.GetByTestId("bottom-sheet-layout");
    ILocator GetOpenCloseButton() => Page.GetByTestId("open-close-button");


    [Test]
    public Task Test_SlowDragInDirection()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = GetSheetLayout();
            var sheet = sheetLayout.BottomSheet();

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await GetOpenCloseButton().ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(sheet.NormalMarker()).ToBeInViewportAsync();
            await Expect(sheet.Footer()).Not.ToBeInViewportAsync();

            // drag up to maximized expansion
            await sheet.Handle().PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(sheet.Footer()).ToBeInViewportAsync();

            // drag down to normal expansion
            await sheet.Handle().PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(sheet.Footer()).Not.ToBeInViewportAsync();
            await Expect(sheet.NormalMarker()).ToBeInViewportAsync();

            // drag down to minimized expansion
            await sheet.Handle().PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(sheet.NormalMarker()).Not.ToBeInViewportAsync();
            await Expect(sheet.MinimizedMarker()).ToBeInViewportAsync();

            // drag down to closed expansion
            await sheet.Handle().PanAsync(0, Page.ViewportSize.Height / 6, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("closed");
            await Expect(sheet.MinimizedMarker()).Not.ToBeInViewportAsync();
            await Expect(sheet).Not.ToBeInViewportAsync();

        });
    }


    [Test]
    public Task Test_FastDragInDirection()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = GetSheetLayout();
            var sheet = sheetLayout.BottomSheet();

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await GetOpenCloseButton().ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(sheet.NormalMarker()).ToBeInViewportAsync();
            await Expect(sheet.Footer()).Not.ToBeInViewportAsync();

            // slow drag to minimized expansion
            await sheet.Handle().PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(sheet.NormalMarker()).Not.ToBeInViewportAsync();
            await Expect(sheet.MinimizedMarker()).ToBeInViewportAsync();

            // fast drag to maximized expansion
            await sheet.Handle().PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(sheet.Footer()).ToBeInViewportAsync();

            // fast drag to minimized expansion
            await sheet.Handle().PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(sheet.NormalMarker()).Not.ToBeInViewportAsync();
            await Expect(sheet.MinimizedMarker()).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_SlowDragToPosition()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = GetSheetLayout();
            var sheet = sheetLayout.BottomSheet();

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await GetOpenCloseButton().ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(sheet.NormalMarker()).ToBeInViewportAsync();
            await Expect(sheet.Footer()).Not.ToBeInViewportAsync();

            // drag up to maximized expansion
            await sheet.Handle().PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(sheet.Footer()).ToBeInViewportAsync();

            // drag down to minimized expansion
            await sheet.Handle().PanAsync(0, Page.ViewportSize.Height - 100, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(sheet.NormalMarker()).Not.ToBeInViewportAsync();
            await Expect(sheet.MinimizedMarker()).ToBeInViewportAsync();

            // drag up to maximized expansion
            await sheet.Handle().PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(sheet.Footer()).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_ToggleIsOpen()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = GetSheetLayout();
            var sheet = sheetLayout.BottomSheet();

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            var samplePageInstanceIdElm = await Page.GetByTestId("sample-instance-id").ElementHandleAsync();
            var samplePageInstanceId = Guid.Parse(await samplePageInstanceIdElm.TextContentAsync());

            var samplePage = TestHelper.ActiveBasicSamplePages[samplePageInstanceId];
            var basicSampleViewModel = samplePage.ViewModel;
            var syncContextDispatcher = samplePage.SyncContextDispatcher;

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(true));
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(sheet.NormalMarker()).ToBeInViewportAsync();
            await Expect(sheet.Footer()).Not.ToBeInViewportAsync();

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(false));
            await sheet.WhenBoundsStable();
            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");
        });
    }

    [Test]
    public Task Test_ScrollableDrag()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = GetSheetLayout();
            var sheet = sheetLayout.BottomSheet();

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await GetOpenCloseButton().ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(sheet.NormalMarker()).ToBeInViewportAsync();
            await Expect(sheet.Footer()).Not.ToBeInViewportAsync();

            var scrollable = sheet.Scrollable();
            await Expect(scrollable).ToHaveJSPropertyAsync("scrollTop", 0);

            // drag up to maximized expansion with overscroll
            const int Overscroll = 100;
            var sheetBounds = await sheet.BoundingBoxAsync();
            var (lastTouchX, lastTouchY) = await scrollable.TouchStartAsync();
            (lastTouchX, lastTouchY) = await scrollable.TouchMoveAsync(lastTouchX, lastTouchY, 0, (int)-sheetBounds.Y - Overscroll, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await sheet.AssertClientYAsync(0);
            var scrollTop = await scrollable.AssertScrollTopInRangeAsync(Overscroll - 1, Overscroll);

            // scroll down
            (lastTouchX, lastTouchY) = await scrollable.TouchMoveAsync(lastTouchX, lastTouchY, 0, scrollTop, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await sheet.AssertClientYAsync(0);
            await scrollable.AssertScrollTopInRangeAsync(0, 1);

            // drag down a bit (but stay near maximized)
            const int DragDownDistance = 100;
            (lastTouchX, lastTouchY) = await scrollable.TouchMoveAsync(lastTouchX, lastTouchY, DragDownDistance, scrollTop, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await sheet.AssertClientYInRangeAsync(DragDownDistance - 1, DragDownDistance);
            await scrollable.AssertScrollTopAsync(0);

            // end the drag and check expansion state
            await scrollable.TouchEndAsync();
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(sheet.Footer()).ToBeInViewportAsync();
        });
    }
}
