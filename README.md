# MRoessler.BlazorBottomSheet
**.NET Blazor BottomSheet implementation for mobile apps**   

[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![build](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![Line of Code](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/blob/build-results/badges/badge_lines_of_code.svg)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![Line Coverage](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/blob/build-results/badges/badge_linecoverage.svg)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/MRoessler.BlazorBottomSheet.svg?style=flat-square&label=NuGet)](https://www.nuget.org/packages/MRoessler.BlazorBottomSheet)

<img src="https://i.imgur.com/VknaVam.png" alt="light-mode" height="400">
<img src="https://i.imgur.com/UzvOXS8.png" alt="dark-mode" height="400">

## Sample App
**[Start the Sample App in your browser](https://markusroessler.github.io/MRoessler.BlazorBottomSheet/)** - *see Sample-projects for source code*  
(!) For the best experience enable device and touch simulation in your browser's debug tools - for example in Firefox:
![browser debug tools](https://i.imgur.com/ViNpl4X.png)


## Getting Started
- Install [nuget package](https://www.nuget.org/packages/MRoessler.BlazorBottomSheet)

- Register BottomSheet services in `Program.cs`
```csharp
using MRoessler.BlazorBottomSheet;
...
builder.Services.AddBottomSheet();
```

- Add `BottomSheetOutlet` to `MainLayout.razor` (your BottomSheets will be rendered at this location)
```xml
@inherits LayoutComponentBase
...
<BottomSheetOutlet />
```

- Add BottomSheet to your Razor Component
```xml
<BottomSheet>
    <div>
        <span>Your content that must be visible in minimized state</span>
        <MinimizedExpansionMarker />

        <span>Your content that must be visible in normal state</span>
        <NormalExpansionMarker />
    </div>
</BottomSheet>
```

- see sample app for more advanced usage examples
- see XML Docs for detailed API description