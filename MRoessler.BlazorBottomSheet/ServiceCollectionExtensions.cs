using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MRoessler.BlazorBottomSheet;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the BottomSheet services
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="config">Optional configuration</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddBottomSheet(this IServiceCollection services, BottomSheetServicesConfiguration? config = null)
    {
        return services
            .AddSingleton(config ?? new())
            .AddScoped<BottomSheetOutletState>();
    }
}
