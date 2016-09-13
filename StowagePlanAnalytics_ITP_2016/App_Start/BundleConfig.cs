using System.Web;
using System.Web.Optimization;

namespace StowagePlanAnalytics_ITP_2016
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/dashboard.css",
                        "~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/Admin").Include(
                        "~/Content/Admin/AdminCSS.css"));

            bundles.Add(new StyleBundle("~/Content/Account").Include(
                        "~/Content/Account/AccountCSS.css"));

            bundles.Add(new StyleBundle("~/Content/Roles").Include(
                        "~/Content/Roles/RolesCSS.css"));

            //-- DataAnalysis Bundles
            bundles.Add(new StyleBundle("~/Content/DataAnalysis").Include(
                        "~/Scripts/jqPlot/jquery.jqplot.css",
                        "~/Content/DataAnalysis/DataAnalysisList.css"));
            bundles.Add(new ScriptBundle("~/bundles/jqPlot").Include(
                        "~/Scripts/jqPlot/jquery.jqplot.min.js",
                        "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.min.js", // Pie Creation
                        "~/Scripts/jqPlot/plugins/jqplot.meterGaugeRenderer.js", //Meter Gauge Creation
                        "~/Scripts/jqPlot/plugins/jqplot.barRenderer.js", //graph Creation
                        "~/Scripts/jqPlot/plugins/jqplot.categoryAxisRenderer.js", //graph Creation
                        "~/Scripts/jqPlot/plugins/jqplot.pointLabels.js", //graph Creation
                        "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js", //Pie Creation
                        "~/Scripts/jqPlot/plugins/jqplot.donutRenderer.js", //Pie Creation
                        "~/Scripts/jqPlot/plugins/jqplot.canvasTextRenderer.js", //Plot Creation
                        "~/Scripts/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.js", //Plot Creation
                        "~/Scripts/jqPlot/plugins/jqplot.CanvasAxisTickRenderer.js", // AxisTickRender
                        "~/Scripts/jqPlot/plugins/jqplot.highlighter.js")); //PieChart Higherlighter

            bundles.Add(new ScriptBundle("~/bundles/DataAnalysis").Include(
                        "~/Scripts/DataAnalysis/dataanalysisPriority.js",
                        "~/Scripts/DataAnalysis/dataanalysisDiagrams.js",
                        "~/Scripts/DataAnalysis/dataanalysisValidation.js",
                        "~/Scripts/DataAnalysis/dropdownlistFunctions.js",
                        "~/Scripts/DataAnalysis/exportcsv.js"));

            //-- Port Bundles
            bundles.Add(new ScriptBundle("~/bundles/Port").Include(
                        "~/Scripts/Port/portValidation.js",
                        "~/Scripts/Port/portTableSetting.js"));

            //-- Vessel Bundles
            bundles.Add(new ScriptBundle("~/bundles/Vessel").Include(
                        "~/Scripts/Vessel/vesselValidation.js",
                        "~/Scripts/Vessel/vesselTableSetting.js"));

            //-- Service Bundles
            bundles.Add(new ScriptBundle("~/bundles/Service").Include(
                        "~/Scripts/Service/servicePriority.js",
                        "~/Scripts/Service/serviceValidation.js",
                        "~/Scripts/Service/serviceTableSetting.js"));

            //-- STIFFile Bundles
            bundles.Add(new ScriptBundle("~/bundles/STIFFileManagement").Include(
                        "~/Scripts/STIFFileManagement/stiffTableSetting.js"));

            //-- LogData Bundles
            bundles.Add(new ScriptBundle("~/bundles/LogData").Include(
                        "~/Scripts/LogData/LogData.js"));


            //-- Roles Bundles
            bundles.Add(new ScriptBundle("~/bundles/Roles").Include(
                        "~/Scripts/Roles/rolesValidation.js",
                        "~/Scripts/Roles/rolesTableSetting.js"));


            //-- File Bundles
            bundles.Add(new StyleBundle("~/Content/File").Include(
                        "~/Content/jQuery.FileUpload/css/FileCSS.css"));

            bundles.Add(new ScriptBundle("~/bundles/File").Include(
                        "~/Scripts/jQuery.FileUpload/FileJs.js"));

            //-- Modal Bundles
            bundles.Add(new ScriptBundle("~/bundles/Modal").Include(
                        "~/Scripts/Modal/popup.js"));
        }
    }
}
