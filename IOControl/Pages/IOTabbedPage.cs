using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

//using XLabs.Forms.Controls;

namespace IOControl
{
	public partial class IOTabbedPage : TabbedPage
    {
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class Constructor
        {
            public String Title { get; set; }
            public List<string> Tabs { get; set; }

            public Type ObType { get; set; }
            public Object Object { get; set; }
        }

        public Constructor Ctor { get; set; }


        Task<int> moduleThread;
        bool moduleThreadActive = false;
        int moduleThreadTimer;

        const int TIMER_REFRESH_MS = 5000;
        const int TIMER_INTERVAL_MS = 20;

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public IOTabbedPage(Constructor ctor)
        {
            Ctor = ctor;
            Title = Ctor.Title;

            Action PageChanging = () =>
            {
                IOContentPage page = (IOContentPage)this.CurrentPage;
                
                Sess.Log("page-init");
                page.Init();
                IOContentPageSelect(page);
            };


            if (Ctor.ObType == typeof(ETHModule.Module))
            {
                foreach (var tab in Ctor.Tabs)
                {
                    Children.Add(new IOContentPage(new IOContentPage.Constructor()
                    {
                        ViewType = IOContentPage.ViewType.MODULE,
                        Module = (ETHModule.Module)Ctor.Object,
                        Title = tab,
                        IOType = GetIOType(tab)
                    }));
                }
            }
            else
            {
                foreach(var group in ((XML.XMLView)Ctor.Object).Groups)
                { 
                    Children.Add(new IOContentPage(new IOContentPage.Constructor()
                    {
                        ViewType = IOContentPage.ViewType.CUSTOM,
                        Group = group,
                        Title = group.Name
                    }));
                }

                ToolbarItems.Add(new ToolbarItem()
                {
                    Text = "Settings",
                    Icon = "btn_setting.png",
                    Command = new Command(async () =>
                    {
                        AppearancePage ap = new AppearancePage(new AppearancePage.Constructor()
                        {
                            ViewType = AppearancePage.ViewType.IO,
                            Object = ((IOContentPage)this.CurrentPage).Ctor.Group
                        });

                        Sess.Log("ToolbarItems AA");
                        await Navigation.PushAsync(ap);
                        Sess.Log("ToolbarItems BB");
                        await ap.InitIO();
                        Sess.Log("ToolbarItems CC");
                        await ap.PageCloseTaskIO;

                        Sess.Log("IOs geadded -> PageChanging");
                        PageChanging();
                    })
                });
            }
            
            CurrentPageChanged += (s, e) =>
            {
                PageChanging();
            };

            IOContentPageSelect((IOContentPage)CurrentPage);
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            moduleThreadActive = false;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        void IOContentPageSelect(IOContentPage page)
        {
            if (moduleThreadActive)
            {
                moduleThreadActive = false;
                //try
                { 
                    moduleThread.Wait();
                }
                /*
                catch (Exception e)
                {
                    Sess.Log(e.ToString());
                }*/
                moduleThreadTimer = 0;
            }

            bool refreshAnimation;

            moduleThread = new Task<int>(() =>
            {
                try
                { 
                Sess.Log("Starte Thread!!");

                do
                {
                    //Sess.Log("Thread Runde");

                    // --------------------
                    // User Aktionen

                    foreach (var userAction in page.UserActions)
                    {
                        userAction();
                    }
                    page.UserActions.Clear();

                    // --------------------
                    // Check, ob refresht werden muss

                    if (((moduleThreadTimer % TIMER_REFRESH_MS) == 0) || (moduleThreadTimer == 0))
                    {
                        new Task<int>(() =>
                        {
                            Sess.Log("inner task start");

                            refreshAnimation = true;
                            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                            sw.Start();

                            Device.BeginInvokeOnMainThread(() => IsBusy = true);

                            while (refreshAnimation);

                            // künstlicher sleep damit die search animation durchkommt
                            while (sw.ElapsedMilliseconds < GUI.TIME_ANIMATION_MIN_MS)
                            {
                                System.Threading.Tasks.Task.Delay(TIMER_INTERVAL_MS).Wait();
                            }

                            Device.BeginInvokeOnMainThread(() => IsBusy = false);

                            Sess.Log("inner task end");

                            return 0;
                        }).Start();

                        // jedes "verwendete" Modul auf der Form hat einen eigenen RefreshTask
                        foreach (var refreshTask in page.RefreshTasks)
                        {
                            if (refreshTask.module.OpenModule() == 0)
                            {
                                Sess.Log(String.Format("Modul {0} konnte nicht geöffnet werden (handle=0)", refreshTask.module.tcp_hostname));
                                return 1;
                            }

                            foreach (var task in refreshTask.tasks)
                            {

                                uint min = task.Params.Min(p => p.ch);
                                uint max = task.Params.Max(p => p.ch);

                                Sess.Log(String.Format("IP: {3} - task: {2} - min: {0} - max: {1}", min, max, task.IOType, refreshTask.module.tcp_hostname));

                                uint longVal0 = 0;
                                uint longVal1 = 0;
                                uint longVal2 = 0;
                                uint longVal3 = 0;

                                float floatVal = 0;

                                switch (task.IOType)
                                {
                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case XML.IOTypes.DI:
                                    case XML.IOTypes.DO:

                                        if ((min <= 31) || (max <= 31))
                                        {
                                            switch (task.IOType)
                                            {
                                                case XML.IOTypes.DI: longVal0 = DT.Delib.DapiDIGet32(task.Module.handle, 0); break;
                                                case XML.IOTypes.DO: longVal0 = DT.Delib.DapiDOReadback32(task.Module.handle, 0); break;
                                            }
                                        }
                                        else if ((min <= 63) || (max <= 63))
                                        {
                                            switch (task.IOType)
                                            {
                                                case XML.IOTypes.DI: longVal1 = DT.Delib.DapiDIGet32(task.Module.handle, 32); break;
                                                case XML.IOTypes.DO: longVal1 = DT.Delib.DapiDOReadback32(task.Module.handle, 32); break;
                                            }
                                        }
                                        else if ((min <= 95) || (max <= 95))
                                        {
                                            switch (task.IOType)
                                            {
                                                case XML.IOTypes.DI: longVal2 = DT.Delib.DapiDIGet32(task.Module.handle, 64); break;
                                                case XML.IOTypes.DO: longVal2 = DT.Delib.DapiDOReadback32(task.Module.handle, 64); break;
                                            }
                                        }
                                        else
                                        {
                                            switch (task.IOType)
                                            {
                                                case XML.IOTypes.DI: longVal3 = DT.Delib.DapiDIGet32(task.Module.handle, 96); break;
                                                case XML.IOTypes.DO: longVal3 = DT.Delib.DapiDOReadback32(task.Module.handle, 96); break;
                                            }
                                        }
                                        foreach (var request in task.Params)
                                        {
                                            if (request.ioCfg == XML.GUIConfigs.SWITCH)
                                            { 
                                                Device.BeginInvokeOnMainThread(() =>
                                                { 
                                                    page.BlockUserAction = true;

                                                    if (request.ch <= 31)
                                                    {
                                                        ((Switch)page.PageControls[request.id]).IsToggled = (((((int)longVal0 >> (int)(request.ch & 0x1f)) & 1)) == 1 ? true : false);
                                                    }
                                                    else if (request.ch <= 63)
                                                    {
                                                        ((Switch)page.PageControls[request.id]).IsToggled = (((((int)longVal1 >> (int)(request.ch & 0x1f)) & 1)) == 1 ? true : false);
                                                    }
                                                    else if (request.ch <= 95)
                                                    {
                                                        ((Switch)page.PageControls[request.id]).IsToggled = (((((int)longVal2 >> (int)(request.ch & 0x1f)) & 1)) == 1 ? true : false);
                                                    }
                                                    else
                                                    {
                                                        ((Switch)page.PageControls[request.id]).IsToggled = (((((int)longVal3 >> (int)(request.ch & 0x1f)) & 1)) == 1 ? true : false);
                                                    }

                                                    page.BlockUserAction = false;
                                                });
                                            }
                                        }

                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case XML.IOTypes.AD:
                                        DT.Delib.DapiSpecialCommand(task.Module.handle, DT.Delib.DAPI_SPECIAL_CMD_AD, DT.Delib.DAPI_SPECIAL_AD_READ_MULTIPLE_AD, min, max);
                                        foreach (var request in task.Params)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                ((Label)page.PageControls[request.id]).Text = String.Format("{0} V", DT.Delib.DapiADGetVolt(task.Module.handle, request.ch | 0x8000).ToString("0.000"));
                                            });
                                        }
                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case XML.IOTypes.DA:
                                        DT.Delib.DapiSpecialCommand(task.Module.handle, DT.Delib.DAPI_SPECIAL_CMD_DA, DT.Delib.DAPI_SPECIAL_DA_READBACK_MULIPLE_DA, min, max);
                                        foreach (var request in task.Params)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                page.BlockUserAction = true;
                                                ((Label)page.PageControls[request.id]).Text = String.Format("{0} V", DT.Delib.DapiDAGetVolt(task.Module.handle, request.ch | 0x8000).ToString("0.000"));
                                                page.BlockUserAction = false;
                                            });
                                        }
                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case XML.IOTypes.PWM:
                                        DT.Delib.DapiSpecialCommand(task.Module.handle, DT.Delib.DAPI_SPECIAL_CMD_PWM, DT.Delib.DAPI_SPECIAL_PWM_READBACK_MULIPLE_PWM, min, max);
                                        foreach (var request in task.Params)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                page.BlockUserAction = true;
                                                ((Slider)page.PageControls[request.id]).Value = (int)DT.Delib.DapiPWMOutReadback(task.Module.handle, request.ch | 0x8000);
                                                page.BlockUserAction = false;
                                            });
                                        }
                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case XML.IOTypes.TEMP:
                                        DT.Delib.DapiSpecialCommand(task.Module.handle, DT.Delib.DAPI_SPECIAL_CMD_TEMP, DT.Delib.DAPI_SPECIAL_TEMP_READ_MULIPLE_TEMP, min, max);
                                        foreach (var request in task.Params)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            { 
                                                floatVal = DT.Delib.DapiTempGet(task.Module.handle, request.ch | 0x4000);
                                                ((Label)page.PageControls[request.id]).Text = ((floatVal != -9999) ? String.Format("{0} °C", floatVal.ToString("0.00")) : "disconnected");
                                            });
                                        }
                                        break;

                                }   // switch (task.ioType)
                            }   // foreach (var task in refreshTask.tasks)

                            refreshTask.module.CloseModule();

                        }   // foreach (var refreshTask in page.RefreshTasks)


                        refreshAnimation = false;

                    }   // if (((moduleThreadTimer % TIMER_REFRESH_MS) == 0) || (moduleThreadTimer == 0))


                    System.Threading.Tasks.Task.Delay(TIMER_INTERVAL_MS).Wait();
                    moduleThreadTimer += TIMER_INTERVAL_MS;

                } while (moduleThreadActive);

                Sess.Log("Thread weg..");
                }
                catch (Exception e)
                {
                    Sess.Log(e.ToString());
                }

                return 0;
            });


            if (page != null)
            { 
                if (page.RefreshTasks.Count != 0)
                { 
                    moduleThread.Start();
                    moduleThreadActive = true;
                }
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        XML.IOTypes GetIOType(String title)
        {
            switch (title)
            {
                case "DI": return XML.IOTypes.DI;
                case "DO": return XML.IOTypes.DO;
                case "DO_Timer": return XML.IOTypes.DO_TIMER;
                case "PWM": return XML.IOTypes.PWM;
                case "AD": return XML.IOTypes.AD;
                case "DA": return XML.IOTypes.DA;
                case "Temp": return XML.IOTypes.TEMP;
            }

            return XML.IOTypes.UNKNOWN;
        }
    }
}
