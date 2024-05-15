using System.Drawing;
using System.Web;
using System.Web.Optimization;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using System.Data.Entity.Infrastructure;

namespace AssitADSOproyect
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/assets").Include(
                        "~/assets/js/jquery-3.5.1.min.js",
                        "~/assets/js/icons/feather-icon/feather.min.js",
                        "~/assets/js/icons/feather-icon/feather-icon.js",
                        "~/assets/js/sidebar-menu.js",
                        "~/assets/js/config.js",
                        "~/assets/js/bootstrap/popper.min.js",
                        "~/assets/js/bootstrap/bootstrap.min.js",
                        "~/assets/js/chart/chartist/chartist.js",
                        "~/assets/js/chart/chartist/chartist-plugin-tooltip.js",
                        "~/assets/js/chart/knob/knob.min.js",
                        "~/assets/js/chart/knob/knob-chart.js",
                        "~/assets/js/chart/apex-chart/apex-chart.js",
                        "~/assets/js/chart/apex-chart/stock-prices.js",
                        "~/assets/js/prism/prism.min.js",
                        "~/assets/js/clipboard/clipboard.min.js",
                        "~/assets/js/counter/jquery.waypoints.min.js",
                        "~/assets/js/counter/jquery.counterup.min.js",
                        "~/assets/js/counter/counter-custom.js",
                        "~/assets/js/custom-card/custom-card.js",
                        "~/assets/js/notify/bootstrap-notify.min.js",
                        "~/assets/js/vector-map/jquery-jvectormap-2.0.2.min.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-world-mill-en.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-us-aea-en.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-uk-mill-en.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-au-mill.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-chicago-mill-en.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-in-mill.js",
                        "~/assets/js/vector-map/map/jquery-jvectormap-asia-mill.js",
                        "~/assets/js/dashboard/default.js",
                        "~/assets/js/notify/index.js",
                        "~/assets/js/datepicker/date-picker/datepicker.js",
                        "~/assets/js/datepicker/date-picker/datepicker.en.js",
                        "~/assets/js/datepicker/date-picker/datepicker.custom.js",
                        "~/assets/js/datatable/datatables/jquery.dataTables.min.js",
                        "~/assets/js/datatable/datatables/datatable.custom.js",
                        "~/assets/js/owlcarousel/owl.carousel.js",
                        "~/assets/js/general-widget.js",
                        "~/assets/js/height-equal.js",
                        "~/assets/js/tooltip-init.js",
                        "~/assets/js/script.js",
                        "~/assets/js/theme-customizer/customizer.js"
                        ));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios.  De esta manera estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/assets").Include(
                      "~/assets/css/fontawesome.css",
                      "~/assets/css/icofont.css",
                      "~/assets/css/flag-icon.css",
                      "~/assets/css/feather-icon.css",
                      "~/assets/css/animate.css",
                      "~/assets/css/chartist.css",
                      "~/assets/css/date-picker.css",
                      "~/assets/css/prism.css",
                      "~/assets/css/vector-map.css",
                      "~/assets/css/datatables.css",
                      "~/assets/css/owlcarousel.css",
                      "~/assets/css/whether-icon.css",
                      "~/assets/css/bootstrap.css",
                      "~/assets/css/style.css",
                      "~/assets/css/color-1.css",
                      "~/assets/css/responsive.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

        }
    }
}
