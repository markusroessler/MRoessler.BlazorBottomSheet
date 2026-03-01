using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class MainLayoutInteractions
{
    public static async Task SelectDarkModeAsync(IPage page)
    {
        await MainLayoutLocators.ThemeMenu(page).ClickAsync();
        await MainLayoutLocators.DarkThemeMenuItem(page).ClickAsync();
    }
}
