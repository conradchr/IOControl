using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System.ComponentModel;    // INotifyPropertyChanged
using Xamarin.Forms;
using System.Diagnostics;
using System.Linq;
using Rg.Plugins.Popup.Extensions;

namespace IOControl
{
    public partial class AppearancePage : ContentPage
    {
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public enum ViewType
        {
            MAIN,
            GROUP,
            IO
        }

        public class Constructor
        {
            public ViewType ViewType { get; set; }
            public Object Object { get; set; }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public Constructor Ctor { get; set; }

        
        public Task<bool> PageCloseTaskIO { get { return tcs.Task; } }
        public bool taskComplete = true;
        TaskCompletionSource<bool> tcs;
        
        

        ObservableCollection<HeaderModel> items;
        ListView listView;

        ItemModel selectedItem = null;
        Grid gridFooter;
        Label labelFooter;

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class ItemModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Name { get; set; }
            public Type Type { get; set; }
            public Object Object { get; set; }

            // für Listview, damit dieser Wert sich in der Anzeige aktualisiert
            bool isSelected = false;
            public bool IsSelected
            {
                get { return isSelected; }
                set
                {
                    if (isSelected != value)
                    {
                        isSelected = value;
                        PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                    }
                }
            }


            // Spezial für IOs
            public event PropertyChangedEventHandler Changing;
            public XML.IOTypes IOType { get; set; }
            public String ModuleInfo { get; set; }

            XML.GUIConfigs iocfg = XML.GUIConfigs.NONE;
            public XML.GUIConfigs IOCfg
            {
                get { return iocfg; }
                set
                {
                    if (iocfg != value)
                    {
                        iocfg = value;
                        Changing(this, new PropertyChangedEventArgs("IOCfg"));
                    }
                }
            }
        }

        public class HeaderModel : ObservableCollection<ItemModel>
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public Type Type { get; set; }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class ItemViewCell : ViewCell
        {
            StackLayout slPicker;
            Picker picker;
            Label labelModule;

            public ItemViewCell()
            {
                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(10, 10, 10, 10)
                };

                Color temp = layout.BackgroundColor;
                Switch sw = new Switch() { IsVisible = false };
                sw.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));
                sw.Toggled += new EventHandler<ToggledEventArgs>((s, e) =>
                {
                    layout.BackgroundColor = sw.IsToggled ? GUI.COLOR_SELECTED : temp;
                });
                layout.Children.Add(sw);

