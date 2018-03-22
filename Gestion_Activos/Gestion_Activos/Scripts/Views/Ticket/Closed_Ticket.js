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


    $(function () {
        $('#datepicker').datepicker({
            autoclose: true
        });
        $('#datepicker1').datepicker({
            autoclose: true
        });
    });


    $("#btn_search").click(function () {
        if (validar_busqueda()) {
            $("#table1").find('tbody').hide('fast');
            $("#loading_table").show('fast');
            get_closed_tickets();
        }
    });

    $('input[type=text]').on('keydown', function (e) {
        if (e.which == 13) {
            if (validar_busqueda()) {
                $("#table1").find('tbody').hide('fast');
                $("#loading_table").show('fast');
                get_closed_tickets();
            }
        }
    });
    
    function validar_busqueda() {
        var date1 = $("#datepicker").val();
        $("#datepicker").removeAttr("style");
        var bool = (date1.trim() == "") ? false : true;
        if (!bool) {
            $("#datepicker").css('border-color', 'red');
            $("#datepicker").attr('placeholder', 'Este campo es requerido');
        }
        
        if(bool){
            var date2 = $("#datepicker1").val();
            $("#datepicker1").removeAttr("style");
            var bool = (date2.trim() == "") ? false : true;
            if (!bool) {
                $("#datepicker1").css('border-color', 'red');
                $("#datepicker1").attr('placeholder', 'Este campo es requerido');
            }
        }
        return bool;
    }


    function get_closed_tickets() {
        var date_start = $("#datepicker").val();
        var date_end   = $("#datepicker1").val();
        
        $.ajax({
            url: "/Ticket/Get_Closed",
            type: "POST",
            datatype: "JSON",
            traditional: "true",
            data:
                {
                    date_start: date_start,
                    date_end: date_end
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
                { data: "id" },
                { data: "type" },
                { data: "local_name" },
                { data: "contact" },
                { data: "membership_number" },
                {
                    data: "date",
                    "type": "date",
                    "render": function (value) {
                        if (value === null) return "";
                        var pattern = /Date\(([^)]+)\)/;
                        var results = pattern.exec(value);
                        var dt = new Date(parseFloat(results[1]));
                        return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                    }
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
