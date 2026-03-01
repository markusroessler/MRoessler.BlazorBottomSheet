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

    public static ILocator Footer(ILocator parent) => parent.GetByTestId("footer");

    public static ILocator Scrollable(ILocator parent) => parent.GetByTestId("scrollable");

    public static ILocator OpenCloseButton(IPage page) => page.GetByTestId("open-close-button");
}
