"use strict";

const axes = [
    { name: "X", property: "x", stepperId: 0 },
    { name: "Y", property: "y", stepperId: 1 },
    { name: "Z", property: "z", stepperId: 2 },
    { name: "U", property: "u", stepperId: 3 },
    { name: "V", property: "v", stepperId: 4 }
];

const fixedJogSteps = [
    -1, -2, -4, -8, -16, -32, -64, -128, -256, -512,
    1, 2, 4, 8, 16, 32, 64, 128, 256, 512
];

const optPoints = [
    ["msrSwipingStartPoint", "MSR swiping start"],
    ["msrSwipingStopPoint", "MSR swiping stop"],
    ["contactInsertingStartPoint", "Contact inserting start"],
    ["contactInsertingStopPoint", "Contact inserting stop"],
    ["contactlessTappingStartPoint", "Contactless tapping start"],
    ["contactlessTappingStopPoint", "Contactless tapping stop"],
    ["barcodeReadingStartPoint", "Barcode reading start"],
    ["barcodeReadingStopPoint", "Barcode reading stop"],
    ["screenTopLeft", "Screen top left"],
    ["screenTopRight", "Screen top right"],
    ["screenBottomRight", "Screen bottom right"],
    ["screenBottomLeft", "Screen bottom left"]
];

let coordinateConfig = null;
let livePoint = null;
let allAxesReady = false;
let statusRequestInFlight = false;
let messageTimer = null;

function createAxisRows() {
    const body = document.getElementById("axis-rows");
    body.innerHTML = axes.map(axis => `
        <tr>
            <td class="axis-name">${axis.name}</td>
            <td>${axis.stepperId}</td>
            <td><span id="axis-state-${axis.stepperId}" class="axis-state axis-not-ready">UNKNOWN</span></td>
            <td><output id="axis-position-${axis.stepperId}">—</output></td>
            <td>
                <input id="axis-steps-${axis.stepperId}" class="jog-input" type="number" min="1" max="2048" step="1" value="10" aria-label="${axis.name} jog steps">
            </td>
            <td>
                <div class="movement-controls">
                    <button class="jog-button" type="button" data-stepper-id="${axis.stepperId}" data-direction="-1" disabled>− ${axis.name}</button>
                    <button class="jog-button" type="button" data-stepper-id="${axis.stepperId}" data-direction="1" disabled>+ ${axis.name}</button>
                </div>
            </td>
            <td>
                <div class="fixed-step-controls" aria-label="${axis.name} fixed-step movement">
                    ${fixedJogSteps.map(steps => `
                        <button class="jog-button fixed-step-button" type="button"
                            data-stepper-id="${axis.stepperId}" data-fixed-steps="${steps}" disabled>${steps > 0 ? `+${steps}` : steps}</button>
                    `).join("")}
                </div>
            </td>
        </tr>
    `).join("");

    document.querySelectorAll(".jog-button").forEach(button => {
        button.addEventListener("click", () => moveAxis(button));
    });
}

function populateStaticSelectors() {
    document.getElementById("card-bay-select").innerHTML = [0, 1, 2]
        .map(index => `<option value="${index}">Card bay ${index + 1}</option>`)
        .join("");

    document.getElementById("opt-point-select").innerHTML = optPoints
        .map(([value, label]) => `<option value="${value}">${label}</option>`)
        .join("");
}

async function requestJson(url, options = {}) {
    const response = await fetch(url, options);
    if (!response.ok) {
        let detail = `${response.status} ${response.statusText}`;
        try {
            const error = await response.json();
            detail = error.error || error.detail || detail;
        } catch {
            detail = detail;
        }
        throw new Error(detail);
    }

    return response.json();
}

async function requestText(url, options = {}) {
    const response = await fetch(url, options);
    const text = await response.text();
    if (!response.ok) {
        throw new Error(text || `${response.status} ${response.statusText}`);
    }
    return text;
}

function jsonOptions(method, body) {
    return {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    };
}

