"use strict";

let mappedCardNames = new Set();
let cardBays = [];

function normalizeCardName(cardName) {
    return cardName.trim().toLowerCase();
}

function updateCardActionButtons() {
    const cardName = normalizeCardName(document.getElementById("card-name").value);
    const mappingExists = mappedCardNames.has(cardName);
    const slotSelected = document.getElementById("card-slot").selectedIndex >= 0;

    document.getElementById("add-card").disabled = mappingExists || !slotSelected;
    document.getElementById("delete-card").disabled = !mappingExists;
}

function renderCardSlots() {
    const cardBayIndex = Number(document.getElementById("card-bay").value);
    const cards = cardBays[cardBayIndex]?.cards;
    const slotNumbers = cards && typeof cards === "object"
        ? Object.keys(cards)
            .map(Number)
            .filter(Number.isInteger)
            .sort((first, second) => first - second)
        : [];
    const cardSlotSelect = document.getElementById("card-slot");

    cardSlotSelect.replaceChildren(...slotNumbers.map(slotNumber => {
        const option = document.createElement("option");
        option.value = slotNumber;
        option.textContent = slotNumber + 1;
        return option;
    }));
    cardSlotSelect.disabled = slotNumbers.length === 0;
    updateCardActionButtons();
}

function renderCardSelectors(coordinates) {
    if (!Array.isArray(coordinates.cardBays))
        throw new Error("The coordinate configuration does not contain card bays.");

    cardBays = coordinates.cardBays;
    const cardBaySelect = document.getElementById("card-bay");
    cardBaySelect.replaceChildren(...cardBays.map((_, cardBayIndex) => {
        const option = document.createElement("option");
        option.value = cardBayIndex;
        option.textContent = cardBayIndex + 1;
        return option;
    }));
    cardBaySelect.disabled = cardBays.length === 0;
    renderCardSlots();
}

async function loadCardSelectors() {
    const response = await fetch("/api/coordinates/");
    const coordinates = await response.json();

    if (!response.ok)
        throw new Error(coordinates.error || coordinates.detail || "Unable to load card coordinates.");

    renderCardSelectors(coordinates);
}

function renderCardMappings(mappings) {
    const tableBody = document.getElementById("card-mapping-table-body");
    mappedCardNames = new Set(Object.keys(mappings).map(normalizeCardName));
    tableBody.replaceChildren();

    Object.entries(mappings)
        .sort(([firstName], [secondName]) => firstName.localeCompare(secondName))
        .forEach(([cardName, slot]) => {
            const row = tableBody.insertRow();
            row.dataset.cardName = cardName;
            [cardName, slot.cardBay + 1, slot.cardSlotNumber + 1].forEach(value => {
                const cell = row.insertCell();
                cell.textContent = value;
            });
        });

    updateCardActionButtons();
}

function cardMappingSlotExists(slot) {
    if (!Number.isInteger(slot?.cardBay) || !Number.isInteger(slot?.cardSlotNumber))
        return false;

    const cards = cardBays[slot.cardBay]?.cards;
    return cards != null && Object.prototype.hasOwnProperty.call(cards, slot.cardSlotNumber);
}

async function requestCardMappingDeletion(cardName) {
    const response = await fetch(`/api/card-mappings/${encodeURIComponent(cardName)}`, {
        method: "DELETE"
    });

    if (!response.ok)
        throw new Error(await getResponseError(response, `Unable to delete the mapping for ${cardName}.`));
}

async function removeInvalidCardMappings(mappings) {
    const invalidCardNames = Object.entries(mappings)
        .filter(([, slot]) => !cardMappingSlotExists(slot))
        .map(([cardName]) => cardName);

    await Promise.all(invalidCardNames.map(requestCardMappingDeletion));
    invalidCardNames.forEach(cardName => delete mappings[cardName]);
    return mappings;
}

async function loadCardMappings() {
    const response = await fetch("/api/card-mappings");
    const mappings = await response.json();

    if (!response.ok)
        throw new Error(mappings.error || mappings.detail || "Unable to load card mappings.");

    renderCardMappings(await removeInvalidCardMappings(mappings));
}

