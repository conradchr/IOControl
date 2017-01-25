﻿using System.Collections.Generic;
using Xamarin.Forms;

using System;
//using Java.Net;

/*
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DELIB;
using Android.Net.Wifi;
using Android.Util;
*/

using System.Threading.Tasks;
using System.Text;
//using System.Collections.Generic;
using System.Threading;
using Java.Net;

using Acr.UserDialogs;


namespace IOControl
{
	public class ControlDemo : ContentPage
	{
        public void MachWas()
        {

        }

		public ControlDemo ()
		{
            var settings = new ToolbarItem
            {
                Text = "Settings",
                Command = new Command(MachWas)
            };
            ToolbarItems.Add(settings);

            Button btn = new Button();
            btn.Clicked += Btn_Clicked;
            btn.Text = "Relais an RO-ETH + RO-ETH/LC";
            btn.BackgroundColor = Color.White;
            btn.TextColor = Color.Black;

            Button btn3 = new Button();
            btn3.Clicked += Btn3_Clicked;
            btn3.Text = "Relais aus RO-ETH + RO-ETH/LC";
            btn3.BackgroundColor = Color.White;
            btn3.TextColor = Color.Black;

            Button btn4 = new Button();
            btn4.Clicked += Btn4_Clicked;
            btn4.Text = "WLAN Check";
            btn4.BackgroundColor = Color.White;
            btn4.TextColor = Color.Black;

            Button btn5 = new Button();
            btn5.Clicked += Btn5_Clicked;
            btn5.Text = "Menü aktualisieren";
            btn5.BackgroundColor = Color.White;
            btn5.TextColor = Color.Black;

            Button btn6 = new Button();
            btn6.Clicked += Btn6_Clicked;
            btn6.Text = "ACR Dialog Test";
            btn6.BackgroundColor = Color.White;
            btn6.TextColor = Color.Black;


            DT.Log(Device.OS.ToString());

            switch (Device.OS)
            {
                case TargetPlatform.Other:
                    BackgroundColor = Color.Black;
                    break;
                case TargetPlatform.iOS:
                    break;
                case TargetPlatform.Android:
                    //BackgroundColor = Color.Blue;
                    break;
                case TargetPlatform.WinPhone:
                    BackgroundColor = Color.Black;
                    break;
                case TargetPlatform.Windows:
                    BackgroundColor = Color.Black;
                    break;
                default:
                    BackgroundColor = Color.Black;
                    break;
            }

            Title = "Control Demo";

            Content = new ScrollView()
            {
                Content = new StackLayout()
                {
                    Children =
                    {
                        /*
                        DTControl.Slider(),
                        DTControl.Separator(),
                        DTIOControl.DOSwitch(),
                        DTControl.Separator(),
                        DTIOControl.DISwitch(),
                        DTControl.Separator(),
                        DTIOControl.DISwitch2(),
                        DTControl.Separator(),
                        DTIOControl.ADView(),
                        DTControl.Separator(),
                        DTIOControl.DAView(),
                        DTControl.Separator(),
                        DTIOControl.TempView(),
                        DTControl.Separator(),
                        DTIOControl.TempView2(),
                        */
                        DTControl.Separator(),
                        btn,
                        DTControl.Separator(),
                        btn3,
                        DTControl.Separator(),
                        btn4,
                        DTControl.Separator(),
                        btn5,
                        DTControl.Separator(),
                        btn6,
                        DTControl.Separator()
                    }
                }
            };
        }

        private void Btn_Clicked(object sender, System.EventArgs e)
        {
            uint handle = DT.Delib.DapiOpenModuleEx("192.168.1.219", 9912, 1000, "hallo");
            uint handle2 = DT.Delib.DapiOpenModuleEx("192.168.1.2", 9912, 1000, "hallo");

            DT.Delib.DapiDOSet1(handle, 0, 1);
            DT.Delib.DapiCloseModule(handle);

            DT.Delib.DapiDOSet1(handle2, 0, 1);
            DT.Delib.DapiCloseModule(handle2);
        }

