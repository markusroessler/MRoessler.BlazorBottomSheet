using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BasicSampleInteractions
{
    public static async Task ToggleMudBlazorStylingChipSelectionAsync(IPage page, bool select)
    {
        var chip = BasicSampleLocators.MudBlazorStylingChip(page);
        var cssClass = await chip.GetAttributeAsync("class");
        if (select && !cssClass.Contains("mud-chip-selected")
            || !select && cssClass.Contains("mud-chip-selected"))
            await chip.ClickAsync();
    }
}