async function loadCoordinates() {
    coordinateConfig = await requestJson("/api/coordinates/");
    renderCoordinateConfig();
    setSaveButtonsEnabled(allAxesReady);
}

async function pollStatus() {
    if (statusRequestInFlight) {
        return;
    }

    statusRequestInFlight = true;
    try {
        const rawStatus = await requestText("/get/Status");
        if (!rawStatus) {
            throw new Error("The controller status is not ready.");
        }

        updateAxisStatus(JSON.parse(rawStatus));
        setConnectionState(true, "Controller online");
    } catch (error) {
        setConnectionState(false, error.message);
        disableMovement();
    } finally {
        statusRequestInFlight = false;
    }
}

function updateAxisStatus(status) {
    if (!Array.isArray(status.steppers) || status.steppers.length < axes.length) {
        throw new Error("The controller returned incomplete stepper status.");
    }

    const nextPoint = {};
    let readyCount = 0;

    axes.forEach(axis => {
        const stepper = status.steppers[axis.stepperId];
        const state = stepper.status.state;
        const position = stepper.status.offset;
        const ready = state === "READY";

        nextPoint[axis.property] = position;
        document.getElementById(`axis-position-${axis.stepperId}`).textContent = position;

        const stateElement = document.getElementById(`axis-state-${axis.stepperId}`);
        stateElement.textContent = state;
        stateElement.className = `axis-state ${ready ? "axis-ready" : "axis-not-ready"}`;

        document.querySelectorAll(`[data-stepper-id="${axis.stepperId}"]`).forEach(button => {
            button.disabled = !ready;
        });

        if (ready) {
            readyCount += 1;
        }
    });

    livePoint = nextPoint;
    allAxesReady = readyCount === axes.length;
    document.getElementById("live-point-value").textContent = formatPoint(livePoint);
    setSaveButtonsEnabled(allAxesReady);
}

function disableMovement() {
    livePoint = null;
    allAxesReady = false;
    document.querySelectorAll(".jog-button").forEach(button => {
        button.disabled = true;
    });
    document.getElementById("live-point-value").textContent = "X — · Y — · Z — · U — · V —";
    setSaveButtonsEnabled(false);
}

function setSaveButtonsEnabled(enabled) {
    const hasConfig = coordinateConfig !== null;
    const cardAction = document.getElementById("card-action-select").value;
    const pointSelectionValid = cardAction === "add" || document.getElementById("card-point-select").value !== "";
    document.getElementById("save-idle-button").disabled = !(enabled && hasConfig);
    document.getElementById("save-card-button").disabled = !(enabled && hasConfig && pointSelectionValid);
    document.getElementById("save-opt-button").disabled = !(enabled && hasConfig);
}

async function moveAxis(button) {
    const stepperId = Number(button.dataset.stepperId);
    const fixedSteps = button.dataset.fixedSteps === undefined
        ? null
        : Number(button.dataset.fixedSteps);
    const direction = fixedSteps === null ? Number(button.dataset.direction) : Math.sign(fixedSteps);
    const stepsInput = document.getElementById(`axis-steps-${stepperId}`);
    const steps = fixedSteps === null ? stepsInput.valueAsNumber : Math.abs(fixedSteps);

    if (!Number.isInteger(steps) || steps < 1 || steps > 2048) {
        showMessage("Jog steps must be a whole number between 1 and 2048.", true);
        if (fixedSteps === null) {
            stepsInput.focus();
        }
        return;
    }

    document.querySelectorAll(`[data-stepper-id="${stepperId}"]`).forEach(item => {
        item.disabled = true;
    });

    try {
        const directionResult = await requestText("/post/setStepperForward", jsonOptions("POST", {
            stepperId,
            forward: direction > 0
        }));
        if (directionResult !== "success") {
            throw new Error(directionResult);
        }

        const result = await requestText("/post/runStepper", jsonOptions("POST", {
            stepperId,
            mode: "active",
            steps
        }));

        if (result !== "success") {
            throw new Error(result);
        }

        showMessage(`Stepper ${stepperId} movement started.`);
        await pollStatus();
    } catch (error) {
        showMessage(`Unable to move stepper ${stepperId}: ${error.message}`, true);
        await pollStatus();
    }
}

