using System.Web;
using System.Web.Optimization;

namespace Gestion_Activos
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/knopjs").Include(
                 "~/Scripts/plugins/knob/jquery.knob.js"));

            bundles.Add(new StyleBundle("~/bundles/Template/Style").Include(
                        "~/Content/dist/css/AdminLTE.min.css",
                        "~/Content/dist/css/skins/_all-skins.min.css",
                        "~/Content/custom.css",
                        "~/Scripts/plugins/datatables/dataTables.bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css"));
            
            bundles.Add(new StyleBundle("~/Assets/Fonts"));
            
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/sketch").Include(
                        "~/Scripts/plugins/skecth/sketch.js"));
            
            //separar esto despues cuando este mas robusto para traer solo lo necesario
            bundles.Add(new ScriptBundle("~/bundle/scripts/plugins").Include(
                        "~/Scripts/dist/js/app.min.js",
                        "~/Content/dist/js/app.min.js",
                        "~/Scripts/plugins/sparkline/jquery.sparkline.min.js",
                        "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundle/scripts/dataTables").Include(
                    "~/Scripts/plugins/datatables/jquery.dataTables.min.js",
                    "~/Scripts/plugins/datatables/dataTables.bootstrap.min.js"
                ));

            #region checkbox
            bundles.Add(new ScriptBundle("~/bundle/scripts/checkboxjs").Include(
                    "~/Scripts/plugins/iCheck/icheck.min.js"));
            bundles.Add(new StyleBundle("~/bundle/scripts/checkboxcss").Include(
                    "~/Scripts/plugins/iCheck/all.css"));
            #endregion

            #region graficos
            bundles.Add(new ScriptBundle("~/bundle/scripts/morrisjs").Include(
                    "~/Scripts/plugins/morris/morris.min.js",
                    "~/Scripts/plugins/knob/jquery.knob.js"));
            
            bundles.Add(new StyleBundle("~/bundle/scripts/morriscss").Include(
                   "~/Scripts/plugins/morris/morris.css"));
            #endregion
            
            #region table_export
            bundles.Add(new ScriptBundle("~/bundle/scripts/table_export").Include(
                    "~/Scripts/plugins/table_export/tableExport.js",
                    "~/Scripts/plugins/table_export/jquery.base64.js",
                    "~/Scripts/plugins/table_export/html2canvas.js",
                    "~/Scripts/plugins/table_export/jspdf/libs/sprintf.js",
                    "~/Scripts/plugins/table_export/jspdf/jspdf.js",
                    "~/Scripts/plugins/table_export/jspdf/libs/base64.js"));
            #endregion

            #region datetime

            bundles.Add(new ScriptBundle("~/bundle/scripts/datetimejs").Include(
                    "~/Scripts/plugins/daterangepicker/daterangepicker.js",
                    "~/Scripts/plugins/datepicker/bootstrap-datepicker.js"));

            bundles.Add(new StyleBundle("~/bundle/styles/datepickercss").Include(
                   "~/Scripts/plugins/daterangepicker/daterangepicker.css",
                   "~/Scripts/plugins/datepicker/datepicker3.css"));
            #endregion
            
            #region select_multiple
            bundles.Add(new StyleBundle("~/bundle/styles/select2css").Include(
                   "~/Scripts/plugins/select2/select2.min.css"));

            bundles.Add(new ScriptBundle("~/bundle/scripts/select_multi").Include(
                  "~/Scripts/plugins/select2/select2.full.min.js"));
            #endregion

            #region pace_progress_bar
            bundles.Add(new ScriptBundle("~/bundle/scripts/pacejs").Include(
                   "~/Scripts/plugins/pace/pace.js"));

            bundles.Add(new StyleBundle("~/bundle/styles/pacetheme").Include(
                   "~/Scripts/plugins/pace/themes/pace-big-counter.css"));
            #endregion

            #region
            bundles.Add(new ScriptBundle("~/bundle/scripts/file-upload").Include(
                "~/Scripts/plugins/jQueryUI/jquery-ui.min.js",
                "~/Scripts/plugins/file-uploader/jquery.fileupload.js",
                "~/Scripts/plugins/file-uploader/jquery.ui.widget.js",
                "~/Scripts/plugins/file-uploader/jquery.iframe-transport.js"));
            #endregion
        }
    }
}
