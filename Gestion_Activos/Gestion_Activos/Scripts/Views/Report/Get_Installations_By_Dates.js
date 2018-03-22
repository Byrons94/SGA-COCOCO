$(function () {
    $('#datepicker').datepicker({
        autoclose: true
    });
    $('#datepicker1').datepicker({
        autoclose: true
    });
});

var table;
$(document).ready(function (){
    table = $('#table1').DataTable({
        "paging":       true,
        "lengthChange": true,
        "searching":    true,
        "ordering":     true,
        "info":         true,
        "autoWidth":    false,
        aLengthMenu: [
        [10, 50, 100, 200, -1],
        [10, 50, 100, 200, "Total"]
        ],
        iDisplayLength: 10
    });

    var estatus = '@TempData["cmb_estatus"]';
    $('input[name="t_estatus"]').each(function () {
        if ($(this).val() == estatus) {
            $(this).attr("checked", true);
        }
    });
});

$(".select").change(function () {
    var id  = $("#provincias>option:selected").html();
    var foo = $(this).val();
    var filter_Data = table
                            .columns(5)
                            .data()
                            .eq(0)
                            .filter(function (value) {
                                return (value == id) ? true : false;
                            });
    table.search(filter_Data[0]).draw();
});

$("#btn_buscar").click(function () {
    $("#modal_cargando").modal("show");
});

$(".checkbox").click(function () {
    var n_column = $(this).attr("identity").trim();
    var column   = table.column(n_column);
    column.visible(!column.visible());
});
   