using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Diagnostics;

/*
 * xlab anleitung
http://www.matrixguide.ch/Datenablage/diverses/How_to_Install_and_Setup_XLabs.pdf
*/


namespace IOControl
{
	public class MainPage : MasterDetailPage
	{
		MenuNavigation menuNavigation;

		public MainPage ()
		{
            MessagingCenter.Subscribe<ContentPage>(this, DT.Const.MSG_REFRESH, (s) =>
            {
                DT.Log("refresh cmd bekommen");
                RefreshNavigation();
            });

            DT.Log(App.Density.ToString());
			
            Detail = new NavigationPage(new ControlDemo())
            {
                BarBackgroundColor = DT.COLOR,
                BarTextColor = Color.FromHex("#FFFFFF")
            };

            if (Device.OS == TargetPlatform.Windows)
            {
				Master.Icon = "swap.png";
			}

            RefreshNavigation();
        }

        public void RefreshNavigation()
        {
            if (Device.OS != TargetPlatform.Windows)
            {
                DT.Session.xmlContent.Load();
            }

            menuNavigation = new MenuNavigation();
            menuNavigation.ListView.ItemTapped += OnItemSelected;

            Master = menuNavigation;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        
        async void OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            menuNavigation.ListView.SelectedItem = null;
            IsPresented = false;

            var item = e.Item as MenuNaviModel;
            if (item != null)
            {
                List<String> tabs = new List<String>();
                Module module = null;
                NavigationPage navPage = null;
                String msg = null;

                Task<bool> openModule = new Task<bool>(() =>
                {
                    if (item.GroupType == typeof(ContentLocation))
                    {
                        var location = (ContentLocation)item.Object;
                        List<Module> locModules = new List<Module>();

                        foreach (var group in location.groups)
                        {
                            foreach (var io in group.io)
                            {
                                var mod = DT.Session.xmlContent.modules.Find(x => x.mac == io.moduleMAC);
                                if (!locModules.Contains(mod))
                                {
                                    locModules.Add(mod);
                                }
                            }
                        }

                        foreach (var mod in locModules)
                        {
                            if (mod.OpenModule() == 0)
                            {
                                msg = String.Format(Resx.AppResources.MSG_OpenError, mod.boardname, mod.tcp_hostname);
                                mod.CloseModule();
                                return false;
                            }

                            mod.IOInit();
                            mod.CloseModule();
                        }

                        navPage = new NavigationPage(new IOTabbedPage(new IOTabbedPage.Constructor()
                        {
                            Title = location.name,
                            ObType = location.GetType(),
                            Object = location
                        }));
                    }
                    else if (item.GroupType == typeof(Module))
                    {
                        module = (Module)item.Object;

                        if (module.OpenModule() == 0)
                        {
                            msg = String.Format(Resx.AppResources.MSG_OpenError, module.boardname, module.tcp_hostname);
                            module.CloseModule();
                            return false;
                        }

                        module.IOInit();
                        module.CloseModule();

                        if (module.IO.cnt_di > 0) tabs.Add("DI");
                        if (module.IO.cnt_do > 0) tabs.Add("DO");
                        if (module.IO.cnt_do_timer > 0) tabs.Add("DO_Timer");
                        if (module.IO.cnt_do_pwm > 0) tabs.Add("PWM");
                        if (module.IO.cnt_ai > 0) tabs.Add("AD");
                        if (module.IO.cnt_ao > 0) tabs.Add("DA");
                        if (module.IO.cnt_temp > 0) tabs.Add("Temp");

                        navPage = new NavigationPage(new IOTabbedPage(new IOTabbedPage.Constructor()
                        {
                            Tabs = tabs,
                            Title = module.boardname,
                            ObType = module.GetType(),
                            Object = module
                        }));
                    }
                    else if (item.GroupType == typeof(AppearancePage))
                    {
                        navPage = new NavigationPage(new AppearancePage(new AppearancePage.Constructor()
                        {
                            ViewType = AppearancePage.ViewType.MAIN
                        }));
                    }

                    return true;
                }); // Task openModule

                Stopwatch sw = new Stopwatch();
                sw.Start();
                openModule.Start();

                if (item.GroupType == typeof(AppearancePage))
                {
                    UserDialogs.Instance.ShowLoading(Resx.AppResources.MSG_PleaseWait);
                }
                else
                {
                    UserDialogs.Instance.ShowLoading(Resx.AppResources.Main_Loading);
                }
                bool ok = await openModule;

                // künstlicher sleep damit die search animation durchkommt
                //while (sw.ElapsedMilliseconds < DT.Const.TIME_ANIMATION_MIN_MS)
                while (sw.ElapsedMilliseconds < 1000)
                {
                    await Task.Delay(DT.Const.TIME_ANIMATION_MIN_MS); //.Wait();
                }

                UserDialogs.Instance.HideLoading();

                if (ok)
                { 
                    navPage.BarBackgroundColor = DT.COLOR;
                    navPage.BarTextColor = Color.FromHex("#FFFFFF");
                    Detail = navPage;
                }
                else
                {
                    UserDialogs.Instance.Toast(msg);
                }

            }   // if (item != null)
        }
            
    }
}
