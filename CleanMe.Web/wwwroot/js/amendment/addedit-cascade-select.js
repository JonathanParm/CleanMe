$(document).ready(function () {
    $('#clientId').change(function () {
        /*debugger;*/

        var clientId = $('#clientId').val();
        var areaId = $('#areaId').val();
        var locationId = $('#assetLocationId').val();
        var itemCodeId = $('#itemCodeId').val();

        $('#areaId').empty();
        $('#assetLocationId').empty();
        $('#itemCodeId').empty();
        $('#assetId').empty();

        if (clientId) {
            /*console.log("Loading areas for clientId:", clientId);*/

            $.getJSON(`/Amendment/GetAreas?clientId=${clientId}`, function (data) {
                /*console.log("Received area data:", data);*/
                $('#areaId').append($('<option>').text('-- Select Area --').attr('value', ''));
                $.each(data, function (i, item) {
                    /*console.log("Adding item:", item);*/
                    $('#areaId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetAssetLocations?clientId=${clientId}&areaId=${areaId}`, function (data) {
                $('#assetLocationId').append($('<option>').text('-- Select area --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#assetLocationId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetItemCodes?clientId=${clientId}`, function (data) {
                $('#itemCodeId').append($('<option>').text('-- Select item code --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#itemCodeId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetAssets?clientId=${clientId}&areaId=${areaId}&assetLocationId=${locationId}&itemCodeId=${itemCodeId}`, function (data) {
                $('#assetId').append($('<option>').text('-- Select asset --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#assetId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });
        }
    });

    $('#areaId').change(function () {
        /*debugger;*/
        var clientId = $('#clientId').val();
        var areaId = $('#areaId').val();
        var locationId = $('#assetLocationId').val();

        $('#assetLocationId').empty();
        $('#assetId').empty();

        if (clientId && areaId) {
            $.getJSON(`/Amendment/GetAssetLocations?clientId=${clientId}&areaId=${areaId}`, function (data) {
                $('#areaId').append($('<option>').text('-- Select area --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#areaId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetAssets?clientId=${clientId}&areaId=${areaId}&assetLocationId=${locationId}`, function (data) {
                $('#assetId').append($('<option>').text('-- Select asset --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#assetId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });
        }
    });

    $('#assetLocationId').change(function () {
        /*debugger;*/
        var clientId = $('#clientId').val();
        var areaId = $('#areaId').val();
        var locationId = $('#assetLocationId').val();

        $('#assetId').empty();

        if (clientId && locationId) {
            $.getJSON(`/Amendment/GetAssets?clientId=${clientId}&areaId=${areaId}&assetLocationId=${locationId}`, function (data) {
                $('#assetId').append($('<option>').text('-- Select asset --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#assetId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });
        }
    });
});