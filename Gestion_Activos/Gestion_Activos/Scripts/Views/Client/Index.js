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
    
    $('input[type=text]').on('keydown', function(e) {
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
        var estatus  = $("select[name='t_estatus'] option:selected").val();
        $.ajax({
            url: "/Client/Get_Clients_By_Search",
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
                { data: "membership_number" },
                { data: "local_name" },
                { data: "contact" },
                { data: "phone" },
                {
                    "mData": null,
                    "bSortable": false,
                    "mRender": function (data) {
                        var label;
                        var description;
                        switch (data.status_varchar) {
                            case 'A':
                                label = "label-success";
                                description = "Activo";
                                break
                            case 'I':
                                label = "label-danger";
                                description = "Inactivo";
                                break;
                            case 'R':
                                label = "label-warning";
                                description = "Retiro";
                                break;
                            case 'L':
                                label = "label-danger";
                                description = "Legal";
                                break;
                            case 'X':
                                label = "label-danger";
                                description = "Irrecuperable";
                                break;
                        }
                        return '<center><span class="label ' + label + '">' + description + '</span></center>';
                    }
                },
                {
                    "mData": null,
                    "bSortable": false,
                    "mRender": function (data) {
                        if (data.updated) {
                            return '<center><span class="label label-success">Actualizado</span></center>';
                        }
                        else {
                            return '<center><span class="label label-warning">Pendiende</span></center>';
                        }
                    }
                },
                {
                    "mData": null,
                    "bSortable": false,
                    "mRender": function (data) { return '<a type="button" class="btn btn-block btn-info btn-sm" href="/Client/Details_C?id_client=' + data.id + '">Ver</a>'; }
                }
            ]
        }).draw();
        $("#table1").find('tbody').show('fast');
        $("#loading_table").hide('fast');
    }