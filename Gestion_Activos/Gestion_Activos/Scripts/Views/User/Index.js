
$(document).ready(function () {
    $('#table1').DataTable({
        "paging": false,
        "lengthChange": false,
        "searching": false,
        "ordering": true,
        "info": false,
        "autoWidth": true
    });
});

$("#create_user").click(function () {
    if($("#rolList").empty()){
        var rols = get_all_rols("#rolList");
    }
});

function edit_User(value){
    var datos = get_user(value);
}

function get_user(id){
    if(id!="" || id!=null){
        $.ajax({
            url: '/User/Get_Json_User_Id',
            type: 'POST',
            dataType: 'text',
            traditional: true,
            data: {id_user: id},
            success: function (result){
                var datos = JSON.parse(result);
                llenar_form_edit(datos);
            },
            error: function (XMLHttpRequest) {
                alert("poner aqui el llamado al modal de la excepcion");
            }
        });
    }
}   

function get_all_rols(modal_id){
    $.ajax({
        url: '/Rol_Permissions/Get_Rols',
        type: 'POST',
        dataType: "json",
        traditional: true,
        success: function (result){
            llenar_combo_rols(result, modal_id);
        },
        error: function (XMLHttpRequest) {
            alert("poner aqui el llamado al modal de la excepcion");
        }
    });
}

function llenar_combo_rols(json_object, modal_id){
    $(json_object).each(function(key, value){
        $.each(value, function(key, value){
            $(modal_id).append($("<option></option>")
                .attr("value",key)
                .text(value)); 
        });
    }); 
    $(modal_id).val($("#rolList_id").val());
    $("#rolList_id").val("");
}
    
function llenar_form_edit(datos){
    $("#rolList_id").val(datos[0].rol);
    if($("#rolList_e").empty()){
        var rols = get_all_rols("#rolList_e");
    }
    $("#id_user").val(datos[0].id);
    $("#nombre_e").val(datos[0].name);
    $("#apellido_e").val(datos[0].last_name);
    $("#login_e").val(datos[0].login);
    $("#password_e").val(datos[0].password);
    $("#cod_tec_e").val(datos[0].cod_tec);
    $('#modal_user_edit').modal('show');
        
    if(datos[0].status == true){
        $('#estatus_activo_e').attr('checked', 'checked');
    }
    else{
        $('#estatus_inactivo_e').attr('checked', 'checked');
    }
}

function delete_User(id){
    $("#inp_eliminar").val(id);
}
    
$("#confirm_delete_user").click(function(){
    var id = $("#inp_eliminar").val();
    $.ajax({
        url: '/User/Delete_User_Id',
        type: 'POST',
        dataType: "json",
        traditional: true,
        data: {
            id_user: id
        },
        success: function (result){
            location.reload();
        },
        error: function (XMLHttpRequest) {
            alert("error al cargar intente de nuevo");
        }
    });
    return false;
});