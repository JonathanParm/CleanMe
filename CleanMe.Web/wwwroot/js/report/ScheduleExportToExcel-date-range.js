function formatDateLocal(date) {
    return date.getFullYear() + "-" +
        String(date.getMonth() + 1).padStart(2, "0") + "-" +
        String(date.getDate()).padStart(2, "0");
}

function updateMonthDateRange() {
    const year = parseInt(document.getElementById("year")?.value);
    const month = parseInt(document.getElementById("month")?.value);

    const first = new Date(year, month - 1, 1);
    const last = new Date(year, month, 0);

    document.getElementById("DateFrom").value = formatDateLocal(first);
    document.getElementById("DateTo").value = formatDateLocal(last);
}

function toggleDateRangeInputs() {
    /*debugger;*/

    /*const type = document.getElementById("dateRangeType").value;*/
    /*const isMonth = type === "Month";*/
    const isMonth = true; // Force month view for now

    document.getElementById("month-year-wrapper").style.display = isMonth ? "block" : "none";
    document.getElementById("week-range-wrapper").style.display = isMonth ? "none" : "block";

    if (isMonth) updateMonthDateRange();
}

document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("dateRangeType")?.addEventListener("change", toggleDateRangeInputs);
    document.getElementById("year")?.addEventListener("change", updateMonthDateRange);
    document.getElementById("month")?.addEventListener("change", updateMonthDateRange);

    toggleDateRangeInputs(); // initial load

    debugger;
    document.querySelector("form").addEventListener("submit", function () {
        console.log("📨 Form is about to submit");

        let from = document.getElementById("DateFrom")?.value;
        let to = document.getElementById("DateTo")?.value;

        console.log("DateFrom value before submit:", from);
        console.log("DateTo value before submit:", to);

        if (document.getElementById("dateRangeType").value === "Month") {
            updateMonthDateRange();
        }

        from = document.getElementById("DateFrom")?.value;
        to = document.getElementById("DateTo")?.value;

        console.log("DateFrom value after submit:", from);
        console.log("DateTo value after submit:", to);
    });
});
