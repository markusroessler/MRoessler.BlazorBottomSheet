using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class MainLayoutLocators
{
    public static ILocator ThemeMenu(IPage page) => page.GetByTestId("theme-menu");

    public static ILocator LightThemeMenuItem(IPage page) => page.GetByTestId("light-theme-menuitem");

    public static ILocator DarkThemeMenuItem(IPage page) => page.GetByTestId("dark-theme-menuitem");
}
