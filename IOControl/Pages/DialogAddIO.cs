using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using System.Threading.Tasks;
using System.ComponentModel;    // INotifyPropertyChanged

using Xamarin.Forms;
using System.Collections.ObjectModel;

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
        }

        // ------------------------------------
        // ------------------------------------
        // ------------------------------------

        public class ItemModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Name { get; set; }
            public bool AlreadyAdded { get; set; }
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

        public class ItemViewCell : ViewCell
        {
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
                    layout.BackgroundColor = sw.IsToggled ? DT.COLOR_SELECTED : temp;
                });
                layout.Children.Add(sw);

                // Label Name
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));
                layout.Children.Add(labelName);

                // Already Added
                Image img = new Image() { Source = ImageSource.FromFile("btn_ok.png"), HorizontalOptions = LayoutOptions.End };
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

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public DialogAddIO(Constructor ctor)
        {
            Ctor = ctor;

            StackLayout slMain;
            StackLayout slScan;
            ListView listView;
            Grid footer;
            TapGestureRecognizer tgp;

            slMain = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0, 10, 0, 10) };


            // ------------------------------------
            // listView
            listView = new ListView();
            listView.ItemsSource = items;
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("LongName");
            listView.GroupShortNameBinding = new Binding("ShortName");
            //listView.SeparatorVisibility = SeparatorVisibility.None;
            //listView.GroupHeaderTemplate = new DataTemplate(() => { return new HeaderViewCell(this); });
            listView.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            listView.HasUnevenRows = true;

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
            footer.Children.Add(imgRefresh, 1, 0);

            slMain.Children.Add(footer);








            Content = slMain;
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
                        case IOType.DI:         ret = (m.IO.cnt_di > 0);        break;
                        case IOType.DO:         ret = (m.IO.cnt_do > 0);        break;
                        case IOType.PWM:        ret = (m.IO.cnt_do_pwm > 0);    break;
                        //case IOType.DO_TIMER:   ret = (m.IO.cnt_do_timer > 0);  break;
                        case IOType.AD:         ret = (m.IO.cnt_ai > 0);        break;
                        case IOType.DA:         ret = (m.IO.cnt_ao > 0);        break;
                        case IOType.TEMP:       ret = (m.IO.cnt_temp > 0);      break;
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
                        case IOType.DI:         ret = m.IOName.di;      break;
                        case IOType.DO:         ret = m.IOName.dout;    break;
                        case IOType.PWM:        ret = m.IOName.pwm;     break;
                        //case IOType.DO_TIMER:   ret = m.IOName.do_timer;break;
                        case IOType.AD:         ret = m.IOName.ai;      break;
                        case IOType.DA:         ret = m.IOName.ao;      break;
                        case IOType.TEMP:       ret = m.IOName.temp;    break;
                    }
                    return ret;
                };

                foreach(var module in modules)
                {
                    var group = new HeaderModel()
                    {
                        LongName = string.Format("{0}({1}:{2}", module.boardname, module.tcp_hostname, module.tcp_port).ToUpper(),
                        ShortName = module.boardname.ToUpper().Substring(0, 1)
                    };

                    var moduleIOs = getIONames(module);
                    if (moduleIOs != null)
                    {
                        foreach (var ioName in moduleIOs)
                        {
                            group.Add(new ItemModel()
                            {
                                Name = ioName,
                                Object = module,
                                AlreadyAdded = false
                            });
                        }
                    }
                    
                    items.Add(group);
                }

                return true;
            });


            // ui stuff davor

            scanning.Start();
            await scanning;

            // ui stuff danach

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------


    }
}
