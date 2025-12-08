/** @type {HTMLElement} */
let _layoutElm

/** @type {HTMLElement} */
let _sheetElm

/** @type {DotNetObject} */
let _razorComp

/** @type {boolean} */
let _isDragging

/** @type {number} */
let _dragStartHeight

/** @type {number} */
let _dragPointY

/** @type {number} */
let _dragVelocity

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
const MinDragDistance = 10
const FastDragVelocity = 50

// new: observers for layout property/size changes
/** @type {MutastionObserver} */
let _layoutAttributesObserver = null

/** @param razorComp { DotNetObject } */
export function init(layoutElm, sheetElm, razorComp) {
    _layoutElm = layoutElm
    _sheetElm = sheetElm
    _razorComp = razorComp

    _sheetElm.addEventListener("pointerdown", handlePointerDown)
    _layoutElm.addEventListener("pointerup", handlePointerUp)
    _layoutElm.addEventListener("pointermove", handlePointerMove)
    _layoutElm.addEventListener("pointerleave", handlePointerUp)

    // watch attribute changes (eg. style/class) and dispatch a custom event
    _layoutAttributesObserver = new MutationObserver(handleLayoutAttributes)
    _layoutAttributesObserver.observe(_layoutElm, {
        attributes: true, attributeFilter: [
            "data-allow-minimized-expansion", "data-allow-normal-expansion", "data-allow-maximized-expansion",
            "data-expansion", "data-visible"
        ]
    })

    updateVisible(_layoutElm.hasAttribute("data-visible"))
    updateExpansion(Number(_layoutElm.getAttribute("data-expansion")))
}

/** @param evt {PointerEvent} */
function handlePointerDown(evt) {
    // evt.preventDefault()
    _isDragging = true
    _dragStartHeight = _sheetElm.clientHeight
    _dragPointY = evt.clientY - _sheetElm.getBoundingClientRect().y
}

/** @param evt {PointerEvent} */
function handlePointerMove(evt) {
    // evt.preventDefault()
    if (!_isDragging)
        return
    _dragVelocity = evt.movementY

    const currentBounds = _sheetElm.getBoundingClientRect()
    const targetY = evt.clientY
    const distanceToTargetY = currentBounds.y - targetY
    const newHeight = currentBounds.height + distanceToTargetY + _dragPointY

    _sheetElm.style.height = `${newHeight}px`

    console.debug(`targetY: ${targetY}, newHeight: ${newHeight}`);
}

/** @param evt {PointerEvent} */
async function handlePointerUp(evt) {
    if (!_isDragging)
        return
    _isDragging = false

    const currentExpansion = getCurrentExpansion()
    const direction = computeDragMoveDirection()
    const nearestSnapPointInDirection = computeNearestSnapPointInDirection(direction)
    const nearestSnapPointAtDragPos = computeNearestSnapPointAtPos()
    const newExpansion = computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos)
    updateExpansion(newExpansion)
    _sheetElm.style.removeProperty("height")

    console.info(
        `Updated expansion after drag-end: ${newExpansion} (currentExpansion: ${currentExpansion}, nearestSnapPointInDirection: ${nearestSnapPointInDirection}, nearestSnapPointAtDragPos: ${nearestSnapPointAtDragPos}, dragVelocity: ${_dragVelocity})`)
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

function computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos) {
    if (_dragVelocity > FastDragVelocity)
        return ExpansionClosed
    else if (_dragVelocity < -FastDragVelocity)
        return ExpansionMaximized

    const currentExpansion = getCurrentExpansion()
    if (nearestSnapPointInDirection != currentExpansion)
        return nearestSnapPointInDirection
    else
        return nearestSnapPointAtDragPos
}

async function updateExpansion(expansion) {
    if (getCurrentExpansion() == expansion)
        return;

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

    await _razorComp.invokeMethodAsync("SetExpansionAsync", expansion)
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
function handleLayoutAttributes(mutations) {
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
        _handleElm?.removeEventListener("pointerdown", handlePointerDown)
        _layoutElm?.removeEventListener("pointerup", handlePointerUp)
        _layoutElm?.removeEventListener("pointerleave", handlePointerUp)
        _layoutElm?.removeEventListener("pointermove", handlePointerMove)

        if (_layoutAttributesObserver) {
            _layoutAttributesObserver.disconnect()
            _layoutAttributesObserver = null
        }
    } catch (e) {
        console.error("Error during BottomSheet cleanup:", e)
    }
}