
    $(document).ready(function (){
        $('#table1').DataTable({
            "paging": false,
            "lengthChange": false,
            "searching": false,
            "ordering": true,
            "info": false,
            "autoWidth": true
        });
    });

    function delete_User(id) {
        $("#inp_id_rol").val(id);
    }

    $("#btn_confirmacion_eliminar").click(function(){
       var id = $("#inp_id_rol").val();
        $.ajax({
            url: '/Rol_Permissions/Delete_Rol_Id',
            type: 'POST',
            dataType: "json",
            traditional: true,
            data: {
                id_rol: id
            },
            success: function (result) {
                location.reload();
            },
            error: function (XMLHttpRequest) {
                alert("poner aqui el llamado al modal de la excepcion");
            }
        });
        return false;
    });