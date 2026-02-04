using MRoessler.BlazorBottomSheet;
using MRoessler.BlazorBottomSheet.Sample.Components;
using MRoessler.BlazorBottomSheet.Sample.Utils;
using MRoessler.BlazorBottomSheet.Sample.ViewModels;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services
    .AddMudServices()
    .AddBottomSheet();

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddScoped<SynchronizationContextDispatcher>()
    .AddScoped<BasicSampleViewModel>()
    .AddSingleton<TestHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
