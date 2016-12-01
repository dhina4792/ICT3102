using System.Web;
using System.Web.Optimization;

namespace StowagePlanAnalytics_ITP_2016
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/allJavaScripts").Include(
            "~/Scripts/jquery-2.2.3.js").Include(
            "~/Scripts/jquery-2.2.3.min.js").Include(
            "~/Scripts/jquery.validate*").Include(
            "~/Scripts/modernizr-*").Include(
            "~/Scripts/bootstrap.min.js").Include(
            "~/Scripts/jqPlot/jquery.jqplot.min.js").Include(
                        "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.min.js").Include( // Pie Creation
                        "~/Scripts/jqPlot/plugins/jqplot.meterGaugeRenderer.js").Include( //Meter Gauge Creation
                        "~/Scripts/jqPlot/plugins/jqplot.barRenderer.js").Include( //graph Creation
                        "~/Scripts/jqPlot/plugins/jqplot.categoryAxisRenderer.js").Include( //graph Creation
                        "~/Scripts/jqPlot/plugins/jqplot.pointLabels.js").Include( //graph Creation
                        "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js").Include( //Pie Creation
                        "~/Scripts/jqPlot/plugins/jqplot.donutRenderer.js").Include( //Pie Creation
                        "~/Scripts/jqPlot/plugins/jqplot.canvasTextRenderer.js").Include( //Plot Creation
                        "~/Scripts/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.js").Include( //Plot Creation
                        "~/Scripts/jqPlot/plugins/jqplot.CanvasAxisTickRenderer.js").Include( // AxisTickRender
                        "~/Scripts/jqPlot/plugins/jqplot.highlighter.js").Include(
                        "~/Scripts/DataAnalysis/dataanalysisPriority.js").Include(
                        "~/Scripts/DataAnalysis/dataanalysisDiagrams.js").Include(
                        "~/Scripts/DataAnalysis/dataanalysisValidation.js").Include(
                        "~/Scripts/DataAnalysis/dropdownlistFunctions.js").Include(
                        "~/Scripts/DataAnalysis/exportcsv.js").Include(
                        "~/Scripts/Port/portValidation.js").Include(
                        "~/Scripts/Port/portTableSetting.js").Include(
                       "~/Scripts/Vessel/vesselValidation.js").Include(
                        "~/Scripts/Vessel/vesselTableSetting.js").Include(
                         "~/Scripts/Service/servicePriority.js").Include(
                        "~/Scripts/Service/serviceValidation.js").Include(
                        "~/Scripts/Service/serviceTableSetting.js").Include(
                        "~/Scripts/STIFFileManagement/stiffTableSetting.js").Include(
                         "~/Scripts/LogData/LogData.js").Include(
                         "~/Scripts/Roles/rolesValidation.js").Include(
                        "~/Scripts/Roles/rolesTableSetting.js").Include(
                        "~/Scripts/jQuery.FileUpload/FileJs.js").Include(
                         "~/Scripts/Modal/popup.js")); //PieChart Higherlighter));

            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-2.2.3.js",
            //            "~/Scripts/jquery-2.2.3.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //            "~/Scripts/bootstrap.min.js"));
            bundles.Add(new StyleBundle("~/Content/allCss").Include(
            //"~/Content/dashboard.css").Include(
            //"~/Content/bootstrap.min.css").Include(
            //"~/Content/Admin/AdminCSS.css").Include(
            //"~/Content/Account/AccountCSS.css").Include(
            //"~/Content/Roles/RolesCSS.css").Include(
            //                        "~/Scripts/jqPlot/jquery.jqplot.css").Include(
            //            "~/Content/DataAnalysis/DataAnalysisList.css").Include(
            //                                    "~/Content/jQuery.FileUpload/css/FileCSS.css"
                        ));
            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //            "~/Content/dashboard.css",
            //            "~/Content/bootstrap.min.css"));

            //bundles.Add(new StyleBundle("~/Content/Admin").Include(
            //            "~/Content/Admin/AdminCSS.css"));

            //bundles.Add(new StyleBundle("~/Content/Account").Include(
            //            "~/Content/Account/AccountCSS.css"));

            //bundles.Add(new StyleBundle("~/Content/Roles").Include(
            //            "~/Content/Roles/RolesCSS.css"));

            ////-- DataAnalysis Bundles
            //bundles.Add(new StyleBundle("~/Content/DataAnalysis").Include(
            //            "~/Scripts/jqPlot/jquery.jqplot.css",
            //            "~/Content/DataAnalysis/DataAnalysisList.css"));
            //bundles.Add(new ScriptBundle("~/bundles/jqPlot").Include(
            //            "~/Scripts/jqPlot/jquery.jqplot.min.js",
            //            "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.min.js", // Pie Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.meterGaugeRenderer.js", //Meter Gauge Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.barRenderer.js", //graph Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.categoryAxisRenderer.js", //graph Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.pointLabels.js", //graph Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.pieRenderer.js", //Pie Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.donutRenderer.js", //Pie Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.canvasTextRenderer.js", //Plot Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.canvasAxisLabelRenderer.js", //Plot Creation
            //            "~/Scripts/jqPlot/plugins/jqplot.CanvasAxisTickRenderer.js", // AxisTickRender
            //            "~/Scripts/jqPlot/plugins/jqplot.highlighter.js")); //PieChart Higherlighter

            //bundles.Add(new ScriptBundle("~/bundles/DataAnalysis").Include(
            //            "~/Scripts/DataAnalysis/dataanalysisPriority.js",
            //            "~/Scripts/DataAnalysis/dataanalysisDiagrams.js",
            //            "~/Scripts/DataAnalysis/dataanalysisValidation.js",
            //            "~/Scripts/DataAnalysis/dropdownlistFunctions.js",
            //            "~/Scripts/DataAnalysis/exportcsv.js"));

            ////-- Port Bundles
            //bundles.Add(new ScriptBundle("~/bundles/Port").Include(
            //            "~/Scripts/Port/portValidation.js",
            //            "~/Scripts/Port/portTableSetting.js"));

            ////-- Vessel Bundles
            //bundles.Add(new ScriptBundle("~/bundles/Vessel").Include(
            //            "~/Scripts/Vessel/vesselValidation.js",
            //            "~/Scripts/Vessel/vesselTableSetting.js"));

            ////-- Service Bundles
            //bundles.Add(new ScriptBundle("~/bundles/Service").Include(
            //            "~/Scripts/Service/servicePriority.js",
            //            "~/Scripts/Service/serviceValidation.js",
            //            "~/Scripts/Service/serviceTableSetting.js"));

            ////-- STIFFile Bundles
            //bundles.Add(new ScriptBundle("~/bundles/STIFFileManagement").Include(
            //            "~/Scripts/STIFFileManagement/stiffTableSetting.js"));

            ////-- LogData Bundles
            //bundles.Add(new ScriptBundle("~/bundles/LogData").Include(
            //            "~/Scripts/LogData/LogData.js"));


            ////-- Roles Bundles
            //bundles.Add(new ScriptBundle("~/bundles/Roles").Include(
            //            "~/Scripts/Roles/rolesValidation.js",
            //            "~/Scripts/Roles/rolesTableSetting.js"));


            //-- File Bundles
            //bundles.Add(new StyleBundle("~/Content/File").Include(
            //            "~/Content/jQuery.FileUpload/css/FileCSS.css"));

            //bundles.Add(new ScriptBundle("~/bundles/File").Include(
            //            "~/Scripts/jQuery.FileUpload/FileJs.js"));

            ////-- Modal Bundles
            //bundles.Add(new ScriptBundle("~/bundles/Modal").Include(
            //            "~/Scripts/Modal/popup.js"));
        }
    }
}

