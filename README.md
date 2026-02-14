# MRoessler.BlazorBottomSheet
**✨ .NET Blazor BottomSheet for mobile apps ✨**   

[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![build](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![Line of Code](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/blob/build-results/badges/badge_lines_of_code.svg)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![Line Coverage](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/blob/build-results/badges/badge_linecoverage.svg)](https://github.com/markusroessler/MRoessler.BlazorBottomSheet/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/MRoessler.BlazorBottomSheet.svg?style=flat-square&label=NuGet)](https://www.nuget.org/packages/MRoessler.BlazorBottomSheet)

|   **Light Theme**  |  **Dark Theme**    |
| --- | --- |
|  <img src="https://i.imgur.com/EbHxylH.png" alt="light-mode" height="400">   |   <img src="https://i.imgur.com/mguiU46.png" alt="dark-mode" height="400">  |

## Features
- Dragging is recognized on the complete sheet (not just the drag handle)
- Dynamically toggles between scrolling and dragging when touching scrollable areas
- Optimized for touch input *(mouse works too but not as fluent as touch input)*
- Supports Server and WebAssembly render modes 
- Customizable via CSS/JS
- No dependencies beside `Microsoft.AspNetCore.Components.Web`

## Sample App
**[🔥 Start the Sample App in your browser](https://markusroessler.github.io/MRoessler.BlazorBottomSheet/)**  
💡 For the best experience enable device and touch simulation in your browser's debug tools - Firefox example:
![browser debug tools](https://i.imgur.com/ViNpl4X.png)
*see Sample-projects for source code*  

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

- See sample app for more advanced usage examples
- See XML Docs for detailed API description