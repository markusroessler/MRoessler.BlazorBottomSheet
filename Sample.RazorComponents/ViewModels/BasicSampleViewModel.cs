using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.ViewModels;

public sealed class BasicSampleViewModel
{
    public BasicSampleState State
    {
        get => field;
        private set
        {
            field = value;
            StateChanged?.Invoke();
        }
    } = new();

    public event Action? StateChanged;

    public void SetIsOpen(bool isOpen) => State = State with { IsOpen = isOpen };
}

public record BasicSampleState(bool IsOpen = false);