function renderCoordinateConfig() {
    renderIdlePoint();
    renderCardSelectors();
    renderCardTable();
    renderOptSelectors();
    renderOptTable();
    renderSelectedOptValue();
}

function renderIdlePoint() {
    document.getElementById("idle-point-value").textContent = formatPoint(coordinateConfig.idle);
}

function renderCardSelectors() {
    const bayIndex = Number(document.getElementById("card-bay-select").value || 0);
    const action = document.getElementById("card-action-select").value;
    const pointSelect = document.getElementById("card-point-select");
    const selectedCard = pointSelect.value;
    const cards = Object.keys(coordinateConfig.cardBays[bayIndex].cards)
        .sort((left, right) => Number(left) - Number(right));

    pointSelect.disabled = action !== "adjust";
    pointSelect.innerHTML = cards.length === 0
        ? '<option value="">No saved cards</option>'
        : cards.map(index => `<option value="${index}">Card ${Number(index) + 1}</option>`).join("");

    if (cards.includes(selectedCard)) {
        pointSelect.value = selectedCard;
    }

    setSaveButtonsEnabled(allAxesReady);
}

function renderCardTable() {
    const rows = [];
    coordinateConfig.cardBays.forEach((bay, bayIndex) => {
        Object.entries(bay.cards).forEach(([pointIndex, point]) => {
            rows.push(`
                <tr>
                    <td>${bayIndex + 1}</td>
                    <td>${Number(pointIndex) + 1}</td>
                    ${pointCells(point)}
                    <td>
                        <button class="card-update-button" type="button"
                            data-bay-index="${bayIndex}" data-card-index="${pointIndex}">
                            Update
                        </button>
                    </td>
                </tr>
            `);
        });
    });

    document.getElementById("card-coordinate-rows").innerHTML = rows.length > 0
        ? rows.join("")
        : '<tr class="empty-row"><td colspan="8">No card coordinates saved yet.</td></tr>';
}

function selectCardForUpdate(button) {
    const pointSelect = document.getElementById("card-point-select");
    document.getElementById("card-bay-select").value = button.dataset.bayIndex;
    document.getElementById("card-action-select").value = "adjust";
    renderCardSelectors();
    pointSelect.value = button.dataset.cardIndex;
    pointSelect.focus();
    setSaveButtonsEnabled(allAxesReady);
}

function renderOptSelectors() {
    const select = document.getElementById("opt-select");
    const selectedValue = select.value;
    select.innerHTML = coordinateConfig.opTs
        .map((opt, index) => `<option value="${index}">${escapeHtml(opt.name || `OPT ${index + 1}`)}</option>`)
        .join("");

    if (selectedValue !== "" && Number(selectedValue) < coordinateConfig.opTs.length) {
        select.value = selectedValue;
    }
}

function renderOptTable() {
    const rows = [];
    coordinateConfig.opTs.forEach((opt, optIndex) => {
        optPoints.forEach(([property, label]) => {
            rows.push(`
                <tr>
                    <td>${escapeHtml(opt.name || `OPT ${optIndex + 1}`)}</td>
                    <td>${label}</td>
                    ${pointCells(opt[property])}
                </tr>
            `);
        });
    });
    document.getElementById("opt-coordinate-rows").innerHTML = rows.join("");
}

function renderSelectedOptValue() {
    if (!coordinateConfig) {
        return;
    }

    const optIndex = Number(document.getElementById("opt-select").value || 0);
    const pointName = document.getElementById("opt-point-select").value;
    const point = coordinateConfig.opTs[optIndex][pointName];
    document.getElementById("selected-opt-value").textContent = formatPoint(point);
}

