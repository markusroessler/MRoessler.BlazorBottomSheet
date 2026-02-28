using MRoessler.BlazorBottomSheet;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Pages;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.ViewModels;
using MRoessler.BlazorBottomSheet.Sample.WebApp.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services
    .AddSampleAppServices(builder.Configuration);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(BasicSample).Assembly);

app.Run();
