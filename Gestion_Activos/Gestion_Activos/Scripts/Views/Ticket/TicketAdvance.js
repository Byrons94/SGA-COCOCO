
    /***********script avance tiquete***************/
    $("#btn_confirmacion_update").click(function () {
        var id_cerrar = $("#id_ticket").val();
        var comentario = $("#comentario").val();
        var type = $("#tip_mov").val();
        disableButton(this);

        $.ajax({
            url: "/Ticket/Add_Ticket_Progress",
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
                addAdvaceToTable(result);
                $("#modal_avance").modal("hide");
            },
            error: function (XMLHttpRequest) {
                alert("Error al agregar, intente de nuevo");
            }
        });
    });

    function disableButton(button) {
        $(button).attr("disabled", "disabled");
        setTimeout(enableButton(button), 3000);
    }

    function enableButton(button) {
        $(button).removeAttr('disabled');
    }

    function addAdvaceToTable(jsonResult) {
        if (jsonResult != "") {
            $("#tableAvance").append('<tr>' +
                                       '<td>' + jsonResult.id + '</td>' +
                                       '<td>' + jsonResult.user + '</td>' +
                                       '<td>' + convertToDateTime(jsonResult.date) + '</td>' +
                                       '<td>' + jsonResult.detail + '</td>' +
                                    '</tr>');
        }
        $("#comentario").val(" ");
    }


    function convertToDateTime(value) {
        if (value === null) return "";
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
    }
    /*************************************************/