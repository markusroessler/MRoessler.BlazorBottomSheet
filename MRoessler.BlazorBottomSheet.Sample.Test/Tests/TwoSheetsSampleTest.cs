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
    Task<IResponse> GotoSamplePageAsync() => Page.GotoAsync($"{WebAppFactory.ClientOptions.BaseAddress}two-sheets");

    ILocator GetSheetLayout1() => Page.GetByTestId("bottom-sheet-layout-1");
    ILocator GetSheetLayout2() => Page.GetByTestId("bottom-sheet-layout-2");
    ILocator GetOpenCloseButton1() => Page.GetByTestId("open-close-button-1");
    ILocator GetOpenCloseButton2() => Page.GetByTestId("open-close-button-2");


    [Test]
    public Task Test_Open()
    {
        return TestAsync(async () =>
        {
            await GotoSamplePageAsync();

            var sheetLayout1 = GetSheetLayout1();
            var sheetLayout2 = GetSheetLayout2();
            var sheet1 = sheetLayout1.BottomSheet();
            var sheet2 = sheetLayout2.BottomSheet();

            await Expect(sheet1).Not.ToBeInViewportAsync();
            await Expect(sheet1).ToContainClassAsync("closed");

            await GetOpenCloseButton1().ClickAsync();

            // open and close first sheet
            await sheet1.WhenBoundsStable();
            await Expect(sheet1).ToBeInViewportAsync();
            await Expect(sheetLayout1).ToContainClassAsync("minimized");
            await Expect(sheet1.MinimizedMarker()).ToBeInViewportAsync();
            await Expect(sheet1.NormalMarker()).Not.ToBeInViewportAsync();

            await sheetLayout1.Background().ClickAsync();
            await sheet1.WhenBoundsStable();
            await Expect(sheet1).Not.ToBeInViewportAsync();
            await Expect(sheet1).ToContainClassAsync("closed");

            await GetOpenCloseButton2().ClickAsync();

            // open and close second sheet
            await sheet2.WhenBoundsStable();
            await Expect(sheet2).ToBeInViewportAsync();
            await Expect(sheetLayout2).ToContainClassAsync("minimized");
            await Expect(sheet2.MinimizedMarker()).ToBeInViewportAsync();
            await Expect(sheet2.NormalMarker()).Not.ToBeInViewportAsync();

            await sheetLayout2.Background().ClickAsync();
            await sheet2.WhenBoundsStable();
            await Expect(sheet2).Not.ToBeInViewportAsync();
            await Expect(sheet2).ToContainClassAsync("closed");
        });
    }
}
