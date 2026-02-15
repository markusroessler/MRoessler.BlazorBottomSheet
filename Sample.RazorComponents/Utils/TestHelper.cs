using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MRoessler.BlazorBottomSheet.Sample.RazorComponents.Pages;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;

public class TestHelper
{
    public ConcurrentDictionary<Guid, BasicSample> ActiveBasicSamplePages { get; } = [];
}
