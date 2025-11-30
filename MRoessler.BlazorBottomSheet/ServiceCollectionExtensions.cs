using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MRoessler.BlazorBottomSheet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBottomSheet(this IServiceCollection services)
    {
        return services.AddScoped<BottomSheetOutletState>();
    }
}
