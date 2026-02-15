using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "created by DI container")]
internal sealed class BottomSheetOutletState : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly HashSet<Guid> _sectionContentIds = [];
    public IReadOnlySet<Guid> SectionContentIds => _sectionContentIds;

    public event Action<Guid>? OnSectionContentIdAdded;
    public event Action<Guid>? OnSectionContentIdRemoved;

    public BottomSheetColorScheme ColorScheme
    {
        get => field;
        set
        {
            if (field != value)
            {
                field = value;
                PropertyChanged?.Invoke(this, new(nameof(ColorScheme)));
            }
        }
    }

    public void RegisterSectionContentId(Guid id)
    {
        if (_sectionContentIds.Add(id))
            OnSectionContentIdAdded?.Invoke(id);
    }

    public void DeregisterSectionContentId(Guid id)
    {
        if (_sectionContentIds.Remove(id))
            OnSectionContentIdRemoved?.Invoke(id);
    }
}
