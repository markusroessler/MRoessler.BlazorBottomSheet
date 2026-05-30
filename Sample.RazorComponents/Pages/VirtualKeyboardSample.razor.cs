using System.Globalization;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Pages;

public partial class VirtualKeyboardSample
{
    private bool _isOpen;
    private string _input = "";

    private void ToggleButtonSheetOpen() => _isOpen = !_isOpen;
}
