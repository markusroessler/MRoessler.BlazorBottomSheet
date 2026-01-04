/** @type {number} */
const ExpansionClosed = 0

/** @type {number} */
const ExpansionMinimized = 1

/** @type {number} */
const ExpansionNormal = 2

/** @type {number} */
const ExpansionMaximized = 3

const HiddenStyleClass = "hidden"
const ClosedStyleClass = "closed"
const MinimizedStyleClass = "minimized"
const NormalStyleClass = "normal"
const MaximizedStyleClass = "maximized"
const DraggingStyleClass = "dragging"
const MinDragDistance = 50
const NearestSnapPointLeeway = 100
const FastDragMinDistance = 150
const FastDragTimespan = 250

/** @type {HTMLElement} */
let _rootElm

/** @type {HTMLElement} */
let _layoutElm

/** @type {HTMLElement} */
let _sheetElm

/** @type {HTMLElement} */
let _handleElm

/** @type {HTMLElement} */
let _minimizedExpansionMarker

/** @type {HTMLElement} */
let _normalExpansionMarker

/** @type {DotNetObject} */
let _razorComp

/** @type {boolean} */
let _isDragging

/** @type {number} */
let _dragStartTouchY

/** @type {number} */
let _dragStartSheetY

/** @type {number} */
let _dragAnchorY

/** @type {number} */
let _dragStartTime

/** @type {MutastionObserver} */
let _layoutAttributesObserver = null

/** @param razorComp { DotNetObject } */
export function init(rootElm, razorComp) {
    _rootElm = rootElm
    _razorComp = razorComp

    _layoutElm = rootElm.querySelector("div.bottom-sheet-layout")
    _sheetElm = _layoutElm.querySelector("div.bottom-sheet")
    _handleElm = _sheetElm.querySelector("div.bottom-sheet-handle")

    _minimizedExpansionMarker = _sheetElm.querySelector("div[data-expansion-marker='1']")
    _normalExpansionMarker = _sheetElm.querySelector("div[data-expansion-marker='2']")

    // note: not using pointer events because they get canceled when scrolling an element
    _sheetElm.addEventListener("touchstart", handlePointerDown, { passive: true })
    _layoutElm.addEventListener("touchend", handlePointerUp, { passive: true })
    _layoutElm.addEventListener("touchmove", handlePointerMove, { passive: true })
    _layoutElm.addEventListener("touchcancel", handlePointerUp, { passive: true })

    // watch attribute changes (eg. style/class) and dispatch a custom event
    _layoutAttributesObserver = new MutationObserver(handleLayoutAttributeChanges)
    _layoutAttributesObserver.observe(_layoutElm, {
        attributes: true, attributeFilter: [
            "data-allow-minimized-expansion", "data-allow-normal-expansion", "data-allow-maximized-expansion",
            "data-expansion", "data-visible"
        ]
    })

    updateVisible(_layoutElm.hasAttribute("data-visible"))
    updateExpansion(Number(_layoutElm.getAttribute("data-expansion")))
}

/** @param evt {TouchEvent} */
function handlePointerDown(evt) {
    console.debug(`handlePointerDown - _isDragging: ${_isDragging}`)
    if (_isDragging)
        return
    let firstTouch = evt.touches[0]

    _isDragging = true
    _dragStartTime = Date.now()
    _dragStartTouchY = firstTouch.clientY
    _dragAnchorY = computeDragAnchor(firstTouch)
    _dragStartSheetY = _sheetElm.getBoundingClientRect().y
}

