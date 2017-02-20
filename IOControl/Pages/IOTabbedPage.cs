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
            //TintColor = Color.Black;
            //BarTintColor = Color.Red;
            //BackgroundColor = Color.Green;
            //SwipeEnabled = true;

            Ctor = ctor;
            Title = Ctor.Title;

            if (Ctor.ObType == typeof(Module))
            {
                foreach (var tab in Ctor.Tabs)
                {
                    Children.Add(new IOContentPage(new IOContentPage.Constructor()
                    {
                        ViewType = IOContentPage.ViewType.MODULE,
                        Module = (Module)Ctor.Object,
                        Title = tab,
                        IOType = GetIOType(tab)
                    }));
                }
            }
            else
            {
                foreach(var group in ((ContentLocation)Ctor.Object).groups)
                { 
                    Children.Add(new IOContentPage(new IOContentPage.Constructor()
                    {
                        ViewType = IOContentPage.ViewType.CUSTOM,
                        Group = group,
                        Title = group.name
                    }));
                }
            }
            

            



            /*
            OnSwipeLeft += (s, e) =>
            {
                CurrentPage = Children[1];
            };

            OnSwipeRight += (s, e) =>
            {
                CurrentPage = Children[0];
            };

            /*
            OnCurrentPageChanged += (s, e) =>
            {
                IOContentPage page = (IOContentPage)((IOTabbedPage)s).CurrentPage;
                //IOContentPage page = (IOContentPage)CurrentPage;

                DT.Log(page.Title);
                page.Init();
                IOContentPageSelect(page);
            };
            */

            
            CurrentPageChanged += (s, e) =>
            //CurrentPageChanged += () =>
            {
                IOContentPage page = (IOContentPage)((IOTabbedPage)s).CurrentPage;
                //IOContentPage page = (IOContentPage)CurrentPage;

                DT.Log(page.Title);
                page.Init();
                IOContentPageSelect(page);
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
                    DT.Log(e.ToString());
                }*/
                moduleThreadTimer = 0;
            }

            bool refreshAnimation;

            moduleThread = new Task<int>(() =>
            {
                try
                { 
                DT.Log("Starte Thread!!");

                do
                {
                    //DT.Log("Thread Runde");

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
                            DT.Log("inner task start");

                            refreshAnimation = true;
                            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                            sw.Start();

                            Device.BeginInvokeOnMainThread(() => IsBusy = true);

                            while (refreshAnimation);

                            // künstlicher sleep damit die search animation durchkommt
                            while (sw.ElapsedMilliseconds < DT.Const.TIME_ANIMATION_MIN_MS)
                            {
                                System.Threading.Tasks.Task.Delay(TIMER_INTERVAL_MS).Wait();
                            }

                            Device.BeginInvokeOnMainThread(() => IsBusy = false);

                            DT.Log("inner task end");

                            return 0;
                        }).Start();

                        // jedes "verwendete" Modul auf der Form hat einen eigenen RefreshTask
                        foreach (var refreshTask in page.RefreshTasks)
                        {
                            if (refreshTask.module.OpenModule() == 0)
                            {
                                DT.Log(String.Format("Modul {0} konnte nicht geöffnet werden (handle=0)", refreshTask.module.tcp_hostname));
                                return 1;
                            }

                            foreach (var task in refreshTask.tasks)
                            {

                                uint min = task.param.Min(p => p.ch);
                                uint max = task.param.Max(p => p.ch);

                                DT.Log(String.Format("IP: {3} - task: {2} - min: {0} - max: {1}", min, max, task.ioType, refreshTask.module.tcp_hostname));

                                uint longVal0 = 0;
                                uint longVal1 = 0;
                                uint longVal2 = 0;
                                uint longVal3 = 0;

                                float floatVal = 0;

                                switch (task.ioType)
                                {
                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case IOType.DI:
                                    case IOType.DO:

                                        if ((min <= 31) || (max <= 31))
                                        {
                                            switch (task.ioType)
                                            {
                                                case IOType.DI: longVal0 = DT.Delib.DapiDIGet32(task.module.handle, 0); break;
                                                case IOType.DO: longVal0 = DT.Delib.DapiDOReadback32(task.module.handle, 0); break;
                                            }
                                        }
                                        else if ((min <= 63) || (max <= 63))
                                        {
                                            switch (task.ioType)
                                            {
                                                case IOType.DI: longVal1 = DT.Delib.DapiDIGet32(task.module.handle, 32); break;
                                                case IOType.DO: longVal1 = DT.Delib.DapiDOReadback32(task.module.handle, 32); break;
                                            }
                                        }
                                        else if ((min <= 95) || (max <= 95))
                                        {
                                            switch (task.ioType)
                                            {
                                                case IOType.DI: longVal2 = DT.Delib.DapiDIGet32(task.module.handle, 64); break;
                                                case IOType.DO: longVal2 = DT.Delib.DapiDOReadback32(task.module.handle, 64); break;
                                            }
                                        }
                                        else
                                        {
                                            switch (task.ioType)
                                            {
                                                case IOType.DI: longVal3 = DT.Delib.DapiDIGet32(task.module.handle, 96); break;
                                                case IOType.DO: longVal3 = DT.Delib.DapiDOReadback32(task.module.handle, 96); break;
                                            }
                                        }

                                        foreach (var request in task.param)
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

                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case IOType.AD:
                                        DT.Delib.DapiSpecialCommand(task.module.handle, DT.Delib.DAPI_SPECIAL_CMD_AD, DT.Delib.DAPI_SPECIAL_AD_READ_MULTIPLE_AD, min, max);
                                        foreach (var request in task.param)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                ((Label)page.PageControls[request.id]).Text = String.Format("{0} V", DT.Delib.DapiADGetVolt(task.module.handle, request.ch | 0x8000).ToString("0.000"));
                                            });
                                        }
                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case IOType.DA:
                                        DT.Delib.DapiSpecialCommand(task.module.handle, DT.Delib.DAPI_SPECIAL_CMD_DA, DT.Delib.DAPI_SPECIAL_DA_READBACK_MULIPLE_DA, min, max);
                                        foreach (var request in task.param)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                page.BlockUserAction = true;
                                                ((Label)page.PageControls[request.id]).Text = String.Format("{0} V", DT.Delib.DapiDAGetVolt(task.module.handle, request.ch | 0x8000).ToString("0.000"));
                                                page.BlockUserAction = false;
                                            });
                                        }
                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case IOType.PWM:
                                        DT.Delib.DapiSpecialCommand(task.module.handle, DT.Delib.DAPI_SPECIAL_CMD_PWM, DT.Delib.DAPI_SPECIAL_PWM_READBACK_MULIPLE_PWM, min, max);
                                        foreach (var request in task.param)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                page.BlockUserAction = true;
                                                ((Slider)page.PageControls[request.id]).Value = (int)DT.Delib.DapiPWMOutReadback(task.module.handle, request.ch | 0x8000);
                                                page.BlockUserAction = false;
                                            });
                                        }
                                        break;

                                    // ------------------------------------
                                    // ------------------------------------
                                    // ------------------------------------

                                    case IOType.TEMP:
                                        DT.Delib.DapiSpecialCommand(task.module.handle, DT.Delib.DAPI_SPECIAL_CMD_TEMP, DT.Delib.DAPI_SPECIAL_TEMP_READ_MULIPLE_TEMP, min, max);
                                        foreach (var request in task.param)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            { 
                                                floatVal = DT.Delib.DapiTempGet(task.module.handle, request.ch | 0x4000);
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

                DT.Log("Thread weg..");
                }
                catch (Exception e)
                {
                    DT.Log(e.ToString());
                }

                return 0;
            });


            if (page.RefreshTasks.Count != 0)
            { 
                moduleThread.Start();
                moduleThreadActive = true;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        IOType GetIOType(String title)
        {
            switch (title)
            {
                case "DI": return IOType.DI;
                case "DO": return IOType.DO;
                case "DO_Timer": return IOType.DO_TIMER;
                case "PWM": return IOType.PWM;
                case "AD": return IOType.AD;
                case "DA": return IOType.DA;
                case "Temp": return IOType.TEMP;
            }

            return IOType.UNKNOWN;
        }
    }
}