async function saveCardPoint() {
    if (!livePoint) {
        showMessage("Live stepper positions are not available.", true);
        return;
    }

    const bayIndex = Number(document.getElementById("card-bay-select").value);
    const action = document.getElementById("card-action-select").value;

    try {
        if (action === "add") {
            await requestJson(`/api/coordinates/card-bays/${bayIndex}/points`, jsonOptions("POST", livePoint));
            showMessage(`Card coordinate added to card bay ${bayIndex + 1}.`);
        } else {
            const pointIndex = Number(document.getElementById("card-point-select").value);
            await requestJson(`/api/coordinates/card-bays/${bayIndex}/points/${pointIndex}`, jsonOptions("PUT", livePoint));
            showMessage(`Card ${pointIndex + 1} in card bay ${bayIndex + 1} was updated.`);
        }

        await loadCoordinates();
    } catch (error) {
        showMessage(`Unable to save card coordinate: ${error.message}`, true);
    }
}

async function saveIdlePoint() {
    if (!livePoint) {
        showMessage("Live stepper positions are not available.", true);
        return;
    }

    try {
        await requestJson("/api/coordinates/idle", jsonOptions("PUT", livePoint));
        showMessage("Idle coordinate was updated.");
        await loadCoordinates();
    } catch (error) {
        showMessage(`Unable to save idle coordinate: ${error.message}`, true);
    }
}

async function saveOptPoint() {
    if (!livePoint) {
        showMessage("Live stepper positions are not available.", true);
        return;
    }

    const optIndex = Number(document.getElementById("opt-select").value);
    const pointName = document.getElementById("opt-point-select").value;
    const pointLabel = optPoints.find(([name]) => name === pointName)[1];

    try {
        await requestJson(`/api/coordinates/opts/${optIndex}/points/${pointName}`, jsonOptions("PUT", livePoint));
        showMessage(`${coordinateConfig.opTs[optIndex].name}: ${pointLabel} was updated.`);
        await loadCoordinates();
    } catch (error) {
        showMessage(`Unable to save OPT coordinate: ${error.message}`, true);
    }
}

function pointCells(point) {
    if (!point) {
        return axes.map(() => "<td>—</td>").join("");
    }
    return axes.map(axis => `<td>${point[axis.property]}</td>`).join("");
}

function formatPoint(point) {
    if (!point) {
        return "—";
    }
    return axes.map(axis => `${axis.name} ${point[axis.property]}`).join(" · ");
}

function setConnectionState(online, text) {
    const element = document.getElementById("connection-state");
    element.textContent = online ? "Controller online" : "Controller unavailable";
    element.title = text;
    element.className = `status-pill ${online ? "status-online" : "status-offline"}`;
}

function showMessage(text, isError = false) {
    const element = document.getElementById("message");
    element.textContent = text;
    element.className = `message message-visible${isError ? " message-error" : ""}`;

    clearTimeout(messageTimer);
    messageTimer = setTimeout(() => {
        element.className = "message";
    }, 4500);
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

async function initialize() {
    createAxisRows();
    populateStaticSelectors();

    document.getElementById("card-bay-select").addEventListener("change", renderCardSelectors);
    document.getElementById("card-action-select").addEventListener("change", renderCardSelectors);
    document.getElementById("card-point-select").addEventListener("change", () => setSaveButtonsEnabled(allAxesReady));
    document.getElementById("card-coordinate-rows").addEventListener("click", event => {
        const button = event.target.closest(".card-update-button");
        if (button) {
            selectCardForUpdate(button);
        }
    });
    document.getElementById("opt-select").addEventListener("change", renderSelectedOptValue);
    document.getElementById("opt-point-select").addEventListener("change", renderSelectedOptValue);
    document.getElementById("save-idle-button").addEventListener("click", saveIdlePoint);
    document.getElementById("save-card-button").addEventListener("click", saveCardPoint);
    document.getElementById("save-opt-button").addEventListener("click", saveOptPoint);

    try {
        await loadCoordinates();
    } catch (error) {
        showMessage(`Unable to load coordinates: ${error.message}`, true);
    }

    await pollStatus();
    window.setInterval(pollStatus, 1000);
}

initialize();
