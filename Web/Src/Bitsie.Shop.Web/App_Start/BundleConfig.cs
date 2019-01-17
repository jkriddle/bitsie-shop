using System;
using System.Web;
using System.Web.Optimization;

namespace Bitsie.Shop.Web
{
    public class BundleConfig
    {
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            // Disabling temporarily - minifier is causing less files to not compile
            System.Web.Optimization.BundleTable.EnableOptimizations = false;

            /**********************************************************
             * Scripts
             **********************************************************/
            // Core libraries
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/core").Include(
                "~/Content/js/jquery.js",
                "~/Content/js/bootstrap.3.2.min.js",
                "~/Content/js/moment.js",
                "~/Content/js/handlebars.js",
                "~/Content/js/handlebars-helpers.js"));

            bundles.Add(new ScriptBundle("~/Bundles/Scripts/admin-core").Include(
                "~/Content/js/jquery.js",
                "~/Content/js/bootstrap.3.2.min.js",
                "~/Content/js/typeahead.bundle.js",
                "~/Content/js/moment.js",
                "~/Content/js/handlebars.js",
                "~/Content/js/handlebars-helpers.js"));

            // Plugins to the core libraries
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/plugins").Include(
                "~/Content/js/plugins/jquery-ui/jquery.ui.custom.js",
                "~/Content/js/plugins/jquery.cookie.js",
                "~/Content/js/plugins/jquery.livequery.js",
                "~/Content/js/plugins/select2/jquery.select2.js",
                "~/Content/js/plugins/jquery.serializeObject.js",
                "~/Content/js/plugins/jquery.ui.custom.js",
                "~/Content/js/plugins/jquery.form.js",
                "~/Content/js/plugins/bootbox.custom.js",
                "~/Content/js/plugins/fuelux/fuelux.min.js",
                "~/Content/js/plugins/datepicker/bootstrap-datepicker.js",
                "~/Content/js/plugins/daterangepicker/daterangepicker.js",
                "~/Content/js/plugins/toggle-buttons/jquery.toggle.buttons.js",
                "~/Content/js/plugins/validation/jquery.validate.js",
                "~/Content/js/plugins/validation/additional-methods.js",
                "~/Content/js/plugins/jquery.qrcode.js",
                "~/Content/js/plugins/colorpicker/js/bootstrap-colorpicker.js",
                "~/Content/js/plugins/tags-input/jquery.tagsinput.js",
                "~/Content/js/plugins/bip32/aes.js",
                "~/Content/js/plugins/bip32/crypto-min.js",
                "~/Content/js/plugins/bip32/bitcoinjs-min.js",
                "~/Content/js/plugins/bip32/sha512.js",
                "~/Content/js/plugins/bip32/modsqrt.js",
                "~/Content/js/plugins/bip32/qrcode.js",
                "~/Content/js/plugins/bip32/rfc1751.js",
                "~/Content/js/plugins/bip32/mnemonic.js",
                "~/Content/js/plugins/bip32/armory.js",
                "~/Content/js/plugins/bip32/bip32.js",
                "~/Content/js/plugins/bip32/electrum.js",
                "~/Content/js/plugins/bip32/tx.js",
                "~/Content/js/plugins/bip32/bitcoinsig.js",
                "~/Content/js/plugins/bip32/bip39.js"          
            ));

            // Bitsie.Shop Application
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/app").Include(
                "~/Content/js/app/core/inheritance.js",
                "~/Content/js/app/core/observable.js",
                "~/Content/js/app/core/app.js",
                "~/Content/js/app/core/config.js",
                "~/Content/js/app/core/template.js",
                "~/Content/js/app/core/api.js",                
                "~/Content/js/app/*.js",
                "~/Content/js/plugins/bip32/bitsie-hdwallet.js"));

            bundles.UseCdn = true;
            // Public site
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/public").Include(
                "~/Content/js/bootstrap.3.2.min.js",
                "~/Content/js/init.js",
                "~/Content/js/public.js"));

            bundles.Add(new ScriptBundle("~/Bundles/Scripts/mobile").Include(
                "~/Content/js/bootstrap.3.2.min.js",
                "~/Content/js/jquery.address-1.5.min.js",
                "~/Content/js/init.js",
                "~/Content/js/mobile.js"));

            // Manage area
            bundles.Add(new ScriptBundle("~/Bundles/Scripts/manage").Include(
                "~/Content/js/init.js",
                "~/Content/js/manage.js"                
                ));


            /**********************************************************
             * Styles
             **********************************************************/
            // Core library styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/core").Include(
                "~/Content/css/jquery.ui.custom.css"));

            bundles.Add(new StyleBundle("~/Bundles/Styles/admin-core").Include(
                "~/Content/css/bootstrap.3.2.0.css",
                "~/Content/css/bootstrap-responsive.css",
                "~/Content/css/jquery.ui.custom.css"));

            // Plugins styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/plugins").Include(
                "~/Content/js/plugins/jquery-ui/jquery.ui.custom.css",
                "~/Content/js/plugins/fuelux/fuelux.min.css",
                "~/Content/js/plugins/select2/select2.css",
                "~/Content/js/plugins/datepicker/datepicker.less",
                "~/Content/js/plugins/daterangepicker/daterangepicker.css",
                "~/Content/js/plugins/toggle-buttons/bootstrap.toggle.buttons.css",
                "~/Content/js/plugins/colorpicker/css/bootstrap-colorpicker.css",
                "~/Content/js/plugins/tags-input/tags-input.css"                
                            ));

            // Share app styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/app").Include(
                "~/Content/css/app/shared.less",
                "~/Content/css/app/buybutton.css"
                            ));

            // Public site styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/public").Include(
                "~/Content/css/bootstrap.3.2.0.css",
                "~/Content/css/app/public.less"
                            ));

            // Mobile site styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/mobile").Include(
                "~/Content/css/bootstrap.3.2.0.css",
                "~/Content/css/app/mobile.less"
                            ));

            // Checkout page styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/checkout").Include(
                "~/Content/css/app/checkout.less"
                            ));

            // Tipsie page styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/tipsie").Include(
                "~/Content/css/app/tipsie.less"
                            ));

            // Manage styles
            bundles.Add(new StyleBundle("~/Bundles/Styles/manage").Include(
                "~/Content/css/app/admin-main.css",
                "~/Content/css/app/admin-brand.less",                
                "~/Content/css/app/manage.less"));
        }
    }
}