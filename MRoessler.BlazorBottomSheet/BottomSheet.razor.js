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
const DragInDirectionMinDistance = 50
const NearestSnapPointLeeway = 100
const FastDragMinSpeed = 1000
const FastDragMinDistance = 100

/** @type {HTMLElement} */
let _layoutElm

/** @type {HTMLElement} */
let _sheetElm

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
let _dragLastTouchY

/** @type {number} */
let _dragStartSheetY

/** @type {number} */
let _dragAnchorY

/** @type {number} */
let _dragLastTime

/** @type {number} */
let _dragSpeed

/** @type {number} */
let _dragStartMinTranslateY

/** @type {number} */
let _dragStartMaxTranslateY

/** @type {MutastionObserver} */
let _layoutAttributesObserver = null

/** @type {ResizeObserver} */
let _layoutResizeObserver = null

/** @param razorComp { DotNetObject } */
export function init(layoutElm, razorComp) {
    _layoutElm = layoutElm
    _razorComp = razorComp

    _sheetElm = _layoutElm.querySelector("div.bottom-sheet")

    _minimizedExpansionMarker = _sheetElm.querySelector("div[data-expansion-marker='1']")
    _normalExpansionMarker = _sheetElm.querySelector("div[data-expansion-marker='2']")

    // note: not using pointer events because they get canceled when scrolling an element
    _sheetElm.addEventListener("touchstart", handleTouchStart, { passive: true })
    _layoutElm.addEventListener("touchmove", handleTouchMove, { passive: true })
    _layoutElm.addEventListener("touchend", handlePointerUp, { passive: true })
    _layoutElm.addEventListener("touchcancel", handlePointerUp, { passive: true })

    _sheetElm.addEventListener("mousedown", handleMouseDown, { passive: true })
    _layoutElm.addEventListener("mousemove", handleMouseMove, { passive: true })
    _layoutElm.addEventListener("mouseup", handlePointerUp, { passive: true })
    _layoutElm.addEventListener("mouseleave", handlePointerUp, { passive: true })

    // watch attribute changes (eg. style/class) and dispatch a custom event
    _layoutAttributesObserver = new MutationObserver(handleLayoutAttributeChanges)
    _layoutAttributesObserver.observe(_layoutElm, {
        attributes: true, attributeFilter: [
            "data-allow-closed-expansion", "data-allow-minimized-expansion", "data-allow-normal-expansion", "data-allow-maximized-expansion",
            "data-expansion", "data-visible"
        ]
    })

    _layoutResizeObserver = new ResizeObserver(handleLayoutResize);
    _layoutResizeObserver.observe(_layoutElm);

    updateVisible(_layoutElm.hasAttribute("data-visible"))
    updateExpansion(Number(_layoutElm.getAttribute("data-expansion")))
}

/** @param evt {TouchEvent} */
function handleTouchStart(evt) {
    let firstTouch = evt.touches[0]
    handlePointerDown(firstTouch.clientY)
}

/** @param evt {MouseEvent} */
function handleMouseDown(evt) {
    handlePointerDown(evt.clientY)
}

/** @param clientY {number} */
function handlePointerDown(clientY) {
    console.debug(`handlePointerDown - _isDragging: ${_isDragging}`)
    if (_isDragging)
        return

    _isDragging = true
    _dragStartTouchY = clientY
    _dragAnchorY = computeDragAnchor(clientY)
    _dragStartSheetY = _sheetElm.getBoundingClientRect().y

    const allowExpansions = getAllowedExpansions()
    _dragStartMinTranslateY = computeSheetTranslateYByExpansion(allowExpansions.at(-1))
    _dragStartMaxTranslateY = computeSheetTranslateYByExpansion(allowExpansions.at(0))
}

/** @param evt {TouchEvent} */
function handleTouchMove(evt) {
    const firstTouch = evt.touches[0]
    handlePointerMove(evt, firstTouch.clientY)
}

/** @param evt {MouseEvent} */
function handleMouseMove(evt) {
    handlePointerMove(evt, evt.clientY)
}

/** 
 * @param event {UIEvent}
 * @param clientY {number} 
 **/
