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
using MRoessler.BlazorBottomSheet.Sample.Components;
using MRoessler.BlazorBottomSheet.Sample.Utils;
using MudBlazor.Services;
using NUnit.Framework.Interfaces;

namespace MRoessler.BlazorBottomSheet.Sample.Test;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BasicSampleTest : PageTest
{
    const int SlowDragStepDelayMs = 100;
    const int FastDragStepDelayMs = 0;

    WebApplicationFactory<Program> _webAppFactory;

    ILocator _bottomSheet;
    ILocator _bottomSheetLayout;
    ILocator _minimizedMarker;
    ILocator _normalMarker;
    ILocator _footer;
    ILocator _handle;
    ILocator _openCloseButton;
    TestHelper _testHelper;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _webAppFactory = new WebApplicationFactory<Program>();
        _webAppFactory.UseKestrel(5001);
        _webAppFactory.StartServer();
        _testHelper = _webAppFactory.Services.GetRequiredService<TestHelper>();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardownAsync()
    {
        await _webAppFactory.DisposeAsync();
    }

    [SetUp]
    public void Setup()
    {
        _bottomSheet = Page.GetByTestId("bottom-sheet");
        _bottomSheetLayout = Page.GetByTestId("bottom-sheet-layout");
        _minimizedMarker = Page.GetByTestId("minimized-marker");
        _normalMarker = Page.GetByTestId("normal-marker");
        _footer = Page.GetByTestId("footer");
        _handle = Page.GetByTestId("bottom-sheet-handle");
        _openCloseButton = Page.GetByTestId("open-close-button");
    }

    [TearDown]
    public async Task TeardownAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            var dir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Screenshots");
            Directory.CreateDirectory(dir);

            var fileName = $"failure_{TestContext.CurrentContext.Test.Name}.png";
            var path = Path.Combine(dir, fileName);
            await Page.ScreenshotAsync(new() { Path = path });
            // TestContext.AddTestAttachment(path);
        }
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return Playwright.Devices[Environment.GetEnvironmentVariable("DEVICE_NAME")];
    }

    private Task<IResponse> GotoBasicSamplePageAsync() => Page.GotoAsync("http://localhost:5001");

    /// <summary>
    /// note: can't use Teardown method for coverage export because it runs too late (Page-Context is gone)
    /// </summary>
    private async Task TestAsync(Func<Task> test)
    {
        try
        {
            await test();
        }
        finally
        {
            await TryExportCoverage();
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "it's a 'try'-method")]
    private async Task TryExportCoverage()
    {
        try
        {
            var coverageJson = await Page.EvaluateAsync<ExpandoObject>("window.__coverage__");
            if (coverageJson != null)
            {
                var outDir = Path.Combine(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", ".."), ".nyc_output");
                Directory.CreateDirectory(outDir);
                File.WriteAllText(Path.Combine(outDir, $"{TestContext.CurrentContext.Test.MethodName}.json"), JsonSerializer.Serialize(coverageJson));
            }
        }
        catch (Exception ex)
        {
            TestContext.Error.WriteLine("Failed to export coverage " + ex);
        }
    }

    [Test]
    public Task Test_SlowDragInDirection()
    {
        return TestAsync(async () =>
        {
            await GotoBasicSamplePageAsync();

            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");

            await _openCloseButton.ClickAsync();

            // check default expansion (normal)
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).ToBeInViewportAsync();
            await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
            await Expect(_normalMarker).ToBeInViewportAsync();
            await Expect(_footer).Not.ToBeInViewportAsync();

            // drag up to maximized expansion
            await _handle.PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
            await Expect(_footer).ToBeInViewportAsync();

            // drag down to normal expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
            await Expect(_footer).Not.ToBeInViewportAsync();
            await Expect(_normalMarker).ToBeInViewportAsync();

            // drag down to minimized expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
            await Expect(_normalMarker).Not.ToBeInViewportAsync();
            await Expect(_minimizedMarker).ToBeInViewportAsync();

            // drag down to closed expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height / 6, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("closed");
            await Expect(_minimizedMarker).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).Not.ToBeInViewportAsync();

        });
    }


    [Test]
    public Task Test_FastDragInDirection()
    {
        return TestAsync(async () =>
        {
            await GotoBasicSamplePageAsync();

            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");

            await _openCloseButton.ClickAsync();

            // check default expansion (normal)
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).ToBeInViewportAsync();
            await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
            await Expect(_normalMarker).ToBeInViewportAsync();
            await Expect(_footer).Not.ToBeInViewportAsync();

            // slow drag to minimized expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height / 4, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
            await Expect(_normalMarker).Not.ToBeInViewportAsync();
            await Expect(_minimizedMarker).ToBeInViewportAsync();

            // fast drag to maximized expansion
            await _handle.PanAsync(0, -Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
            await Expect(_footer).ToBeInViewportAsync();

            // fast drag to minimized expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height / 2, stepDelayMs: FastDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
            await Expect(_normalMarker).Not.ToBeInViewportAsync();
            await Expect(_minimizedMarker).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_SlowDragToPosition()
    {
        return TestAsync(async () =>
        {
            await GotoBasicSamplePageAsync();

            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");

            await _openCloseButton.ClickAsync();

            // check default expansion (normal)
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).ToBeInViewportAsync();
            await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
            await Expect(_normalMarker).ToBeInViewportAsync();
            await Expect(_footer).Not.ToBeInViewportAsync();

            // drag up to maximized expansion
            await _handle.PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
            await Expect(_footer).ToBeInViewportAsync();

            // drag down to minimized expansion
            await _handle.PanAsync(0, Page.ViewportSize.Height - 100, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("minimized");
            await Expect(_normalMarker).Not.ToBeInViewportAsync();
            await Expect(_minimizedMarker).ToBeInViewportAsync();

            // drag up to maximized expansion
            await _handle.PanAsync(0, -Page.ViewportSize.Height, steps: 2, stepDelayMs: SlowDragStepDelayMs);
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheetLayout).ToContainClassAsync("maximized");
            await Expect(_footer).ToBeInViewportAsync();
        });
    }

    [Test]
    public Task Test_ToggleIsOpen()
    {
        return TestAsync(async () =>
        {
            await GotoBasicSamplePageAsync();

            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");

            var samplePageInstanceIdElm = await Page.GetByTestId("sample-instance-id").ElementHandleAsync();
            var samplePageInstanceId = Guid.Parse(await samplePageInstanceIdElm.TextContentAsync());

            var samplePage = _testHelper.ActiveBasicSamplePages[samplePageInstanceId];
            var basicSampleViewModel = samplePage.ViewModel;
            var syncContextDispatcher = samplePage.SyncContextDispatcher;

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(true));
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).ToBeInViewportAsync();
            await Expect(_bottomSheetLayout).ToContainClassAsync("normal");
            await Expect(_normalMarker).ToBeInViewportAsync();
            await Expect(_footer).Not.ToBeInViewportAsync();

            syncContextDispatcher.Invoke(() => basicSampleViewModel.SetIsOpen(false));
            await _bottomSheet.WhenBoundsStable();
            await Expect(_bottomSheet).Not.ToBeInViewportAsync();
            await Expect(_bottomSheet).ToContainClassAsync("closed");
        });
    }
}
