var table;
$(document).ready(function (){
    table = $('#table1').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": false,
        "info": true,
        "autoWidth": true
    });
});

//crear 
$(".btn-submit").click(function () {
    var desc = $("#descripcion").val();
    if  (desc.trim() != "") {
        ajax_crear(desc);
    }
    else {
        $("#descripcion").css('border-color', '#dd4b39');
        $("#descripcion").attr("placeholder", "Debe agregar una descripción");
    }
});

function ajax_crear(descripcion) {
    $.ajax({
        url: '/Configuration/Create_Category',
        type: 'POST',
        dataType: 'text',
        traditional: true,
        data: { desc: descripcion },
        success: function (result) {
            location.reload();
        },
        error: function (XMLHttpRequest) {
            alert("Error al llamar método");
        }
    });
}

//modificar
$(".btn-modificar").click(function () {
    $("#id_modificar").val($(this).attr('id').trim());
    $("#descripcion_e").val($(this).attr('tg2').trim());
});

$("#confirm_update").click(function () {
    var id   = $("#id_modificar").val();
    var descr = $("#descripcion_e").val();
    if (descr.trim() != "") {
        ajax_modificar(id, descr);
    }
    else {
        $("#descripcion_e").css('border-color', '#dd4b39');
        $("#descripcion_e").attr("placeholder", "Debe agregar una descripción");
    }
});

function ajax_modificar(idval, description) {
    console.log(description);
    $.ajax({
        url: '/Configuration/Update_Category',
        type: 'POST',
        dataType: 'text',
        traditional: true,
        data: {
            id:   idval,
            desc: description
        },
        success: function (result) {
            location.reload();
        },
        error: function (XMLHttpRequest) {
            alert("Error al llamar método");
        }
    });
}
    
//eliminar
$(".btn-eliminar").click(function () {
    $("#id_eliminar").val($(this).attr('id'));
});

$("#confirm_delete").click(function () {
    var id = $("#id_eliminar").val();
    ajax_eliminar(id);
});

function ajax_eliminar(idval){
    $.ajax({
        url: '/Configuration/Delete_Category',
        type: 'POST',
        dataType: 'text',
        traditional: true,
        data: {
            id: idval
        },
        success: function (result) {
            location.reload();
        },
        error: function (XMLHttpRequest) {
            alert("Error al llamar método");
        }
    });
}