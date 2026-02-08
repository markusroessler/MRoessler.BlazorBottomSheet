import { BottomSheet, BottomSheetMoveEvent, SheetMoveEventName } from "/_content/MRoessler.BlazorBottomSheet/BottomSheet.razor.js"

export function createDynamicContentSample(rootElm, bottomSheet) {
    return new DynamicContentSample(rootElm, bottomSheet)
}

export class DynamicContentSample {
    /** @type {HTMLElement} */
    #rootElm

    /** @type {BottomSheet} */
    #sheet

    /** @type {HTMLElement} */
    #revealedElm

    /** @type {HTMLElement} */
    #mainContentElm

    /** @type {HTMLElement} */
    #minExpansionMarker

    /** @type {HTMLElement} */
    #normalExpansionMarker


    /**
     * @param rootElm {HTMLElement}
     * @param sheet {BottomSheet}
     */
    constructor(rootElm, sheet) {
        this.#rootElm = rootElm
        this.#sheet = sheet

        this.#revealedElm = rootElm.querySelector("*[data-revealed-content]");
        this.#mainContentElm = rootElm.querySelector("*[data-main-content]");
        this.#minExpansionMarker = sheet.minimizedExpansionMarker
        this.#normalExpansionMarker = sheet.normalExpansionMarker

        sheet.addEventListener(SheetMoveEventName, (evt) => this.#layoutRevealedContent(evt), { passive: true })
    }

    /**
     * @param evt {BottomSheetMoveEvent}
     */
    #layoutRevealedContent(evt) {
        const viewportHeight = document.documentElement.clientHeight
        const revealedElmBounds = this.#revealedElm.getBoundingClientRect()
        const sheetElmBounds = this.#sheet.sheetElement.getBoundingClientRect()
        const revealedElmRelativeBottom = revealedElmBounds.bottom - sheetElmBounds.top

        const mainContentTranslateYUnbounded = viewportHeight - evt.sheetTranslateY - revealedElmRelativeBottom
        const mainContentTranslateY = this.#clamp(mainContentTranslateYUnbounded, 0, revealedElmBounds.height)
        const revealedElmOpacity = this.#clamp((mainContentTranslateYUnbounded - revealedElmBounds.height) / (viewportHeight / 4), 0.0, 1.0)

        this.#mainContentElm.style.transform = `translateY(${mainContentTranslateY}px)`
        this.#minExpansionMarker.style.transform = `translateY(${-mainContentTranslateY}px)`
        this.#normalExpansionMarker.style.transform = `translateY(${-mainContentTranslateY}px)`
        this.#revealedElm.style.opacity = revealedElmOpacity
    }

    #clamp(val, min, max) {
        return Math.max(Math.min(val, max), min)
    }

    dispose() {
        this.#sheet.removeEventListener(SheetMoveEventName)
    }
}