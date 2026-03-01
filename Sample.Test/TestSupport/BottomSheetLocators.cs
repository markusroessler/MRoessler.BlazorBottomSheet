using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BottomSheetLocators
{
    public static ILocator HandleRect(ILocator parent) => parent.GetByTestId("bottom-sheet-handle-rect");

    public static ILocator BackgroundOverlay(ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet-background");

    public static ILocator Sheet(ILocator parent) => parent.GetByTestId("bottom-sheet");
}
