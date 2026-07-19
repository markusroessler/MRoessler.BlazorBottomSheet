using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BasicSampleLocators
{
    private const string SheetLayoutTestId = "bottom-sheet-layout";

    public static ILocator SheetLayout(ILocator parent) => parent.GetByTestId(SheetLayoutTestId);

    public static ILocator SheetLayout(IPage parent) => parent.GetByTestId(SheetLayoutTestId);

    public static ILocator MudBlazorStylingChip(IPage page) => page.GetByTestId("mud-blazor-styling-chip");

    public static ILocator CloseSheetButton(ILocator parent) => parent.GetByTestId("close-sheet-button");

    public static ILocator Footer(ILocator parent) => parent.GetByTestId("footer");

    public static ILocator Scrollable(ILocator parent) => parent.GetByTestId("scrollable");

    public static ILocator OpenCloseButton(IPage page) => page.GetByTestId("open-close-button");

    public static ILocator ToggleDynamicContentButton(IPage page) => page.GetByTestId("toggle-dyn-content-button");

    public static ILocator HeaderText(ILocator parent) => parent.GetByTestId("sample-bs-header-text");

    public static ILocator ScrollableAreaHeaderText(ILocator parent) => parent.GetByTestId("scrollable-area-header-text");
}