/** @param evt {TouchEvent} */
function handlePointerMove(evt) {
    console.debug(`handlePointerMove - _isDragging: ${_isDragging}`)
    if (!_isDragging)
        return

    const firstTouch = evt.touches[0]
    const dragDeltaY = firstTouch.clientY - _dragStartTouchY

    if (shouldDragSheet(evt, dragDeltaY)) {
        _rootElm.classList.add(DraggingStyleClass)
        _layoutElm.classList.add(DraggingStyleClass)

        const translate = firstTouch.clientY - _dragAnchorY
        if (translate > 0)
            _sheetElm.style.transform = `translateY(${translate}px)`
        else {
            _sheetElm.style.removeProperty('transform')
            _rootElm.classList.remove(DraggingStyleClass)
            _layoutElm.classList.remove(DraggingStyleClass)
        }
    } else {
        console.debug(`handlePointerMove - shouldHandlePointerEvent returned false`)
        _dragStartTouchY = firstTouch.clientY
        _dragAnchorY = computeDragAnchor(firstTouch)
        _rootElm.classList.remove(DraggingStyleClass)
        _layoutElm.classList.remove(DraggingStyleClass)
    }
}

async function handlePointerUp() {
    console.debug(`handlePointerUp - _isDragging: ${_isDragging}`)
    if (!_isDragging)
        return
    _isDragging = false
    _rootElm.classList.remove(DraggingStyleClass)
    _layoutElm.classList.remove(DraggingStyleClass)

    const currentExpansion = getCurrentExpansion()
    const direction = computeDragMoveDirection()
    const nearestSnapPointInDirection = computeNearestSnapPointInDirection(direction)
    const nearestSnapPointAtDragPos = computeNearestSnapPointAtPos()
    const fastDragDirection = computeFastDragDirection()
    const newExpansion = computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos, fastDragDirection)
    await updateExpansion(newExpansion)

    console.info(
        `Updated expansion after drag-end: ${newExpansion} (currentExpansion: ${currentExpansion}, nearestSnapPointInDirection: ${nearestSnapPointInDirection}, nearestSnapPointAtDragPos: ${nearestSnapPointAtDragPos}, fastDragDirection: ${fastDragDirection})`)
}

function computeFastDragDirection() {
    const dragTimespan = Date.now() - _dragStartTime
    if (dragTimespan > FastDragTimespan)
        return 0

    const dragDistance = _sheetElm.getBoundingClientRect().y - _dragStartSheetY
    console.debug(`dragDistance: ${dragDistance}`)

    if (dragDistance > FastDragMinDistance)
        return -1
    else if (dragDistance < -FastDragMinDistance)
        return 1

    return 0
}

/** @param firstTouch {Touch} */
function computeDragAnchor(firstTouch) {
    return firstTouch.clientY - _sheetElm.getBoundingClientRect().y
}

/** 
 * @param evt {UIEvent} 
 * @param dragDeltaY {Number}
 * @returns {boolean}
 */
function shouldDragSheet(evt, dragDeltaY) {
    const scrollable = findScrollable(evt)
    if (!scrollable)
        return true

    const scrollTop = Math.round(scrollable.scrollTop)
    return !(dragDeltaY > 0 && scrollTop > 0 || dragDeltaY < 0 && !_sheetElm.style.transform)
}

/** 
 * @param evt {UIEvent} 
 * @returns {HTMLElement}
 */
function findScrollable(evt) {
    /** @type {HTMLElement} */
    let currentElement = evt.target
    while (currentElement != null) {
        const elementStyle = window.getComputedStyle(currentElement)
        const overflowValue = elementStyle.getPropertyValue('overflow-y')

        if (overflowValue == 'scroll' || overflowValue == 'auto')
            return currentElement;

        currentElement = currentElement.parentElement
    }

    return null
}

function computeDragMoveDirection() {
    const sheetPosY = _sheetElm.getBoundingClientRect().y
    if (sheetPosY > _dragStartSheetY + MinDragDistance)
        return -1
    else if (sheetPosY < _dragStartSheetY - MinDragDistance)
        return 1
    else
        return 0
}

function computeNearestSnapPointInDirection(direction) {
    const currentExpansion = getCurrentExpansion()
    return Math.max(ExpansionClosed, Math.min(ExpansionMaximized, currentExpansion + direction))
}

