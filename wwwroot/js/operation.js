"use strict";

let mappedCardNames = new Set();

function normalizeCardName(cardName) {
    return cardName.trim().toLowerCase();
}

function updateCardActionButtons() {
    const cardName = normalizeCardName(document.getElementById("card-name").value);
    const mappingExists = mappedCardNames.has(cardName);

    document.getElementById("add-card").disabled = mappingExists;
    document.getElementById("delete-card").disabled = !mappingExists;
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

async function loadCardMappings() {
    const response = await fetch("/api/card-mappings");
    const mappings = await response.json();

    if (!response.ok)
        throw new Error(mappings.error || mappings.detail || "Unable to load card mappings.");

    renderCardMappings(mappings);
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
        const response = await fetch(`/api/card-mappings/${encodeURIComponent(cardName)}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            throw new Error(await getResponseError(response, "Unable to delete card mapping."));
        }

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
document.getElementById("card-mapping-table-body").addEventListener("click", selectCardMapping);
document.getElementById("delete-card").addEventListener("click", deleteCardMapping);
loadCardMappings().catch(error => {
    alert(error.message);
});
