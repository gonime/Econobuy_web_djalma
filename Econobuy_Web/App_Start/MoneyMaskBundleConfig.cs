using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Econobuy_Web.App_Start.MoneyMaskBundleConfig), "RegisterBundles")]

namespace Econobuy_Web.App_Start
{
	public class MoneyMaskBundleConfig
	{
		public static void RegisterBundles()
		{
			BundleTable.Bundles.Add(new ScriptBundle("~/bundles/moneymask").Include("~/Scripts/jquery.moneymask.js"));
		}
	}
}