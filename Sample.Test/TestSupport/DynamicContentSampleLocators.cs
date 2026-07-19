using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class DynamicContentSampleLocators
{
    private const string SheetLayoutTestId = "bottom-sheet-layout";

    public static ILocator SheetLayout(ILocator parent) => parent.GetByTestId(SheetLayoutTestId);

    public static ILocator SheetLayout(IPage parent) => parent.GetByTestId(SheetLayoutTestId);

    public static ILocator OpenCloseButton(IPage page) => page.GetByTestId("open-close-button");

    public static ILocator RevealedContent(ILocator locator) => locator.GetByTestId("revealed-content");

    public static ILocator MainContent(ILocator locator) => locator.GetByTestId("main-content");
}
