using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Microsoft.Playwright.TestAdapter;
using MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;
using NUnit.Framework.Interfaces;

namespace MRoessler.BlazorBottomSheet.Sample.Test.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class BasicSampleTest_Common : CustomPageTest
{
    private Task<IResponse> GotoBasicSamplePageAsync() => Page.GotoAsync(WebAppFactory.ClientOptions.BaseAddress.ToString());

    [Test]
    public Task Test_ToggleIsOpen()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            var samplePageInstanceIdElm = await Page.GetByTestId("sample-instance-id").ElementHandleAsync();
            var samplePageInstanceId = Guid.Parse(await samplePageInstanceIdElm.TextContentAsync());

            var samplePage = TestHelper.ActiveBasicSamplePages[samplePageInstanceId];
            var basicSampleViewModel = samplePage.ViewModel;
            var syncContextDispatcher = samplePage.SyncContextDispatcher;

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(true));
            await sheet.WhenBoundsStable();
            await Expect(sheet).ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(BottomSheetLocators.NormalMarker(sheet)).ToBeInViewportAsync();
            await Expect(BasicSampleLocators.Footer(sheet)).Not.ToBeInViewportAsync();

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(false));
            await sheet.WhenBoundsStable();
            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");
        });
    }

    [Test]
    public Task Test_ColorScheme_Manual()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);
            var handleRect = BottomSheetLocators.HandleRect(sheet);
            var backgroundOverlay = BottomSheetLocators.BackgroundOverlay(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await BasicSampleInteractions.ToggleMudBlazorStylingChipSelectionAsync(Page, false);
            await MainLayoutInteractions.SelectLightModeAsync(Page);

            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();

            await Expect(sheetLayout).ToContainClassAsync("normal");
            await BasicSampleAssertions.ExpectLightColorSchemeAsync(sheetLayout);

            await BasicSampleLocators.CloseSheetButton(sheet).ClickAsync();
            await MainLayoutInteractions.SelectDarkModeAsync(Page);
            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();
            await BasicSampleAssertions.ExpectDarkColorSchemeAsync(sheetLayout);
        });
    }

    [Test]
    public Task Test_ColorScheme_SystemTheme()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = BottomSheetLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);
            var handleRect = BottomSheetLocators.HandleRect(sheet);
            var backgroundOverlay = BottomSheetLocators.BackgroundOverlay(sheetLayout);

            await GotoBasicSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheet).ToContainClassAsync("closed");

            await BasicSampleInteractions.ToggleMudBlazorStylingChipSelectionAsync(Page, false);
            await MainLayoutInteractions.SelectAutoModeAsync(Page);

            await BasicSampleLocators.OpenCloseButton(Page).ClickAsync();
            await Expect(sheetLayout).ToContainClassAsync("normal");

            await Page.EmulateMediaAsync(new() { ColorScheme = ColorScheme.Light });
            await BasicSampleAssertions.ExpectLightColorSchemeAsync(sheetLayout);

            await Page.EmulateMediaAsync(new() { ColorScheme = ColorScheme.Dark });
            await BasicSampleAssertions.ExpectDarkColorSchemeAsync(sheetLayout);
        });
    }
}
