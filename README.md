# MRoessler.BlazorBottomSheet
**.NET Blazor BottomSheet implementation for mobile apps** 
   
<img src="https://i.imgur.com/RZPqpeT.png" alt="screenshot-1" height="400">


## Getting Started
- Install nuget package (TODO)

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