async function getResponseError(response, fallbackMessage) {
    const responseText = await response.text();
    if (!responseText)
        return `${fallbackMessage} (HTTP ${response.status})`;

    try {
        const result = JSON.parse(responseText);
        return result.error || result.detail || fallbackMessage;
    } catch {
        return responseText;
    }
}

async function initializeMachine() {
    if (!confirm("Warning: Check that all steppers are at the correct positions before initializing the machine. Continue?"))
        return;

    const button = document.getElementById("initialize-machine");
    button.disabled = true;

    try {
        const response = await fetch("/api/operations/initialize", { method: "POST" });
        if (!response.ok)
            throw new Error(await getResponseError(response, "Unable to initialize the machine."));

        alert("Machine initialized.");
    } catch (error) {
        alert(error.message);
    } finally {
        button.disabled = false;
    }
}

async function performCardOperation(operation) {
    const selectedRow = document.querySelector("#card-mapping-table-body tr.selected");
    if (!selectedRow) {
        alert(`Select a card mapping to ${operation}.`);
        return;
    }

    const button = document.getElementById(`${operation}-card`);
    button.disabled = true;

    try {
        const response = await fetch(`/api/operations/${operation}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                cardName: selectedRow.dataset.cardName,
                opt: Number(document.querySelector('input[name="OPT-option"]:checked').value)
            })
        });

        if (!response.ok)
            throw new Error(await getResponseError(response, `Unable to ${operation} the card.`));
    } catch (error) {
        alert(error.message);
    } finally {
        button.disabled = false;
    }
}

function selectCardMapping(event) {
    const row = event.target.closest("tr");
    if (!row)
        return;

    const tableBody = event.currentTarget;
    tableBody.querySelector("tr.selected")?.classList.remove("selected");
    row.classList.add("selected");
    document.getElementById("card-name").value = row.dataset.cardName;
    updateCardActionButtons();
}

async function deleteCardMapping() {
    const cardNameInput = document.getElementById("card-name");
    const cardName = cardNameInput.value.trim();
    if (!cardName) {
        alert("Enter a card name to delete.");
        cardNameInput.focus();
        return;
    }

    const deleteButton = document.getElementById("delete-card");
    deleteButton.disabled = true;

    try {
        await requestCardMappingDeletion(cardName);

        await loadCardMappings();
        alert(`Deleted ${cardName}.`);
        cardNameInput.value = "";
        cardNameInput.focus();
    } catch (error) {
        alert(error.message);
    } finally {
        updateCardActionButtons();
    }
}

async function addCardMapping() {
    const addButton = document.getElementById("add-card");
    const cardNameInput = document.getElementById("card-name");
    const cardName = cardNameInput.value.trim();

    if (!cardName) {
        alert("Enter a card name.");
        cardNameInput.focus();
        return;
    }

    addButton.disabled = true;

    try {
        const response = await fetch("/api/card-mappings", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                cardName,
                cardBay: Number(document.getElementById("card-bay").value),
                cardSlotNumber: Number(document.getElementById("card-slot").value)
            })
        });
        const result = await response.json();

        if (!response.ok)
            throw new Error(result.error || result.detail || "Unable to save card mapping.");

        await loadCardMappings();
        alert(`Added ${cardName}.`);
        cardNameInput.value = "";
        cardNameInput.focus();
    } catch (error) {
        alert(error.message);
    } finally {
        updateCardActionButtons();
    }
}

document.getElementById("add-card").addEventListener("click", addCardMapping);
document.getElementById("card-name").addEventListener("input", updateCardActionButtons);
document.getElementById("card-bay").addEventListener("change", renderCardSlots);
document.getElementById("card-mapping-table-body").addEventListener("click", selectCardMapping);
document.getElementById("delete-card").addEventListener("click", deleteCardMapping);
document.getElementById("initialize-machine").addEventListener("click", initializeMachine);
for (const operation of ["swipe", "tap", "barcode", "insert", "extract"]) {
    document.getElementById(`${operation}-card`).addEventListener("click", () => performCardOperation(operation));
}

async function initializePage() {
    await loadCardSelectors();
    await loadCardMappings();
}

initializePage().catch(error => {
    alert(error.message);
});
