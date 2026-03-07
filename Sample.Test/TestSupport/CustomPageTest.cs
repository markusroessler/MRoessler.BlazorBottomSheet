using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;
using NUnit.Framework.Interfaces;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public abstract class CustomPageTest : PageTest
{
    protected WebApplicationFactory<Program> WebAppFactory { get; private set; }

    protected TestHelper TestHelper { get; private set; }

    [OneTimeSetUp]
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
    public void OneTimeSetup()
    {
        WebAppFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Parse("127.0.0.1"), 0);
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddFakeLogging();
                    });
                });
            });
        WebAppFactory.UseKestrel(0);
        WebAppFactory.StartServer();
        TestHelper = WebAppFactory.Services.GetRequiredService<TestHelper>();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardownAsync()
    {
        await WebAppFactory.DisposeAsync();
    }

    [SetUp]
    public void Setup()
    {
        var logger = WebAppFactory.Services.GetRequiredService<ILogger<CustomPageTest>>();
        Page.Console += (_, msg) =>
        {
            logger.LogInformation("JS: [{MsgType}] {MsgText} ({MsgLocation})",
             msg.Type, msg.Text, new Uri(msg.Location).Segments.Last());
        };
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

        var fakeLogCollector = WebAppFactory.Services.GetFakeLogCollector();
        var logs = fakeLogCollector.GetSnapshot(clear: true).Select(r => r.ToString());
        TestContext.Out.WriteLine($"--- .NET Logs ---\n {string.Join("\n", logs)}");
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return Playwright.Devices[Environment.GetEnvironmentVariable("DEVICE_NAME")];
    }

    /// <summary>
    /// note: can't use Teardown method for coverage export because it runs too late (Page-Context is gone)
    /// </summary>
    protected async Task TestAsync(Func<Task> test)
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
}