                // Label Name
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));
                layout.Children.Add(labelName);

                // Label Name
                labelModule = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelModule.SetBinding(Label.TextProperty, new Binding("ModuleInfo"));
                layout.Children.Add(labelModule);

                // Picker
                slPicker = new StackLayout() { Orientation = StackOrientation.Horizontal };
                slPicker.Children.Add(new Label()
                {
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Text = Resx.AppResources.IO_CFG_Configuration + ":"
                });

                picker = new Picker() { Title = Resx.AppResources.IO_CFG_Configuration };
                slPicker.Children.Add(picker);

                layout.Children.Add(slPicker);

                View = layout;
            }

            protected override void OnBindingContextChanged()
            {
                ItemModel itemModel;

                base.OnBindingContextChanged();

                if ((itemModel = ((ItemModel)BindingContext)) != null)
                {
                    if (itemModel.Type == typeof(XML.XMLViewGroupIO))
                    {
                        var io = (XML.XMLViewGroupIO)itemModel.Object;
                        switch (io.IOType)
                        {
                            case XML.IOTypes.DO:
                                picker.Items.Add(Resx.AppResources.IO_CFG_DO_OnOffSwitch);
                                picker.Items.Add(Resx.AppResources.IO_CFG_DO_PushButton);
                                switch (io.GUICfg)
                                {
                                    case XML.GUIConfigs.SWITCH: picker.SelectedIndex = 0; break;
                                    case XML.GUIConfigs.BUTTON: picker.SelectedIndex = 1; break;
                                }

                                picker.SelectedIndexChanged += (s, e) =>
                                {
                                    switch (picker.SelectedIndex)
                                    {
                                        case 0: io.GUICfg = XML.GUIConfigs.SWITCH; break;
                                        case 1: io.GUICfg = XML.GUIConfigs.BUTTON; break;
                                    }
                                    Sess.Xml.Save();
                                };
                                break;

                            case XML.IOTypes.AD:
                            case XML.IOTypes.DA:
                                picker.Items.Add(Resx.AppResources.IO_CFG_ADDA_Voltage);
                                picker.Items.Add(Resx.AppResources.IO_CFG_ADDA_Current);
                                switch (io.GUICfg)
                                {
                                    case XML.GUIConfigs.VOLTAGE: picker.SelectedIndex = 0; break;
                                    case XML.GUIConfigs.CURRENT: picker.SelectedIndex = 1; break;
                                }

                                picker.SelectedIndexChanged += (s, e) =>
                                {
                                    switch (picker.SelectedIndex)
                                    {
                                        case 0: io.GUICfg = XML.GUIConfigs.VOLTAGE; break;
                                        case 1: io.GUICfg = XML.GUIConfigs.CURRENT; break;
                                    }
                                    Sess.Xml.Save();
                                };
                                break;

                            default:
                                slPicker.IsVisible = false;
                                break;
                        }
                    }
                    else
                    {
                        labelModule.IsVisible = false;
                        slPicker.IsVisible = false;
                    }
                }
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class HeaderViewCell : ViewCell
        {
            public HeaderViewCell(ContentPage context)
            {
                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 5, 10, 5),
                    BackgroundColor = DT.COLOR
                };

                // Label Name
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("LongName"));
                layout.Children.Add(labelName);

                // Add Button
                Image imgAdd = new Image() { Source = ImageSource.FromFile("btn_add.png") };
                var imgAddTapped = new TapGestureRecognizer();
                imgAddTapped.Tapped += async (s, e) =>
                {
                    if (((HeaderModel)BindingContext).Type == typeof(ETHModule.Module))
                    {
                        await ((AppearancePage)context).AddModule();
                    }
                    else if (((HeaderModel)BindingContext).Type == typeof(XML.XMLView))
                    {
                        await ((AppearancePage)context).AddLocation();
                    }
                    else if (((HeaderModel)BindingContext).Type == typeof(XML.XMLViewGroup))
                    {
                        await ((AppearancePage)context).AddLocation();
                    }
                    else if (((HeaderModel)BindingContext).Type == typeof(XML.XMLViewGroupIO))
                    {
                        ((AppearancePage)context).taskComplete = false;
                        Sess.Log("await AddIO");
                        await ((AppearancePage)context).AddIO();
                        Sess.Log("AddIO fertig");
                        ((AppearancePage)context).taskComplete = true;
                    }
                };
                imgAdd.GestureRecognizers.Add(imgAddTapped);
                layout.Children.Add(imgAdd);


#if (LV_SEP)
                StackLayout outerLayout = new StackLayout() { Orientation = StackOrientation.Vertical };
                outerLayout.Children.Add(layout);
                outerLayout.Children.Add(DTControl.ListViewSeparator());
                View = outerLayout;
#else
                View = layout;
