    $(document).ready(function () {
        $(function () {
            $('#simple_sketch').sketch();
        });
        $('#table1').DataTable({
            "paging": false,
            "lengthChange": false,
            "searching": true,
            "ordering": false,
            "info": false,
            "autoWidth": false
        });
    });
    
    var seriesChanged = []; //array de series a cambiar
    var listInfo      = [];
    
    $(document).ready(function () {
        $('#formI').validate({
            rules: {
                chk_otros: {
                    required: true
                }
            },
            messages: {
                chk_otros: {
                    required: ""
                }
            }, highlight: function (element, errorClass) {
                $(element).closest(".form-group").addClass("has-error");

                modal_error("Existen campos obligatorios");
                $("#btn_cerrar_i").removeAttr('disabled');
            },
            unhighlight: function (element, errorClass) {
                $(element).closest(".form-group").removeClass("has-error");
            }, 
        });        
    }); 
    
    $("#btn_cerrar_i").click(function () {
        var tipo_movimiento = $("#tip_mov").val();
        if(validate_instalaciones()){
            listInfo = getSeriesDetalle();
            if(listInfo.length>0) {
                consumir_servicio_pc_track(); // se consume el servicio de pc track
                $("#modal_confirmacion").modal("show");
            }
            else {
                modal_error("No se ha instalado ningun producto");
            }
        }
        else { 
            modal_error("Se deben validar las series");
        }
    });
    

    $("#btn-confirmacion").click(function(){
        var id_doc = $("#id_ticket").val();
        $("#email").val($("#email_enviar").val());//mjs de envio de email
        console.log($("#email").val());
        var result = ajax_instalaciones(id_doc, listInfo);
        if (result==true){
            $("#formI").submit();
        }
    });
    

    $("#btn_cambiar_serie").click(function () {
        $("#error_msj").hide("slow");
        var num_ticket  = $("#id_ticket").val();
        var nueva_serie = $("#txt_serie_nueva").val();
        var old_serie =   $("#txt_serie_actual").val();
        var consecutive = $("#num_doc_serie").val();
        
        if (nueva_serie.length>0) {
            $("#txt_serie_nueva").removeAttr('style');
            cambiar_serie(old_serie, nueva_serie, num_ticket, consecutive);
        }
        else {
            $("#txt_serie_nueva").css('border-color', '#dd4b39');
            $("#txt_serie_nueva").attr("placeholder", "Debe Agregar una serie");
        }
    });
    
    $("#btn_limpiar_cambios").click(function() {
        var num_ticket  = $("#id_ticket").val();
        var nueva_serie = $("#txt_serie_nueva").val();
        var old_serie =   $("#txt_serie_actual").val();
        delete_gestion(old_serie);
        clearTr(old_serie);
    });
    
    //series
    $(".btn_serie").click(function () {
        $("#error_msj").hide();
        $("#succes_mjs").hide();
        clean_texbox("txt_serie_nueva");

        var serie = $(this).closest("tr").find(".td_serie").attr('id');
        var descripcion = $(this).closest("tr").find(".td_descripcion").text();
        var num_cons = $(this).closest("tr").find(".td_serie").attr('consecutive');
        console.log(serie);
        $("#num_doc_serie").val(num_cons);
        $("#txt_serie_actual").val(serie);
        $("#dd_descip").text(descripcion);
    });


    //ajax
    function ajax_instalaciones(num_doc, products_array) {
        var products_array = JSON.stringify(products_array);
        var series_array = JSON.stringify(seriesChanged);
        var successful = true;
        $.ajax({
            url: "/Product/Update_Product_Info",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data:
                {
                    id_boleta: num_doc,
                    codigos: products_array,
                    series_c: series_array
                },
            success: function (result) {
                successful = true;
            },
            error: function (XMLHttpRequest) {
                alert("Error al guardar productos");
                successful = false;
            }
        });
        return successful;
    }
    
    function cambiar_serie(n_old, n_serie, n_boleta, consecutive_serial) { //aqui se cambia la serie
        $.ajax({
            url: "/Product/Validate_Serial",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data:
                {
                    n_serie_verificar: n_serie,
                    n_boleta: n_boleta
                },
            success: function (result) {
                var response = JSON.stringify(result).replace(/"/g,"");
                changes = {};
                changes["old_serial"] = n_old;
                changes["new_serial"] = n_serie;
                changes["consecutive"] = consecutive_serial;
                if (response == "DISPONIBLE") {
                    var existe = false;
                    for (var key in seriesChanged) {
                        var obj = seriesChanged[key];
                        if(obj.old == n_old) {
                            existe = true;
                            obj.nueva = n_serie;
                        }
                    }

                    if(!existe) {
                       seriesChanged.push(changes);
                       $("#"+n_old.trim()).attr('serial2C', n_serie);
                    }
                    $("#succes_mjs").show("fast");
                    $(".td_serie").each(function() {
                        if ($(this).attr('consecutive') == consecutive_serial) { //if($(this).attr('id')==n_old) {
                            $(this).closest('tr').children('td,th').css('background-color','#C9E8F7');
                            $(this).closest('tr').find('[name="serie2"]').attr('correct_serial', n_serie);
                            $(this).html(n_serie);
                        }
                    });
                    setTimeout(function() {$('#modal_serie').modal('hide');}, 3000);
                }
                else {
                    $("#error_mjs_p").text(response+": esta serie no está disponible.");
                    $("#error_msj").show("fast");
                }
            },
            error: function (XMLHttpRequest) {
                $("#error_mjs_p").text("Error de conexion");
                $("#error_msj").show("fast");
            }
        });
        return false;
    }


    //respuesta de ajax en web service para los productos instalados
    function consumir_servicio_pc_track() {
        $("#error_pc_track").hide();
        $("#msj_pc_track").val('');

        var series = [];
        $(".td_serie").each(function (index) {
            var category = $(this).attr("category");
            if (category != null) {
                category = category.trim();
            } else {
                alert("Revisa, uno de los productos no posee categoría");
            }
            if (category == "CPU" || category == "BRIX") {
                serial = {};
                serial["serial_number"] = $(this).attr("serial2C").trim();
                series.push(serial);
            }
        });

        var afiliado = $("#membership_number").val();
        $.ajax({
            url: "/Product/Call_External_Service",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data: {
                tipo: "1",
                series: JSON.stringify(series),
                afiliado: afiliado
            },
            success: function (result_ajax) {
                var error = "";
                var result = false;
                $.each(result_ajax, function (i, item) {
                    if (item == "0") {
                        error += "No se ha registrado la serie: " + i + " en el agente de PC_TRACK.\ "
                        result = true;
                    }
                });
                if (result == true) {
                    $("#error_pc_track").show();
                    $("#msj_pc_track").text(error);
                }
            },
            error: function (XMLHttpRequest) {
                alert("No se pudo conectar al agente http://pctrack.net/");
            }
        });
    };

    //end ajax
    function modal_error(error) {
        $("#mjsError").text(error);
        $("#modal_error").modal("show");
        $("#modal_confirmacion").modal("hide");
    }

    function clearTr(old_serie) {
        $(".td_serie").each(function () {
            if ($(this).attr('id') == old_serie) {
                $(this).closest('tr').children('td,th').removeAttr('style');
                $(this).closest('tr').find('[name="serie2"]').attr('correct_serial', old_serie);
                $(this).closest('tr').find('select').removeAttr('disabled');
                $(this).html(old_serie);
            }
        });
    }

    function clean_texbox(name) {
        $("#" + name).removeAttr('style');
        $("#" + name).val("");
    }

    function validar_series() { /*validar paste*/
        var success = true;
        $('input[name=serie2]').each(function () {
            var serial = $(this).attr('correct_serial').trim();
            var inserted = $(this).val().trim();
            if(serial != "---SIN SERIE---")
            {           
                if (serial != inserted) 
                {
                    $(this).css('border-color', '#dd4b39');
                    success = false;
                }
                else 
                {
                    $(this).removeAttr('style');
                }
            }
        });
        return success;
    }

    function getSeriesDetalle() {
        var jsonObj = [];
        $('select[name=estado_producto]').each(function () {
            var id          = $(this).attr('id');
            var value       = $(this).find('option:selected').text();
            var cod_prod    = $(this).attr("cod_prod");
            var item        = {};
            item["id"]       = id;
            item["state"]    = value;
            item["cod_prod"] = cod_prod;
            item["action"]   = "Instalacion";
            jsonObj.push(item);
        });
        return jsonObj;
    }

    function validate_instalaciones() {
        var correct = validar_series();
        if (!correct)
        {
            $('.nav-tabs a[href="#tab_1"]').tab('show');
            return false;
        }
        return true;
    }
