$(function () {
    var fileCounter = 0;
    $(document).ready(function () {
        $("#div_files_modal").append(modalTemplate('Manejo de evidencias', 'fa-file-pdf-o'));
        loadEvidences();
    });

    var modalTemplate = function (title, icon) {
        var modal = '';
        modal  += '<div class="modal fade" id="modal_file_upload" tabindex="-1" role="dialog">"'; 
            modal  +='<div class="modal-dialog modal-lg">';
                modal  += '<div class="modal-content">';
                    modal  +=  '<div class="modal-header">';
                        modal  += '<button type="button" class="close" data-dismiss="modal" aria-label="Close">';
                            modal  += '<span aria-hidden="true">&times;</span>';
                        modal  += '</button>';
                        modal += '<h4 class="modal-title"><i class="fa ' + icon + '"></i> ' + title + ' </h4>';
                    modal += '</div>';
                    modal += '<div class="modal-body">';
                    modal += modalBody(); // add body to modal
                    modal += '</div>';
                modal  += '</div>';
            modal  += '</div>';
        modal += '</div>';
        return modal;
    };


    var modalBody = function () {
        var body = '';
            body += '<ul class="nav nav-tabs">';
                body += '<li class="active"><a id="load-evidences" data-toggle="tab" href="#tab-uploaded-files"><i class="fa fa-file"></i> Evidencias adjuntas</a></li>';
                body += '<li><a data-toggle="tab" href="#tab-upload-files"><i class="fa fa-upload"></i> Añadir nueva evidencia</a></li>';
            body += '</ul>';
            body += '<div class="tab-content">';
                body += tab1();
                body += tab2();
            body += '</div>';
        body +='</div>'; 
        return body;
    }
    
    var tab1 = function () {
        var tab1 = '';
        tab1 += '<div id="tab-uploaded-files" class="tab-pane fade in active">';
            tab1 += '<table id="table-evidences" class="table table-hover">';
            tab1 += '<thead><th>Id</th><th>Formato</th><th>Nombre</th><th>Fecha almacenada</th><th>Acción</th></thead>';
            tab1 += '<tbody></tbody>';
            tab1 += '</table>';
        tab1 += '</div>';
        return tab1;
    }

    var tab2 = function () {
        var tab2 = '';
        tab2 += '<div id="tab-upload-files" class="tab-pane fade">';
             tab2 += '<div id="upload">';
                tab2 += '<div id="drop">Arrastre su archivo aquí<br> Formatos aceptados (PDF, JPEG, PNG, JPG)<br>';
                    tab2 += '<a><i class="fa fa-search"></i> Buscar</a>';
                    tab2 += '<input type="file" name="files[]" multiple />';
                tab2 += '</div>';
                tab2 += '<h4 id="state-message">Archivos pendientes de subir:<span id="fileCounter">0</span></h4>';
                tab2 += '<ul></ul>'
                tab2 += '<div id="div-btns">';
                    tab2 += '<br>';
                    tab2 += '<button id="btn-upload-file" class="btn btn-success pull-right" style="margin-right: 10px;"><i class="fa fa-upload"></i> Subir archivos</button>';
                    tab2 += '<button class="btn btn-warning pull-right" data-dismiss="modal" style="margin-right: 10px;"><i class="fa fa-ban"></i> Cancelar</button>';
                    tab2 += '<br>';
                tab2 += '</div>'
             tab2 += '</div>';
        tab2 += '</div>';
        return tab2;
    }

    $("#div_files_modal").on("shown.bs.modal", function () {
        var ul = $('#upload ul');
        var dataUpload = [];

        $('#drop a').click(function () {
            $(this).parent().find('input').click();
        });
       
        $('#upload').fileupload({
            dropZone: $('#drop'),
            url: '/FileManagement/UploadFile',
            autoUpload: false,
            singleFileUploads: false,
            formData: {
                idTicket: $("#id_bitacora").val(),
                membershipCode: $("#membership_number").val()
            },
     
            add: function (e, data) {
                var uploadErrors = [];
                var acceptFileTypes = /\/(pdf|jpe?g|jpg|png|jpg)$/i;
                if (data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
                    uploadErrors.push('Error, No se acepta el formato del archivo');
                }
                if (data.originalFiles[0]['size'].length && data.originalFiles[0]['size'] > 36700160) {
                    uploadErrors.push('Error, El archivo es demasiado grande. Max-> 35MB');
                }

                if (uploadErrors.length > 0) {
                    alert(uploadErrors.join("\n"));
                }
                else {
                    var tpl = $('<li class="working"><input type="text" value="0" data-width="48" data-height="48"' +
                        ' data-fgColor="#0788a5" data-readOnly="1" data-bgColor="#3e4043" /><p></p><span></span>' +
                        '</li>');
                    
                    tpl.find('p').text(data.files[0].name)
                                    .append('<i>' + formatFileSize(data.files[0].size) + '</i>');
                    
                    data.context = tpl.appendTo(ul);

                    tpl.find('input').knob();
                    dataUpload.push(data);

                    tpl.find('span').click(function () {
                        if (tpl.hasClass('working')) {
                            tpl.remove();
                            var idx = dataUpload.indexOf(data);
                            if (idx !== -1) {
                                dataUpload.splice(idx, 1);
                            }
                        }
                        tpl.fadeOut(function () {
                            fileCounter = (fileCounter <= 0) ? 0 : (fileCounter - 1);
                            $("#fileCounter").text(' ' +fileCounter);
                        });
                    });
                    fileCounter++;
                    $("#fileCounter").text(' ' + fileCounter);
                }
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                data.context.find('input').val(progress).change();
                if (progress == 100) {
                    data.context.removeClass('working');
                }
            },
            done: function (e, data) {
                if (data.textStatus == "success") {
                    fileCounter = (fileCounter <= 0) ? 0 : (fileCounter - 1);
                    $("#fileCounter").text(' ' + fileCounter);
                    dataUpload.splice(0, 1);
                    data.context.addClass('success');
                    console.log(data);
                }
                else{
                    data.context.addClass('warning');
                    console.log(data);
                }
            },
            fail: function (e, data) {
                data.context.addClass('error');
            }
        });

        $("#upload #div-btns #btn-upload-file").click(function () {
            var size = dataUpload.length;
            $.each(dataUpload, function (i, file) {
                file.submit();
            }, 1000);
        });

        
        $(document).on('drop dragover', function (e) {
            e.preventDefault();
        });

        function formatFileSize(bytes) {
            if (typeof bytes !== 'number') {
                return '';
            }

            if (bytes >= 1000000000) {
                return (bytes / 1000000000).toFixed(2) + ' GB';
            }

            if (bytes >= 1000000) {
                return (bytes / 1000000).toFixed(2) + ' MB';
            }

            return (bytes / 1000).toFixed(2) + ' KB';
        }


        $("#upload #div-btns #btn-upload-file").click(function () {
            $.each(dataUpload, function (i, file) {
                file.submit();
            }, 1000);
        });


        $(document).off("click", "button.btn-download").on("click", "button.btn-download", function (e) {
            e.preventDefault();
            var id = $(this).parent().siblings(":first").text();
            downloadEvidenceFile(id);
        });


        $("#load-evidences").unbind().click(function (e) {
            e.preventDefault();
            if (!$(this).parent().hasClass("active")) { 
                cleanEvidencesTable();
                loadEvidences();
            }
        });

        function cleanEvidencesTable() {
            $("#table-evidences tbody").empty();
        }
    });

    function downloadEvidenceFile(idFile) {
        $.ajax(
          {
              url: '/FileManagement/FileExists',
              type: "POST",
              datatype: "JSON",
              traditional: "true",
              data: { idFile: idFile},
              success: function (data) {
                  if (data == true) {
                      var url = "/FileManagement/DownloadFile?idFile=" + idFile;
                      window.location = url;
                  }
                  else {
                      alert("El archivo que buscas no existe");
                  }
              },
              error: function (XMLHttpRequest) {
                  alert("Se ha generado un error al descargar el archivo");
              }
          });
    }


    function loadEvidences() {
        $.ajax({
            url: "/FileManagement/GetEvidencesByTicketId",
            type: "POST",
            datatype: "JSON",
            traditional: "true",
            data:
                {
                    tickedId: $("#id_bitacora").val()
                },
            success: function (data) {
                var result = JSON.parse(data);
                result = (result != "") ? result : {};
                fillEvidencesTable(result);
            },
            error: function (XMLHttpRequest) {
                alert("Se ha generado un error al cargar las evidencias");
                fillEvidencesTable(null);
            }
        });


        function fillEvidencesTable(data) {
            var rows = '';
            if (data == null) {
                rows += '<tr><td colspan="5" style="font-weight:bold; text-align:center;">No hay archivos adjuntos</td></tr>';
            }
            else {
                $.each(data, function (i, val) {
                    rows += '<tr>';
                        rows += '<td>' + val.id + '</td>';
                        rows += '<td>' + val.extension + '</td>';
                        rows += '<td>' + val.name + val.extension + '</td>';
                        rows += '<td>' + converToDate(val.dateUploaded) + '</td>';                              
                        rows += '<td><button name="btn-download" type="button" class="btn btn-info btn-download">Descargar</button></td>';
                    rows += '</tr>';
                });
            }
            $("#table-evidences tbody").append(rows);;
        }

        function converToDate(value) {
            var pattern = /Date\(([^)]+)\)/;
            var results = pattern.exec(value);
            var dt = new Date(parseFloat(results[1]));
            return dt.getFullYear() + "/" + (dt.getMonth() + 1) + "/" + dt.getDate() +
                    " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
        }
    }
});
