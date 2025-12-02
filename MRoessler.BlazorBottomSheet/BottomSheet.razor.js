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
        _sheetElm.style.height = `${_preferedHeight}px`
        _isMaximized = false
        await _razorComp.invokeMethodAsync("SetExpansionAsync", ExpansionNormal)

    } else if (currentBounds.height > _preferedHeight + 10) {
        _sheetElm.style.height = "100%"
        _isMaximized = true
        await _razorComp.invokeMethodAsync("SetExpansionAsync", ExpansionMaximized)

    } else {
        await _razorComp.invokeMethodAsync("SetClosedAsync")
        _sheetElm.style.removeProperty("height")
        _isMaximized = false
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

export function dispose() {
}