using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;
using MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

namespace MRoessler.BlazorBottomSheet.Sample.Test.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class DynamicContentSampleTest : CustomPageTest
{
    Task<IResponse> GotoSamplePageAsync() => Page.GotoAsync($"{WebAppFactory.ClientOptions.BaseAddress}dynamic-content");


    [Test]
    public Task Test_DynamicContent()
    {
        return TestAsync(async () =>
        {
            var sheetLayout = DynamicContentSampleLocators.SheetLayout(Page);
            var sheet = BottomSheetLocators.BottomSheet(sheetLayout);
            var revealedContent = DynamicContentSampleLocators.RevealedContent(sheet);
            var mainContent = DynamicContentSampleLocators.MainContent(sheet);
            var bsHandle = BottomSheetLocators.Handle(sheet);

            await GotoSamplePageAsync();

            await Expect(sheet).Not.ToBeInViewportAsync();
            await Expect(sheetLayout).ToContainClassAsync("closed");

            await DynamicContentSampleLocators.OpenCloseButton(Page).ClickAsync();
            await sheet.ExpectBoundsToBeStable();
            await Expect(sheetLayout).ToContainClassAsync("minimized");
            await Expect(revealedContent).ToHaveCSSAsync("opacity", "0");
            {
                var revealedContentBounds = await revealedContent.BoundingBoxAsync();
                await mainContent.ExpectClientYToBeInRangeAsync(revealedContentBounds.Y - 1, revealedContentBounds.Y + 1);
            }

            // drag up to normal expansion
            await bsHandle.PanAsync(0, -Page.ViewportSize.Height / 12, stepDelayMs: 0, steps: 100, useMouseEvents: !ContextOptions().IsMobile.GetValueOrDefault());
            await sheet.ExpectBoundsToBeStable();
            await Expect(sheetLayout).ToContainClassAsync("normal");
            await Expect(revealedContent).ToHaveCSSAsync("opacity", "1");
            {
                var revealedContentBounds = await revealedContent.BoundingBoxAsync();
                var mainContentExpectedY = revealedContentBounds.Y + revealedContentBounds.Height;
                await mainContent.ExpectClientYToBeInRangeAsync(mainContentExpectedY - 1, mainContentExpectedY + 1);
            }
        });
    }
}