function computeNearestSnapPointAtPos() {
    const sheetBounds = _sheetElm.getBoundingClientRect()
    if (sheetBounds.y < _layoutElm.getBoundingClientRect().y + NearestSnapPointLeeway)
        return ExpansionMaximized

    else if (_normalExpansionMarker.getBoundingClientRect().y < sheetBounds.height + NearestSnapPointLeeway)
        return ExpansionNormal

    else if (_minimizedExpansionMarker.getBoundingClientRect().y < sheetBounds.height + NearestSnapPointLeeway)
        return ExpansionMinimized

    else
        return ExpansionClosed
}

function computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos, fastDragDirection) {
    const currentExpansion = getCurrentExpansion()

    if (fastDragDirection == -1)
        return Math.max(ExpansionClosed, currentExpansion - 2)
    else if (fastDragDirection == 1)
        return Math.min(ExpansionMaximized, currentExpansion + 2)

    if (nearestSnapPointAtDragPos != currentExpansion)
        return nearestSnapPointAtDragPos
    else
        return nearestSnapPointInDirection
}

async function updateExpansion(expansion) {
    const expansionChanged = getCurrentExpansion() == expansion

    if (expansion == ExpansionClosed) {
        _rootElm.classList.add(ClosedStyleClass)
        _sheetElm.style.transform = 'translateY(100%)'
    }
    else
        _rootElm.classList.remove(ClosedStyleClass)

    if (expansion == ExpansionMinimized) {
        _rootElm.classList.add(MinimizedStyleClass)
        _sheetElm.style.transform = `translateY(${computeSheetTranslateY(_minimizedExpansionMarker)}px)`
    }
    else
        _rootElm.classList.remove(MinimizedStyleClass)

    if (expansion == ExpansionNormal) {
        _rootElm.classList.add(NormalStyleClass)
        _sheetElm.style.transform = `translateY(${computeSheetTranslateY(_normalExpansionMarker)}px)`
    }
    else
        _rootElm.classList.remove(NormalStyleClass)

    if (expansion == ExpansionMaximized) {
        _rootElm.classList.add(MaximizedStyleClass)
        _sheetElm.style.removeProperty('transform')
    }
    else
        _rootElm.classList.remove(MaximizedStyleClass)

    if (expansionChanged)
        await _razorComp.invokeMethodAsync("SetExpansionAsync", expansion)
}

/** @param expansionMarker {HTMLElement} */
function computeSheetTranslateY(expansionMarker) {
    const sheetBounds = _sheetElm.getBoundingClientRect()
    return sheetBounds.height - (expansionMarker.getBoundingClientRect().top - sheetBounds.top)
}

async function updateVisible(visible) {
    if (visible)
        _rootElm.classList.remove(HiddenStyleClass)
    else
        _rootElm.classList.add(HiddenStyleClass)
}

function getCurrentExpansion() {
    if (_rootElm.classList.contains(MaximizedStyleClass))
        return ExpansionMaximized

    if (_rootElm.classList.contains(NormalStyleClass))
        return ExpansionNormal

    if (_rootElm.classList.contains(MinimizedStyleClass))
        return ExpansionMinimized

    if (_rootElm.classList.contains(ClosedStyleClass))
        return ExpansionClosed

    return -1
}

// new: mutation observer callback
function handleLayoutAttributeChanges(mutations) {
    for (const m of mutations) {
        if (m.type === "attributes") {
            const attributeName = m.attributeName
            const newValue = _layoutElm.getAttribute(attributeName)

            if (attributeName == "data-expansion")
                updateExpansion(Number(newValue))
            else if (attributeName == "data-visible")
                updateVisible(_layoutElm.hasAttribute(attributeName))
        }
    }
}

export function dispose() {
    try {
        _sheetElm?.removeEventListener("touchstart", handlePointerDown)
        _layoutElm?.removeEventListener("touchend", handlePointerUp)
        _layoutElm?.removeEventListener("touchcancel", handlePointerUp)
        _layoutElm?.removeEventListener("touchmove", handlePointerMove)

        if (_layoutAttributesObserver) {
            _layoutAttributesObserver.disconnect()
            _layoutAttributesObserver = null
        }
    } catch (e) {
        console.error("Error during BottomSheet cleanup:", e)
    }
}