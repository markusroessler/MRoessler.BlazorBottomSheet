import { BottomSheet, BottomSheetMoveEvent, SheetMoveEventName } from "/_content/MRoessler.BlazorBottomSheet/BottomSheet.razor.js"

export function createDynamicContentSample(rootElm, bottomSheet) {
    return new DynamicContentSample(rootElm, bottomSheet)
}

export class DynamicContentSample {
    /** @type {HTMLElement} */
    #rootElm

    /** @type {BottomSheet} */
    #sheet

    /** @type {number} */
    #sheetTranslateY

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

        sheet.addEventListener(SheetMoveEventName, (evt) => this.#layoutRevealedContent(evt.sheetTranslateY), { passive: true, signal: this.#abortController.signal })
        this.#layoutRevealedContent(sheet.sheetTranslateY)
    }

    /**
     * @param sheetTranslateY {number}
     */
    #layoutRevealedContent(sheetTranslateY) {
        this.#logDebug(`layoutRevealedContent - sheetTranslateY: ${sheetTranslateY}, #animationFramePending: ${this.#animationFramePending}`)
        this.#sheetTranslateY = sheetTranslateY

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
            const mainContentTranslateYUnbounded = viewportHeight - this.#sheetTranslateY - revealedElmRelativeBottom
            const mainContentTranslateY = this.#clamp(mainContentTranslateYUnbounded, 0, revealedElmBounds.height)
            const revealedElmOpacity = this.#clamp((mainContentTranslateYUnbounded - revealedElmBounds.height) / (viewportHeight / 10), 0.0, 1.0)

            this.#logDebug(`layoutRevealedContent animation frame - #sheetTranslateY: ${this.#sheetTranslateY}, mainContentTranslateYUnbounded: ${mainContentTranslateYUnbounded}, revealedElmBounds.height: ${revealedElmBounds.height}, viewportHeight: ${viewportHeight}`)

            this.#rootElm.style.setProperty("--main-content-transform", `translateY(${mainContentTranslateY}px)`)
            this.#rootElm.style.setProperty("--expansion-marker-transform", `translateY(${-mainContentTranslateY}px)`)
            this.#rootElm.style.setProperty("--revealed-content-opacity", revealedElmOpacity)

            this.#animationFramePending = false
        })
    }

    #clamp(val, min, max) {
        return Math.max(Math.min(val, max), min)
    }

    /** @param msg {String} */
    #logDebug(msg) {
        // if (msg.startsWith('refreshHeight'))
        console.debug(msg)
    }

    dispose() {
        this.#abortController.abort()
    }
}