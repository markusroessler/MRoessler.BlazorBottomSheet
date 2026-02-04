# MRoessler.BlazorBottomSheet
**.NET Blazor BottomSheet implementation for mobile apps**   

[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![build](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![Line Coverage](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/blob/badges/badge_linecoverage.svg)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![Branch Coverage](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/blob/badges/badge_branchcoverage.svg)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/MRoessler.BlazorBottomSheet.svg?style=flat-square&label=NuGet)](https://www.nuget.org/packages/MRoessler.BlazorBottomSheet)

<img src="https://i.imgur.com/RZPqpeT.png" alt="screenshot-1" height="400">


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

- see `MRoessler.BlazorBottomSheet.Sample` app for more advanced usage examples
- see XML Docs for detailed API description