function sortTable(column) {
    let sortColumn = new URLSearchParams(window.location.search).get("sortColumn");
    let sortOrder = new URLSearchParams(window.location.search).get("sortOrder") === "ASC" ? "DESC" : "ASC";
    let params = new URLSearchParams(window.location.search);
    params.set("sortColumn", column);
    params.set("sortOrder", sortOrder);
    window.location.search = params.toString();
}