using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

// TODO refactor to static methods like MainLayoutLocators (ILocator extensions could lead to conflicts)
public static class BottomSheetLocatorExtensions
{
    public static ILocator BottomSheet(this ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet");

    public static ILocator MinimizedMarker(this ILocator parentLocator) => parentLocator.GetByTestId("minimized-marker");

    public static ILocator NormalMarker(this ILocator parentLocator) => parentLocator.GetByTestId("normal-marker");

    public static ILocator Handle(this ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet-handle");

    public static ILocator Background(this ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet-background");

}
