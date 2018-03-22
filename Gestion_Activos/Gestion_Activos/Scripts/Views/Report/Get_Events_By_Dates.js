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

    $(".checkbox").each(function (i, obj) {
        var identity = $(this).attr("identity").trim();
        var checked = $(this).find("input[type='checkbox']").is(":checked");
           
        if (!checked)
        {
            var column = table.column(identity);
            column.visible(!column.visible());
        }
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
        if (validar_busqueda()) {
            $("#table1").find('tbody').hide('fast');
            $("#loading_table").show('fast');
            consultar();
        }
    });

function consultar() {
    var f_inicio = $("#datepicker").val();
    var f_fin    = $("#datepicker1").val();
    var filter = $('input[name="t_estatus"]:checked').val();
    var t_fecha = $('input[name="t_fecha"]:checked').val();
    var t_movimiento = $('input[name="t_movimiento"]:checked').val();
    var t_estatus = $('select[name="t_estatus_afi"] option:selected').val();
    
    $.ajax({
        url: "/Report/Get_Events_By_Dates_Json",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                date_start:     f_inicio,
                date_end:       f_fin,
                filter:         filter,
                t_fecha:        t_fecha,
                t_movimiento:   t_movimiento,
                t_estatus:      t_estatus
            },
        success: function (result) {
            //var result = JSON.parse(result);
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
    var f_fin = $("#datepicker1").val();

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
            {
                data: "date",
                "type": "date",
                "visible": show_row(0),
                "render": function (value) {
                    if (value === null) return "";
                    var pattern = /Date\(([^)]+)\)/;
                    var results = pattern.exec(value);
                    var dt = new Date(parseFloat(results[1]));
                    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
                }
            },
            {
                data: "hour_programmed",
                "visible": show_row(1)
            },
            {
                data: "ticket_cred",
                "visible": show_row(2)
            },
            {
                data: "id",
                "visible": show_row(3)
            },
            {
                data: "type",
                "visible": show_row(4)
            },
            {
                data: "province",
                "visible": show_row(5)
            },
            {
                data: "membership_number",
                "visible": show_row(6)
            },
            {
                data: "contact",
                "visible": show_row(7)
            },
            {
                data: "phone",
                "visible": show_row(8)
            },
            {
                data: "local_name",
                "visible": show_row(9)
            },
            {
                data: "address",
                "visible": show_row(10)
            },
            {
                data: "coordinator",
                "visible": show_row(11)
            },
            {
                data: "ejecutive",
                "visible": show_row(12)
            },
            {
                data: "details",
                "visible": show_row(13)
            },
            {
                data: "internet",
                "visible": show_row(14)
            },
            {
                data: "desktop",
                "visible": show_row(15)
            },
            {
                data: "electricity",
                "visible": show_row(16)
            },
            {
                data: "extra_1",
                "visible": show_row(17)
            },
            {
                data: "extra_2",
                "visible": show_row(18)
            }
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
    
$(".checkbox").click(function () {
    var n_column = $(this).attr("identity").trim();
    var column = table.column(n_column);
    column.visible(!column.visible());
});

function show_row(idRow) {
    var retorno = false;
    $(".checkbox").each(function(i, obj){
        if ($(this).attr("identity").trim() == idRow) {
            retorno = $(this).find("input[type='checkbox']").is(":checked");
        };
    });
    return retorno;
}

$('input[name="t_reporte"]').click(function () { 
    cambiar_parametrizacion_reporte($(this).val());
});

function cambiar_parametrizacion_reporte(t_reporte)
{
    var array = [];
    if (t_reporte == "D")
    {
        array = [0, 2, 3, 4, 5, 6, 9, 10, 17, 18];
    } else if(t_reporte == "T")
    {
        array = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18];
    }
    $(".checkbox").find("input[type='checkbox']").prop('checked', false);

    $.each(array, function (index, value) {
        $(".checkbox").each(function (i, obj) {
            if (i == value)
            {
                $(obj).find("input[type='checkbox']").prop('checked', true);
            }
        });
    });
    recompile_table();
}

function recompile_table()
{
    $(".checkbox").each(function (i, obj) {
        var identity = $(this).attr("identity").trim();
        var checked = $(this).find("input[type='checkbox']").is(":checked");
        var column = table.column(identity);
        if (!checked) {
            column.visible(false);
        } else
        {
            column.visible(true);
        }
    });
}