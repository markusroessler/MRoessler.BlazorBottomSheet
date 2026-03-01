using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BasicSampleLocators
{
    public static ILocator MudBlazorStylingChip(IPage page) => page.GetByTestId("mud-blazor-styling-chip");

    public static ILocator CloseSheetButton(ILocator parent) => parent.GetByTestId("close-sheet-button");
}
