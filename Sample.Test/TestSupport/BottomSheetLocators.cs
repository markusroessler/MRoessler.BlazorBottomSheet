using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BottomSheetLocators
{
    private const string SheetLayoutTestId = "bottom-sheet-layout";

    public static ILocator SheetLayout(ILocator parent) => parent.GetByTestId(SheetLayoutTestId);

    public static ILocator SheetLayout(IPage parent) => parent.GetByTestId(SheetLayoutTestId);

    public static ILocator BottomSheet(ILocator parent) => parent.GetByTestId("bottom-sheet");

    public static ILocator Handle(ILocator parent) => parent.GetByTestId("bottom-sheet-handle");

    public static ILocator HandleRect(ILocator parent) => parent.GetByTestId("bottom-sheet-handle-rect");

    public static ILocator BackgroundOverlay(ILocator parent) => parent.GetByTestId("bottom-sheet-background");

    public static ILocator MinimizedMarker(ILocator parent) => parent.GetByTestId("minimized-marker");

    public static ILocator NormalMarker(ILocator parent) => parent.GetByTestId("normal-marker");
}
