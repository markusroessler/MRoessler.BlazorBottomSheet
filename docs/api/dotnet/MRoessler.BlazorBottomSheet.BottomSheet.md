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
| [AdditionalAttributes](MRoessler.BlazorBottomSheet.BottomSheet.AdditionalAttributes.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AdditionalAttributes') | Additional HTML attributes to render \(see https://learn\.microsoft\.com/en\-us/aspnet/core/blazor/components/splat\-attributes\-and\-arbitrary\-parameters?view=aspnetcore\-10\.0\#arbitrary\-attributes\) |
| [AllowClosedExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowClosedExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowClosedExpansion') | Whether to allow the user to close the sheet by dragging it \(Default: true\) |
| [AllowMaximizedExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowMaximizedExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowMaximizedExpansion') | Whether to allow the user to maximize the sheet by dragging it \(Default: false\) |
| [AllowMinimizedExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowMinimizedExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowMinimizedExpansion') | Whether to allow the user to minimize the sheet by dragging it \(Default: false\) |
| [AllowNormalExpansion](MRoessler.BlazorBottomSheet.BottomSheet.AllowNormalExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.AllowNormalExpansion') | Whether to allow the user to expand the sheet by dragging it \(Default: true\) |
| [BackgroundClass](MRoessler.BlazorBottomSheet.BottomSheet.BackgroundClass.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.BackgroundClass') | style class to apply on the BottomSheet background \(visible when [IsModal](MRoessler.BlazorBottomSheet.BottomSheet.IsModal.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsModal') is true\) |
| [ChildContent](MRoessler.BlazorBottomSheet.BottomSheet.ChildContent.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.ChildContent') | The BottomSheet's content\. Place [MinimizedExpansionMarker](MRoessler.BlazorBottomSheet.MinimizedExpansionMarker.md 'MRoessler\.BlazorBottomSheet\.MinimizedExpansionMarker') and [NormalExpansionMarker](MRoessler.BlazorBottomSheet.NormalExpansionMarker.md 'MRoessler\.BlazorBottomSheet\.NormalExpansionMarker') within this content for automatic height computation |
| [Class](MRoessler.BlazorBottomSheet.BottomSheet.Class.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Class') | style class to apply on the BottomSheet element |
| [CloseOnBackgroundClick](MRoessler.BlazorBottomSheet.BottomSheet.CloseOnBackgroundClick.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.CloseOnBackgroundClick') | Whether to close the sheet when the background overlay is clicked \(Default: false\) |
| [DefaultExpansion](MRoessler.BlazorBottomSheet.BottomSheet.DefaultExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DefaultExpansion') | [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') to be applied when [IsOpen](MRoessler.BlazorBottomSheet.BottomSheet.IsOpen.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsOpen') is set to true \(Default: [Normal](MRoessler.BlazorBottomSheet.BottomSheetExpansion.md#MRoessler.BlazorBottomSheet.BottomSheetExpansion.Normal 'MRoessler\.BlazorBottomSheet\.BottomSheetExpansion\.Normal')\) |
| [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') | the expansion state to apply \(Default: [Closed](MRoessler.BlazorBottomSheet.BottomSheetExpansion.md#MRoessler.BlazorBottomSheet.BottomSheetExpansion.Closed 'MRoessler\.BlazorBottomSheet\.BottomSheetExpansion\.Closed')\) |
| [ExpansionChanged](MRoessler.BlazorBottomSheet.BottomSheet.ExpansionChanged.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.ExpansionChanged') | |
| [Handle](MRoessler.BlazorBottomSheet.BottomSheet.Handle.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Handle') | Use this to overwrite the default drag handle |
| [IsModal](MRoessler.BlazorBottomSheet.BottomSheet.IsModal.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsModal') | Whether to prevent interaction with the overlayed content \(Default: false\) |
| [IsOpen](MRoessler.BlazorBottomSheet.BottomSheet.IsOpen.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsOpen') | Alternative to [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') for easier ViewModel bindings\. When set to true [DefaultExpansion](MRoessler.BlazorBottomSheet.BottomSheet.DefaultExpansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DefaultExpansion') will be applied\. |
| [IsOpenChanged](MRoessler.BlazorBottomSheet.BottomSheet.IsOpenChanged.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsOpenChanged') | |
| [IsVisible](MRoessler.BlazorBottomSheet.BottomSheet.IsVisible.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.IsVisible') | Whether this sheet should generally be visible \(Default: true\)\. <br/> Note: this is separate from [Expansion](MRoessler.BlazorBottomSheet.BottomSheet.Expansion.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.Expansion') |
| [JavaScriptObjRef](MRoessler.BlazorBottomSheet.BottomSheet.JavaScriptObjRef.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.JavaScriptObjRef') | Reference to the JavaScript BottomSheet object\. You may use this to add event listeners\. |

| Methods | |
| :--- | :--- |
| [BuildRenderTree\(RenderTreeBuilder\)](MRoessler.BlazorBottomSheet.BottomSheet.BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder).md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.BuildRenderTree\(Microsoft\.AspNetCore\.Components\.Rendering\.RenderTreeBuilder\)') | |
| [DisposeAsync\(\)](MRoessler.BlazorBottomSheet.BottomSheet.DisposeAsync().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DisposeAsync\(\)') | |
| [DisposeAsyncCore\(\)](MRoessler.BlazorBottomSheet.BottomSheet.DisposeAsyncCore().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.DisposeAsyncCore\(\)') | Can be overriden in sub\-classes to dispose resources\. |
| [OnAfterRenderAsync\(bool\)](MRoessler.BlazorBottomSheet.BottomSheet.OnAfterRenderAsync(bool).md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.OnAfterRenderAsync\(bool\)') | |
| [OnInitialized\(\)](MRoessler.BlazorBottomSheet.BottomSheet.OnInitialized().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.OnInitialized\(\)') | |
| [OnParametersSetAsync\(\)](MRoessler.BlazorBottomSheet.BottomSheet.OnParametersSetAsync().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.OnParametersSetAsync\(\)') | |
| [SetExpansionAsync\(int\)](MRoessler.BlazorBottomSheet.BottomSheet.SetExpansionAsync(int).md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.SetExpansionAsync\(int\)') | INTERNAL API |
| [WhenRenderedOnce\(\)](MRoessler.BlazorBottomSheet.BottomSheet.WhenRenderedOnce().md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.WhenRenderedOnce\(\)') | Use this to wait for the sheet render at least once\. <br/> Note: [JavaScriptObjRef](MRoessler.BlazorBottomSheet.BottomSheet.JavaScriptObjRef.md 'MRoessler\.BlazorBottomSheet\.BottomSheet\.JavaScriptObjRef') is initialized after the first render\. |
