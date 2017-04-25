using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace IOControl
{
	public partial class IOContentPage : ContentPage
    {
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        int valID = 0;

        public enum ViewType
        {
            MODULE,
            CUSTOM
        }

        public class Constructor
        {
            // Allgemein
            public String Title { get; set; }
            public ViewType ViewType { get; set; }

            // ModulView
            public XML.IOTypes IOType { get; set; }
            public ETHModule.Module Module { get; set; }

            // CustomView
            public XML.XMLViewGroup Group { get; set; }
            
        }
        public Constructor Ctor { get; set; }

        public Dictionary<int, View> PageControls { get; set; }
        public List<Action> UserActions { get; set; }
        public bool BlockUserAction { get; set; }
        public List<ModuleTask> RefreshTasks { get; set; }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public IOContentPage(Constructor ctor)
        {
            Ctor = ctor;
            Title = Ctor.Title;

            Sess.Log(String.Format("VType={0}, XML.IOTypes={1}, Title={2}", Ctor.ViewType, Ctor.IOType, Title));

            Init();
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public void Init()
        {
            uint i;

            Sess.Buttons.Clear();

            ScrollView sv = new ScrollView();
            sv.Scrolled += (s, e) =>
            {
                foreach (var ctrl in Sess.Buttons)
                {
                    if (ctrl.IsPressed)
                    {
                        Sess.Log("Release Button");
                        ctrl.Release();
                    }
                }
            };

            PageControls = new Dictionary<int, View>();
            UserActions = new List<Action>();
            RefreshTasks = new List<ModuleTask>();

            StackLayout sl = new StackLayout();

            sl.Children.Add(DTControl.Separator());


            switch (Ctor.ViewType)
            {
                // ------------------------------------
                // ------------------------------------
                // ------------------------------------

                case ViewType.MODULE:

                    switch (Ctor.IOType)
                    {
                        // ------------------------------------
                        case XML.IOTypes.DI:
                            for (i = 0; i != Ctor.Module.IO.cnt_di; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case XML.IOTypes.DO:
                            for (i = 0; i != Ctor.Module.IO.cnt_do; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case XML.IOTypes.AD:
                            for (i = 0; i != Ctor.Module.IO.cnt_ai; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case XML.IOTypes.DA:
                            for (i = 0; i != Ctor.Module.IO.cnt_ao; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case XML.IOTypes.PWM:
                            for (i = 0; i != Ctor.Module.IO.cnt_do_pwm; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case XML.IOTypes.DO_TIMER:
                            for (i = 0; i != Ctor.Module.IO.cnt_do_timer; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case XML.IOTypes.TEMP:
                            for (i = 0; i != Ctor.Module.IO.cnt_temp; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                    }
                    break;

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------

                case ViewType.CUSTOM:
                    
                    if (Ctor.Group.IOs.Count != 0)
                    { 
                        foreach (var io in Ctor.Group.IOs)
                        {
                            var module = Sess.Xml.Modules.Find(x => x.mac == io.MAC);
                            AddIOView(sl, PageControls, io.IOType, module, io.Channel, io.GUICfg);
                        }
                    }
                    else
                    {
                        sl.Children.Add(new Label()
                        {
                            Text = Resx.AppResources.IO_InvalidGroup,
                            TextColor = Color.White,
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center
                        });
                    }

                    break;

                // ------------------------------------
                // ------------------------------------
                // ------------------------------------
            }
            
            sv.Content = sl;
            Content = sv;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public void AddIOView(StackLayout sl, Dictionary<int, View> dict, XML.IOTypes ioType, ETHModule.Module module, uint ch)
        {
            AddIOView(sl, dict, ioType, module, ch, XML.GUIConfigs.NONE);
        }

        public void AddIOView(StackLayout sl, Dictionary<int, View> dict, XML.IOTypes ioType, ETHModule.Module module, uint ch, XML.GUIConfigs ioCfg)
        {
            Action userAction;

            Sess.Log(string.Format("XML.IOTypes={0}, ch={1} ioCfg={2}", ioType, ch, ioCfg));

            if (module.GetIOName(ioType, ch) != null)
            {
                switch (ioType)
                {
                    // ------------------------------------
                    case XML.IOTypes.DI:
                        module.GetIOName(ioType, ch);
                        sl.Children.Add(DTIOControl.DISwitch(dict, module.IOName.di[(int)ch], valID));
                        break;
                    // ------------------------------------
                    case XML.IOTypes.DO:

                        if (ioCfg == XML.GUIConfigs.BUTTON)
                        {
                            sl.Children.Add(DTIOControl.DOButton(dict, module.IOName.dout[(int)ch], valID));
                            ((DTControl.DOButton)dict[valID]).Pressed += (s, e) =>
                            {
                                module.OpenModule();
                                DT.Delib.DapiDOSet1(module.handle, ch, 1);
                                module.CloseModule();
                            };

                            ((DTControl.DOButton)dict[valID]).Released += (s, e) =>
                            {
                                module.OpenModule();
                                DT.Delib.DapiDOSet1(module.handle, ch, 0);
                                module.CloseModule();
                            };
                        }
                        else
                        {
                            sl.Children.Add(DTIOControl.DOSwitch(dict, module.IOName.dout[(int)ch], valID));
                            userAction = CreateTodoEvent(ioType, module, dict, ch, valID);
                            ((Switch)dict[valID]).Toggled += (s, e) =>
                            {
                                if (!BlockUserAction)
                                {
                                    UserActions.Add(userAction);
                                }
                            };
                        }
                        break;

                    // ------------------------------------
                    case XML.IOTypes.AD:
                        sl.Children.Add(DTIOControl.ADView(dict, module.IOName.ai[(int)ch], valID));
                        break;
                    // ------------------------------------
                    case XML.IOTypes.DA:
                        sl.Children.Add(DTIOControl.DAView(dict, module.IOName.ao[(int)ch], valID));
                        userAction = CreateTodoEvent(ioType, module, dict, ch, valID);

                        DTControl.SpecialLabel label = dict[valID] as DTControl.SpecialLabel;
                        TapGestureRecognizer tgr = new TapGestureRecognizer();
                        tgr.Tapped += async (s, e) =>
                        {
                            PopupSetDA da = new PopupSetDA(new PopupSetDA.Constructor()
                            {
                                Name = module.IOName.ao[(int)ch],
                                Value = label.Text
                            });
                            await Navigation.PushPopupAsync(da);
                            float? value = await da.PageCloseTask;
                            if (value != null)
                            {
                                label.Value = value;
                                if (!BlockUserAction)
                                {
                                    UserActions.Add(userAction);
                                }
                            }
                        };
                        label.GestureRecognizers.Add(tgr);
                        break;
                    // ------------------------------------
                    /*
                case "DO Timer":
                    DT.Mobile.GUI.AddDigitalOutputTimer(view.Context, layout, "Digital Output Timer", textID++, "0h 0m 0s", valID++, "Timer", helpID++);
                    DT.Mobile.GUI.AddSpacer(view.Context, layout);
                    break;
                    */
                    // ------------------------------------
                    case XML.IOTypes.PWM:
                        sl.Children.Add(DTIOControl.PWMView(dict, module.IOName.pwm[(int)ch], valID));
                        userAction = CreateTodoEvent(ioType, module, dict, ch, valID);
                        ((Slider)dict[valID]).ValueChanged += (s, e) =>
                        {
                            if (!BlockUserAction)
                            {
                                UserActions.Add(userAction);
                            }
                        };
                        break;

                    // ------------------------------------
                    case XML.IOTypes.TEMP:
                        sl.Children.Add(DTIOControl.ADView(dict, module.IOName.temp[(int)ch], valID));
                        break;
                        // ------------------------------------
                }   // switch (ioType)

                AddModuleTask(module, ioType, new ETHModule.ModuleTaskParam(ch, valID, ioCfg), dict);
                valID++;
            }   // if (channelValid)
            else
            {
                
                sl.Children.Add(DTIOControl.Placeholder(string.Format(Resx.AppResources.IO_InvalidIOText,
                    module.tcp_hostname, ioType, ch
                )));
            }

            sl.Children.Add(DTControl.Separator());
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public Action CreateTodoEvent(XML.IOTypes ioType, ETHModule.Module module, Dictionary<int, View> dict, uint ch, int id)
        {
            return CreateTodoEvent(ioType, module, dict, ch, id, DT.NULL);
        }

        public Action CreateTodoEvent(XML.IOTypes ioType, ETHModule.Module module, Dictionary<int, View> dict, uint ch, int id, int id2)
        {
            return new Action(() => {

                if (module.OpenModule() == 0)
                {
                    Sess.Log(String.Format("CreateTodoEvent: IP {0} : handle = 0", module.tcp_hostname));
                    return;
                }

                switch (ioType)
                {
                    // ------------------------------------
                    case XML.IOTypes.DO:
                        DT.Delib.DapiDOSet1(module.handle, ch,  (((Switch)PageControls[id]).IsToggled ? (uint)1 : (uint)0));
                        break;
                    // ------------------------------------
                    case XML.IOTypes.DA:
                        DTControl.SpecialLabel label = PageControls[id] as DTControl.SpecialLabel;
                        Device.BeginInvokeOnMainThread(() => label.Text = String.Format("{0} V", ((float)label.Value).ToString("0.000")));
                        DT.Delib.DapiDASetVolt(module.handle, ch, Convert.ToSingle(label.Value));
                        break;
                    // ------------------------------------
                    case XML.IOTypes.PWM:
                        DT.Delib.DapiPWMOutSet(module.handle, ch, (float)((Slider)PageControls[id]).Value);
                        break;
                }

                module.CloseModule();
            });
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class ModuleTask
        {
            public ETHModule.Module module;
            public List<ETHModule.ModuleTask> tasks = new List<ETHModule.ModuleTask>();

            public ModuleTask(ETHModule.Module module)
            {
                this.module = module;
            }
        }

        public void AddModuleTask(ETHModule.Module module, XML.IOTypes ioType, ETHModule.ModuleTaskParam param, Dictionary<int, View> dict)
        {
            ModuleTask mTask;
            ETHModule.ModuleTask task;

            // Modul in die TaskListe aufnehmen
            if ((mTask = RefreshTasks.Find(x => x.module == module)) == null)
            {
                mTask = new ModuleTask(module);
                RefreshTasks.Add(mTask);
            }

            // ModulTask 
            if ((task = mTask.tasks.Find(x => x.IOType == ioType)) == null)
            {
                task = new ETHModule.ModuleTask() { IOType = ioType, Module = module, dict = dict };
                mTask.tasks.Add(task);
            }

            // IOTask
            task.Params.Add(param);
        }
    }
}