function handlePointerMove(event, clientY) {
    // console.debug(`handlePointerMove - _isDragging: ${_isDragging}`)
    if (!_isDragging)
        return

    const dragDeltaY = clientY - _dragStartTouchY

    _dragSpeed = (clientY - _dragLastTouchY) / (Date.now() - _dragLastTime) * 1000
    _dragLastTouchY = clientY
    _dragLastTime = Date.now()
    console.debug(`handlePointerMove - _dragSpeed: ${_dragSpeed}`)

    if (shouldDragSheet(event, dragDeltaY)) {
        _layoutElm.classList.add(DraggingStyleClass)

        const translate = clientY - _dragAnchorY
        if (translate < _dragStartMinTranslateY) {
            _sheetElm.style.transform = `translateY(${_dragStartMinTranslateY}px)`
            _layoutElm.classList.remove(DraggingStyleClass) // enable scroll
        } else if (translate > _dragStartMaxTranslateY) {
            _sheetElm.style.transform = `translateY(${_dragStartMaxTranslateY}px)`

        } else if (translate > 0) {
            _sheetElm.style.transform = `translateY(${translate}px)`

        } else {
            _sheetElm.style.removeProperty('transform')
            _layoutElm.classList.remove(DraggingStyleClass) // enable scroll
        }

    } else {
        // console.debug(`handlePointerMove - shouldHandlePointerEvent returned false`)
        _dragStartTouchY = clientY
        _dragAnchorY = computeDragAnchor(clientY)
        _layoutElm.classList.remove(DraggingStyleClass)
    }
}

async function handlePointerUp() {
    console.debug(`handlePointerUp - _isDragging: ${_isDragging}`)
    if (!_isDragging)
        return
    _isDragging = false
    _layoutElm.classList.remove(DraggingStyleClass)

    const currentExpansion = getCurrentExpansion()
    const direction = computeDragMoveDirection()
    const nearestSnapPointInDirection = computeNearestSnapPointInDirection(direction)
    const nearestSnapPointAtDragPos = computeNearestSnapPointAtPos()
    const fastDragDirection = computeFastDragDirection()

    let newExpansion = computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos, fastDragDirection)
    newExpansion = coerceExpansion(newExpansion)
    await updateExpansion(newExpansion)

    console.info(
        `Updated expansion after drag-end: ${newExpansion} (currentExpansion: ${currentExpansion}, nearestSnapPointInDirection: ${nearestSnapPointInDirection}, nearestSnapPointAtDragPos: ${nearestSnapPointAtDragPos}, fastDragDirection: ${fastDragDirection})`)
}

function computeFastDragDirection() {
    const sheetPosY = _sheetElm.getBoundingClientRect().y
    if (Math.abs(_dragStartSheetY - sheetPosY) < FastDragMinDistance)
        return 0

    if (_dragSpeed > FastDragMinSpeed)
        return -1
    else if (_dragSpeed < -FastDragMinSpeed)
        return 1
    else
        return 0
}

