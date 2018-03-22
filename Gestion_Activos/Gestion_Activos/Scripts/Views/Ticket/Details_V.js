$(document).ready(function () {
    $('#formV').validate({
        rules: {
            chk_pruebas: {
                required: true
            },
            solucion:{
                required: true
            },
            diagnostico:{
                required: true
            },
            averia:{
                required: true
            },
        },
        messages: {
            chk_pruebas: {
                required: ""
            },
            solucion: {
                required: ""
            },
            diagnostico: {
                required: ""
            },
            averia: {
                required: ""
            },
        }, highlight: function (element, errorClass) {
            $(element).closest(".form-group").addClass("has-error");
            modal_error("Existen campos obligatorios");
        },
        unhighlight: function (element, errorClass) {
            $(element).closest(".form-group").removeClass("has-error");
        },
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

$(".btn_change_product").click(function(){
    var select = $(this).closest('tr').find("select");
    var btn_series = $(this).closest('tr').find(".btn_serie");
       
    if(select.attr('disabled')){
        select.removeAttr('disabled');
        btn_series.removeAttr('disabled');
        btn_series.addClass('btn-info');
        $(this).removeClass('btn-primary');
        $(this).addClass('btn-warning');
    }
    else{
        select.attr('disabled', true);
        btn_series.attr('disabled', true);
        $(this).removeClass('btn-warning');
        btn_series.removeClass('btn-info');
        $(this).addClass('btn-primary');
    }
});

$("#btn_cerrar_v").click(function () {
    $("#mjsErrorM").text("");
    if($('input[name="chk_mantenimiento_general"]').is(':checked')){
        openConfirmationModal();
    }
    else {
        openMainteinaceModal();
    }
});

function openConfirmationModal() {
    $("#modal_confirmacion").modal("show");
    consumir_servicio_pc_track();
}

function openMainteinaceModal() {
    $("#modal_mantenimiento").modal("show");
}

$(".option_mantenimiento").click(function () {
    var text = "";
    if ($(this).val() == 1) {
        $('input[name="chk_mantenimiento_general"]').prop("checked", true);
        text = "Mantenimiento realizado";
    } else { 
        text = "No se realizó el mantenimiento";
    }
    $("#mjsErrorM").text(text);
    setTimeout(function () {
        $("#modal_mantenimiento").modal("hide");
         openConfirmationModal();
    }, 2000);

});

$("#btn-confirmacion").click(function(){
    var id_doc = $("#id_ticket").val();
    var listInfo = [];
    $("select:enabled").each(function () {
        var id = $(this).attr('id');
        var value = $(this).find('option:selected').text();
        var cod_prod = $(this).attr("cod_prod");
        item = {};
        item["id"] = id;
        item["state"] = value;
        item["cod_prod"] = cod_prod;
        listInfo.push(item);
    });
        
    $("#email").val($("#email_enviar").val());
    $("#btn_cerrar_v").attr('disabled','disabled');
    ajax_cambios(id_doc, listInfo);
});
    
function modal_error(error){
    $("#modal_confirmacion").modal("hide");
    $("#mjsError").text(error);
    $("#modal_error").modal("show");
}

function ajax_cambios(num_doc, products_array) {
    var successful = true;
    if (products_array.length > 0){ 
        products_array = JSON.stringify(products_array);
        series_array   = JSON.stringify(seriesChanged);
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
            },
            error: function (XMLHttpRequest) {
                alert("Error al guardar productos");
                successful =  false;
            }
        });
    }
    if(successful==true){
        $("#formV").submit();
    }
    return false;
}

$("#btn_cambiar_serie").click(function () {
    $("#error_msj").hide("slow");
    var num_ticket  = $("#id_ticket").val();
    var nueva_serie = $("#txt_serie_nueva").val();
    var old_serie   = $("#txt_serie_actual").val();
    var consecutive = $("#num_doc_serie").val();
    var category    = $("#category_serial").val();
    if (nueva_serie.length>0) {
        $("#txt_serie_nueva").removeAttr('style');
        cambiar_serie(old_serie, nueva_serie, num_ticket, consecutive, category.trim());
    }
    else {
        $("#txt_serie_nueva").css('border-color', '#dd4b39');
        $("#txt_serie_nueva").attr("placeholder", "Debe Agregar una serie");
    }
});

//desgestionar la serie 
$("#btn_limpiar_cambios").click(function(){
    var num_ticket  = $("#id_ticket").val();
    var nueva_serie = $("#txt_serie_nueva").val();
    var old_serie =   $("#txt_serie_actual").val();
    delete_gestion(old_serie);
    clearTr(old_serie);
});

function clearTr(old_serie){
    $(".td_serie").each(function(){
        if($(this).attr('id')==old_serie){
            $(this).closest('tr').children('td,th').removeAttr('style');
            $(this).closest('tr').find('[name="serie2"]').attr('correct_serial', old_serie);
            $(this).closest('tr').find('select').removeAttr('disabled');
            $(this).html(old_serie);
        }
    });
}

function delete_gestion(serie){
    for(var key in seriesChanged){
        var obj = seriesChanged[key];
        if(obj.old_serial==serie){
            seriesChanged.splice(key, 1);
        }
    }
}

//end desgestionar la serie
function cambiar_serie(n_old, n_serie, n_boleta, consecutive_serial, category) { //aqui se cambia la serie
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
            changes["old_serial"]   = n_old.trim();
            changes["new_serial"]   = n_serie.trim();
            changes["consecutive"]  = consecutive_serial.trim();
            changes["category"]     = category.trim();
            if (response == "DISPONIBLE") {
                var existe = false;
                for(var key in seriesChanged){
                    var obj = seriesChanged[key];
                    if(obj.old==n_old)
                    {
                        existe = true;
                        obj.nueva = n_serie;
                    }
                }
                if(!existe){
                    seriesChanged.push(changes);
                }

                $("#succes_mjs").show("fast");
                $(".td_serie").each(function(){
                    if ($(this).attr('consecutive') == consecutive_serial) { //if($(this).attr('id')==n_old) {
                        $(this).closest('tr').children('td,th').css('background-color','#C9E8F7');
                        $(this).closest('tr').find('[name="serie2"]').attr('correct_serial', n_serie);
                        $(this).closest('tr').find('select').removeAttr('disabled');
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

//series
$(".btn_serie").click(function () {
    $("#error_msj").hide();
    $("#succes_mjs").hide();
    clean_texbox("txt_serie_nueva");

    var serie       = $(this).closest("tr").find(".td_serie").attr('id');
    var categoria   = $(this).closest("tr").find(".td_serie").attr('category');
    var descripcion = $(this).closest("tr").find(".td_descripcion").text();
    var num_cons    = $(this).closest("tr").find(".td_serie").attr('consecutive');

    $("#num_doc_serie").val(num_cons);
    $("#category_serial").val(categoria);
    $("#txt_serie_actual").val(serie);
    $("#dd_descip").text(descripcion);
});

function clean_texbox(name) {
    $("#" + name).removeAttr('style');
    $("#" + name).val("");
}

function validar_series() { /*validar paste*/
    var success = true;
    $('input[name=serie2]').each(function () {
        var serial    = $(this).attr('correct_serial').trim();
        var inserted  = $(this).val().trim();

        if(serial != "---SIN SERIE---"){
            if (serial != inserted) {
                $(this).css('border-color', '#dd4b39');
                success = false;
            }
            else {
                $(this).removeAttr('style');
            }
        }
    });
    return success;
}

function getSeriesDetalle() {
    var jsonObj = [];
    $('select[name=estado_producto]').each(function () {
        var id = $(this).attr('id');
        var value = $(this).find('option:selected').text();
        var cod_prod = $(this).attr("cod_prod");
        item = {};
        item["id"] = id;
        item["state"] = value;
        item["cod_prod"] = cod_prod;
        item["action"] = "Cambio";
        jsonObj.push(item);
    });
    return jsonObj;
}

function validate_instalaciones() {
    var correct = validar_series();
    if (!correct) {
        $('.nav-tabs a[href="#tab_1"]').tab('show');
        return false;
    }
    return true;
}

/**************PC TRACK*****************/
function consumir_servicio_pc_track() {
    $("#error_pc_track").hide();
    $("#msj_pc_track").val('');
    if (seriesChanged.length > 0) { 
        var afiliado = $("#membership_number").val();
        $.ajax({
            url:  "/Product/Call_External_Service",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data: {
                tipo:     "2",
                series: JSON.stringify(seriesChanged),
                afiliado: afiliado
            },
            success: function (result_ajax)
            {
                var error = "";
                var result = false;
                $.each(result_ajax, function(i, item)
                {
                    if(item=="0")
                    {
                        error += "No se ha registrado la serie: " + i + " en el agente de PC_TRACK.\ "
                        result =  true;
                    }
                });
                if(result==true){
                    $("#error_pc_track").show();
                    $("#msj_pc_track").text(error);
                }      
            },
            error: function (XMLHttpRequest)
            {
                alert("No se pudo conectar al agente http://pctrack.net/");
            }
        });
    }
};