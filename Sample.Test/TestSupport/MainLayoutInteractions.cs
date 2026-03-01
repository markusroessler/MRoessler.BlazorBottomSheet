using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class MainLayoutInteractions
{
    public static async Task SelectAutoModeAsync(IPage page)
    {
        await MainLayoutLocators.ThemeMenu(page).ClickAsync();
        await MainLayoutLocators.AutoThemeMenuItem(page).ClickAsync();
    }

    public static async Task SelectLightModeAsync(IPage page)
    {
        await MainLayoutLocators.ThemeMenu(page).ClickAsync();
        await MainLayoutLocators.LightThemeMenuItem(page).ClickAsync();
    }

    public static async Task SelectDarkModeAsync(IPage page)
    {
        await MainLayoutLocators.ThemeMenu(page).ClickAsync();
        await MainLayoutLocators.DarkThemeMenuItem(page).ClickAsync();
    }
}
