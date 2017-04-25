using Xamarin.Forms;
using System.Reflection;
using System;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IOControl
{
	public class App : Application
	{
        static public int Density;

        public App ()
		{
            /*
            System.Diagnostics.Debug.WriteLine("====== resource debug info =========");
            var assembly = typeof(App).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
            System.Diagnostics.Debug.WriteLine("====================================");
            */

            // This lookup NOT required for Windows platforms - the Culture will be automatically set
            if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
            {
                // determine the correct, supported .NET culture
                var localizeService = DependencyService.Get<ILocalizeUtils>();
                var ci = localizeService.GetCurrentCultureInfo();
                Resx.AppResources.Culture = ci; // set the RESX for resource localization
                localizeService.SetLocale(ci); // set the Thread for locale-aware methods
            }

            MainPage = new IOControl.MainPage ();
		}

		protected override void OnStart ()
		{
            // Handle when your app starts
            Sess.Log("OnStart");
		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            Sess.Log("OnSleep");
        }

		protected override void OnResume ()
		{
            // Handle when your app resumes
            Sess.Log("OnResume");
        }
	}
}

