$(document).ready(function () {
    $('#clientId').change(function () {
        /*debugger;*/

        var clientId = $('#AmendmentCurrent_clientId').val();
        var areaId = $('#AmendmentCurrent_areaId').val();
        var locationId = $('#AmendmentCurrent_assetLocationId').val();
        var itemCodeId = $('#AmendmentCurrent_itemCodeId').val();

        $('#AmendmentCurrent_areaId').empty();
        $('#AmendmentCurrent_assetLocationId').empty();
        $('#AmendmentCurrent_itemCodeId').empty();
        $('#AmendmentCurrent_assetId').empty();

        if (clientId) {
            /*console.log("Loading areas for clientId:", clientId);*/

            $.getJSON(`/Amendment/GetAreas?clientId=${clientId}`, function (data) {
                /*console.log("Received area data:", data);*/
                $('#AmendmentCurrent_areaId').append($('<option>').text('-- Select Area --').attr('value', ''));
                $.each(data, function (i, item) {
                    /*console.log("Adding item:", item);*/
                    $('#AmendmentCurrent_areaId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetAssetLocations?clientId=${clientId}&areaId=${areaId}`, function (data) {
                $('#AmendmentCurrent_assetLocationId').append($('<option>').text('-- Select area --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#AmendmentCurrent_assetLocationId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetItemCodes?clientId=${clientId}`, function (data) {
                $('#AmendmentCurrent_itemCodeId').append($('<option>').text('-- Select item code --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#AmendmentCurrent_itemCodeId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetAssets?clientId=${clientId}&areaId=${areaId}&assetLocationId=${locationId}&itemCodeId=${itemCodeId}`, function (data) {
                $('#AmendmentCurrent_assetId').append($('<option>').text('-- Select asset --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#AmendmentCurrent_assetId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });
        }
    });

    $('#AmendmentCurrent_areaId').change(function () {
        /*debugger;*/
        var clientId = $('#AmendmentCurrent_clientId').val();
        var areaId = $('#AmendmentCurrent_areaId').val();
        var locationId = $('#AmendmentCurrent_assetLocationId').val();

        $('#AmendmentCurrent_assetLocationId').empty();
        $('#AmendmentCurrent_assetId').empty();

        if (clientId && areaId) {
            $.getJSON(`/Amendment/GetAssetLocations?clientId=${clientId}&areaId=${areaId}`, function (data) {
                $('#AmendmentCurrent_areaId').append($('<option>').text('-- Select area --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#AmendmentCurrent_areaId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

            $.getJSON(`/Amendment/GetAssets?AmendmentCurrent_clientId=${clientId}&areaId=${areaId}&assetLocationId=${locationId}`, function (data) {
                $('#AmendmentCurrent_assetId').append($('<option>').text('-- Select asset --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#AmendmentCurrent_assetId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });
        }
    });

    $('#AmendmentCurrent_assetLocationId').change(function () {
        debugger;
        var clientId = $('#AmendmentCurrent_clientId').val();
        var areaId = $('#AmendmentCurrent_areaId').val();
        var locationId = $('#AmendmentCurrent_assetLocationId').val();

        $('#AmendmentCurrent_assetId').empty();

        if (clientId && locationId) {
            $.getJSON(`/Amendment/GetAssets?clientId=${clientId}&areaId=${areaId}&assetLocationId=${locationId}`, function (data) {
                $('#AmendmentCurrent_assetId').append($('<option>').text('-- Select asset --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#AmendmentCurrent_assetId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });
        }
    });
});