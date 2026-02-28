using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.ViewModels;
using MudBlazor.Services;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSampleAppServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddMudServices()
            .AddBottomSheet(new() { UseMinifiedJavaScripts = config.GetValue("SampleApp:UseMinifiedJavaScripts", true) })
            .AddScoped<SynchronizationContextDispatcher>()
            .AddScoped<BasicSampleViewModel>()
            .AddSingleton<TestHelper>();
        return services;
    }
}
