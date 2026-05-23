using System.Globalization;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Pages;

public partial class TimetrackingFilterSample
{
    private bool _isOpen;

    private IReadOnlyList<int> _years = [];
    private int _year = 2026;

    private IReadOnlyList<string> _months = [];
    private string _month = "";

    private bool _overdue;

    private IReadOnlyCollection<string> _selectedChips = [];


    private void ToggleButtonSheetOpen() => _isOpen = !_isOpen;

    private Task<IEnumerable<int>> SearchYearAsync(string value, CancellationToken cancellationToken)
    {
        return Task.FromResult(_years.Where(y => y.ToString(CultureInfo.InvariantCulture).Contains(value)));
    }

    private Task<IEnumerable<string>> SearchMonthAsync(string value, CancellationToken cancellationToken)
    {
        return Task.FromResult(_months.Where(m => m.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }

    private async Task ApplyAsync()
    {

    }

    private async Task ResetAsync()
    {

    }
}
