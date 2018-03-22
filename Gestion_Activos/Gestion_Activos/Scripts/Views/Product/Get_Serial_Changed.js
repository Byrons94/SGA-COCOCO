
$(function () {
    $('#datepicker').datepicker({
        autoclose: true
    });
    $('#datepicker1').datepicker({
        autoclose: true
    });
});


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


$("#btn_buscar").click(function () {
    if (validar_busqueda()) {
        $("#table1").find('tbody').hide('fast');
        $("#loading_table").show('fast');
        consultar();
    }
});

 
function consultar() {
    var f_inicio = $("#datepicker").val();
    var f_fin = $("#datepicker1").val();
    console.log("asas");
    $.ajax({
        url: "/Product/Get_Serial_Changed_J",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                date_start: f_inicio,
                date_end: f_fin
            },
        success: function (result) {
            var result = JSON.parse(result);
            result = (result != "") ? result : {};
            update_dataTable(result);
            $("#msj-ajax").removeAttr('style');
        },
        error: function (XMLHttpRequest) {
            $("#msj-ajax").text("Error al conectar al servidor");
            $("#msj-ajax").css('color', 'red');
        }
    });
}


function validar_busqueda() {
    var f_inicio = $("#datepicker").val();
    var f_fin    = $("#datepicker1").val();

    $("#datepicker").removeAttr("style");
    $("#datepicker1").removeAttr("style");

    var bool = (f_inicio.trim() == "") ? false : true;
    if (!bool) {
        $("#datepicker").css('border-color', 'red');
        $("#datepicker").attr('placeholder', 'Debe agregar una fecha');
        return false;
    }
    var bool = (f_fin.trim() == "") ? false : true;
    if (!bool) {
        $("#datepicker1").css('border-color', 'red');
        $("#datepicker1").attr('placeholder', 'Debe agregar una fecha');
        return false;
    }
    return bool;
}


function update_dataTable(jsonresult) {
    console.log(jsonresult);
    if (table) {
        table.destroy();
    }
    table = $("#table1").DataTable({
        "destroy": true,
        "oLanguage": {
            "sEmptyTable": "No se encontraron registros",
        },
        "aaData": jsonresult,
        "aoColumns": [
            { data: "serial" },
            {
                data: "date",
                "type": "date",
                "render": function (value) {
                    if (value === null) return "";
                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));
                    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
                }
            },
            { data: "details"},
            { data: "user" }
        ]
    }).draw();
    $("#table1").find('tbody').show('fast');
    $("#loading_table").hide('fast');
}