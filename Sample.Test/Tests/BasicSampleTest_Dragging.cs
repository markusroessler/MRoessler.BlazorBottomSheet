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
public class BasicSampleTest_Dragging : CustomPageTest
{
    const int SlowDragStepDelayMs = 100;
    const int FastDragStepDelayMs = 20;

    private Task<IResponse> GotoBasicSamplePageAsync() => Page.GotoAsync(WebAppFactory.ClientOptions.BaseAddress.ToString());

    [Test]
    public Task Test_SlowDragInDirection()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).ToBeInViewportAsync();
            await Expect(BasicSampleLocators.Footer(sheet)).Not.ToBeInViewportAsync();

            // drag up to maximized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(BasicSampleLocators.Footer(sheet)).ToBeInViewportAsync();

            // drag down to normal expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(BasicSampleLocators.Footer(sheet)).Not.ToBeInViewportAsync();
            await Expect(BottomSheetLocators.NormalMarker(sheet)).ToBeInViewportAsync();

            // drag down to minimized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).Not.ToBeInViewportAsync();
            await Expect(BottomSheetLocators.MinimizedMarker(sheet)).ToBeInViewportAsync();

            // drag down to closed expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, Page.ViewportSize.Height / 6, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("closed");
            await Expect(BottomSheetLocators.MinimizedMarker(sheet)).Not.ToBeInViewportAsync();
            await Expect(sheet).Not.ToBeInViewportAsync();

        });
    }


    [Test]
    public Task Test_FastDragInDirection()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).ToBeInViewportAsync();
            await Expect(BasicSampleLocators.Footer(sheet)).Not.ToBeInViewportAsync();

            // slow drag to minimized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).Not.ToBeInViewportAsync();
            await Expect(BottomSheetLocators.MinimizedMarker(sheet)).ToBeInViewportAsync();

            // fast drag to maximized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(BasicSampleLocators.Footer(sheet)).ToBeInViewportAsync();

            // fast drag to minimized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).Not.ToBeInViewportAsync();
            await Expect(BottomSheetLocators.MinimizedMarker(sheet)).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_SlowDragToPosition()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).ToBeInViewportAsync();
            await Expect(BasicSampleLocators.Footer(sheet)).Not.ToBeInViewportAsync();

            // drag up to maximized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(BasicSampleLocators.Footer(sheet)).ToBeInViewportAsync();

            // drag down to minimized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, Page.ViewportSize.Height - 100, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).Not.ToBeInViewportAsync();
            await Expect(BottomSheetLocators.MinimizedMarker(sheet)).ToBeInViewportAsync();

            // drag up to maximized expansion
            await BottomSheetLocators.Handle(sheet).PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await sheet.WhenBoundsStable();
            await Expect(sheetLayout).ToContainClassAsync("maximized");
            await Expect(BasicSampleLocators.Footer(sheet)).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_ScrollableDrag()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();

            // check default expansion (normal)
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");

            var scrollable = BasicSampleLocators.Scrollable(sheet);
            await Expect(scrollable).ToHaveJSPropertyAsync("scrollTop", 0);

            // drag up to maximized expansion with overscroll
            const int Overscroll = 100;
            var sheetBounds = await sheet.BoundingBoxAsync();
            var (lastTouchX, lastTouchY) = await scrollable.TouchStartAsync();
            (lastTouchX, lastTouchY) = await scrollable.TouchMoveAsync(lastTouchX, lastTouchY, 0, (double)-sheetBounds.Y - Overscroll, stepDelayMs: SlowDragStepDelayMs);
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
            await Expect(BasicSampleLocators.Footer(sheet)).ToBeInViewportAsync();
        });
    }
}
