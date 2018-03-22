var table;
$(document).ready(function (){
    table = $('#table1').DataTable({
        "paging": true,
        "lengthChange": false,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": true,
        "iDisplayLength": 8
    });
});
 

$(document).ready(function () {
    $.ajax({
        url: "/Dashboard/Get_History_Tickets",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        success: function (result) {
            var json = JSON.parse(result);
            load_graph(json[1], 'line-chart', "black", "#292929");
            load_graph_resume(json[1], "knob_c", "knob_c1", "knob_c2");

            load_graph(json[0], 'line-chart2', "#fff", "#efefef");
            load_graph_resume(json[0], "knob", "knob1", "knob2");
        },
        error: function (XMLHttpRequest) {
            alert("no se ha podido cargar el resumen de tickets");
        }
    });

    $.ajax({
        url: "/Client/Get_Clients_Summary",
        type: "POST",
        datatype: "JSON",
        traditional: "true",
        success: function (result) {
            var json = JSON.parse(result);
            $("#n_activos").text(json[0]);
            $("#n_inactivos").text(json[1]);
            $("#n_total").text(json[2]);
        },
        error: function (XMLHttpRequest) {
            alert("no se ha podido cargar resumen de afiliados");
        }
    });
});


function load_graph(data2, class_g, color, color2) {
    var line = new Morris.Line({
        element: class_g,
        resize: true,
        data: data2,
        xkey: "name_day",
        ykeys:  ['installed', 'visited', 'retired'],
        labels: ['Instalaciones', 'Visita ', 'Retiro'],
        lineColors: ['#00C0EF', '#00A65A', '#F39C12'],
        lineWidth: 2,
        hideHover: 'auto',
        gridTextColor: color,
        parseTime:false,
        gridStrokeWidth: 0.4,
        pointSize: 4,
        pointStrokeColors: ["#efefef"],
        gridLineColor: color2,
        gridTextFamily: "Open Sans",
        yLabelFormat: function (y) { return y != Math.round(y) ? '' : y; },
        gridTextSize: 10
    });        
}


function load_graph_resume(json, class1, class2, class3) {
    var install = 0;
    var visited = 0;
    var retired = 0;
    $.each(json, function (i, item) {
        install += item.installed;
        visited += item.visited;
        retired += item.retired;
    });
    var total = install + visited + retired;
    charge_char(class1, install, total);
    charge_char(class2, visited, total);
    charge_char(class3, retired, total);
}

        
function charge_char(n_class, value, total) {
    $("." + n_class).knob();
    $('.' + n_class).val(value).trigger('change');
    $('.' + n_class).trigger('configure', {
        max: total
    });
    $('.' + n_class).animate({ value: value }, {
        duration: 1000,
        easing: 'swing',
        step: function () {
            $('.dial').val(this.value).trigger('change');
        }
    });
}