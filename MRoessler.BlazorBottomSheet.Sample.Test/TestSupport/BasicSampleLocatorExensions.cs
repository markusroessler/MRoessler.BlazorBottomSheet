using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class BasicSampleLocatorExensions
{
    public static ILocator Footer(this ILocator parentLocator) => parentLocator.GetByTestId("footer");
}
