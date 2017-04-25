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
            public XML.IOTypes IOType { get; set; }
            public XML.XMLViewGroup ViewGroup { get; set; }
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
            public ETHModule.Module Module { get; set; }

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
                Image img = new Image() { Source = ImageSource.FromFile("btn_ok.png"), HeightRequest = 20, WidthRequest = 20, Aspect = Aspect.AspectFit };
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
        public Task<List<XML.XMLViewGroupIO>> PageCloseTask { get { return tcs.Task; } }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        ObservableCollection<HeaderModel> items;
        StackLayout slScan;
        ListView listView;
        Grid footer;
        ItemModel selectedItem;
        TaskCompletionSource<List<XML.XMLViewGroupIO>> tcs;
        List<XML.XMLViewGroupIO> taskResult = null;

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public DialogAddIO(Constructor ctor)
        {
            StackLayout slMain;
            TapGestureRecognizer tgp;

            Ctor = ctor;
            Title = Resx.AppResources.CFG_AddIOHeader;

            tcs = new TaskCompletionSource<List<XML.XMLViewGroupIO>>();
            slMain = new StackLayout() { Padding = new Thickness(0, 10, 0, 10) };

            this.Disappearing += (s, e) =>
            {
                Sess.Log("DialogAddIO SetResult");
                tcs.SetResult(taskResult);
            };

            // ------------------------------------
            // listView
            items = new ObservableCollection<HeaderModel>();
            listView = new ListView() { VerticalOptions = LayoutOptions.FillAndExpand, IsVisible = false };
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
            tgp.Tapped += (s, e) => Navigation.PopAsync();
            imgCancel.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgCancel, 0, 0);

            Image imgRefresh = new Image() { Source = ImageSource.FromFile("btn_refresh.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += async (s, e) => await Scan();
            imgRefresh.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgRefresh, 1, 0);

            Image imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            tgp = new TapGestureRecognizer();
            tgp.Tapped += (s, e) => SaveIO();
            imgOK.GestureRecognizers.Add(tgp);
            footer.Children.Add(imgOK, 2, 0);

            slMain.Children.Add(footer);

            Content = slMain;

            //Scan();
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> Scan()
        {
            Task<bool> scanning = new Task<bool>(() =>
            {
                Sess.Log("DA: Scan anfang");
                // alle module scannen
                foreach (var module in Sess.Xml.Modules)
                {
                    if (module.OpenModule() != 0)
                    {
                        module.IOInit();
                    }
                    module.CloseModule();
                }

                // check ob eins der module den gesuchten I/O hat
                Func<ETHModule.Module, bool> checkIO = (m) =>
                {
                    bool ret = false;
                    switch (Ctor.IOType)
                    {
                        case XML.IOTypes.DI: ret = (m.IO.cnt_di > 0); break;
                        case XML.IOTypes.DO: ret = (m.IO.cnt_do > 0); break;
                        case XML.IOTypes.PWM: ret = (m.IO.cnt_do_pwm > 0); break;
                        //case XML.IOTypes.DO_TIMER:   ret = (m.IO.cnt_do_timer > 0);  break;
                        case XML.IOTypes.AD: ret = (m.IO.cnt_ai > 0); break;
                        case XML.IOTypes.DA: ret = (m.IO.cnt_ao > 0); break;
                        case XML.IOTypes.TEMP: ret = (m.IO.cnt_temp > 0); break;
                    }
                    return ret;
                };
                var modules = Sess.Xml.Modules.Where(m => checkIO(m) == true);

                Sess.Log("DA: Scan Module");
                int cnt = 0;
                // module, inkl. I/Os in eine neue gruppe adden und das dann in die hauptliste
                foreach (var module in modules)
                {
                    var group = new HeaderModel()
                    {
                        LongName = string.Format("{0}\n({1}:{2})", module.boardname, module.tcp_hostname, module.tcp_port).ToUpper(),
                        ShortName = module.boardname.ToUpper().Substring(0, 1)
                    };

                    var moduleIOs = module.GetIONames(Ctor.IOType);
                    if (moduleIOs != null)
                    {
                        uint chan = 0;

                        foreach (var ioName in moduleIOs)
                        {
                            XML.XMLViewGroupIO temp = new XML.XMLViewGroupIO(Ctor.IOType, module.mac, chan);
                            group.Add(new ItemModel()
                            {
                                Channel = chan,
                                Name = ioName,
                                Module = module,
                                AlreadyAdded = (Ctor.ViewGroup.IOs.Where(x => x.Equals(temp)).ToList().Count != 0)
                            });
                            chan++;
                        }
                    }

                    items.Add(group);
                    Sess.Log("adde " + cnt.ToString());
                    cnt++;
                }

                Sess.Log("DA: Scan Task ende");
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
            listView.ItemsSource = null;
            items.Clear();
            sw.Start();
            scanning.Start();

            Sess.Log("await");
            await scanning;
            Sess.Log("danach");

            while (sw.ElapsedMilliseconds < GUI.TIME_ANIMATION_MIN_MS)
            {
                await Task.Delay(10);
            }
            Sess.Log("switchGUI");
            // ui stuff danach
            switchGUI(false);
            Sess.Log("DA: Scan Ende");

            listView.ItemsSource = items;

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public void SaveIO()
        {
            taskResult = new List<XML.XMLViewGroupIO>();

            foreach (var header in items)
            {
                foreach (var selectedItem in header.Where(x => x.IsSelected))
                {
                    taskResult.Add(new XML.XMLViewGroupIO(Ctor.IOType, selectedItem.Module.mac, selectedItem.Channel));
                }
            }

            Navigation.PopAsync();
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
    }
}
