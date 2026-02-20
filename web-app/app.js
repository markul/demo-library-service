const resources = {
    books: "/api/books",
    ebooks: "/api/ebooks",
    subscriptionTypes: "/api/subscriptions/types",
    subscriptions: "/api/subscriptions",
    payments: "/api/payments"
};

const defaultApiBaseUrl = "http://localhost:5149";
const apiBaseUrlStorageKey = "libraryService.apiBaseUrl";

const apiBaseUrlInput = document.getElementById("apiBaseUrl");
const refreshAllButton = document.getElementById("refreshAllButton");

const searchForm = document.getElementById("searchForm");
const searchNameInput = document.getElementById("searchName");
const searchSuggestions = document.getElementById("searchSuggestions");
const searchSubmitButton = searchForm ? searchForm.querySelector("button[type='submit']") : null;
const searchStatus = document.getElementById("searchStatus");
const searchResults = document.getElementById("searchResults");
const hasSearchUi = Boolean(searchForm && searchNameInput && searchSuggestions && searchStatus && searchResults);

const autocompleteMinChars = 3;
const autocompleteDelayMs = 250;
const autocompleteMaxItems = 8;

let autocompleteTimer = null;
let autocompleteAbortController = null;

function normalizeBaseUrl(value) {
    const normalized = String(value || "").trim().replace(/\/+$/, "");
    return normalized.length > 0 ? normalized : defaultApiBaseUrl;
}

function initializeApiBaseUrlInput() {
    if (!apiBaseUrlInput) {
        return;
    }

    let initial = apiBaseUrlInput.value;
    try {
        const saved = localStorage.getItem(apiBaseUrlStorageKey);
        if (saved && saved.trim().length > 0) {
            initial = saved;
        }
    } catch {
        // ignore storage errors
    }

    apiBaseUrlInput.value = normalizeBaseUrl(initial);

    const persist = () => {
        const normalized = normalizeBaseUrl(apiBaseUrlInput.value);
        apiBaseUrlInput.value = normalized;
        try {
            localStorage.setItem(apiBaseUrlStorageKey, normalized);
        } catch {
            // ignore storage errors
        }
    };

    apiBaseUrlInput.addEventListener("change", persist);
    apiBaseUrlInput.addEventListener("blur", persist);
}

function getApiBaseUrl() {
    if (apiBaseUrlInput) {
        return normalizeBaseUrl(apiBaseUrlInput.value);
    }

    try {
        return normalizeBaseUrl(localStorage.getItem(apiBaseUrlStorageKey));
    } catch {
        return defaultApiBaseUrl;
    }
}

function setSearchActive(isActive) {
    if (!hasSearchUi) {
        return;
    }

    searchForm.classList.toggle("is-active", isActive);
    searchForm.setAttribute("aria-busy", String(isActive));
    searchStatus.classList.toggle("active", isActive);
    searchNameInput.disabled = isActive;
    if (searchSubmitButton) {
        searchSubmitButton.disabled = isActive;
        searchSubmitButton.textContent = isActive ? "Searching..." : "Search";
    }
}

function formatValue(value) {
    if (value === null || value === undefined) {
        return "-";
    }

    if (typeof value === "object") {
        return JSON.stringify(value);
    }

    return String(value);
}

function renderCollection(container, payload) {
    if (!container) {
        return;
    }

    container.innerHTML = "";

    if (!Array.isArray(payload) || payload.length === 0) {
        const empty = document.createElement("p");
        empty.className = "empty-text";
        empty.textContent = "No data.";
        container.appendChild(empty);
        return;
    }

    payload.forEach((item) => {
        const card = document.createElement("article");
        card.className = "result-card";
        const keys = Object.keys(item).slice(0, 6);

        keys.forEach((key) => {
            const line = document.createElement("p");
            const label = document.createElement("strong");
            label.textContent = key;
            line.appendChild(label);
            line.append(`: ${formatValue(item[key])}`);
            card.appendChild(line);
        });

        container.appendChild(card);
    });
}

async function fetchResource(path) {
    const response = await fetch(`${getApiBaseUrl()}${path}`);
    if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
    }

    return response.json();
}

function setStatus(id, message, isError = false) {
    const statusElement = document.getElementById(`${id}Status`);
    if (!statusElement) {
        return;
    }

    statusElement.textContent = message;
    statusElement.classList.toggle("error", isError);
}

async function loadResource(id) {
    const endpoint = resources[id];
    const container = document.getElementById(`${id}Results`);
    if (!endpoint || !container) {
        return;
    }

    setStatus(id, "Loading...");
    try {
        const data = await fetchResource(endpoint);
        const collection = Array.isArray(data) ? data : [data];
        setStatus(id, `Loaded ${collection.length} item(s).`);
        renderCollection(container, collection);
    } catch (error) {
        setStatus(id, `Failed: ${error.message}`, true);
        container.innerHTML = "";
    }
}

async function loadAll() {
    await Promise.all(Object.keys(resources).map((id) => loadResource(id)));
}

