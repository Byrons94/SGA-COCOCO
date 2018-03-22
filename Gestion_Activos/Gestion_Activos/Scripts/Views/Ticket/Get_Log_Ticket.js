    $(document).ready(function () {
        $('input[type=checkbox]').click(function () {
            return false;
        });

        $('input[type=radio]').click(function () {
            return false;
        });

        var tipo = '@type';
        if (tipo == "INSTALACION") {
            f_instalaciones();
        }
        else if (tipo == "VISITA") {
            f_visitas();
        }
        else if (tipo == "RETIRO") {
            f_retiros();
        }
    });

    function f_instalaciones() {
        var conexion    = '@conexion'.split(',');
        var detalle     = '@detalle'.split(',');
        var exposicion  = '@exposicion'.split(',');
        var pruebas     = '@pruebas'.split(',');
        $.each(conexion, function (number, value) {
            checkbox("chk_conecion", value);
        });
        $.each(detalle, function (number, value) {
            checkbox("chk_detalle_install", value);
        });
        $.each(exposicion, function (number, value) {
            checkbox("chk_exposicion_equip", value);
        });
        $.each(pruebas, function (number, value) {
            checkbox("chk_otros", value);
        });
        console.log(conexion);
    }

    function f_visitas() {
        var radios = ["radio_humedad", "radio_grasa", "radio_voltaje", "radio_plaga", "radio_polvo", "radio_otra"]
        var mantenimiento = '@mantenimiento'.split(',');
        var exposicion    = '@exposicion'.split(',');
        var pruebas       = '@pruebas'.split(',');

        $.each(exposicion, function (number, value) {
            radio_buttons(radios[number], value - 1);
        });
        $.each(mantenimiento, function (number, value) {
            checkbox("chk_mantenimiento", value);
        });
        $.each(pruebas, function (number, value) {
            checkbox("chk_pruebas", value);
        });
    }

    function f_retiros() {
        var exposicion = '@exposicion'.split(',');
        $.each(exposicion, function (number, value) {
            checkbox("chk_exposicion_equipo", value);
        });
    }

    function radio_buttons(name, value) {
        var radio1 = document.getElementsByName(name);
        for (i = 0; i < radio1.length; i++) {
            if (radio1[i].type == 'radio' && i == value) {
                radio1[i].checked = true;
            }
        }
    }

    function checkbox(name, value) {
        var checkboxs = $('input[name=' + name + ']');
        $.each(checkboxs, function (number, checkbox) {
            if ($(this).val() == value) {
                $(checkbox).prop("checked", true);
            }
        });
    }


    $("#print").click(function () {
        window.print();
    });

    $("#btn-confirmacion").click(function () {
        $('#modal_email').modal({ backdrop: 'static', keyboard: false })
        $("#body1").hide();
        $("#body2").show();
        var email = $("#input_email").val();
        var info = $("#membership_number").val() + " " + $("#local_name").val();
            
        if (email.trim() != "" && isEmail(email)) {
            $("#input_email").removeAttr('style');
            var bitacora = $("#id_bitacora").val();
            ajax_email(email, bitacora, info);
        }
        else{
            $("#input_email").css('border-color', '#dd4b39');
            $("#input_email").attr("placeholder", "Debe agregar una correo válido");
        }
        $("#mjs_send").text("Enviando E-mail un momento por favor...");
    });

    function ajax_email(email, n_doc, info) { //aqui se cambia la serie
        $.ajax({
            url: "/Ticket/Send_Email",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data:
                {
                    n_doc: n_doc,
                    email: email,
                    other: info
                },
            success: function (result) {
                $("#mjs_send").text("Email enviado con éxito");
                setTimeout(function () { $("#modal_email").modal("hide") }, 4000);
                setTimeout(function () {
                    $("#body2").hide();
                    $("#body1").show();
                }, 6000);
            },
            error: function (XMLHttpRequest) {
                $("#mjs_send").text("No se ha podido enviar el E-mail, intente de nuevo");
                setTimeout(function () {
                    $("#body2").hide();
                    $("#body1").show();
                }, 3000);
            }
        });
        return false;
    }
            
    function isEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }
