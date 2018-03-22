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
            consulta_afiliados();
        }
    });

    $('input[type=text]').on('keydown', function (e) {
        if (e.which == 13) {
            if (validar_busqueda()) {
                $("#table1").find('tbody').hide('fast');
                $("#loading_table").show('fast');
                consulta_afiliados();
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

    function consulta_afiliados() {
        var busqueda = $("#parametro").val();
        var tipo     = $("input[name='t_busqueda']:checked").val();
        var estatus  = $("input[name='t_estatus']:checked").val();
        $.ajax({
            url: "/Ticket/Get_Tickets_By_Search",
            type: "POST",
            datatype: "JSON",
            traditional: "true",
            data:
                {
                    busqueda: busqueda,
                    tipo: tipo,
                    estatus: estatus
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

    function update_dataTable(jsonresult) {
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
                { data: "id" },
                { data: "type" },
                { data: "local_name" },
                { data: "contact" },
                { data: "membership_number" },
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
                {
                    "mData": null,
                    "bSortable": false,
                    "mRender": function (data) { return '<a type="button" class="btn btn-block btn-info btn-sm" href="/Ticket/Get_Log_Ticket?id_ticket=' + data.id + '"  target="_blank">Ver</a>'; }
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
