import { BottomSheet, BottomSheetDragEvent } from "/_content/MRoessler.BlazorBottomSheet/BottomSheet.razor.js"

export function createDynamicContentSample(rootElm, bottomSheet) {
    return new DynamicContentSample(rootElm, bottomSheet)
}

export class DynamicContentSample {
    /** @type {HTMLElement} */
    #rootElm

    /** @type {HTMLElement} */
    #revealedElm

    /** @type {HTMLElement} */
    #mainContentElm

    /** @type {HTMLElement} */
    #minExpansionMarker

    /** @type {HTMLElement} */
    #normalExpansionMarker


    /**
     * @param bottomSheet {BottomSheet}
     */
    constructor(rootElm, bottomSheet) {
        this.#rootElm = rootElm
        this.#revealedElm = this.#rootElm.querySelector("*[data-revealed-content]");
        this.#mainContentElm = this.#rootElm.querySelector("*[data-main-content]");
        this.#minExpansionMarker = this.#rootElm.querySelector("*[data-expansion-marker='1']")
        this.#normalExpansionMarker = this.#rootElm.querySelector("*[data-expansion-marker='2']")

        bottomSheet.addEventListener('sheet-drag', (evt) => this.#layoutRevealedContent(evt), { passive: true })
    }

    /**
     * @param evt {BottomSheetDragEvent}
     */
    #layoutRevealedContent(evt) {
        const mainContentTranslateY = this.#computeMainContentTranslateY(evt.translateY)
        this.#mainContentElm.style.transform = `translateY(${mainContentTranslateY}px)`
        this.#minExpansionMarker.style.transform = `translateY(${-mainContentTranslateY}px)`
        this.#normalExpansionMarker.style.transform = `translateY(${-mainContentTranslateY}px)`
    }

    /**
     * @param sheetTranslateY {Number}
     */
    #computeMainContentTranslateY(sheetTranslateY) {
        const viewportHeight = document.documentElement.clientHeight
        const revealedElmBounds = this.#revealedElm.getBoundingClientRect()
        const rootElmBounds = this.#rootElm.getBoundingClientRect()
        const revealedElmRelativeBottom = revealedElmBounds.bottom - rootElmBounds.top

        return this.#clamp(viewportHeight - sheetTranslateY - revealedElmRelativeBottom, 0, revealedElmBounds.height)
    }

    #clamp(val, min, max) {
        return Math.max(Math.min(val, max), min)
    }

    dispose() {
        // this.#resizeObserver.disconnect()
    }
}