
    function validate_Form(event) {
        //if (!validate_Name()) {
        var valid = false;
        $("input[type=checkbox]:checked").each(function () {
            valid = true;
        });
        if (!valid) {
            var r = confirm("Desea crear un rol sin permisos?"); // esto va a ser modal
            if (r == false) {
                event.preventDefault();
            }
        }
        //} else {
        //    alert("Ese nombre de rol ya existe");
        //    event.preventDefault();
        //}
    }
$('input[type="checkbox"].flat-red').iCheck({
    checkboxClass: 'icheckbox_flat-green'
});

function validate_Name() {
    var name = $("#name_Rol").val();
    var result = false;
    $.ajax({
        url: '/Rol_Permissions/Get_Rols',
        type: 'POST',
        dataType: "json",
        traditional: true,
        success: function (result) {
            alert(JSON.stringify(result));
        },
        error: function (XMLHttpRequest) {
            alert("poner aqui el llamado al modal de la excepcion");
        }
    });
    return false;
}