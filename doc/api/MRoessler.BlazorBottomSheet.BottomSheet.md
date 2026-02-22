### [MRoessler\.BlazorBottomSheet](MRoessler.BlazorBottomSheet.md 'MRoessler\.BlazorBottomSheet')

## BottomSheet Class

A BottomSheet that displays its contents in a [BottomSheetOutlet](MRoessler.BlazorBottomSheet.BottomSheetOutlet.md 'MRoessler\.BlazorBottomSheet\.BottomSheetOutlet')

```csharp
public class BottomSheet : Microsoft.AspNetCore.Components.ComponentBase, System.IAsyncDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Microsoft\.AspNetCore\.Components\.ComponentBase](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.componentbase 'Microsoft\.AspNetCore\.Components\.ComponentBase') &#129106; BottomSheet

Implements [System\.IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable 'System\.IAsyncDisposable')

| Properties | |
| :--- | :--- |
| [AdditionalAttributes](MRoessler.BlazorBottomSheet.BottomSheet.AdditionalAttributes.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AdditionalAttributes') | |
| [AllowClosedExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowClosedExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowClosedExpansion') | |
| [AllowMaximizedExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowMaximizedExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowMaximizedExpansion') | |
| [AllowMinimizedExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowMinimizedExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowMinimizedExpansion') | |
| [AllowNormalExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowNormalExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowNormalExpansion') | |
| [BackgroundClass](MRoessler.BlazorBottomSheet.BottomSheet.BackgroundClass.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.BackgroundClass') | style class to apply on the BottomSheet background \(visible when [IsModal](MRoessler.BlazorBottomSheet.BottomSheet.IsModal.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsModal') is true\) |
| [ChildContent](MRoessler.BlazorBottomSheet.BottomSheet.ChildContent.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.ChildContent') | The BottomSheet's content\. Place [MinimizedExpansionMarker](MRoessler.BlazorBottomSheet.MinimizedExpansionMarker.md 'MRoessler\.BlazorBottomSheet\.MinimizedExpansionMarker') and [NormalExpansionMarker](MRoessler.BlazorBottomSheet.NormalExpansionMarker.md 'MRoessler\.BlazorBottomSheet\.NormalExpansionMarker') within this content for automatic height computation |
| [Class](MRoessler.BlazorBottomSheet.BottomSheet.Class.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Class') | style class to apply on the BottomSheet element |
| [CloseOnBackgroundClick](MRoessler.BlazorBottomSheet.BottomSheet.CloseOnBackgroundClick.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.CloseOnBackgroundClick') | whether to close the sheet when the background overlay is clicked |
| [DefaultExpansion](MRoessler.BlazorBottomSheet.BottomSheet.DefaultExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DefaultExpansion') | [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') to be applied when [IsOpen](MRoessler.BlazorBottomSheet.BottomSheet.IsOpen.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsOpen') is set to true |
| [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') | the expansion state to apply |
| [ExpansionChanged](MRoessler.BlazorBottomSheet.BottomSheet.ExpansionChanged.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.ExpansionChanged') | |
| [Handle](MRoessler.BlazorBottomSheet.BottomSheet.Handle.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Handle') | Use this to overwrite the default drag handle |
| [IsModal](MRoessler.BlazorBottomSheet.BottomSheet.IsModal.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsModal') | whether to prevent interaction with the overlayed content |
| [IsOpen](MRoessler.BlazorBottomSheet.BottomSheet.IsOpen.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsOpen') | Alternative to [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') for easier ViewModel bindings\. When set to true [DefaultExpansion](MRoessler.BlazorBottomSheet.BottomSheet.DefaultExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DefaultExpansion') will be applied\. |
| [IsOpenChanged](MRoessler.BlazorBottomSheet.BottomSheet.IsOpenChanged.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsOpenChanged') | |
| [IsVisible](MRoessler.BlazorBottomSheet.BottomSheet.IsVisible.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsVisible') | |
| [JavaScriptObjRef](MRoessler.BlazorBottomSheet.BottomSheet.JavaScriptObjRef.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.JavaScriptObjRef') | Reference to the javascript object\. You may use this to add event listeners\. |

| Methods | |
| :--- | :--- |
| [BuildRenderTree\(RenderTreeBuilder\)](MRoessler.BlazorBottomSheet.BottomSheet.BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder).md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.BuildRenderTree\(Microsoft\.AspNetCore\.Components\.Rendering\.RenderTreeBuilder\)') | |
| [DisposeAsync\(\)](MRoessler.BlazorBottomSheet.BottomSheet.DisposeAsync().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DisposeAsync\(\)') | |
| [DisposeAsyncCore\(\)](MRoessler.BlazorBottomSheet.BottomSheet.DisposeAsyncCore().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DisposeAsyncCore\(\)') | |
| [OnAfterRenderAsync\(bool\)](MRoessler.BlazorBottomSheet.BottomSheet.OnAfterRenderAsync(bool).md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.OnAfterRenderAsync\(bool\)') | |
| [OnInitialized\(\)](MRoessler.BlazorBottomSheet.BottomSheet.OnInitialized().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.OnInitialized\(\)') | |
| [OnParametersSetAsync\(\)](MRoessler.BlazorBottomSheet.BottomSheet.OnParametersSetAsync().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.OnParametersSetAsync\(\)') | |
| [SetExpansionAsync\(int\)](MRoessler.BlazorBottomSheet.BottomSheet.SetExpansionAsync(int).md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.SetExpansionAsync\(int\)') | INTERNAL API |
| [WhenRenderedOnce\(\)](MRoessler.BlazorBottomSheet.BottomSheet.WhenRenderedOnce().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.WhenRenderedOnce\(\)') | Use this to wait for the sheet render at least once\. [JavaScriptObjRef](MRoessler.BlazorBottomSheet.BottomSheet.JavaScriptObjRef.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.JavaScriptObjRef') should be set after the first render\. |