async function fetchEbooksByName(name, signal) {
    const query = encodeURIComponent(name);
    const response = await fetch(`${getApiBaseUrl()}/api/ebooks/search?name=${query}`, signal ? { signal } : undefined);
    if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
    }

    return response.json();
}

function normalizeArray(payload) {
    if (payload === null || payload === undefined) {
        return [];
    }

    return Array.isArray(payload) ? payload : [payload];
}

function getSuggestionLabel(item) {
    if (item === null || item === undefined) {
        return "";
    }

    if (typeof item === "string") {
        return item.trim();
    }

    if (typeof item !== "object") {
        return String(item).trim();
    }

    const preferredKeys = ["name", "title", "bookName", "ebookName"];
    for (const key of preferredKeys) {
        if (typeof item[key] === "string" && item[key].trim().length > 0) {
            return item[key].trim();
        }
    }

    for (const [key, value] of Object.entries(item)) {
        if (typeof value === "string" && key.toLowerCase().includes("name") && value.trim().length > 0) {
            return value.trim();
        }
    }

    return "";
}

function clearSearchSuggestions() {
    if (!hasSearchUi) {
        return;
    }

    searchSuggestions.innerHTML = "";
    searchSuggestions.hidden = true;
}

function renderSearchSuggestions(items) {
    if (!hasSearchUi) {
        return;
    }

    const limited = items.slice(0, autocompleteMaxItems);
    searchSuggestions.innerHTML = "";

    if (limited.length === 0) {
        searchSuggestions.hidden = true;
        return;
    }

    const dedupe = new Set();
    limited.forEach((item) => {
        const label = getSuggestionLabel(item);
        if (label.length === 0) {
            return;
        }

        const normalized = label.toLowerCase();
        if (dedupe.has(normalized)) {
            return;
        }
        dedupe.add(normalized);

        const row = document.createElement("li");
        const button = document.createElement("button");
        button.type = "button";
        button.textContent = label;
        button.addEventListener("click", () => {
            searchNameInput.value = label;
            clearSearchSuggestions();
            searchEbooksByName(label);
        });
        row.appendChild(button);
        searchSuggestions.appendChild(row);
    });

    searchSuggestions.hidden = searchSuggestions.childElementCount === 0;
}

async function requestAutocomplete(name) {
    if (!hasSearchUi) {
        return;
    }

    if (autocompleteAbortController) {
        autocompleteAbortController.abort();
    }
    autocompleteAbortController = new AbortController();

    try {
        const payload = await fetchEbooksByName(name, autocompleteAbortController.signal);
        if (searchNameInput.value.trim() !== name) {
            return;
        }

        renderSearchSuggestions(normalizeArray(payload));
    } catch (error) {
        if (error.name === "AbortError") {
            return;
        }

        clearSearchSuggestions();
    }
}

async function searchEbooksByName(name) {
    if (!hasSearchUi) {
        return;
    }

    if (autocompleteTimer) {
        clearTimeout(autocompleteTimer);
        autocompleteTimer = null;
    }
    if (autocompleteAbortController) {
        autocompleteAbortController.abort();
    }

    setSearchActive(true);
    searchStatus.textContent = "Searching...";
    searchStatus.classList.remove("error");
    searchResults.innerHTML = "";
    clearSearchSuggestions();

    try {
        const data = normalizeArray(await fetchEbooksByName(name));
        searchStatus.textContent = `Found ${data.length} item(s).`;
        renderCollection(searchResults, data);
    } catch (error) {
        searchStatus.textContent = `Search failed: ${error.message}`;
        searchStatus.classList.add("error");
    } finally {
        setSearchActive(false);
    }
}

function initializeResourceButtons() {
    if (refreshAllButton) {
        refreshAllButton.addEventListener("click", () => {
            loadAll();
        });
    }

    document.querySelectorAll("[data-refresh]").forEach((button) => {
        button.addEventListener("click", () => {
            const id = button.getAttribute("data-refresh");
            if (!id) {
                return;
            }
            loadResource(id);
        });
    });
}

function initializeSearch() {
    if (!hasSearchUi) {
        return;
    }

    searchNameInput.addEventListener("input", () => {
        const value = searchNameInput.value.trim();

        if (autocompleteTimer) {
            clearTimeout(autocompleteTimer);
            autocompleteTimer = null;
        }

        if (value.length < autocompleteMinChars) {
            if (autocompleteAbortController) {
                autocompleteAbortController.abort();
            }
            clearSearchSuggestions();
            return;
        }

        autocompleteTimer = setTimeout(() => {
            requestAutocomplete(value);
        }, autocompleteDelayMs);
    });

    searchNameInput.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            clearSearchSuggestions();
        }
    });

    document.addEventListener("click", (event) => {
        if (!searchForm.contains(event.target)) {
            clearSearchSuggestions();
        }
    });

    searchForm.addEventListener("submit", (event) => {
        event.preventDefault();
        const value = searchNameInput.value.trim();
        if (value.length === 0) {
            searchStatus.textContent = "Enter a name.";
            searchStatus.classList.add("error");
            return;
        }

        searchEbooksByName(value);
    });
}

initializeApiBaseUrlInput();
initializeResourceButtons();
initializeSearch();
