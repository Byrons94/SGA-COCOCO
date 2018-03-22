var table;
$(document).ready(function (){
    table = $('#table1').DataTable({
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": true,
        "lengthMenu": [[10, 50, 250, -1], [10, 50, 250, "All"]]
    });
});

    $("#btn_buscar").click(function () {
        $("#table1").find('tbody').hide('fast');
        $("#loading_table").show('fast');
        consulta_inventario();
    });
    
function consulta_inventario() {
    var tipo = $("input[name='t_busqueda']:checked").val();
    var filtro = "";
    if (tipo == "2" || tipo == "3" || tipo == "4")
    {
        filtro = get_filtro_parte(tipo);
        tipo = "2";
    } else if (tipo == "5") {
        tipo = "3";
    }
    $.ajax({
        url: "/Inventory_Movement/Search_Inventory_By_Parameter",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        data:
            {
                filtro: filtro,
                tipo: tipo
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

function get_filtro_parte(filtro){
    if (filtro == "2") {
        return "00001";
    } else if (filtro == "3") {
        return "00002";
    }else if(filtro == "4"){
        return "00003";
    }
}

function update_dataTable(jsonresult){
    if(table){
        table.destroy();
    }
    table =  $("#table1").DataTable({
        "destroy": true,
        "lengthMenu": [[10, 50, 250, -1], [10, 50, 250, "All"]],
        "oLanguage": {
            "sEmptyTable": "No se encontraron registros",
        },
        "aaData": jsonresult,
        "aoColumns": [
            { data: "cod_prod" },
            { data: "description" },
            { data: "balance_start", "sClass": " text-center" },
            { data: "bought", "sClass": " text-center" },
            { data: "installed", "sClass": " text-center" },
            { data: "retired", "sClass": " text-center" },
            { data: "reconditioned", "sClass": " text-center" },
            { data: "discarded", "sClass": " text-center" },
            { data: "balance_end", "sClass": " text-center" }
        ]
    }).draw();
    $("#table1").find('tbody').show('fast');
    $("#loading_table").hide('fast');
}
