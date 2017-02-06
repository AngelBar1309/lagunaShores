using System.Web;
using System.Web.Optimization;

namespace LagunaShoreResort2
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-ui-{version}.js"
                        ));
                        

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/SalesContract.js",
                       "~/Scripts/modalform.js"));

            bundles.Add(new ScriptBundle("~/bundles/myScripts").Include(
                      "~/Scripts/myScripts.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                     "~/Content/themes/base/jquery.ui.all.css",
                     "~/Content/themes/base/jquery.ui.datepicker.css",
                     "~/Content/themes/base/jquery.ui.core.css"
                     ));

            //////////////////////////////////////////////////////////////////
            bundles.Add(new ScriptBundle("~/app/js/app").Include(
                "~/app/js/app.js",
                "~/app/js/pages.js"
                ));

            bundles.Add(new StyleBundle("~/vendor").Include(
                "~/vendor/animo/animate_animo.css",
                "~/vendor/fontawesome/css/font-awesome.min.css",
                "~/vendor/csspinner/csspinner.min.css"
                ));
            
        }
    }
}
