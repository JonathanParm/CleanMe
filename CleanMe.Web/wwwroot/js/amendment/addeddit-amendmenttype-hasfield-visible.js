function toggleAmendmentFields(flags) {
    debugger;

    $('[data-field=clientId]').toggle(flags.hasClientId);
    $('[data-field=areaId]').toggle(flags.hasAreaId);
    $('[data-field=assetLocationId]').toggle(flags.hasAssetLocationId);
    $('[data-field=itemCodeId]').toggle(flags.hasItemCodeId);
    $('[data-field=assetId]').toggle(flags.hasAssetId);
    $('[data-field=cleanFrequencyId]').toggle(flags.hasCleanFrequencyId);
    $('[data-field=staffId]').toggle(flags.hasStaffId);
    $('[data-field=rate]').toggle(flags.hasRate);
    $('[data-field=isAccessable]').toggle(flags.hasIsAccessable);
}

$('#AmendmentCurrent_amendmentTypeId').change(function () {
    /*debugger;*/

    var amendmentTypeId = $(this).val();
    if (amendmentTypeId) {
        $.getJSON('/Amendment/GetAmendmentTypeFields', { amendmentTypeId }, function (flags) {
            toggleAmendmentFields(flags);
        });
        //else {
        //    toggleAmendmentFields({
        //        hasClientId: false,
        //        hasArea: false,
        //        hasAssetLocation: false,
        //        hasAsset: false,
        //        hasAssetType: false,
        //        hasCleanFrequency: false,
        //        hasStaffId: false,
        //        hasRate: false,
        //        hasIsAccessable: false
        //    });
        //}
    }
});

// For initial load (existing amendments)
$(document).ready(function () {
    console.log("Amendment type fields handliong script loaded.");
    /*debugger;*/

    var existingAmendmentTypeId = $('#AmendmentCurrent_amendmentTypeId').val();
    if (existingAmendmentTypeId) {
        $.getJSON('/Amendment/GetItemCodeHasFields', { amendmentTypeId: existingAmendmentTypeId }, function (flags) {
            toggleAmendmentFields(flags);
        });
    }
});
