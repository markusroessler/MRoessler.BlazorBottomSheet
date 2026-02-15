using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Layout;

public partial class MainLayout
{
    public const string AppBarHeaderName = "AppBarHeader";

    private const string AutoThemeIcon = Icons.Material.Filled.AutoMode;
    private const string LightThemeIcon = Icons.Material.Filled.LightMode;
    private const string DarkThemeIcon = Icons.Material.Filled.DarkMode;

    private MudThemeProvider? _themeProvider;
    private readonly MudTheme _theme;
    private bool _drawerOpen = true;

    private bool _isDarkMode = true;
    private bool _isSystemDarkMode = true;

    private bool _isVisible;

    private string _currentThemeIcon = AutoThemeIcon;

    private readonly PaletteLight _lightPalette = new()
    {
        Black = "#110e2d",
        AppbarText = "#424242",
        AppbarBackground = "rgba(255,255,255,0.8)",
        DrawerBackground = "#ffffff",
        GrayLight = "#e8e8e8",
        GrayLighter = "#f9f9f9",
    };

    private readonly PaletteDark _darkPalette = new()
    {
        Primary = "#7e6fff",
        Surface = "#1e1e2d",
        Background = "#1a1a27",
        BackgroundGray = "#151521",
        AppbarText = "#92929f",
        AppbarBackground = "rgba(26,26,39,0.8)",
        DrawerBackground = "#1a1a27",
        ActionDefault = "#74718e",
        ActionDisabled = "#9999994d",
        ActionDisabledBackground = "#605f6d4d",
        TextPrimary = "#b2b0bf",
        TextSecondary = "#92929f",
        TextDisabled = "#ffffff33",
        DrawerIcon = "#92929f",
        DrawerText = "#92929f",
        GrayLight = "#2a2833",
        GrayLighter = "#1e1e2d",
        Info = "#4a86ff",
        Success = "#3dcb6c",
        Warning = "#ffb545",
        Error = "#ff3f5f",
        LinesDefault = "#33323e",
        TableLines = "#33323e",
        Divider = "#292838",
        OverlayLight = "#1e1e2d80",
    };

    public MainLayout()
    {
        _theme = new()
        {
            PaletteLight = _lightPalette,
            PaletteDark = _darkPalette,
            LayoutProperties = new LayoutProperties(),
            ZIndex =
            {
                AppBar = 1300,
                Popover = 1500,
            }
        };
    }

    private void SetCurrentThemeIcon(string icon)
    {
        _currentThemeIcon = icon;
        UpdateIsDarkMode();
    }

    private void UpdateIsDarkMode()
    {
        if (_currentThemeIcon == LightThemeIcon)
            _isDarkMode = false;

        else if (_currentThemeIcon == DarkThemeIcon)
            _isDarkMode = true;

        else if (_themeProvider != null)
            _isDarkMode = _isSystemDarkMode;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && _themeProvider != null)
        {
            await _themeProvider.WatchSystemDarkModeAsync(OnSystemDarkModeChangedAsync);
            SetCurrentThemeIcon(AutoThemeIcon);
            _isVisible = true;
            StateHasChanged();
        }
    }

    private Task OnSystemDarkModeChangedAsync(bool isDark)
    {
        _isSystemDarkMode = isDark;
        UpdateIsDarkMode();
        StateHasChanged();
        return Task.CompletedTask;
    }

    private BottomSheetColorScheme GetColorScheme()
    {
        return _currentThemeIcon switch
        {
            AutoThemeIcon => BottomSheetColorScheme.Auto,
            LightThemeIcon => BottomSheetColorScheme.Light,
            DarkThemeIcon => BottomSheetColorScheme.Dark,
            _ => throw new NotImplementedException(),
        };
    }

    private void DrawerToggle() => _drawerOpen = !_drawerOpen;
}
