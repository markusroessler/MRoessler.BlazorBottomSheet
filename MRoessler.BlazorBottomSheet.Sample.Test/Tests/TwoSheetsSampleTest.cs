using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

namespace MRoessler.BlazorBottomSheet.Sample.Test.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class TwoSheetsSampleTest : CustomPageTest
{

    ILocator _openCloseButton1;
    ILocator _openCloseButton2;

    ILocator _bottomSheetLayout1;
    ILocator _bottomSheetLayout2;

    ILocator _bottomSheet1;
    ILocator _bottomSheet2;

    ILocator _minimizedMarker1;
    ILocator _minimizedMarker2;
    ILocator _normalMarker1;
    ILocator _normalMarker2;

    [SetUp]
    public void Setup()
    {
        _openCloseButton1 = Page.GetByTestId("open-close-button-1");
        _openCloseButton2 = Page.GetByTestId("open-close-button-2");

        _bottomSheetLayout1 = Page.GetByTestId("bottom-sheet-layout-1");
        _bottomSheetLayout2 = Page.GetByTestId("bottom-sheet-layout-2");

        _bottomSheet1 = _bottomSheetLayout1.GetByTestId("bottom-sheet");
        _bottomSheet2 = _bottomSheetLayout2.GetByTestId("bottom-sheet");

        _minimizedMarker1 = _bottomSheetLayout1.GetByTestId("minimized-marker");
        _minimizedMarker2 = _bottomSheetLayout2.GetByTestId("minimized-marker");
        _normalMarker1 = _bottomSheetLayout1.GetByTestId("normal-marker");
        _normalMarker2 = _bottomSheetLayout2.GetByTestId("normal-marker");
    }

    private Task<IResponse> GotoSamplePageAsync() => Page.GotoAsync($"{WebAppFactory.ClientOptions.BaseAddress}two-sheets");

    [Test]
    public Task Test_Open()
    {
        return TestAsync(async () =>
        {
            await GotoSamplePageAsync();

            await Expect(_bottomSheet1).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet1).ToContainClassAsync("closed");

            await _openCloseButton1.ClickAsync();

            await _bottomSheet1.WhenBoundsStable();
            await Expect(_bottomSheet1).ToBeInViewportAsync();
            await Expect(_bottomSheetLayout1).ToContainClassAsync("minimized");
            await Expect(_minimizedMarker1).ToBeInViewportAsync();
            await Expect(_normalMarker1).Not.ToBeInViewportAsync();
        });
    }
}
