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
            public IOType IOType { get; set; }
            public Module Module { get; set; }

            // CustomView
            public ContentGroup Group { get; set; }
            
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

            DT.Log(String.Format("VType={0}, IOType={1}, Title={2}", Ctor.ViewType, Ctor.IOType, Title));

            PageControls = new Dictionary<int, View>();
            UserActions = new List<Action>();
            RefreshTasks = new List<ModuleTask>();

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

            ScrollView sv = new ScrollView();
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
                        case IOType.DI:
                            for (i = 0; i != Ctor.Module.IO.cnt_di; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case IOType.DO:
                            for (i = 0; i != Ctor.Module.IO.cnt_do; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case IOType.AD:
                            for (i = 0; i != Ctor.Module.IO.cnt_ai; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case IOType.DA:
                            for (i = 0; i != Ctor.Module.IO.cnt_ao; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case IOType.PWM:
                            for (i = 0; i != Ctor.Module.IO.cnt_do_pwm; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case IOType.DO_TIMER:
                            for (i = 0; i != Ctor.Module.IO.cnt_do_timer; i++)
                                AddIOView(sl, PageControls, Ctor.IOType, Ctor.Module, i);
                            break;
                        // ------------------------------------
                        case IOType.TEMP:
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
                    
                    if (Ctor.Group.io.Count != 0)
                    { 
                        foreach (var io in Ctor.Group.io)
                        {
                            var module = DT.Session.xmlContent.modules.Find(x => x.mac == io.moduleMAC);
                            AddIOView(sl, PageControls, io.ioType, module, io.channel);
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

        public void AddIOView(StackLayout sl, Dictionary<int, View> dict, IOType ioType, Module module, uint ch)
        {
            Action userAction;

            DT.Log(string.Format("IOType={0}, ch={1}", ioType, ch));;

            if (module.GetIOName(ioType, ch) != null)
            {
                switch (ioType)
                {
                    // ------------------------------------
                    case IOType.DI:
                        module.GetIOName(ioType, ch);
                        sl.Children.Add(DTIOControl.DISwitch(dict, module.IOName.di[(int)ch], valID));
                        break;
                    // ------------------------------------
                    case IOType.DO:
                        sl.Children.Add(DTIOControl.DOSwitch(dict, module.IOName.dout[(int)ch], valID));
                        userAction = CreateTodoEvent(ioType, module, dict, ch, valID);
                        ((Switch)dict[valID]).Toggled += (s, e) =>
                        {
                            if (!BlockUserAction)
                            {
                                UserActions.Add(userAction);
                            }
                        };
                        break;

                    // ------------------------------------
                    case IOType.AD:
                        sl.Children.Add(DTIOControl.ADView(dict, module.IOName.ai[(int)ch], valID));
                        break;
                    // ------------------------------------
                    case IOType.DA:
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
                    case IOType.PWM:
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
                    case IOType.TEMP:
                        sl.Children.Add(DTIOControl.ADView(dict, module.IOName.temp[(int)ch], valID));
                        break;
                        // ------------------------------------
                }   // switch (ioType)

                AddModuleTask(module, ioType, new ModuleTaskParam(ch, valID), dict);
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

        public Action CreateTodoEvent(IOType ioType, Module module, Dictionary<int, View> dict, uint ch, int id)
        {
            return CreateTodoEvent(ioType, module, dict, ch, id, DT.NULL);
        }

        public Action CreateTodoEvent(IOType ioType, Module module, Dictionary<int, View> dict, uint ch, int id, int id2)
        {
            return new Action(() => {

                if (module.OpenModule() == 0)
                {
                    DT.Log(String.Format("CreateTodoEvent: IP {0} : handle = 0", module.tcp_hostname));
                    return;
                }

                switch (ioType)
                {
                    // ------------------------------------
                    case IOType.DO:
                        DT.Delib.DapiDOSet1(module.handle, ch,  (((Switch)PageControls[id]).IsToggled ? (uint)1 : (uint)0));
                        break;
                    // ------------------------------------
                    case IOType.DA:
                        DTControl.SpecialLabel label = PageControls[id] as DTControl.SpecialLabel;
                        Device.BeginInvokeOnMainThread(() => label.Text = String.Format("{0} V", ((float)label.Value).ToString("0.000")));
                        DT.Delib.DapiDASetVolt(module.handle, ch, Convert.ToSingle(label.Value));
                        break;
                    // ------------------------------------
                    case IOType.PWM:
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
            public Module module;
            public List<IOControl.ModuleTask> tasks = new List<IOControl.ModuleTask>();

            public ModuleTask(Module module)
            {
                this.module = module;
            }
        }

        public void AddModuleTask(Module module, IOType ioType, ModuleTaskParam param, Dictionary<int, View> dict)
        {
            ModuleTask mTask;
            IOControl.ModuleTask task;

            // Modul in die TaskListe aufnehmen
            if ((mTask = RefreshTasks.Find(x => x.module == module)) == null)
            {
                mTask = new ModuleTask(module);
                RefreshTasks.Add(mTask);
            }

            // ModulTask 
            if ((task = mTask.tasks.Find(x => x.ioType == ioType)) == null)
            {
                task = new IOControl.ModuleTask(ioType, module, dict);
                mTask.tasks.Add(task);
            }

            // IOTask
            task.param.Add(param);
        }
    }
}
