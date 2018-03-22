
$(function () {
    $('#datepicker').datepicker({
        autoclose: true
    });
    $('#datepicker1').datepicker({
        autoclose: true
    });
});

$(".select2").select2();

var table;
$(document).ready(function (){
    table = $('#table1').DataTable({
        "paging":       true,
        "lengthChange": true,
        "searching":    true,
        "ordering":     true,
        "info":         true,
        "autoWidth":    false,
        aLengthMenu: [
        [10, 50, 100, 200, -1],
        [10, 50, 100, 200, "Total"]
        ],
        iDisplayLength: 10
    });
    //$(".select2").select2();
});

$("#btn_buscar").click(function () {
    if (validar_busqueda()) {
        $("#table1").find('tbody').hide('fast');
        $("#loading_table").show('fast');
        consulta_ajax();
    }
});

$('input[type=text]').on('keydown', function (e) {
    if (e.which == 13) {
        if (validar_busqueda()) {
            $("#table1").find('tbody').hide('fast');
            $("#loading_table").show('fast');
            consulta_ajax();
        }
    }
});

function validar_busqueda() {
    var date1 = $("#datepicker").val();
    $("#datepicker").removeAttr("style");
    var bool = (date1 == "") ? false : true;
    if (!bool) {
        $("#datepicker").css('border-color', 'red');
        $("#datepicker").attr('placeholder', 'No puede realizar una busqueda en blanco');
    }
    var date2 = $("#datepicker1").val();
    $("#datepicker1").removeAttr("style");
    var bool = (date2 == "") ? false : true;
    if (!bool) {
        $("#datepicker1").css('border-color', 'red');
        $("#datepicker1").attr('placeholder', 'No puede realizar una busqueda en blanco');
    }
    return bool;
}

function consulta_ajax() {
    var finicio  = $("#datepicker").val();
    var ffin     = $("#datepicker1").val();
    var category = $(".categorias").val();
    var movement = $(".movimientos").val();
    $.ajax({
        url: "/Report/Get_Products_Movements_Json",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                date_start: finicio,
                date_end: ffin,
                categorias: JSON.stringify(category),
                movements:  JSON.stringify(movement)
            },
        success: function (result) {    
            result = (result!="")?result: {};
            update_dataTable(result);
            $("#msj-ajax").removeAttr('style');
        },
        error: function (XMLHttpRequest) {
            $("#msj-ajax").text("Error al conectar al servidor");
            $("#msj-ajax").css('color', 'red');
        }
    });
}

function update_dataTable(jsonresult) {
    if(table){
        table.destroy();
    }
    console.log(jsonresult);
    table =  $("#table1").DataTable({
        "destroy": true,
        "oLanguage": {
            "sEmptyTable": "No se encontraron registros",
        },
        "aaData": jsonresult,
        "aoColumns": [
             {
                 data: "date_movement",
                 "type": "date",
                 "render": function (value) {
                     if (value === null) return "";
                     var pattern = /Date\(([^)]+)\)/;
                     var results = pattern.exec(value);
                     var dt = new Date(parseFloat(results[1]));
                     return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                 }
             },
            { data: "local_name" },
            { data: "membership_number" },
            { data: "num_doc" },
            { data: "type_movement" },
            {
                data: "product",
                "render": function (value) {
                    return value.category;
                }
            },
            {
                data: "product",
                "render": function (value) {
                    return value.description;
                }
            },
            {
                data: "product",
                "render": function (value) {
                    return value.serial_number;
                }
            }
        ],
        aLengthMenu: [
           [10, 50, 100, 200, -1],
           [10, 50, 100, 200, "Total"]
        ],
        iDisplayLength: 10
    }).draw();
    $("#loading_table").hide('fast');
    $("#table1").find('tbody').show('fast');
}

function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}
