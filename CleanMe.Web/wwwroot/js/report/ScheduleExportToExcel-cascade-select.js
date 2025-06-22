// wwwroot/js/report-filters.js

const selectListDependencies = {
    clientId: ["regionId", "areaId", "assetLocationId", "assetId"],
    cleanerId: ["regionId", "areaId", "assetLocationId", "assetId"],
    regionId: ["areaId", "assetLocationId", "assetId"],
    areaId: ["assetLocationId", "assetId"],
    assetLocationId: ["assetId"]
};

function capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

function gatherFilterParamsFor(targetId) {
    const dependencies = {
        regionId: ["clientId", "cleanerId"],
        areaId: ["clientId", "cleanerId", "regionId"],
        assetLocationId: ["clientId", "cleanerId", "regionId", "areaId"],
        assetId: ["clientId", "cleanerId", "regionId", "areaId", "assetLocationId"]
    };

    const keys = dependencies[targetId] || [];
    const params = {};
    keys.forEach(key => {
        const val = document.getElementById(key)?.value;
        if (val) params[key] = val;
    });

    return params;
}

function updateSelectList(targetId) {
    const queryParams = gatherFilterParamsFor(targetId);
    const url = `/Report/Get${capitalize(targetId)}SelectList`;

    fetch(`${url}?${new URLSearchParams(queryParams)}`)
        .then(res => res.json())
        .then(data => {
            const select = document.getElementById(targetId);
            if (!select) return;
            select.innerHTML = ""; // Clear previous options
            data.forEach(opt => {
                const option = document.createElement("option");
                option.value = opt.value;
                option.text = opt.text;
                select.appendChild(option);
            });
        });
}

function handleSelectChange(changedId) {
    const affectedIds = selectListDependencies[changedId] || [];
    affectedIds.forEach(updateSelectList);
}

document.addEventListener("DOMContentLoaded", () => {
    ["clientId", "cleanerId", "regionId", "areaId", "assetLocationId"].forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener("change", () => handleSelectChange(id));
        }
    });
    console.log("script loaded");
});
