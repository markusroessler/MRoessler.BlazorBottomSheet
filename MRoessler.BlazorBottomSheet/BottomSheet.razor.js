export const SheetMoveEventName = "sheet-drag"

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


/** @param razorComp { DotNetObject } */
export function createBottomSheet(layoutElm, razorComp) {
    return new BottomSheet(layoutElm, razorComp)
}

/**
 * Event that is raised when the sheet is moved
 */
export class BottomSheetMoveEvent extends Event {
    /** @type {number} */
    #sheetTranslateY

    constructor(sheetTranslateY) {
        super(SheetMoveEventName);
        this.#sheetTranslateY = sheetTranslateY
    }

    /**
     * @returns {Number} the translateY transformation in pixels that was applied on the sheet
     */
    get sheetTranslateY() {
        return this.#sheetTranslateY
    }
}

export class BottomSheet extends EventTarget {
    /** @type {HTMLElement} */
    #layoutElm

    /** @type {HTMLElement} */
    #sheetElm

    /** @type {HTMLElement} */
    #minimizedExpansionMarker

    /** @type {HTMLElement} */
    #normalExpansionMarker

    /** @type {DotNetObject} */
    #razorComp

    /** @type {boolean} */
    #isDragging

    /** @type {number} */
    #dragStartTouchY

    /** @type {number} */
    #dragLastTouchY

    /** @type {number} */
    #dragStartSheetY

    /** @type {number} */
    #dragAnchorY

    /** @type {number} */
    #dragLastTime

    /** @type {number} */
    #dragSpeed

    /** @type {number} */
    #dragStartMinTranslateY

    /** @type {number} */
    #dragStartMaxTranslateY

    /** @type {MutastionObserver} */
    #layoutAttributesObserver = null

    /** @type {ResizeObserver} */
    #layoutResizeObserver = null


