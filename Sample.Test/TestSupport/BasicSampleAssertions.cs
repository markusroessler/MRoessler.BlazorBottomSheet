using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BasicSampleAssertions
{
    public static async Task ExpectLightColorSchemeAsync(ILocator sheetLayout)
    {
        await Assertions.Expect(BottomSheetLocators.BottomSheet(sheetLayout)).ToHaveCSSAsync("background-color", "rgb(238, 238, 238)");
        await Assertions.Expect(BottomSheetLocators.HandleRect(sheetLayout)).ToHaveCSSAsync("fill", "rgb(97, 97, 97)");
        await Assertions.Expect(BottomSheetLocators.BackgroundOverlay(sheetLayout)).ToHaveCSSAsync("background-color", "rgb(15, 15, 15)");
    }

    public static async Task ExpectDarkColorSchemeAsync(ILocator sheetLayout)
    {
        await Assertions.Expect(BottomSheetLocators.BottomSheet(sheetLayout)).ToHaveCSSAsync("background-color", "rgb(15, 15, 15)");
        await Assertions.Expect(BottomSheetLocators.HandleRect(sheetLayout)).ToHaveCSSAsync("fill", "rgb(189, 189, 189)");
        await Assertions.Expect(BottomSheetLocators.BackgroundOverlay(sheetLayout)).ToHaveCSSAsync("background-color", "rgb(66, 66, 66)");
    }
}
