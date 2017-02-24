using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System.ComponentModel;    // INotifyPropertyChanged
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

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

        /*
        public Task<bool> PageCloseTask { get { return tcs.Task; } }
        TaskCompletionSource<bool> tcs;
        bool taskComplete = false;
        */

        ObservableCollection<HeaderModel> items;
        ItemModel selectedItem = null;
        
        Image imgContinue;
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
                        if (PropertyChanged != null)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                        }
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


        public class IOViewCell : ViewCell
        { 
            public IOViewCell()
            {
                StackLayout layout = new StackLayout()
                {
                    //Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 10, 10, 10)
                };

                Color temp = layout.BackgroundColor;
                Switch sw = new Switch() { IsVisible = false };
                sw.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));
                sw.Toggled += new EventHandler<ToggledEventArgs>((s, e) =>
                {
                    layout.BackgroundColor = sw.IsToggled ? DT.COLOR_SELECTED : temp;
                });
                layout.Children.Add(sw);

                // Label Name
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))/*, HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center*/ };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));
                layout.Children.Add(labelName);

                // Picker
                //DT.Log(string.Format("object={0} type={1}", ((ItemModel)BindingContext)., ((ItemModel)BindingContext).Type));

                Picker picker = new Picker();

                var bla = ((ItemModel)BindingContext);
                switch (((ContentIO)bla.Object).ioType)
                {
                    case IOType.DO:                        
                        picker.Items.Add(Resx.AppResources.IO_CFG_DO_OnOffSwitch);
                        picker.Items.Add(Resx.AppResources.IO_CFG_DO_PushButton);
                        break;

                    case IOType.AD:
                    case IOType.DA:
                        picker.Items.Add(Resx.AppResources.IO_CFG_ADDA_Voltage);
                        picker.Items.Add(Resx.AppResources.IO_CFG_ADDA_Current);
                        break;
                }
                
                layout.Children.Add(picker);

                View = layout;

            }
        }

        public class ItemViewCell : ViewCell
        {
            public ItemViewCell()
            {
                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 10, 10, 10)
                };

                Color temp = layout.BackgroundColor;
                Switch sw = new Switch() { IsVisible = false };
                sw.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));
                sw.Toggled += new EventHandler<ToggledEventArgs>((s, e) =>
                {
                    layout.BackgroundColor = sw.IsToggled ? DT.COLOR_SELECTED : temp;
                });
                layout.Children.Add(sw);

                // Label Name
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));
                layout.Children.Add(labelName);

                View = layout;
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
                    if (((HeaderModel)BindingContext).Type == typeof(Module))
                    {
                        await ((AppearancePage)context).AddModule();
                    }
                    else if (((HeaderModel)BindingContext).Type == typeof(ContentLocation))
                    {
                        await ((AppearancePage)context).AddLocation();
                    }
                    else if (((HeaderModel)BindingContext).Type == typeof(ContentGroup))
                    {
                        await ((AppearancePage)context).AddLocation();
                    }
                    else if (((HeaderModel)BindingContext).Type == typeof(ContentIO))
                    {
                        await ((AppearancePage)context).AddIO();
                    }
                };
                imgAdd.GestureRecognizers.Add(imgAddTapped);
                layout.Children.Add(imgAdd);

                /*
                StackLayout outerLayout = new StackLayout();
                outerLayout.Children.Add(layout);
                outerLayout.Children.Add(DTControl.Separator());
                View = outerLayout;
                */

                View = layout;
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
                Type = typeof(ContentLocation)
            };

            foreach (ContentLocation loc in DT.Session.xmlContent.loc)
            {
                groupLocation.Add(new ItemModel()
                {
                    Name = loc.name,
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
                Type = typeof(Module)
            };

            foreach (Module module in DT.Session.xmlContent.modules)
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
            Title = ((ContentLocation)Ctor.Object).name;

            var item = new HeaderModel()
            {
                LongName = Resx.AppResources.Groups.ToUpper(),
                ShortName = Resx.AppResources.Groups.ToUpper().Substring(0, 1),
                Type = typeof(ContentGroup)
            };

            foreach (ContentGroup group in ((ContentLocation)Ctor.Object).groups)
            {
                item.Add(new ItemModel()
                {
                    Name = group.name,
                    Type = group.GetType(),
                    Object = group,
                });
            }

            items.Add(item);
        }


        public async Task<bool> InitIO()
        {
            Title = ((ContentGroup)Ctor.Object).name;

            // --------------------
            // Locations


            Task<bool> t = new Task<bool>(() =>
            {
                var item = new HeaderModel()
                {
                    LongName = Resx.AppResources.IOs.ToUpper(),
                    ShortName = Resx.AppResources.IOs.ToUpper().Substring(0, 1),
                    Type = typeof(ContentIO)
                };

                // führt bei allen verwendeten modulen das IOinit aus
                List<string> macs = ((ContentGroup)Ctor.Object).io.Select(x => x.moduleMAC).Distinct().ToList();
                foreach (var mac in macs)
                {
                    var module = DT.Session.xmlContent.modules.Find(x => x.mac == mac);
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
                foreach (ContentIO io in ((ContentGroup)Ctor.Object).io)
                {
                    var module = DT.Session.xmlContent.modules.Find(x => x.mac == io.moduleMAC);
                    var ioname = module.GetIOName(io.ioType, io.channel);

                    item.Add(new ItemModel()
                    {
                        Name = ((ioname != null) ? ioname : String.Format(Resx.AppResources.CFG_ChNotAvailable, io.channel)),
                        Type = io.GetType(),
                        Object = io,
                    });
                }

                items.Add(item);

                return true;
            });
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            t.Start();
            UserDialogs.Instance.ShowLoading(Resx.AppResources.MSG_PleaseWait);

            await t;
            
            while (sw.ElapsedMilliseconds < DT.Const.TIME_ANIMATION_MIN_MS)
            {
                Task.Delay(20).Wait();
            }
            UserDialogs.Instance.HideLoading();

            return true;
        }

        //protected override 

        public AppearancePage(Constructor ctor)
        {
            items = new ObservableCollection<HeaderModel>();

            /*
            tcs = new TaskCompletionSource<bool>();
            this.Disappearing += (s, e) =>
            {
                if (taskComplete)
                {
                    tcs.SetResult(true);
                    DT.Log(string.Format("AppearancePage {0} weg", Ctor.ViewType));
                }
            };
            */

            Icon = "hamburger.png";
            Ctor = ctor;
            ToolbarItems.Add(new ToolbarItem() { Text = "Help", Icon = "btn_help.png", Command = new Command(ShowHelp) });


            switch (Ctor.ViewType)
            {
                case ViewType.MAIN:
                    InitMain();
                    break;

                case ViewType.GROUP:
                    InitGroup();
                    break;

                case ViewType.IO:
                    //InitIO();
                    break;
            }

            // --------------------
            // ListView

            ListView listView = new ListView();
            listView.ItemsSource = items;
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("LongName");
            listView.GroupShortNameBinding = new Binding("ShortName");
            //listView.SeparatorVisibility = SeparatorVisibility.None;
            listView.GroupHeaderTemplate = new DataTemplate(() => { return new HeaderViewCell(this); });

            if (Ctor.ViewType == ViewType.IO)
            {
                listView.ItemTemplate = new DataTemplate(typeof(IOViewCell));
            }
            else
            {
                listView.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            }
            
            listView.HasUnevenRows = true;

            
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

                    // weiter button kommt NUR, wenns kein modul ist
                    imgContinue.IsVisible = selectedItem.Type != typeof(Module);

                    labelFooter.IsVisible = false;
                    gridFooter.IsVisible = true;
                }
            };

            StackLayout slMain = new StackLayout() { Padding = new Thickness(0, 5, 0, 0) };
            StackLayout slListView = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand };
            slListView.Children.Add(listView);
            slMain.Children.Add(slListView);

            // ------------------------------------
            // footer
            StackLayout slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End, Padding = new Thickness(10, 10, 10, 10) };

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
            Image imgDelete = new Image() { Source = ImageSource.FromFile("btn_delete.png") };
            var imgDeleteTapped = new TapGestureRecognizer();
            imgDeleteTapped.Tapped += async (s, e) => await DeleteItem();
            //imgDeleteTapped.Tapped += async (s, e) => await AddIO();
            imgDelete.GestureRecognizers.Add(imgDeleteTapped);
            gridFooter.Children.Add(imgDelete, 0, 0);
            
            /*
            DTControl.ImageButton imgDelete = new DTControl.ImageButton("btn_down");
            imgDelete.TGR.Tapped += async (s, e) => await DeleteItem();
            gridFooter.Children.Add(imgDelete, 0, 0);
            */

            // Edit Button
            Image imgEdit = new Image() { Source = ImageSource.FromFile("btn_edit.png") };
            var imgEditTapped = new TapGestureRecognizer();
            imgEditTapped.Tapped += async (s, e) => await EditItem();
            //imgEditTapped.Tapped += (s, e) => imgDelete.Enabled = !imgDelete.Enabled;
            imgEdit.GestureRecognizers.Add(imgEditTapped);
            gridFooter.Children.Add(imgEdit, 1, 0);

            // swap up
            Image imgUp = new Image() { Source = ImageSource.FromFile("btn_up.png") };
            var imgUpTapped = new TapGestureRecognizer();
            imgUpTapped.Tapped += (s, e) => SwapItems(Swap.UP);
            imgUp.GestureRecognizers.Add(imgUpTapped);
            gridFooter.Children.Add(imgUp, 2, 0);

            // swap down
            Image imgDown = new Image() { Source = ImageSource.FromFile("btn_down.png") };
            var imgDownTapped = new TapGestureRecognizer();
            imgDownTapped.Tapped += (s, e) => SwapItems(Swap.DOWN);
            imgDown.GestureRecognizers.Add(imgDownTapped);
            gridFooter.Children.Add(imgDown, 3, 0);

            imgContinue = new Image() { Source = ImageSource.FromFile("btn_continue.png"), IsVisible = false };
            var imgContinueTapped = new TapGestureRecognizer();
            imgContinueTapped.Tapped += async (s, e) =>
            {
                switch (Ctor.ViewType)
                {
                    case ViewType.MAIN:
                        await Navigation.PushAsync(new AppearancePage(new Constructor()
                        {
                            ViewType = ViewType.GROUP,
                            Object = selectedItem.Object
                        }));
                        break;

                    case ViewType.GROUP:
                        //UserDialogs.Instance.ShowLoading(Resx.AppResources.MSG_PleaseWait);
                        AppearancePage ap = new AppearancePage(new Constructor()
                        {
                            ViewType = ViewType.IO,
                            Object = selectedItem.Object
                        });
                        //UserDialogs.Instance.HideLoading();
                        await Navigation.PushAsync(ap);
                        await ap.InitIO();
                        break;

                    case ViewType.IO:
                        break;
                }



                /*
                DT.Log("zeige GROUP");
                AppearancePage ap = new AppearancePage(new Constructor()
                {
                    ViewType = ViewType.GROUP,
                    Object = selectedItem.Object
                });
                await Navigation.PushAsync(ap);
                DT.Log("warte auf GROUP ende");
                await ap.PageCloseTask;
                DT.Log("ende GROUP");
                */


            };

            imgContinue.GestureRecognizers.Add(imgContinueTapped);
            gridFooter.Children.Add(imgContinue, 4, 0);

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
                var ioType = ioTypes[io];
                DialogAddIO da = new DialogAddIO(new DialogAddIO.Constructor()
                {
                    IOType = ioType,
                    ContentGroup = Ctor.Object as ContentGroup
                });
                DT.Log("AP: AA");
                await Navigation.PushAsync(da);
                await da.Scan();

                // warte auf antwort
                List<ContentIO> ret = await da.PageCloseTask;
                if (ret != null)
                {
                    foreach (var selectedIO in ret)
                    {
                        items[0].Add(new ItemModel()
                        {
                            Name = DT.Session.xmlContent.modules.Find(x => x.mac == selectedIO.moduleMAC).GetIOName(ioType, selectedIO.channel),
                            Object = selectedIO,
                            Type = selectedIO.GetType()
                        });
                    }

                    ((ContentGroup)Ctor.Object).io.AddRange(ret);
                    DT.Session.xmlContent.Save();
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
                List<Module> ret = await db.PageCloseTask;

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
                
                    DT.Session.xmlContent.modules.AddRange(ret);
                    DT.Session.xmlContent.Save();
                    MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);
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
                Module ret = await dnc.PageCloseTask;

                if (ret != null)
                {
                    if (DT.Session.xmlContent.modules.Find(x => x.mac == ret.mac) == null)
                    {
                        // modul ist nicht drin
                        items[1].Add(new ItemModel()
                        {
                            Name = ret.boardname,
                            Object = ret,
                            Type = ret.GetType()
                        });

                        DT.Session.xmlContent.modules.Add(ret);
                        DT.Session.xmlContent.Save();
                        MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);

                        DTControl.ShowToast("module geadded");
                        return true;
                    }

                    DTControl.ShowToast("module schon drin");
                }
            }

            return true;
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
                        DT.Log("MAIN");
                        ContentLocation cl = new ContentLocation(pm.Text);
                        items[0].Add(new ItemModel()
                        {
                            Type = typeof(ContentLocation),
                            Object = cl,
                            Name = pm.Text
                        });
                        DT.Session.xmlContent.loc.Add(cl);
                        break;

                    case ViewType.GROUP:
                        DT.Log("GROUP");
                        ContentGroup cg = new ContentGroup(pm.Text);
                        items[0].Add(new ItemModel()
                        {
                            Type = typeof(ContentGroup),
                            Object = cg,
                            Name = pm.Text
                        });
                        ((ContentLocation)Ctor.Object).groups.Add(cg);
                        break;
                }

                DT.Session.xmlContent.Save();
                MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);
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
            bool isModule = (selectedItem.Type == typeof(Module));
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
                            DT.Log("MAIN");
                            DT.Session.xmlContent.loc[index].name = pm.Text;
                            break;

                        case ViewType.GROUP:
                            DT.Log("GROUP");
                            ((ContentLocation)Ctor.Object).groups[index].name = pm.Text;
                            break;
                    }
                    
                    DT.Session.xmlContent.Save();
                    MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);
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
                isModule = (selectedItem.Type == typeof(Module));
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
                        DT.Session.xmlContent.modules[index] = DT.Session.xmlContent.modules[indexNew];
                        DT.Session.xmlContent.modules[indexNew] = selectedItem.Object as Module;
                    }
                    else
                    {
                        switch (Ctor.ViewType)
                        {
                            case ViewType.MAIN:
                                DT.Log("MAIN");
                                DT.Session.xmlContent.loc[index] = DT.Session.xmlContent.loc[indexNew];
                                DT.Session.xmlContent.loc[indexNew] = selectedItem.Object as ContentLocation;
                                break;

                            case ViewType.GROUP:
                                DT.Log("GROUP");
                                ((ContentLocation)Ctor.Object).groups[index] = ((ContentLocation)Ctor.Object).groups[indexNew];
                                ((ContentLocation)Ctor.Object).groups[indexNew] = selectedItem.Object as ContentGroup;
                                break;

                            case ViewType.IO:
                                DT.Log("IO");
                                ((ContentGroup)Ctor.Object).io[index] = ((ContentGroup)Ctor.Object).io[indexNew];
                                ((ContentGroup)Ctor.Object).io[indexNew] = selectedItem.Object as ContentIO;
                                break;
                        }
                    }

                    DT.Session.xmlContent.Save();
                    MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);
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
                    isModule = (selectedItem.Type == typeof(Module));
                    var itemList = (isModule ? items[1] : items[0]);
                
                    itemList.Remove(selectedItem);

                    if (isModule)
                    {
                        var module = DT.Session.xmlContent.modules.Find(x => x.mac == ((Module)selectedItem.Object).mac);
                        DT.Session.xmlContent.modules.Remove(module);
                    }
                    else
                    {
                        switch (Ctor.ViewType)
                        {
                            case ViewType.MAIN:
                                DT.Log("MAIN");
                                DT.Log("Items vor löschen " + DT.Session.xmlContent.loc.Count.ToString());
                                var loc = DT.Session.xmlContent.loc.Find(x => x.name == ((ContentLocation)selectedItem.Object).name);
                                DT.Session.xmlContent.loc.Remove(loc);
                                DT.Log("Items NACH löschen " + DT.Session.xmlContent.loc.Count.ToString());
                                break;

                            case ViewType.GROUP:
                                DT.Log("GROUP");
                                DT.Log("Items vor löschen " + ((ContentLocation)Ctor.Object).groups.Count.ToString());
                                var group = ((ContentLocation)Ctor.Object).groups.Find(x => x.name == ((ContentGroup)selectedItem.Object).name);
                                ((ContentLocation)Ctor.Object).groups.Remove(group);
                                DT.Log("Items NACH löschen " + ((ContentLocation)Ctor.Object).groups.Count.ToString());
                                break;

                            case ViewType.IO:
                                DT.Log("IO");
                                DT.Log("Items vor löschen " + ((ContentGroup)Ctor.Object).io.Count.ToString());
                                ((ContentGroup)Ctor.Object).io.Remove(selectedItem.Object as ContentIO);
                                DT.Log("Items NACH löschen " + ((ContentGroup)Ctor.Object).io.Count.ToString());
                                break;
                        }
                    }

                    DT.Session.xmlContent.Save();
                    MessagingCenter.Send<ContentPage>(this, DT.Const.MSG_REFRESH);
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
        }
    }
}
