import { BottomSheet, BottomSheetMoveEvent, SheetMoveEventName } from "/_content/MRoessler.BlazorBottomSheet/BottomSheet.razor.min.js"

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

    #abortController = new AbortController()

    #animationFramePending = false


    /**
     * @param rootElm {HTMLElement}
     * @param sheet {BottomSheet}
     */
    constructor(rootElm, sheet) {
        this.#rootElm = rootElm
        this.#sheet = sheet
        this.#revealedElm = rootElm.querySelector(".revealed-content");

        sheet.addEventListener(SheetMoveEventName, (evt) => this.#layoutRevealedContent(evt), { passive: true, signal: this.#abortController.signal })
    }

    /**
     * @param evt {BottomSheetMoveEvent}
     */
    #layoutRevealedContent(evt) {
        if (this.#animationFramePending)
            return;
        this.#animationFramePending = true

        window.requestAnimationFrame(_ => {
            const viewportHeight = document.documentElement.clientHeight
            const revealedElmBounds = this.#revealedElm.getBoundingClientRect()
            const sheetElmBounds = this.#sheet.sheetElement.getBoundingClientRect()
            const revealedElmRelativeBottom = revealedElmBounds.bottom - sheetElmBounds.top

            // first reveal by increasing height and then by opacity
            // mainContent is the element below the revealed element
            const mainContentTranslateYUnbounded = viewportHeight - evt.sheetTranslateY - revealedElmRelativeBottom
            const mainContentTranslateY = this.#clamp(mainContentTranslateYUnbounded, 0, revealedElmBounds.height)
            const revealedElmOpacity = this.#clamp((mainContentTranslateYUnbounded - revealedElmBounds.height) / (viewportHeight / 10), 0.0, 1.0)

            this.#rootElm.style.setProperty("--main-content-transform", `translateY(${mainContentTranslateY}px)`)
            this.#rootElm.style.setProperty("--expansion-marker-transform", `translateY(${-mainContentTranslateY}px)`)
            this.#rootElm.style.setProperty("--revealed-content-opacity", revealedElmOpacity)

            this.#animationFramePending = false
        })
    }

    #clamp(val, min, max) {
        return Math.max(Math.min(val, max), min)
    }

    dispose() {
        this.#abortController.abort()
    }
}