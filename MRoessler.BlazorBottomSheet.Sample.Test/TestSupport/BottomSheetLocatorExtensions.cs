using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BottomSheetLocatorExtensions
{
    public static ILocator GetBottomSheet(this ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet");

    public static ILocator GetMinimizedMarker(this ILocator parentLocator) => parentLocator.GetByTestId("minimized-marker");

    public static ILocator GetNormalMarker(this ILocator parentLocator) => parentLocator.GetByTestId("normal-marker");

    public static ILocator GetHandle(this ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet-handle");

    public static ILocator GetBackground(this ILocator parentLocator) => parentLocator.GetByTestId("bottom-sheet-background");

}
