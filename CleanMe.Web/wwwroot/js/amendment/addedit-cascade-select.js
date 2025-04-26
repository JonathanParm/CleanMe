$(document).ready(function () {
    $('#ClientId').change(function () {
        var clientId = $(this).val();
        var areaId = $('#areaId').val();
        var locationId = $('#assetLocationId').val();

        $('#areaId').empty();
        $('#assetLocationId').empty();
        $('#assetId').empty();

        if (clientId && areaId && locationId) {
            $.getJSON(`/Amendment/GetAreas?clientId=${clientId}`, function (data) {
                $('#areaId').append($('<option>').text('-- Select Area --').attr('value', ''));
                $.each(data, function (i, item) {
                    $('#areaId').append($('<option>').text(item.text).attr('value', item.value));
                });
            });

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

    $('#areaId').change(function () {
        var clientId = $('#clientId').val();
        var areaId = $(this).val();
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
        var clientId = $('#clientId').val();
        var areaId = $('#areaId').val();
        var locationId = $(this).val();

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