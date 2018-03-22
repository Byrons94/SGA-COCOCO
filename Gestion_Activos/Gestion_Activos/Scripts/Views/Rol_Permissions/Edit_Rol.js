$(document).ready(function () {
    var editable = $("#edit").val();
    if (editable) {
        $("input[type=checkbox]").each(function(){
            $(this).attr('disabled', 'disabled');
        });
    }
});

$('input[type="checkbox"].flat-red').iCheck({
    checkboxClass: 'icheckbox_flat-green',
    radioClass: 'iradio_flat-green'
});

$("#click_post").click(function () {
    var array     = [];
    var $form     = $("#formEdit");
    var id_edit_r = $("#id_Rol").val();
    var name_r    = $("#name_Rol").val();

    $('input:checked').each(function () {
        array.push($(this).val());
    });

    $("#modal_editar_rol").modal("show");
    $.ajax({
        url: '/Rol_Permissions/Save_Edited_Rol',
        type: 'POST',
        dataType: "json",
        traditional: true,
        data: {
            id_Rol: id_edit_r,
            name_rol: name_r,
            permisos: array
        },
        success: function (result) {
            $("#mjs").text("Guardado, redireccionando");
            window.location.href = "/Rol_Permissions/List_Roles";
        },
        error: function (XMLHttpRequest) {
            alert("poner aqui el llamado al modal de la excepcion"); 
        }
    });
    return false;
});