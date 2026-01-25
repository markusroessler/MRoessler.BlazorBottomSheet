using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright.NUnit;
using MRoessler.BlazorBottomSheet.Sample.Components;
using MudBlazor.Services;

namespace MRoessler.BlazorBottomSheet.Sample.Test;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    WebApplicationFactory<Program> _webAppFactory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _webAppFactory = new WebApplicationFactory<Program>();
        _webAppFactory.UseKestrel(5001);
        _webAppFactory.StartServer();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardownAsync()
    {
        await _webAppFactory.DisposeAsync();
    }

    [Test]
    public async Task HasTitle()
    {
        await Page.GotoAsync("http://localhost:5001");

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Home"));
    }
}