#endif
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        void InitMain()
        {
            Title = Resx.AppResources.NAV_Configuration;

            // --------------------
            // Locations

            var groupLocation = new HeaderModel()
            {
                LongName = Resx.AppResources.Location.ToUpper(),
                ShortName = Resx.AppResources.Location.ToUpper().Substring(0, 1),
                Type = typeof(XML.XMLView)
            };

            foreach (XML.XMLView loc in Sess.Xml.Views)
            {
                groupLocation.Add(new ItemModel()
                {
                    Name = loc.Name,
                    Type = loc.GetType(),
                    Object = loc,
                });
            }

            items.Add(groupLocation);

            // --------------------
            // Module

            var groupModule = new HeaderModel()
            {
                LongName = Resx.AppResources.Modules.ToUpper(),
                ShortName = Resx.AppResources.Modules.ToUpper().Substring(0, 1),
                Type = typeof(ETHModule.Module)
            };

            foreach (ETHModule.Module module in Sess.Xml.Modules)
            {
                groupModule.Add(new ItemModel()
                {
                    Name = module.boardname,
                    Type = module.GetType(),
                    Object = module
                });
            }

            items.Add(groupModule);
        }

        void InitGroup()
        {
            Title = ((XML.XMLView)Ctor.Object).Name;

            var item = new HeaderModel()
            {
                LongName = Resx.AppResources.Groups.ToUpper(),
                ShortName = Resx.AppResources.Groups.ToUpper().Substring(0, 1),
                Type = typeof(XML.XMLViewGroup)
            };

            foreach (XML.XMLViewGroup group in ((XML.XMLView)Ctor.Object).Groups)
            {
                item.Add(new ItemModel()
                {
                    Name = group.Name,
                    Type = group.GetType(),
                    Object = group,
                });
            }

            items.Add(item);
        }


        public async Task<bool> InitIO()
        {
            Title = ((XML.XMLViewGroup)Ctor.Object).Name;

            // --------------------
            // Locations


            Task<bool> t = new Task<bool>(() =>
            {
                var item = new HeaderModel()
                {
                    LongName = Resx.AppResources.IOs.ToUpper(),
                    ShortName = Resx.AppResources.IOs.ToUpper().Substring(0, 1),
                    Type = typeof(XML.XMLViewGroupIO)
                };

                // führt bei allen verwendeten modulen das IOinit aus
                List<string> macs = ((XML.XMLViewGroup)Ctor.Object).IOs.Select(x => x.MAC).Distinct().ToList();
                foreach (var mac in macs)
                {
                    var module = Sess.Xml.Modules.Find(x => x.mac == mac);
                    if (module.OpenModule() != 0)
                    {
                        module.IOInit();
                        module.CloseModule();
                    }
                    else
                    {
                        DTControl.ShowToast(string.Format(Resx.AppResources.MSG_OpenError, module.boardname, module.tcp_hostname));
                        Task.Delay(500).Wait();
                    }
                }
                
                // added die ios mit richtigen namen
                foreach (XML.XMLViewGroupIO io in ((XML.XMLViewGroup)Ctor.Object).IOs)
                {
                    var module = Sess.Xml.Modules.Find(x => x.mac == io.MAC);
                    var ioname = module.GetIOName(io.IOType, io.Channel);

                    item.Add(new ItemModel()
                    {
                        Name = ((ioname != null) ? ioname : String.Format(Resx.AppResources.CFG_ChNotAvailable, io.Channel)),
                        Type = io.GetType(),
                        Object = io,
                        IOType = io.IOType,
                        IOCfg = io.GUICfg,
                        ModuleInfo = String.Format("{0} ({1})", module.boardname, module.tcp_hostname)
                    });
                }

                items.Add(item);

                return true;
            });

            await GUIAnimation.ShowLoading(t);

            listView.ItemsSource = items;
            listView.ItemTemplate = new DataTemplate(typeof(ItemViewCell));

            return true;
        }

        //protected override 

        public AppearancePage(Constructor ctor)
        {
            Ctor = ctor;

            items = new ObservableCollection<HeaderModel>();
            tcs = new TaskCompletionSource<bool>();

            if (Ctor.ViewType == ViewType.IO)
            { 
                this.Disappearing += (s, e) =>
                {
                    if (taskComplete)
                    { 
                        tcs.SetResult(true);
                        Sess.Log(string.Format("AppearancePage {0} weg", Ctor.ViewType));
                    }
                };
            }

            Icon = "hamburger.png";
            
            ToolbarItems.Add(new ToolbarItem() { Text = "Help", Icon = "btn_help.png", Command = new Command(ShowHelp) });


            switch (Ctor.ViewType)
            {
                case ViewType.MAIN:
                    InitMain();
                    break;

                case ViewType.GROUP:
                    InitGroup();
                    break;
            }

            // --------------------
            // ListView


            listView = VCModels.ListViewInit();
            listView.GroupHeaderTemplate = new DataTemplate(() => { return new VCModels.CfgHeader(this); });
            if (Ctor.ViewType != ViewType.IO)
            {
                listView.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
                listView.ItemsSource = items;
            }

            listView.ItemTapped += (s, e) =>
            {
                selectedItem = e.Item as ItemModel;
                if (selectedItem != null)
                {
                    foreach (var header in items)
                    {
                        foreach (var item in header)
                        {
                            item.IsSelected = false;
                        }
                    }

                    selectedItem.IsSelected = true;
                    ((ListView)s).SelectedItem = null;

                    SetFooterIcons();
                    labelFooter.IsVisible = false;
                    gridFooter.IsVisible = true;
                }
            };

            StackLayout slMain = new StackLayout() { Padding = VCModels.PAD_MAIN };
            StackLayout slListView = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand };
            slListView.Children.Add(listView);
            slMain.Children.Add(slListView);

            // ------------------------------------
            // footer
            StackLayout slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End, Padding = VCModels.PAD_FOOTER };

            labelFooter = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Text = Resx.AppResources.CFG_SelectEntry,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            slFooter.Children.Add(labelFooter);

            gridFooter = new Grid() { IsVisible = false };

            // Delete Button
            DTControl.ImageButton imgDelete = new DTControl.ImageButton("btn_delete");
            imgDelete.TGR.Tapped += async (s, e) => await DeleteItem();
            gridFooter.Children.Add(imgDelete, (int)Buttons.DELETE, 0);

            // Edit Button
            DTControl.ImageButton imgEdit = new DTControl.ImageButton("btn_edit");
            imgEdit.TGR.Tapped += async (s, e) => await EditItem();
            gridFooter.Children.Add(imgEdit, (int)Buttons.EDIT, 0);

            // swap up
            DTControl.ImageButton imgUp = new DTControl.ImageButton("btn_up");
            imgUp.TGR.Tapped += (s, e) => SwapItems(Swap.UP);
            gridFooter.Children.Add(imgUp, (int)Buttons.UP, 0);

            // swap down
            DTControl.ImageButton imgDown = new DTControl.ImageButton("btn_down");
            imgDown.TGR.Tapped += (s, e) => SwapItems(Swap.DOWN);
            gridFooter.Children.Add(imgDown, (int)Buttons.DOWN, 0);

            DTControl.ImageButton imgContinue = new DTControl.ImageButton("btn_continue");
            imgContinue.TGR.Tapped += async (s, e) =>
            {
                switch (Ctor.ViewType)
                {
                    case ViewType.MAIN:

                        if (selectedItem.Type == typeof(ETHModule.Module))
                        {
                            DialogNetworkConfig dnc = new DialogNetworkConfig(new DialogNetworkConfig.Constructor()
                            {
                                ViewType = DialogNetworkConfig.ViewType.EDIT,
                                Module = selectedItem.Object as ETHModule.Module
                            });
                            await Navigation.PushAsync(dnc);
                            ETHModule.Module ret = await dnc.PageCloseTask;
                            if (ret != null)
                            {
                                // alles ok
                                selectedItem.Name = ret.boardname;
                                Sess.Xml.Save();
                                MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);
                            }
                        }
                        else
                        {
                            await Navigation.PushAsync(new AppearancePage(new Constructor()
                            {
                                ViewType = ViewType.GROUP,
                                Object = selectedItem.Object
                            }));
                        }
                        break;

                    case ViewType.GROUP:
                        AppearancePage ap = new AppearancePage(new Constructor()
                        {
                            ViewType = ViewType.IO,
                            Object = selectedItem.Object
                        });
                        
                        await Navigation.PushAsync(ap);
                        await ap.InitIO();
                        break;

                    case ViewType.IO:
                        break;
                }
            };
            gridFooter.Children.Add(imgContinue, (int)Buttons.CONTINUE, 0);

            slFooter.Children.Add(gridFooter);
            slMain.Children.Add(slFooter);

            Content = slMain;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> AddIO()
        {
            Dictionary<string, XML.IOTypes> ioTypes = new Dictionary<string, XML.IOTypes>();
            ioTypes.Add(Resx.AppResources.Module_DI, XML.IOTypes.DI);
            ioTypes.Add(Resx.AppResources.Module_DO, XML.IOTypes.DO);
            ioTypes.Add(Resx.AppResources.Module_PWM, XML.IOTypes.PWM);
            ioTypes.Add(Resx.AppResources.Module_AI, XML.IOTypes.AD);
            ioTypes.Add(Resx.AppResources.Module_AO, XML.IOTypes.DA);
            ioTypes.Add(Resx.AppResources.Module_TEMP, XML.IOTypes.TEMP);

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
                var ioType = ioTypes[io];
                DialogAddIO da = new DialogAddIO(new DialogAddIO.Constructor()
                {
                    IOType = ioType,
                    ViewGroup = Ctor.Object as XML.XMLViewGroup
                });
                Sess.Log("AP: AA");
                await Navigation.PushAsync(da);
                await da.Scan();

                // warte auf antwort
                List<XML.XMLViewGroupIO> ret = await da.PageCloseTask;
                if (ret != null)
                {
                    foreach (var selectedIO in ret)
                    {
                        var module = Sess.Xml.Modules.Find(x => x.mac == selectedIO.MAC);

                        items[0].Add(new ItemModel()
                        {
                            Name = module.GetIOName(ioType, selectedIO.Channel),
                            Object = selectedIO,
                            Type = selectedIO.GetType(),
                            IOType = selectedIO.IOType,
                            IOCfg = selectedIO.GUICfg,
                            ModuleInfo = String.Format("{0} ({1})", module.boardname, module.tcp_hostname)
                        });
                    }

                    ((XML.XMLViewGroup)Ctor.Object).IOs.AddRange(ret);
                    Sess.Xml.Save();
                }
            }

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> AddModule()
        {
            var answer = await UserDialogs.Instance.ConfirmAsync(
                Resx.AppResources.AddNewModuleText, 
                Resx.AppResources.AddNewModuleHeader, 
                Resx.AppResources.AddNewModuleSearch, 
                Resx.AppResources.AddNewModuleAdd
            );

            if (answer)
            {
                // scan
                DialogBroadcast db = new DialogBroadcast();
                await Navigation.PushAsync(db);
                List<ETHModule.Module> ret = await db.PageCloseTask;

                if (ret != null)
                { 
                    foreach (var selectedModule in ret)
                    {
                        items[1].Add(new ItemModel()
                        {
                            Name = selectedModule.boardname,
                            Object = selectedModule,
                            Type = selectedModule.GetType(),
                        });
                    }
                
                    Sess.Xml.Modules.AddRange(ret);
                    Sess.Xml.Save();
                    MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);
                }
            }
            else
            {
                // add
                DialogNetworkConfig dnc = new DialogNetworkConfig(new DialogNetworkConfig.Constructor()
                {
                    ViewType = DialogNetworkConfig.ViewType.ADD
                });
                await Navigation.PushAsync(dnc);
                ETHModule.Module ret = await dnc.PageCloseTask;

                if (ret != null)
                {
                    if (Sess.Xml.Modules.Find(x => x.mac == ret.mac) == null)
                    {
                        // modul ist nicht drin
                        items[1].Add(new ItemModel()
                        {
                            Name = ret.boardname,
                            Object = ret,
                            Type = ret.GetType()
                        });

                        Sess.Xml.Modules.Add(ret);
                        Sess.Xml.Save();
                        MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);

                        DTControl.ShowToast(Resx.AppResources.NC_AddModuleToastOK);
                        return true;
                    }

                    // modul ist schon geadded
                    await UserDialogs.Instance.AlertAsync(
                        Resx.AppResources.NC_AddHeader,
                        Resx.AppResources.NC_AddModuleExistText,
                        Resx.AppResources.MSG_OK
                    );
                }
            }

            return false;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> AddLocation()
        {
            PromptResult pm = await UserDialogs.Instance.PromptAsync(new PromptConfig()
            {
                Title = Resx.AppResources.CFG_AddNewLocationHeader,
                Placeholder = Resx.AppResources.CFG_AddNewLocationPlaceholder,
                Message = Resx.AppResources.CFG_AddNewLocationText,
                OkText = Resx.AppResources.MSG_OK,
                CancelText = Resx.AppResources.MSG_Cancel
            });

            if (pm.Ok)
            {
                switch (Ctor.ViewType)
                {
                    case ViewType.MAIN:
                        Sess.Log("MAIN");
                        XML.XMLView cl = new XML.XMLView() { Name = pm.Text };
                        items[0].Add(new ItemModel()
                        {
                            Type = typeof(XML.XMLView),
                            Object = cl,
                            Name = pm.Text
                        });
                        Sess.Xml.Views.Add(cl);
                        break;

                    case ViewType.GROUP:
                        Sess.Log("GROUP");
                        XML.XMLViewGroup cg = new XML.XMLViewGroup() { Name = pm.Text };
                        items[0].Add(new ItemModel()
                        {
                            Type = typeof(XML.XMLViewGroup),
                            Object = cg,
                            Name = pm.Text
                        });
                        ((XML.XMLView)Ctor.Object).Groups.Add(cg);
                        break;
                }

                Sess.Xml.Save();
                MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);
            }

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> EditItem()
        {
            bool isModule = (selectedItem.Type == typeof(ETHModule.Module));
            var itemList = (isModule ? items[1] : items[0]);
            int index;

            if (!isModule)
            {
                PromptResult pm = await UserDialogs.Instance.PromptAsync(new PromptConfig()
                {
                    Title = String.Format(Resx.AppResources.CFG_EditLocationHeader, Resx.AppResources.CFG_Location),
                    Placeholder = Resx.AppResources.CFG_AddNewLocationPlaceholder,
                    Message = String.Format(Resx.AppResources.CFG_EditLocationText, selectedItem.Name),
                    OkText = Resx.AppResources.MSG_OK,
                    CancelText = Resx.AppResources.MSG_Cancel,
                    Text = selectedItem.Name
                });

                if (pm.Ok)
                {
                    index = itemList.IndexOf(selectedItem);
                    
                    // nur den namen ändern reicht nicht zum resetten...
                    selectedItem.Name = pm.Text;
                    itemList.RemoveAt(index);
                    itemList.Insert(index, selectedItem);

                    switch (Ctor.ViewType)
                    {
                        case ViewType.MAIN:
                            Sess.Log("MAIN");
                            Sess.Xml.Views[index].Name = pm.Text;
                            break;

                        case ViewType.GROUP:
                            Sess.Log("GROUP");
                            ((XML.XMLView)Ctor.Object).Groups[index].Name = pm.Text;
                            break;
                    }
                    
                    Sess.Xml.Save();
                    MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);
                }
            }



            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        enum Swap
        {
            DOWN,
            UP
        }

        void SwapItems(Swap swap)
        {
            int index;
            int indexNew;
            bool isSwapable = false;
            bool isModule = false;

            if (selectedItem != null)
            {
                // swap in der internen liste
                isModule = (selectedItem.Type == typeof(ETHModule.Module));
                var itemList = (isModule ? items[1] : items[0]);
                index = itemList.IndexOf(selectedItem);

                if (swap == Swap.DOWN)
                {
                    isSwapable = (index < (itemList.Count - 1));
                    indexNew = index + 1;
                }
                else
                {
                    isSwapable = (index > 0);
                    indexNew = index - 1;
                }

                if (isSwapable)
                {
                    itemList[index] = itemList[indexNew];
                    itemList[indexNew] = selectedItem;

                    if (isModule)
                    {
                        Sess.Xml.Modules[index] = Sess.Xml.Modules[indexNew];
                        Sess.Xml.Modules[indexNew] = selectedItem.Object as ETHModule.Module;
                    }
                    else
                    {
                        switch (Ctor.ViewType)
                        {
                            case ViewType.MAIN:
                                Sess.Log("MAIN");
                                Sess.Xml.Views[index] = Sess.Xml.Views[indexNew];
                                Sess.Xml.Views[indexNew] = selectedItem.Object as XML.XMLView;
                                break;

                            case ViewType.GROUP:
                                Sess.Log("GROUP");
                                ((XML.XMLView)Ctor.Object).Groups[index] = ((XML.XMLView)Ctor.Object).Groups[indexNew];
                                ((XML.XMLView)Ctor.Object).Groups[indexNew] = selectedItem.Object as XML.XMLViewGroup;
                                break;

                            case ViewType.IO:
                                Sess.Log("IO");
                                ((XML.XMLViewGroup)Ctor.Object).IOs[index] = ((XML.XMLViewGroup)Ctor.Object).IOs[indexNew];
                                ((XML.XMLViewGroup)Ctor.Object).IOs[indexNew] = selectedItem.Object as XML.XMLViewGroupIO;
                                break;
                        }
                    }

                    Sess.Xml.Save();
                    if (Ctor.ViewType != ViewType.IO)
                    { 
                        // bei I/Os nicht refreshen, da sonst die namen weg sind
                        MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);
                    }
                    SetFooterIcons();
                }
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> DeleteItem()
        {
            bool isModule = false;

            if (selectedItem != null)
            {
                var answer = await UserDialogs.Instance.ConfirmAsync(
                    String.Format(Resx.AppResources.CFG_DeleteEntryText, selectedItem.Name),
                    Resx.AppResources.CFG_DeleteEntryHeader,
                    Resx.AppResources.MSG_Yes,
                    Resx.AppResources.MSG_No
                );

                if (answer)
                { 
                    isModule = (selectedItem.Type == typeof(ETHModule.Module));
                    var itemList = (isModule ? items[1] : items[0]);
                
                    itemList.Remove(selectedItem);

                    if (isModule)
                    {
                        var module = Sess.Xml.Modules.Find(x => x.mac == ((ETHModule.Module)selectedItem.Object).mac);
                        Sess.Xml.Modules.Remove(module);
                    }
                    else
                    {
                        switch (Ctor.ViewType)
                        {
                            case ViewType.MAIN:
                                Sess.Log("MAIN");
                                Sess.Log("Items vor löschen " + Sess.Xml.Views.Count.ToString());
                                var loc = Sess.Xml.Views.Find(x => x.Name == ((XML.XMLView)selectedItem.Object).Name);
                                Sess.Xml.Views.Remove(loc);
                                Sess.Log("Items NACH löschen " + Sess.Xml.Views.Count.ToString());
                                break;

                            case ViewType.GROUP:
                                Sess.Log("GROUP");
                                Sess.Log("Items vor löschen " + ((XML.XMLView)Ctor.Object).Groups.Count.ToString());
                                var group = ((XML.XMLView)Ctor.Object).Groups.Find(x => x.Name == ((XML.XMLViewGroup)selectedItem.Object).Name);
                                ((XML.XMLView)Ctor.Object).Groups.Remove(group);
                                Sess.Log("Items NACH löschen " + ((XML.XMLView)Ctor.Object).Groups.Count.ToString());
                                break;

                            case ViewType.IO:
                                Sess.Log("IO");
                                Sess.Log("Items vor löschen " + ((XML.XMLViewGroup)Ctor.Object).IOs.Count.ToString());
                                Sess.Log("Lösche " + ((XML.XMLViewGroupIO)selectedItem.Object).Channel.ToString());
                                ((XML.XMLViewGroup)Ctor.Object).IOs.Remove(selectedItem.Object as XML.XMLViewGroupIO);
                                Sess.Log("Items NACH löschen " + ((XML.XMLViewGroup)Ctor.Object).IOs.Count.ToString());
                                break;
                        }
                    }

                    Sess.Xml.Save();
                    if (Ctor.ViewType != ViewType.IO)
                    {
                        // bei I/Os nicht refreshen, da sonst die namen weg sind
                        MessagingCenter.Send<ContentPage>(this, Sess.MC_MSG_REFRESH);
                    }
                    selectedItem = null;
                }
            }

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async void ShowHelp()
        {
            PopUpHelp help;
            bool val;

            help = new PopUpHelp(new PopUpHelp.Constructor()
            {
                Title = "Wussten Sie schon?",
                Content = ContentHelp.ContentHelpLocation()
            });
            await Navigation.PushPopupAsync(help);
            val = await help.PageCloseTask;

            help = new PopUpHelp(new PopUpHelp.Constructor()
            {
                Title = "Wussten Sie schon?",
                Content = ContentHelp.ContentHelpLocationButtons()
            });
            await Navigation.PushPopupAsync(help);
            val = await help.PageCloseTask;

            /*
            switch (Ctor.ViewType)
            {
                case ViewType.MAIN:

                    await UserDialogs.Instance.AlertAsync("machwas", "MAIN");
                    break;

                case ViewType.GROUP:
                    await UserDialogs.Instance.AlertAsync("machwas", "GROUP");
                    break;

                case ViewType.IO:
                    await UserDialogs.Instance.AlertAsync("machwas", "IO");
                    break;
            }
            */
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        enum Buttons
        {
            DELETE = 0,
            EDIT,
            UP,
            DOWN,
            CONTINUE
        }

        public void SetFooterIcons()
        {
            int index;
            bool isModule = false;

            Func<Buttons, DTControl.ImageButton> GetButton = (btn) =>
            {
                return (DTControl.ImageButton)gridFooter.Children.FirstOrDefault(x => ((Grid.GetRow(x) == 0) && (Grid.GetColumn(x) == (int)btn)));
            };

            if (selectedItem != null)
            {
                isModule = (selectedItem.Type == typeof(ETHModule.Module));
                var itemList = (isModule ? items[1] : items[0]);
                index = itemList.IndexOf(selectedItem);
                
                GetButton(Buttons.UP).Enabled = (index != 0);
                GetButton(Buttons.DOWN).Enabled = (index != (itemList.Count - 1));
                GetButton(Buttons.EDIT).Enabled = ((selectedItem.Type != typeof(ETHModule.Module)) && (selectedItem.Type != typeof(XML.XMLViewGroupIO)));
                GetButton(Buttons.CONTINUE).Enabled = (selectedItem.Type != typeof(XML.XMLViewGroupIO));
            }
        }
    }
}
