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

    $("#btn_afiliado").click(function () {
        if (validar_busqueda()) {
            $("#table1").find('tbody').hide('fast');
            $("#loading_table").show('fast');
            consulta_series();
        }
    });

$('input[type=text]').on('keydown', function (e) {
    if (e.which == 13) {
        if (validar_busqueda()) {
            $("#table1").find('tbody').hide('fast');
            $("#loading_table").show('fast');
            consulta_series();
        }
    }
});

function validar_busqueda() {
    var busqueda = $("#parametro").val();
    $("#parametro").removeAttr("style");
    var bool = (busqueda.trim() == "") ? false : true;
    if (!bool) {
        $("#parametro").css('border-color', 'red');
        $("#parametro").attr('placeholder', 'No puede realizar una busqueda en blanco');
    }
    return bool;
}

function consulta_series() {
    var busqueda = $("#parametro").val();
    $.ajax({
        url: "/Product/Get_Serial_By_Search",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                search: busqueda
            },
        success: function (result) {
            var result = JSON.parse(result);
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

function update_dataTable(jsonresult){
    if(table){
        table.destroy();
    }
    table =  $("#table1").DataTable({
        "destroy": true,
        "oLanguage": {
            "sEmptyTable": "No se encontraron registros",
        },
        "aaData": jsonresult,
        "aoColumns": [
            { data: "serial" },
            { data: "state"  },
            {
                data: "date",
                "type": "date",
                "render":function (value) {
                    if (value === null) return "";
                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));
                    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();}
            },
           { data: "other" },
           {
               "mData": null,
               "bSortable": false,
               "mRender": function (data) { return '<a type="button" class="btn btn-block btn-info btn-sm" href="/Product/Get_Serial_Details?serial=' + data.serial + '">Ver</a>'; }
           }
        ]
    }).draw();
    $("#table1").find('tbody').show('fast');
    $("#loading_table").hide('fast');
}

function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}

/* 
a que producto le pertenece
todo el historial de la misma
estado de la serie (nueva o usada)
estado de instalacion (bodega, en camino, instalada, retirada, desechada
*/