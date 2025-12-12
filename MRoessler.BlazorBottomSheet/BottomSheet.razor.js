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
const FastDragMinDistance = 150
const FastDragTimespan = 250

/** @type {HTMLElement} */
let _rootElm

/** @type {HTMLElement} */
let _layoutElm

/** @type {HTMLElement} */
let _sheetElm

/** @type {HTMLElement} */
let _hiddenSheetElm

/** @type {HTMLElement} */
let _minimizedSectionEndElm

/** @type {HTMLElement} */
let _normalSectionEndElm

/** @type {DotNetObject} */
let _razorComp

/** @type {boolean} */
let _isDragging

/** @type {number} */
let _dragStartHeight

/** @type {number} */
let _dragStartY

/** @type {number} */
let _dragStartTime

/** @type {MutastionObserver} */
let _layoutAttributesObserver = null

/** @param razorComp { DotNetObject } */
export function init(rootElm, razorComp) {
    _rootElm = rootElm
    _razorComp = razorComp

    _layoutElm = rootElm.querySelector("div.bottom-sheet-layout")
    _sheetElm = rootElm.querySelector("div.bottom-sheet")

    _hiddenSheetElm = rootElm.querySelector("div.hidden-bottom-sheet-layout div.bottom-sheet")
    _minimizedSectionEndElm = _hiddenSheetElm.querySelector("div[data-section-end='1']")
    _normalSectionEndElm = _hiddenSheetElm.querySelector("div[data-section-end='2']")

    // note: not using pointer events because they get canceled when scrolling an element
    _sheetElm.addEventListener("touchstart", handlePointerDown)
    _layoutElm.addEventListener("touchend", handlePointerUp)
    _layoutElm.addEventListener("touchmove", handlePointerMove)
    _layoutElm.addEventListener("touchcancel", handlePointerUp)

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
    _isDragging = true
    _dragStartTime = Date.now()
    let firstTouch = evt.touches[0]
    _dragStartHeight = _sheetElm.clientHeight
    _dragStartY = computeSheetRelativeTouchPosY(firstTouch)
}

/** @param evt {TouchEvent} */
function handlePointerMove(evt) {
    console.debug(`handlePointerMove - _isDragging: ${_isDragging}`)
    if (!_isDragging)
        return

    let firstTouch = evt.touches[0]
    let touchY = computeSheetRelativeTouchPosY(firstTouch)
    let dragDeltaY = touchY - _dragStartY

    if (!shouldHandlePointerEvent(evt, dragDeltaY)) {
        console.debug(`handlePointerMove - shouldHandlePointerEvent returned false`)
        _dragStartY = touchY
        _rootElm.classList.remove(DraggingStyleClass)
        return
    }
    _rootElm.classList.add(DraggingStyleClass)

    const currentBounds = _sheetElm.getBoundingClientRect()
    const targetY = firstTouch.clientY
    const distanceToTargetY = currentBounds.y - targetY
    const newHeight = currentBounds.height + distanceToTargetY + _dragStartY

    _sheetElm.style.height = `${newHeight}px`
    console.debug(`targetY: ${targetY}, newHeight: ${newHeight}`);
}

/** @param evt {TouchEvent} */
async function handlePointerUp(evt) {
    console.debug(`handlePointerUp - _isDragging: ${_isDragging}`)
    if (!_isDragging)
        return
    _isDragging = false
    _rootElm.classList.remove(DraggingStyleClass)

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
    const dragDistance = _sheetElm.clientHeight - _dragStartHeight
    if (dragTimespan > FastDragTimespan)
        return 0

    console.debug(`dragDistance: ${dragDistance}`)

    if (dragDistance > FastDragMinDistance)
        return 1
    else if (dragDistance < -FastDragMinDistance)
        return -1

    return 0
}

/** @param firstTouch {Touch} */
function computeSheetRelativeTouchPosY(firstTouch) {
    return firstTouch.clientY - _sheetElm.getBoundingClientRect().y
}

/** 
 * @param evt {UIEvent} 
 * @param dragDeltaY {Number}
*/
function shouldHandlePointerEvent(evt, dragDeltaY) {
    /** @type {HTMLElement} */
    let currentElement = evt.target
    while (currentElement != null) {
        const elementStyle = window.getComputedStyle(currentElement)
        const overflowValue = elementStyle.getPropertyValue('overflow-y')
        const scrollTop = Math.round(currentElement.scrollTop)
        // check if the event can be handled by a scrollable element
        if ((overflowValue == 'scroll' || overflowValue == 'auto')
            && (dragDeltaY > 0 && scrollTop > 0 || dragDeltaY < 0 && scrollTop < currentElement.scrollHeight - currentElement.clientHeight))
            return false;
        currentElement = currentElement.parentElement
    }
    return true
}

function computeDragMoveDirection() {
    if (_sheetElm.clientHeight > _dragStartHeight + MinDragDistance)
        return 1
    else if (_sheetElm.clientHeight < _dragStartHeight - MinDragDistance)
        return -1
    else
        return 0
}

function computeNearestSnapPointInDirection(direction) {
    const currentExpansion = getCurrentExpansion()
    return Math.max(ExpansionClosed, Math.min(ExpansionMaximized, currentExpansion + direction))
}

function computeNearestSnapPointAtPos() {
    const currentPos = _sheetElm.clientHeight / _layoutElm.clientHeight
    if (currentPos > 0.7)
        return ExpansionMaximized
    else if (currentPos > 0.3)
        return ExpansionNormal
    else if (currentPos > 0.1)
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

    if (nearestSnapPointInDirection != currentExpansion)
        return nearestSnapPointInDirection
    else
        return nearestSnapPointAtDragPos
}

async function updateExpansion(expansion) {
    const expansionChanged = getCurrentExpansion() == expansion

    if (expansion == ExpansionClosed) {
        _rootElm.classList.add(ClosedStyleClass)
        _sheetElm.style.height = '0'
    }
    else
        _rootElm.classList.remove(ClosedStyleClass)

    if (expansion == ExpansionMinimized) {
        _rootElm.classList.add(MinimizedStyleClass)
        _sheetElm.style.height = `${computeSectionEndHeight(_minimizedSectionEndElm)}px`
    }
    else
        _rootElm.classList.remove(MinimizedStyleClass)

    if (expansion == ExpansionNormal) {
        _rootElm.classList.add(NormalStyleClass)
        _sheetElm.style.height = `${computeSectionEndHeight(_normalSectionEndElm)}px`
    }
    else
        _rootElm.classList.remove(NormalStyleClass)

    if (expansion == ExpansionMaximized) {
        _rootElm.classList.add(MaximizedStyleClass)
        _sheetElm.style.height = '100%'
    }
    else
        _rootElm.classList.remove(MaximizedStyleClass)

    if (expansionChanged)
        await _razorComp.invokeMethodAsync("SetExpansionAsync", expansion)
}

/** @param sectionElm {HTMLElement} */
function computeSectionEndHeight(sectionElm) {
    return sectionElm.getBoundingClientRect().bottom - _hiddenSheetElm.getBoundingClientRect().top
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
        _handleElm?.removeEventListener("touchstart", handlePointerDown)
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