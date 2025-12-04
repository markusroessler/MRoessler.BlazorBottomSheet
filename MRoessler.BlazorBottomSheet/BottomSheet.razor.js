/** @type {HTMLElement} */
let _layoutElm

/** @type {HTMLElement} */
let _handleElm

/** @type {HTMLElement} */
let _sheetElm

/** @type {DotNetObject} */
let _razorComp

/** @type {boolean} */
let _isDragging

/** @type {number} */
let _preferedHeight = 200

/** @type {boolean} */
let _isMaximized = false

/** @type {number} */
const ExpansionMinimized = 0

/** @type {number} */
const ExpansionNormal = 1

/** @type {number} */
const ExpansionMaximized = 2

// new: observers for layout property/size changes
/** @type {MutastionObserver} */
let _layoutAttributesObserver = null

/** @param razorComp { DotNetObject } */
export function init(layoutElm, handleElm, sheetElm, razorComp) {
    _layoutElm = layoutElm
    _handleElm = handleElm
    _sheetElm = sheetElm
    _razorComp = razorComp

    _handleElm.addEventListener("mousedown", handleMouseDown)
    _layoutElm.addEventListener("mouseup", handleMouseUp)
    _layoutElm.addEventListener("mouseleave", handleMouseUp)
    _layoutElm.addEventListener("mousemove", handleMouseMove)

    // watch attribute changes (eg. style/class) and dispatch a custom event
    _layoutAttributesObserver = new MutationObserver(handleLayoutAttributes)
    _layoutAttributesObserver.observe(_layoutElm, {
        attributes: true, attributeFilter: [
            "data-allow-minimized-expansion", "data-allow-normal-expansion", "data-allow-maximized-expansion",
            "data-expansion", "data-is-open"
        ]
    })
}

function handleMouseDown() {
    _isDragging = true
}

async function handleMouseUp() {
    if (!_isDragging)
        return
    _isDragging = false

    const currentBounds = _sheetElm.getBoundingClientRect()
    if (_isMaximized && currentBounds.height < _layoutElm.getBoundingClientRect().height - 10) {
        await _razorComp.invokeMethodAsync("SetExpansionAsync", ExpansionNormal)

    } else if (currentBounds.height > _preferedHeight + 10) {
        await _razorComp.invokeMethodAsync("SetExpansionAsync", ExpansionMaximized)

    } else {
        await _razorComp.invokeMethodAsync("SetClosedAsync")
    }
}

/** @param evt {MouseEvent} */
function handleMouseMove(evt) {
    if (!_isDragging)
        return

    const currentBounds = _sheetElm.getBoundingClientRect()
    const targetY = evt.clientY
    const distanceToTargetY = currentBounds.y - targetY
    const newHeight = currentBounds.height + distanceToTargetY

    _sheetElm.style.height = `${newHeight}px`

    console.log(`targetY: ${targetY}, newHeight: ${newHeight}`);
}

// new: mutation observer callback
function handleLayoutAttributes(mutations) {
    for (const m of mutations) {
        if (m.type === "attributes") {
            const attributeName = m.attributeName
            const newValue = _layoutElm.getAttribute(attributeName)

            if (attributeName === "data-expansion")
                updateExpansion(newValue)
            else if (attributeName === "data-is-open")
                updateOpen(newValue)
        }
    }
}

/** @param expansion {string} */
function updateExpansion(expansion) {
    if (expansion === "Minimized") {

    } else if (expansion === "Normal") {
        _sheetElm.style.height = `${_preferedHeight}px`
        _isMaximized = false

    } else if (expansion === "Maximized") {
        _sheetElm.style.height = "100%"
        _isMaximized = true
    }
}

/** @param open {Boolean} */
function updateOpen(open) {
    if (open) {
        // TODO
    } else {
        _sheetElm.style.removeProperty("height")
        _isMaximized = false
    }
}

export function dispose() {
    try {
        _handleElm?.removeEventListener("mousedown", handleMouseDown)
        _layoutElm?.removeEventListener("mouseup", handleMouseUp)
        _layoutElm?.removeEventListener("mouseleave", handleMouseUp)
        _layoutElm?.removeEventListener("mousemove", handleMouseMove)

        if (_layoutAttributesObserver) {
            _layoutAttributesObserver.disconnect()
            _layoutAttributesObserver = null
        }
    } catch (e) {
        console.error("Error during BottomSheet cleanup:", e)
    }
}