/** @param clientY {number} */
function computeDragAnchor(clientY) {
    return clientY - _sheetElm.getBoundingClientRect().y
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

    if (evt instanceof MouseEvent)
        return false; /* let user select text */

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
    if (sheetPosY > _dragStartSheetY + DragInDirectionMinDistance)
        return -1
    else if (sheetPosY < _dragStartSheetY - DragInDirectionMinDistance)
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

function coerceExpansion(newExpansion) {
    const currentExpansion = getCurrentExpansion()
    const allowedExpansions = getAllowedExpansions()
    if (newExpansion == currentExpansion || allowedExpansions.includes(newExpansion))
        return newExpansion

    if (newExpansion > currentExpansion) {
        return allowedExpansions.find(e => e > currentExpansion) ?? currentExpansion
    } else {
        return allowedExpansions.findLast(e => e < currentExpansion) ?? currentExpansion
    }
}

/**
 * @returns {Array}
 */
function getAllowedExpansions() {
    /** @type {Array} */
    let allowedExpansions = []

    if (_layoutElm.hasAttribute('data-allow-closed-expansion'))
        allowedExpansions.push(ExpansionClosed)

    if (_layoutElm.hasAttribute('data-allow-minimized-expansion'))
        allowedExpansions.push(ExpansionMinimized)

    if (_layoutElm.hasAttribute('data-allow-normal-expansion'))
        allowedExpansions.push(ExpansionNormal)

    if (_layoutElm.hasAttribute('data-allow-maximized-expansion'))
        allowedExpansions.push(ExpansionMaximized)

    return allowedExpansions
}

async function updateExpansion(expansion) {
    const expansionChanged = getCurrentExpansion() !== expansion

    if (expansion == ExpansionClosed)
        _layoutElm.classList.add(ClosedStyleClass)
    else
        _layoutElm.classList.remove(ClosedStyleClass)

    if (expansion == ExpansionMinimized)
        _layoutElm.classList.add(MinimizedStyleClass)
    else
        _layoutElm.classList.remove(MinimizedStyleClass)

    if (expansion == ExpansionNormal)
        _layoutElm.classList.add(NormalStyleClass)
    else
        _layoutElm.classList.remove(NormalStyleClass)

    if (expansion == ExpansionMaximized)
        _layoutElm.classList.add(MaximizedStyleClass)
    else
        _layoutElm.classList.remove(MaximizedStyleClass)

    updateTransform(expansion)

    if (expansionChanged)
        await _razorComp.invokeMethodAsync("SetExpansionAsync", expansion)
}

function updateTransform(expansion) {
    if (expansion == ExpansionClosed)
        _sheetElm.style.transform = 'translateY(100%)'

    else if (expansion == ExpansionMinimized)
        _sheetElm.style.transform = `translateY(${computeSheetTranslateYByMarker(_minimizedExpansionMarker)}px)`

    else if (expansion == ExpansionNormal)
        _sheetElm.style.transform = `translateY(${computeSheetTranslateYByMarker(_normalExpansionMarker)}px)`

    else if (expansion == ExpansionMaximized)
        _sheetElm.style.removeProperty('transform')
}

/** @param expansionMarker {HTMLElement} */
function computeSheetTranslateYByMarker(expansionMarker) {
    const sheetBounds = _sheetElm.getBoundingClientRect()
    return Math.max(0, sheetBounds.bottom - expansionMarker.getBoundingClientRect().top)
}

/**
 * @param expansion {number}
 */
function computeSheetTranslateYByExpansion(expansion) {
    if (expansion == ExpansionClosed)
        return _sheetElm.getBoundingClientRect().height

    if (expansion == ExpansionMinimized)
        return computeSheetTranslateYByMarker(_minimizedExpansionMarker)

    else if (expansion == ExpansionNormal)
        return computeSheetTranslateYByMarker(_normalExpansionMarker)

    else
        return 0
}

async function updateVisible(visible) {
    if (visible)
        _layoutElm.classList.remove(HiddenStyleClass)
    else
        _layoutElm.classList.add(HiddenStyleClass)
}

function getCurrentExpansion() {
    if (_layoutElm.classList.contains(MaximizedStyleClass))
        return ExpansionMaximized

    if (_layoutElm.classList.contains(NormalStyleClass))
        return ExpansionNormal

    if (_layoutElm.classList.contains(MinimizedStyleClass))
        return ExpansionMinimized

    if (_layoutElm.classList.contains(ClosedStyleClass))
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

function handleLayoutResize() {
    updateExpansion(getCurrentExpansion())
}

export function dispose() {
    try {
        _sheetElm?.removeEventListener("touchstart", handleTouchStart)
        _layoutElm?.removeEventListener("touchmove", handleTouchMove)
        _layoutElm?.removeEventListener("touchend", handlePointerUp)
        _layoutElm?.removeEventListener("touchcancel", handlePointerUp)

        _sheetElm?.removeEventListener("mousedown", handleMouseDown)
        _layoutElm?.removeEventListener("mousemove", handleMouseMove)
        _layoutElm?.removeEventListener("mouseup", handlePointerUp)
        _layoutElm?.removeEventListener("mouseleave", handlePointerUp)

        if (_layoutAttributesObserver) {
            _layoutAttributesObserver.disconnect()
            _layoutAttributesObserver = null
        }

        if (_layoutResizeObserver) {
            _layoutResizeObserver.disconnect()
            _layoutResizeObserver = null
        }
    } catch (e) {
        console.error("Error during BottomSheet cleanup:", e)
    }
}