var table;
$(document).ready(function (){
    table = $('#table1').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": false,
        "info": true,
        "autoWidth": true,
         aLengthMenu: [
                        [10, 50, 100, 200, -1],
                        [10, 50, 100, 200, "Total"]
        ],
        iDisplayLength: 10
    });
});


$(function () {
    $('#datepicker').datepicker({
        autoclose: true
    });

    $('#datepicker1').datepicker({
        autoclose: true
    });
});


$("#btn_buscar").click(function () {
    var value = $('input[name="t_consulta"]:checked').val();
    if (value == '1') {
        if (validar_busqueda()) {
            $("#table1").find('tbody').hide('fast');
            $("#loading_table").show('fast');
            consultarRealizados();
        }
    } else {
        $("#table1").find('tbody').hide('fast');
        $("#loading_table").show('fast');
        consultaMantenimientosSinRealizar();
    }
});


$("input[name='t_consulta']").click(function(){
    $("#body1").toggle();
    $("#body2").toggle();
});


function consultarRealizados() {
    var f_inicio = $("#datepicker").val();
    var f_fin = $("#datepicker1").val();

    $.ajax({
        url: "/Report/Get_Report_Maintenance_Clients_Json",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                date_start: f_inicio,
                date_end: f_fin
            },
        success: function (result) {
           // var result = JSON.parse(result);
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
            { sTitle: "Tiquete",    data: "other" },
            { sTitle: "Comercio",   data: "local_name" },
            { sTitle: "# Afiliado", data: "membership_number" },
            { sTitle: "Ticket (Service)", data: "other_2" },
            { sTitle: "Dirección",  data: "address" },
            {
                sTitle: "Fecha/Hora",
                data: "buy_date",
                "type": "date",
                "render": function (value)
                {
                    if (value === null) return "";
                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));
                    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
                }
            },
            { sTitle: "	Técnico", data: "other_3" }
        ],
        aLengthMenu: [
            [10, 50, 100, 200, -1],
            [10, 50, 100, 200, "Total"]
        ],
        iDisplayLength: 10
    }).draw();
    $("#table1").find('tbody').show('fast');
    $("#loading_table").hide('fast');
}

function consultaMantenimientosSinRealizar() {
    var meses = $("#meses").val();

    $.ajax({
        url: "/Report/Get_Report_Not_Maintenance_Clients_Json",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                meses: meses
            },
        success: function (result) {
            //var result = JSON.parse(result);
            result = (result != "") ? result : {};
            update_dataTable2(result);
            $("#msj-ajax").removeAttr('style');
        },
        error: function (XMLHttpRequest) {
            $("#msj-ajax").text("Error al conectar al servidor");
            $("#msj-ajax").css('color', 'red');
        }
    });
}

function update_dataTable2(jsonresult) {
    if (table){
        table.destroy();
    }

    table = $("#table1").DataTable({
        "destroy": true,
        "oLanguage": {
            "sEmptyTable": "No se encontraron registros",
        },
        "aaData": jsonresult,
        "aoColumns": [
            { sTitle: "Comercio",   data: "local_name" },
            { sTitle: "# Afiliado", data: "membership_number" },
            { sTitle: "Contacto",   data: "name" },
            { sTitle: "Dirección",  data: "address" },
            { sTitle: "Telefono",   data: "phone" },
            { sTitle: "Provincia",  data: "province" },
            { sTitle: "Canton",     data: "canton" },
        ],
        aLengthMenu: [
            [10, 50, 100, 200, -1],
            [10, 50, 100, 200, "Total"]
        ],
        iDisplayLength: 10
    }).draw();
    $("#table1").find('tbody').show('fast');
    $("#loading_table").hide('fast');
}