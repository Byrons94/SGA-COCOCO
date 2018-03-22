
var seriesChanged  = []; //array de series a cambiar
var seriesDeleted  = []; //array de series a cambiar
$(".btn-info-t").click(function () {
    var number = $(this).attr('id');
    $("tr[name=hidden_div]").each(function () {
        var id = $(this).attr('identify');
        if (number == id) {
            if ($(this).is(':hidden')) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        }
    });
});

$("#btn-confirmacion-actualizar").click(function(){
    ajax_update_serials();
    ajax_elimnar();
    ajax_actualizar();
});
    
$(".btn_abrir_modal").click(function(){
    $("#error_msj").hide();
    $("#succes_mjs").hide();
    clean_texbox("txt_serie_nueva");

    var id =  $(this).attr('id');
    var serial =  $(this).attr('serial');

    $("#num_doc_serie").val(id);
    $("#txt_serie_actual").val(serial);
    $("#modal_serie").modal("show");
});
    
//eliminar productos
$(".btn-eliminar").click(function(){
    var id = $(this).attr('id');
    $("#id_eliminar").val(id);
});

$("#cambiar_inhabilitar").click(function () {
    var status = $("#id_status").val();
    var idClie = $("#id_client").val();
    change_client_status(idClie, status);
});

$("#cambiar_habilitar").click(function () {
    change_client_status($("#id_client").val(), "1");
});
   
$("#btn-confirmacion").click(function(){
    var id =  $("#id_eliminar").val();
    seriesDeleted.push(id);
    $("#modal_eliminar").modal("hide");
    $(".btn-eliminar").each(function(){
        if($(this).attr('id')==id){
            $(this).closest('tr').children('td,th').css('background-color','#ffcccc');
            $(this).closest('tr').find(".btn_abrir_modal").attr("disabled", true);
            $(this).closest('tr').find(".btn_abrir_modal").removeClass("btn-primary");
            $(this).attr('disabled', true);
            $(this).removeClass("btn-danger");
        }
    });
});


$("#btn_cambiar_afiliado").click(function () {
    var provincia, canton, distrito;
    $(".info-dd").each(function (i) {
        switch (i) {
            case 0:
                $("#n_afiliado").val($(this).text().trim());
                break;
            case 1:
                $("#contacto").val($(this).text().trim());
                break;
            case 2:
                $("#comercio").val($(this).text().trim());
                break;
            case 3:
                $("#telefono").val($(this).text().trim());
                break;
            case 4:
                provincia = $(this).text().trim();
                break;
            case 5:
                canton = $(this).text().trim();
                break;
            case 6:
                distrito = $(this).text().trim();
                break;
            case 7:
                $("#direccion").val($(this).text().trim());
                break;
        }
    });
    ajax_get_datos_pais(provincia, canton, distrito);
});


$("#btn_cambiar_serie").click(function () {
    $("#error_msj").hide("slow");
    var nueva_serie = $("#txt_serie_nueva").val();
    var old_serie =   $("#txt_serie_actual").val();
    var consecutive = $("#num_doc_serie").val();
    if  (nueva_serie.length>0) {
        $("#txt_serie_nueva").removeAttr('style');
        cambiar_serie(old_serie, nueva_serie, consecutive);
    }
    else {
        $("#txt_serie_nueva").css('border-color', '#dd4b39');
        $("#txt_serie_nueva").attr("placeholder", "Debe Agregar una serie");
    }
});