        private void Btn3_Clicked(object sender, System.EventArgs e)
        {
            uint handle = DT.Delib.DapiOpenModuleEx("192.168.1.219", 9912, 1000, "hallo");
            uint handle2 = DT.Delib.DapiOpenModuleEx("192.168.1.2", 9912, 1000, "hallo");

            DT.Delib.DapiDOSet1(handle, 0, 0);
            DT.Delib.DapiCloseModule(handle);

            DT.Delib.DapiDOSet1(handle2, 0, 0);
            DT.Delib.DapiCloseModule(handle2);
        }


        
        /*
        private void Btn2_Clicked(object sender, System.EventArgs e)
        {
            //App.Current.MainPage = new NavigationPage(new DialogBroadcast());

            //XLabs.Forms.Controls.PopupLayout pLayout = new XLabs.Forms.Controls.PopupLayout();
            


        }
        */
        

        

        private void Btn4_Clicked(object sender, System.EventArgs e)
        {
            var wifiService = DependencyService.Get<IWifiUtils>();
            bool wifiConnected = wifiService.WlanIsConnected();
            String ip = "";

            if (wifiConnected)
            {
                ip = wifiService.GetDeviceIP();
            }

            DisplayAlert("WLAN Status", String.Format("Connected={0}\nIP={1}", wifiConnected, ip), "ok");
        }

        private void Btn5_Clicked(object sender, System.EventArgs e)
        {
            var xx = DT.Session.xmlContent.loc.Find(x => x.name == "aa");
            if (xx == null)
            {
                DT.Session.xmlContent.loc.Add(new ContentLocation("aa"));
            }
            else
            {
                DT.Session.xmlContent.loc.Remove(xx);
            }
            DT.Session.xmlContent.Save();

            DT.Log("sende refresh request");
            MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);
        }

        private async void Btn6_Clicked(object sender, System.EventArgs e)
        {
            Dictionary<string, IOType> ioTypes = new Dictionary<string, IOType>();
            ioTypes.Add(Resx.AppResources.Module_DI, IOType.DI);
            ioTypes.Add(Resx.AppResources.Module_DO, IOType.DO);
            ioTypes.Add(Resx.AppResources.Module_PWM, IOType.PWM);
            ioTypes.Add(Resx.AppResources.Module_AI, IOType.AD);
            ioTypes.Add(Resx.AppResources.Module_AO, IOType.DA);
            ioTypes.Add(Resx.AppResources.Module_TEMP, IOType.TEMP);

            string[] options = new string[ioTypes.Keys.Count];
            ioTypes.Keys.CopyTo(options, 0);

            var io = await DisplayActionSheet(
                Resx.AppResources.CFG_AddIOHeader,
                Resx.AppResources.MSG_Cancel,
                null,
                options
            );

            if (ioTypes.ContainsKey(io))
            {
                await Navigation.PushAsync(new DialogAddIO(new DialogAddIO.Constructor()
                {
                    IOType = ioTypes[io]
                }));
            }

            //return true;


            /*
            Func<Task<bool>> MachTask = () =>
            {
                return new Task<bool>(() =>
                {
                    System.Threading.Tasks.Task.Delay(2000).Wait();
                    return true;
                });
            };


            Task<bool> wait;

            var action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, "Email", "Twitter", "Facebook");
            DT.Log("Action: " + action);

            var test = await DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", "Photo Roll", "Email");
            DT.Log("Action: " + action);

            var test2 = await UserDialogs.Instance.ActionSheetAsync("test", "cancel", "destructive", null, new string[] { "a", "b", "c" });
            */

            /*
            UserDialogs.Instance.Toast("bla bla bla ... ich bin ein testtoast!!!");
            wait = MachTask();
            wait.Start();
            await wait;

            UserDialogs.Instance.ShowLoading("lade");
            wait = MachTask();
            wait.Start();
            await wait;
            UserDialogs.Instance.HideLoading();
            

            UserDialogs.Instance.ShowError("mööp!");
            wait = MachTask();
            wait.Start();
            await wait;

            UserDialogs.Instance.ShowSuccess("yeah!");
            wait = MachTask();
            wait.Start();
            await wait;

            
            PromptResult pm = await UserDialogs.Instance.PromptAsync(new PromptConfig()
            {
                Title = "mach sachen..",
                Placeholder = "eingabe",
                Message = "gib sachen ein..",
                OkText = "ok",
                CancelText = "weg",
                Text = "abc"
            });
            if (pm.Ok)
            {
                DT.Log(String.Format("text = {0}", pm.Text));
            }

            await wait;
            var action = await UserDialogs.Instance.ActionSheetAsync("test", "cancel", "destructive", null, new string[] { "a", "b", "c" });
            */
        }




    }
}