    /** @param razorComp { DotNetObject } */
    constructor(layoutElm, razorComp) {
        super()

        this.#layoutElm = layoutElm
        this.#razorComp = razorComp

        this.#sheetElm = this.#layoutElm.querySelector("div.bottom-sheet")

        this.#minimizedExpansionMarker = this.#sheetElm.querySelector("div[data-expansion-marker='1']")
        this.#normalExpansionMarker = this.#sheetElm.querySelector("div[data-expansion-marker='2']")

        // note: not using pointer events because they get canceled when scrolling an element
        this.#sheetElm.addEventListener("touchstart", evt => this.#handleTouchStart(evt), { passive: true })
        this.#layoutElm.addEventListener("touchmove", evt => this.#handleTouchMove(evt), { passive: true })
        this.#layoutElm.addEventListener("touchend", () => this.#handleDragStop(), { passive: true })
        this.#layoutElm.addEventListener("touchcancel", () => this.#handleDragStop(), { passive: true })

        this.#sheetElm.addEventListener("mousedown", evt => this.#handleMouseDown(evt), { passive: true })
        this.#layoutElm.addEventListener("mousemove", evt => this.#handleMouseMove(evt), { passive: true })
        this.#layoutElm.addEventListener("mouseup", () => this.#handleDragStop(), { passive: true })
        this.#layoutElm.addEventListener("mouseleave", () => this.#handleDragStop(), { passive: true })

        // watch attribute changes (eg. style/class) and dispatch a custom event
        this.#layoutAttributesObserver = new MutationObserver((mutations) => this.#handleLayoutAttributeChanges(mutations))
        this.#layoutAttributesObserver.observe(this.#layoutElm, {
            attributes: true, attributeFilter: [
                "data-allow-closed-expansion", "data-allow-minimized-expansion", "data-allow-normal-expansion", "data-allow-maximized-expansion",
                "data-expansion", "data-visible"
            ]
        })

        this.#layoutResizeObserver = new ResizeObserver(() => this.#handleLayoutResize());
        this.#layoutResizeObserver.observe(this.#layoutElm);

        this.#updateVisible(this.#layoutElm.hasAttribute("data-visible"))
        this.#updateExpansion(Number(this.#layoutElm.getAttribute("data-expansion")))
    }

    /** @returns {HTMLElement} */
    get minimizedExpansionMarker() { return this.#minimizedExpansionMarker }

    /** @returns {HTMLElement} */
    get normalExpansionMarker() { return this.#normalExpansionMarker }

    /** @param evt {TouchEvent} */
    #handleTouchStart(evt) {
        let firstTouch = evt.touches[0]
        this.#handleDragStart(firstTouch.clientY)
    }

    /** @param evt {MouseEvent} */
    #handleMouseDown(evt) {
        if (!this.#hasSelectableText(evt.target)) /* let user select text */
            this.#handleDragStart(evt.clientY)
    }

    /** @param clientY {number} */
    #handleDragStart(clientY) {
        console.debug(`handlePointerDown - _isDragging: ${this.#isDragging}`)
        if (this.#isDragging)
            return

        this.#isDragging = true
        this.#dragStartTouchY = clientY
        this.#dragAnchorY = this.#computeDragAnchor(clientY)
        this.#dragStartSheetY = this.#sheetElm.getBoundingClientRect().y

        const allowExpansions = this.#getAllowedExpansions()
        this.#dragStartMinTranslateY = this.#computeSheetTranslateYByExpansion(allowExpansions.at(-1))
        this.#dragStartMaxTranslateY = this.#computeSheetTranslateYByExpansion(allowExpansions.at(0))
    }

    /** @param evt {TouchEvent} */
    #handleTouchMove(evt) {
        const firstTouch = evt.touches[0]
        this.#handleDragMove(evt, firstTouch.clientY)
    }

    /** @param evt {MouseEvent} */
    #handleMouseMove(evt) {
        this.#handleDragMove(evt, evt.clientY)
    }

    /** 
     * @param event {UIEvent}
     * @param clientY {number} 
     **/
    #handleDragMove(event, clientY) {
        // console.debug(`handlePointerMove - _isDragging: ${_isDragging}`)
        if (!this.#isDragging)
            return

        const dragDeltaY = clientY - this.#dragStartTouchY

        this.#dragSpeed = (clientY - this.#dragLastTouchY) / (Date.now() - this.#dragLastTime) * 1000
        this.#dragLastTouchY = clientY
        this.#dragLastTime = Date.now()
        console.debug(`handlePointerMove - _dragSpeed: ${this.#dragSpeed}`)

        if (this.#shouldDragSheet(event, dragDeltaY)) {
            this.#layoutElm.classList.add(DraggingStyleClass)

            const translate = clientY - this.#dragAnchorY
            if (translate < this.#dragStartMinTranslateY) {
                this.#updateTranslateY(this.#dragStartMinTranslateY)
                this.#layoutElm.classList.remove(DraggingStyleClass) // enable scroll
            } else if (translate > this.#dragStartMaxTranslateY) {
                this.#updateTranslateY(this.#dragStartMaxTranslateY)

            } else if (translate > 0) {
                this.#updateTranslateY(translate)

            } else {
                this.#updateTranslateY(0)
                this.#layoutElm.classList.remove(DraggingStyleClass) // enable scroll
            }

        } else {
            // console.debug(`handlePointerMove - shouldHandlePointerEvent returned false`)
            this.#dragStartTouchY = clientY
            this.#dragAnchorY = this.#computeDragAnchor(clientY)
            this.#layoutElm.classList.remove(DraggingStyleClass)
        }
    }

    async #handleDragStop() {
        console.debug(`handlePointerUp - _isDragging: ${this.#isDragging}`)
        if (!this.#isDragging)
            return
        this.#isDragging = false
        this.#layoutElm.classList.remove(DraggingStyleClass)

        const currentExpansion = this.#getCurrentExpansion()
        const direction = this.#computeDragMoveDirection()
        const nearestSnapPointInDirection = this.#computeNearestSnapPointInDirection(direction)
        const nearestSnapPointAtDragPos = this.#computeNearestSnapPointAtPos()
        const fastDragDirection = this.#computeFastDragDirection()

        let newExpansion = this.#computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos, fastDragDirection)
        newExpansion = this.#coerceExpansion(newExpansion)
        await this.#updateExpansion(newExpansion)

        console.info(
            `Updated expansion after drag-end: ${newExpansion} (currentExpansion: ${currentExpansion}, nearestSnapPointInDirection: ${nearestSnapPointInDirection}, nearestSnapPointAtDragPos: ${nearestSnapPointAtDragPos}, fastDragDirection: ${fastDragDirection})`)
    }

    /**
     * @param elm {Element}
     */
    #hasSelectableText(elm) {
        if (elm.textContent.trim().length == 0)
            return false

        let currentElement = elm
        while (currentElement != null) {
            if (window.getComputedStyle(currentElement).userSelect === 'none')
                return false
            currentElement = currentElement.parentElement
        }

        return true
    }

    #computeFastDragDirection() {
        const sheetPosY = this.#sheetElm.getBoundingClientRect().y
        if (Math.abs(this.#dragStartSheetY - sheetPosY) < FastDragMinDistance)
            return 0

        if (this.#dragSpeed > FastDragMinSpeed)
            return -1
        else if (this.#dragSpeed < -FastDragMinSpeed)
            return 1
        else
            return 0
    }

    /** @param clientY {number} */
    #computeDragAnchor(clientY) {
        return clientY - this.#sheetElm.getBoundingClientRect().y
    }

    /** 
     * @param evt {UIEvent} 
     * @param dragDeltaY {Number}
     * @returns {boolean}
     */
    #shouldDragSheet(evt, dragDeltaY) {
        if (evt instanceof MouseEvent)
            return true

        const scrollable = this.#findScrollable(evt)
        if (!scrollable)
            return true

        const scrollTop = Math.round(scrollable.scrollTop)
        return !(dragDeltaY > 0 && scrollTop > 0 || dragDeltaY < 0 && !this.#sheetElm.style.transform)
    }

    /** 
     * @param evt {UIEvent} 
     * @returns {HTMLElement}
     */
    #findScrollable(evt) {
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

    #computeDragMoveDirection() {
        const sheetPosY = this.#sheetElm.getBoundingClientRect().y
        if (sheetPosY > this.#dragStartSheetY + DragInDirectionMinDistance)
            return -1
        else if (sheetPosY < this.#dragStartSheetY - DragInDirectionMinDistance)
            return 1
        else
            return 0
    }

    #computeNearestSnapPointInDirection(direction) {
        const currentExpansion = this.#getCurrentExpansion()
        return Math.max(ExpansionClosed, Math.min(ExpansionMaximized, currentExpansion + direction))
    }

    #computeNearestSnapPointAtPos() {
        const sheetBounds = this.#sheetElm.getBoundingClientRect()
        if (sheetBounds.y < this.#layoutElm.getBoundingClientRect().y + NearestSnapPointLeeway)
            return ExpansionMaximized

        else if (this.#normalExpansionMarker.getBoundingClientRect().y < sheetBounds.height + NearestSnapPointLeeway)
            return ExpansionNormal

        else if (this.#minimizedExpansionMarker.getBoundingClientRect().y < sheetBounds.height + NearestSnapPointLeeway)
            return ExpansionMinimized

        else
            return ExpansionClosed
    }

    #computeExpansion(nearestSnapPointInDirection, nearestSnapPointAtDragPos, fastDragDirection) {
        const currentExpansion = this.#getCurrentExpansion()

        if (fastDragDirection == -1)
            return Math.max(ExpansionClosed, currentExpansion - 2)
        else if (fastDragDirection == 1)
            return Math.min(ExpansionMaximized, currentExpansion + 2)

        if (nearestSnapPointAtDragPos != currentExpansion)
            return nearestSnapPointAtDragPos
        else
            return nearestSnapPointInDirection
    }

    #coerceExpansion(newExpansion) {
        const currentExpansion = this.#getCurrentExpansion()
        const allowedExpansions = this.#getAllowedExpansions()
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
    #getAllowedExpansions() {
        /** @type {Array} */
        let allowedExpansions = []

        if (this.#layoutElm.hasAttribute('data-allow-closed-expansion'))
            allowedExpansions.push(ExpansionClosed)

        if (this.#layoutElm.hasAttribute('data-allow-minimized-expansion'))
            allowedExpansions.push(ExpansionMinimized)

        if (this.#layoutElm.hasAttribute('data-allow-normal-expansion'))
            allowedExpansions.push(ExpansionNormal)

        if (this.#layoutElm.hasAttribute('data-allow-maximized-expansion'))
            allowedExpansions.push(ExpansionMaximized)

        return allowedExpansions
    }

    async #updateExpansion(expansion) {
        const expansionChanged = this.#getCurrentExpansion() !== expansion

        if (expansion == ExpansionClosed)
            this.#layoutElm.classList.add(ClosedStyleClass)
        else
            this.#layoutElm.classList.remove(ClosedStyleClass)

        if (expansion == ExpansionMinimized)
            this.#layoutElm.classList.add(MinimizedStyleClass)
        else
            this.#layoutElm.classList.remove(MinimizedStyleClass)

        if (expansion == ExpansionNormal)
            this.#layoutElm.classList.add(NormalStyleClass)
        else
            this.#layoutElm.classList.remove(NormalStyleClass)

        if (expansion == ExpansionMaximized)
            this.#layoutElm.classList.add(MaximizedStyleClass)
        else
            this.#layoutElm.classList.remove(MaximizedStyleClass)

        this.#updateTransform(expansion)

        if (expansionChanged)
            await this.#razorComp.invokeMethodAsync("SetExpansionAsync", expansion)
    }

    #updateTransform(expansion) {
        if (expansion == ExpansionClosed)
            this.#updateTranslateY(document.documentElement.clientHeight)

        else if (expansion == ExpansionMinimized)
            this.#updateTranslateY(this.#computeSheetTranslateYByMarker(this.#minimizedExpansionMarker))

        else if (expansion == ExpansionNormal)
            this.#updateTranslateY(this.#computeSheetTranslateYByMarker(this.#normalExpansionMarker))

        else if (expansion == ExpansionMaximized)
            this.#updateTranslateY(0)
    }

    #updateTranslateY(translateY) {
        if (translateY == 0)
            this.#sheetElm.style.removeProperty('transform')
        else
            this.#sheetElm.style.transform = `translateY(${translateY}px)`
        this.dispatchEvent(new BottomSheetMoveEvent(translateY))
    }

    /** @param expansionMarker {HTMLElement} */
    #computeSheetTranslateYByMarker(expansionMarker) {
        const sheetBounds = this.#sheetElm.getBoundingClientRect()
        return Math.max(0, sheetBounds.bottom - expansionMarker.getBoundingClientRect().top)
    }

    /**
     * @param expansion {number}
     */
    #computeSheetTranslateYByExpansion(expansion) {
        if (expansion == ExpansionClosed)
            return this.#sheetElm.getBoundingClientRect().height

        if (expansion == ExpansionMinimized)
            return this.#computeSheetTranslateYByMarker(this.#minimizedExpansionMarker)

        else if (expansion == ExpansionNormal)
            return this.#computeSheetTranslateYByMarker(this.#normalExpansionMarker)

        else
            return 0
    }

    async #updateVisible(visible) {
        if (visible)
            this.#layoutElm.classList.remove(HiddenStyleClass)
        else
            this.#layoutElm.classList.add(HiddenStyleClass)
    }

    #getCurrentExpansion() {
        if (this.#layoutElm.classList.contains(MaximizedStyleClass))
            return ExpansionMaximized

        if (this.#layoutElm.classList.contains(NormalStyleClass))
            return ExpansionNormal

        if (this.#layoutElm.classList.contains(MinimizedStyleClass))
            return ExpansionMinimized

        if (this.#layoutElm.classList.contains(ClosedStyleClass))
            return ExpansionClosed

        return -1
    }

    // new: mutation observer callback
    #handleLayoutAttributeChanges(mutations) {
        for (const m of mutations) {
            if (m.type === "attributes") {
                const attributeName = m.attributeName
                const newValue = this.#layoutElm.getAttribute(attributeName)

                if (attributeName == "data-expansion")
                    this.#updateExpansion(Number(newValue))
                else if (attributeName == "data-visible")
                    this.#updateVisible(this.#layoutElm.hasAttribute(attributeName))
            }
        }
    }

    #handleLayoutResize() {
        this.#updateExpansion(this.#getCurrentExpansion())
    }

    dispose() {
        try {
            this.#sheetElm?.removeEventListener("touchstart", this.#handleTouchStart)
            this.#layoutElm?.removeEventListener("touchmove", this.#handleTouchMove)
            this.#layoutElm?.removeEventListener("touchend", this.#handleDragStop)
            this.#layoutElm?.removeEventListener("touchcancel", this.#handleDragStop)

            this.#sheetElm?.removeEventListener("mousedown", this.#handleMouseDown)
            this.#layoutElm?.removeEventListener("mousemove", this.#handleMouseMove)
            this.#layoutElm?.removeEventListener("mouseup", this.#handleDragStop)
            this.#layoutElm?.removeEventListener("mouseleave", this.#handleDragStop)

            if (this.#layoutAttributesObserver) {
                this.#layoutAttributesObserver.disconnect()
                this.#layoutAttributesObserver = null
            }

            if (this.#layoutResizeObserver) {
                this.#layoutResizeObserver.disconnect()
                this.#layoutResizeObserver = null
            }
        } catch (e) {
            console.error("Error during BottomSheet cleanup:", e)
        }
    }
}