function clean_texbox(name) {
    $("#" + name).removeAttr('style');
    $("#" + name).val("");
}
    
 
function cambiar_serie(n_old, n_serie, consecutive_serial) { //aqui se cambia la serie
    $.ajax({
        url: "/Product/Validate_Serial",
        type: "POST",
        datatype: "JSON",
        traditional: true,
        data:
            {
                n_serie_verificar: n_serie
            },
        success: function (result) {
            var response = JSON.stringify(result).replace(/"/g,"");
            changes = {};
            changes["old_serial"] = n_old;
            changes["new_serial"] = n_serie;
            changes["consecutive"] = consecutive_serial;
             
            if (response == "DISPONIBLE") {
                var existe = false;
                for(var key in seriesChanged) {
                    var obj = seriesChanged[key];
                    if(obj.old==n_old) {
                        existe = true;
                        obj.nueva = n_serie;
                    }
                }
                if(!existe) {
                    seriesChanged.push(changes);
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


function ajax_update_serials(){
    if(seriesChanged.length > 0){
        series_array   = JSON.stringify(seriesChanged); 
        $.ajax({
            url: "/Product/Update_Serials",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data:
                {
                    series_c: series_array
                },
            success: function (result) {
            },
            error: function (XMLHttpRequest) {
            }
        });
        return false;
    }
}

function ajax_elimnar(){
    if(seriesDeleted.length > 0){
        var id_client = $("#id_client").val();
        series_array   = JSON.stringify(seriesDeleted); 
        $.ajax({
            url: "/Product/Delete_From_Inventary",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            data:
                {
                    cod_afi: id_client,
                    series_c: series_array
                },
            success: function (result) {
            },
            error: function (XMLHttpRequest) {
            }
        });
        return false;
    }
}
    
function ajax_actualizar(){
    var id_client = $("#id_client").val();
    $.ajax({
        url: "/Client/Update_Client",
        type: "POST",
        datatype: "JSON",
        traditional: true,
        data:
            {
                id_client: id_client
            },
        success: function (result) {
            setTimeout(function(){
                location.reload();
            }, 2000);
        },
        error: function (XMLHttpRequest) {
        }
    });
    return false;
}


function ajax_get_datos_pais(provincia, canton, distrito){
    if($("#provincia").empty() ||  $("#canton").empty() || $("#distrito").empty()){
        $.ajax({
            url: "/Client/Get_Provincia_Canton_Distrito",
            type: "POST",
            datatype: "JSON",
            traditional: true,
            success: function (result) {
                var json = JSON.parse(result);
                $.each(json, function (key, data) {
                    if(key=="Item1"){
                        $.each(data, function (index, data) {
                            $("#provincia").append($("<option></option>").attr("value",data["idProvincia"]).text(data["nombreProvincia"])); 
                        });
                        selectValue("provincia", provincia);
                    }
                    if(key=="Item2"){
                        $.each(data, function (index, data) {
                            $("#canton").append($("<option></option>").attr("value",data["idCanton"]).text(data["nombreCanton"])); 
                        });
                        selectValue("canton", canton);
                    }
                    if(key=="Item3"){
                        $.each(data, function (index, data) {
                            $("#distrito").append($("<option></option>").attr("value",data["idDistrito"]).text(data["nombreDistrito"])); 
                        });
                        selectValue("distrito", distrito);
                    }
                });
            },
            error: function (XMLHttpRequest) {
                alert("error al cargar datos, intente de nuevo");
            }
        });
    }
    return false;
}


function selectValue(id_select, value){
    $("#"+id_select+" option").each(function(){
        if($(this).text().trim()==value.trim()){
            $(this).attr('selected', true); 
        };
    });
}


$("#cambiar_afiliado").click(function(){
    $("#n_afiliado").removeAttr('style');
    $("#lb_error").hide();
    if($(".info-dd").first().text().trim() == $("#n_afiliado").val()){
        $("#n_afiliado").css("border-color", "red");
        $("#lb_error").show();
    }
    else {
        ajax_cambiar_afiliado();
    }
});

function ajax_cambiar_afiliado(){
    $.ajax({
        url: "/Client/Change_Membership",
        type: "POST",
        datatype: "JSON",
        data: {
            old_afiliado : $(".info-dd").first().text().trim(),
            n_afiliado   : $("#n_afiliado").val(),
            contacto     : $("#contacto").val(),
            comercio     : $("#comercio").val(),
            telefono     : $("#telefono").val(),
            provincia    : $("#provincia").val(),
            canton       : $("#canton").val().slice(1),
            distrito     : $("#distrito").val().slice(3),
            direccion    : $("#direccion").val(),
            referencia   : $("#referencia").val()
        },
        traditional: true,
        success: function (result) {
            location.reload();
        },
        error: function (XMLHttpRequest) {
            alert("no se pudo conectar al servidor, (cambiar afiliado) ");
        }
    });
}

function change_client_status(id, status)
{
    $.ajax({
        url: "/Client/Change_Client_Status_Client",
        type: "POST",
        datatype: "JSON",
        data:
            {
                id_client : id,
                estatus: status
            },
        traditional: true,
        success: function (result) {
            location.reload();
        },
        error: function (XMLHttpRequest) {
            alert("no se pudo conectar al servidor, (estatus) ");
        }
    });
}


$(".btn-status").click(function () {
    $(".btn-status").removeClass("btn-primary");
    $(this).addClass("btn-primary");
    $("#id_status").val($(this).val());
});

$("#btn_desactivar_afiliado").click(function () {
    var status = $("#id_status").val();
    $(".btn-status").each(function () {
        if ($(this).val() == status){
            $(this).addClass("btn-primary");
        }
    });
});
