function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = 'deleteSpan_' + uniqueId;
    var confirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeleteSpan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeleteSpan).hide();
    }
}

function confirmDeleteRole(uniqueId, isDeleteClicked) {
    var deleteRoleSpan = "deleteRoleSpan_" + uniqueId;
    var confirmDeleteRoleSpan = "confirmDeleteRoleSpan_" + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteRoleSpan).hide();
        $('#' + confirmDeleteRoleSpan).show();
    }
    else {
        $('#' + deleteRoleSpan).show();
        $('#' + confirmDeleteRoleSpan).hide();
    }
}