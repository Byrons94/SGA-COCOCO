
    $(document).ready(function (){
        $('#table1').DataTable({
            "paging": true,
            "lengthChange": false,
            "searching": true,
            "ordering": true,
            "info": true,
            "autoWidth": true
        });
    });

    $(".btn-cerrar").click(function(){
        $("#id_cerrar").val($(this).attr('id'));
        $("#type").val($(this).attr('type'));
    });
        
    $("#btn-confirmacion").click(function(){
        var id_cerrar   = $("#id_cerrar").val();
        var comentario  = $("#comentario").val();
        var type        = $("#type").val();
        console.log(id_cerrar);
        $.ajax({
            url: "/Ticket/Close_Tiquet",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data:
                {
                    id_tiquete: id_cerrar,
                    type: type,
                    comentario: comentario
                },
            success: function (result) {
                $(".btn-cerrar").each(function(){
                    if($(this).attr('id')==id_cerrar){
                        $(this).closest('tr').remove();
                    }
                });
            },
            error: function (XMLHttpRequest) {
            }
        });
        $("#modal_confirmacion").modal("hide");
    });