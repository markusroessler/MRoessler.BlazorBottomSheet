## Members

<dl>
<dt><a href="#BottomSheetMoveEvent">BottomSheetMoveEvent</a></dt>
<dd><p>Event that is raised when the sheet is moved</p>
</dd>
<dt><a href="#BottomSheet">BottomSheet</a></dt>
<dd><p>The BottomSheet handler.
You may use this to listen for BottomSheetMoveEvents</p>
</dd>
</dl>

## Constants

<dl>
<dt><a href="#SheetMoveEventName">SheetMoveEventName</a> : <code>String</code></dt>
<dd><p>the name of the move event</p>
</dd>
</dl>

## Functions

<dl>
<dt><a href="#createBottomSheet">createBottomSheet(razorComp)</a></dt>
<dd><p>INTERNAL API</p>
</dd>
</dl>

<a name="BottomSheetMoveEvent"></a>

## BottomSheetMoveEvent
Event that is raised when the sheet is moved

**Kind**: global variable  
<a name="BottomSheetMoveEvent+sheetTranslateY"></a>

### bottomSheetMoveEvent.sheetTranslateY ⇒ <code>Number</code>
**Kind**: instance property of [<code>BottomSheetMoveEvent</code>](#BottomSheetMoveEvent)  
**Returns**: <code>Number</code> - the translateY transformation in pixels that was applied on the sheet  
<a name="BottomSheet"></a>

## BottomSheet
The BottomSheet handler.
You may use this to listen for BottomSheetMoveEvents

**Kind**: global variable  

* [BottomSheet](#BottomSheet)
    * [.BottomSheet](#BottomSheet+BottomSheet)
        * [new exports.BottomSheet(razorComp)](#new_BottomSheet+BottomSheet_new)
    * [.sheetElement](#BottomSheet+sheetElement) ⇒ <code>HTMLElement</code>
    * [.dispose()](#BottomSheet+dispose)

<a name="BottomSheet+BottomSheet"></a>

### bottomSheet.BottomSheet
**Kind**: instance class of [<code>BottomSheet</code>](#BottomSheet)  
<a name="new_BottomSheet+BottomSheet_new"></a>

#### new exports.BottomSheet(razorComp)

| Param | Type |
| --- | --- |
| razorComp | <code>DotNetObject</code> | 

<a name="BottomSheet+sheetElement"></a>

### bottomSheet.sheetElement ⇒ <code>HTMLElement</code>
**Kind**: instance property of [<code>BottomSheet</code>](#BottomSheet)  
**Returns**: <code>HTMLElement</code> - the sheet element  
<a name="BottomSheet+dispose"></a>

### bottomSheet.dispose()
INTERNAL API

**Kind**: instance method of [<code>BottomSheet</code>](#BottomSheet)  
<a name="SheetMoveEventName"></a>

## SheetMoveEventName : <code>String</code>
the name of the move event

**Kind**: global constant  
<a name="createBottomSheet"></a>

## createBottomSheet(razorComp)
INTERNAL API

**Kind**: global function  

| Param | Type |
| --- | --- |
| razorComp | <code>DotNetObject</code> | 

