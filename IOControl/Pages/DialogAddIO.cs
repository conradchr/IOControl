using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using System.Threading.Tasks;
using System.ComponentModel;    // INotifyPropertyChanged

using Xamarin.Forms;
using System.Collections.ObjectModel;

using System.Diagnostics;   // StopWatch

namespace IOControl
{
    public class DialogAddIO : ContentPage
    {
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class Constructor
        {
            public IOType IOType { get; set; }
            public ContentGroup Group { get; set; }
        }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        public class ItemModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            
            public string Name { get; set; }
            public bool AlreadyAdded { get; set; }

            public uint Channel { get; set; }
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
        }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        public class HeaderModel : ObservableCollection<ItemModel>
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
        }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        public class HeaderViewCell : ViewCell
        {
            public HeaderViewCell()
            {
                StackLayout layout = new StackLayout() { Padding = new Thickness(10, 5, 10, 5), BackgroundColor = DT.COLOR };

                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("LongName"));
                layout.Children.Add(labelName);

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

                // Already Added
                Image img = new Image() { Source = ImageSource.FromFile("btn_ok.png"), HeightRequest = 20, WidthRequest = 20, Aspect = Aspect.AspectFill };
                img.SetBinding(Image.IsVisibleProperty, new Binding("AlreadyAdded"));
                layout.Children.Add(img);

                View = layout;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public Constructor Ctor { get; set; }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        ObservableCollection<HeaderModel> items;
        StackLayout slScan;
        ListView listView;
        Grid footer;
        ItemModel selectedItem;

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public DialogAddIO(Constructor ctor)
        {
            Ctor = ctor;
            Title = Resx.AppResources.CFG_AddIOHeader;

            StackLayout slMain;
            TapGestureRecognizer tgp;

            slMain = new StackLayout() { Padding = new Thickness(0, 10, 0, 10) };


            // ------------------------------------
            // listView
            items = new ObservableCollection<HeaderModel>();
            listView = new ListView() { VerticalOptions = LayoutOptions.FillAndExpand, IsVisible = false };
            listView.ItemsSource = items;
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("LongName");
            listView.GroupShortNameBinding = new Binding("ShortName");
            //listView.SeparatorVisibility = SeparatorVisibility.None;
            listView.GroupHeaderTemplate = new DataTemplate(typeof(HeaderViewCell));
            listView.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            listView.HasUnevenRows = true;
            listView.RowHeight = -1;
            slMain.Children.Add(listView);

            listView.ItemTapped += (s, e) =>
            {
                selectedItem = e.Item as ItemModel;
                if (selectedItem != null)
                {
                    if (!selectedItem.AlreadyAdded)
                    { 
                        selectedItem.IsSelected = !selectedItem.IsSelected;
                    }
                    else
                    {
                        DTControl.ShowToast(Resx.AppResources.CFG_AddIOAlreadyExist);
                    }
                    ((ListView)s).SelectedItem = null;
                }
            };

            // ------------------------------------
            // scan
            slScan = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand };
            ActivityIndicator aiScan = new ActivityIndicator() { Color = Color.Red, IsRunning = true };
            Label lblScan = new Label() { Text = Resx.AppResources.BC_Scan, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            slScan.Children.Add(aiScan);
            slScan.Children.Add(lblScan);
            slMain.Children.Add(slScan);

            // ------------------------------------
            // footer
            footer = new Grid() { VerticalOptions = LayoutOptions.End, IsVisible = false };

            Image imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += async (s, e) => await Scan();
            imgCancel.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgCancel, 0, 0);

            Image imgRefresh = new Image() { Source = ImageSource.FromFile("btn_refresh.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += async (s, e) => await Scan();
            imgRefresh.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgRefresh, 1, 0);

            Image imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += async (s, e) => await Scan();
            imgOK.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgOK, 2, 0);

            slMain.Children.Add(footer);

            Content = slMain;

            Scan();
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        async Task<bool> Scan()
        {
            Task<bool> scanning = new Task<bool>(() =>
            {
                // alle module scannen
                foreach (var module in DT.Session.xmlContent.modules)
                {
                    if (module.OpenModule() != 0)
                    {
                        module.IOInit();
                    }
                    module.CloseModule();
                }

                // check ob eins der module den gesuchten I/O hat
                Func<Module, bool> checkIO = (m) =>
                {
                    bool ret = false;
                    switch (Ctor.IOType)
                    {
                        case IOType.DI: ret = (m.IO.cnt_di > 0); break;
                        case IOType.DO: ret = (m.IO.cnt_do > 0); break;
                        case IOType.PWM: ret = (m.IO.cnt_do_pwm > 0); break;
                        //case IOType.DO_TIMER:   ret = (m.IO.cnt_do_timer > 0);  break;
                        case IOType.AD: ret = (m.IO.cnt_ai > 0); break;
                        case IOType.DA: ret = (m.IO.cnt_ao > 0); break;
                        case IOType.TEMP: ret = (m.IO.cnt_temp > 0); break;
                    }
                    return ret;
                };
                var modules = DT.Session.xmlContent.modules.Where(m => checkIO(m) == true);

                // module, inkl. I/Os in eine neue gruppe adden und das dann in die hauptliste
                Func<Module, List<string>> getIONames = (m) =>
                {
                    List<string> ret = null;
                    switch (Ctor.IOType)
                    {
                        case IOType.DI: ret = m.IOName.di; break;
                        case IOType.DO: ret = m.IOName.dout; break;
                        case IOType.PWM: ret = m.IOName.pwm; break;
                        //case IOType.DO_TIMER:   ret = m.IOName.do_timer;break;
                        case IOType.AD: ret = m.IOName.ai; break;
                        case IOType.DA: ret = m.IOName.ao; break;
                        case IOType.TEMP: ret = m.IOName.temp; break;
                    }
                    return ret;
                };

                foreach (var module in modules)
                {
                    var group = new HeaderModel()
                    {
                        LongName = string.Format("{0}\n({1}:{2})", module.boardname, module.tcp_hostname, module.tcp_port).ToUpper(),
                        ShortName = module.boardname.ToUpper().Substring(0, 1)
                    };
                    
                    var moduleIOs = getIONames(module);
                    if (moduleIOs != null)
                    {
                        int i = 0;

                        foreach (var ioName in moduleIOs)
                        {
                            group.Add(new ItemModel()
                            {
                                Name = ioName,
                                Object = module,
                                AlreadyAdded = (i % 2 == 0)
                            });
                            i++;
                        }
                    }

                    items.Add(group);
                }

                return false;
            });

            Func<bool, bool> switchGUI = (isScan) =>
            {
                listView.IsVisible = !isScan;
                slScan.IsVisible = isScan;
                footer.IsVisible = !isScan;
                return true;
            };

            // ui stuff davor
            switchGUI(true);

            Stopwatch sw = new Stopwatch();
            items.Clear();
            sw.Start();
            scanning.Start();
            await scanning;

            while (sw.ElapsedMilliseconds < DT.Const.TIME_ANIMATION_MIN_MS)
            {
                await Task.Delay(10);
            }

            // ui stuff danach
            switchGUI(false);

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public void AddIO()
        {
            List<ItemModel> list = new List<ItemModel>();

            foreach (var header in items)
            {
                list.AddRange(header.Where(x => x.IsSelected));
            }

            
        }


    }
}
