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
            MessagingCenter.Subscribe<ContentPage>(this, Sess.MC_MSG_REFRESH, (s) =>
            {
                Sess.Log("refresh cmd bekommen");
                RefreshNavigation();
            });

            Sess.Log(App.Density.ToString());
			
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
            IsPresented = true;
        }

        public void RefreshNavigation()
        {
            if (Device.OS != TargetPlatform.Windows)
            {
                Sess.Xml.Load();
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

            var item = e.Item as MainNaviItem;
            if (item != null)
            {
                List<String> tabs = new List<String>();
                ETHModule.Module module = null;
                NavigationPage navPage = null;
                String msg = null;

                Task<bool> openModule = new Task<bool>(() =>
                {
                    if (item.GroupType == typeof(XML.XMLView))
                    {
                        var location = (XML.XMLView)item.Object;
                        List<ETHModule.Module> locModules = new List<ETHModule.Module>();

                        foreach (var group in location.Groups)
                        {
                            foreach (var io in group.IOs)
                            {
                                var mod = Sess.Xml.Modules.Find(x => x.mac == io.MAC);

                                if (mod == null)
                                {
                                    //das ist ein I/O zu dem es KEIN modul mehr gibt..
                                    UserDialogs.Instance.Confirm(new ConfirmConfig()
                                    {
                                        Message = String.Format(Resx.AppResources.NAV_CleanIOs, io.MAC),
                                        Title = Resx.AppResources.MSG_Error,
                                        OnAction = (ok) =>
                                        {
                                            if (ok)
                                            {
                                                foreach(var l in Sess.Xml.Views)
                                                {
                                                    foreach(var g in l.Groups)
                                                    {
                                                        g.IOs.RemoveAll(x => x.MAC == io.MAC);
                                                    }
                                                }
                                            }
                                        }
                                    });
                                }

                                if (!locModules.Contains(mod) && (mod != null))
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
                            Title = location.Name,
                            ObType = location.GetType(),
                            Object = location
                        }));
                    }
                    else if (item.GroupType == typeof(ETHModule.Module))
                    {
                        module = (ETHModule.Module)item.Object;

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

                        if (tabs.Count == 0)
                        {
                            // keine unterstützten I/Os
                            msg = Resx.AppResources.NAV_IOUnavailable;
                            return false;
                        }

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


                if (await GUIAnimation.ShowLoading(openModule))
                {
                    IsPresented = false;
                    navPage.BarBackgroundColor = DT.COLOR;
                    navPage.BarTextColor = Color.FromHex("#FFFFFF");
                    Detail = navPage;
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(msg, Resx.AppResources.MSG_Error);
                }

            }   // if (item != null)
        }
            
    }
}
