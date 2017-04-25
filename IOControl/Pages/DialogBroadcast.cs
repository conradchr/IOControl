using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;    // INotifyPropertyChanged

using Xamarin.Forms;

namespace IOControl
{
    public class DialogBroadcast : ContentPage
    {
        StackLayout slMain;
        StackLayout slHeader;
        StackLayout slInnerHeader;
        StackLayout slScan;
        StackLayout slListView;
        StackLayout slFooter;

        Label lblCountModules;
        Button btnSelectAll;
        Button btnSelectNone;
        Label lblScan;
        ActivityIndicator aiScan;

        Grid grid;
        Image imgCancel;
        Image imgRefresh;
        Image imgOK;

        ToolbarItem tiHelp;

        ListView listView;
        public static ObservableCollection<BCTemplateItem> items;

        public Task<List<ETHModule.Module>> PageCloseTask { get { return tcs.Task; } }
        TaskCompletionSource<List<ETHModule.Module>> tcs;
        List<ETHModule.Module> taskResult = null;

        public class BCTemplateItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Boardname { get; set; }
            public string IP { get; set; }
            public string Mac { get; set; }
            public string Product { get; set; }

            public bool AlreadyAdded { get; set; }


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

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class BCTemplateViewCell : ViewCell
        {
            public BCTemplateViewCell()
            {
                StackLayout slMain = new StackLayout() { Orientation = StackOrientation.Horizontal };
                slMain.Padding = new Thickness(0, 10, 0, 10);

                StackLayout layout = new StackLayout() { HorizontalOptions = LayoutOptions.StartAndExpand };

                Label labelBoardname = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
                Label labelIP = new Label() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
                Label labelMac = new Label() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
                Label labelProduct = new Label() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) };
                Switch sw = new Switch() { IsVisible = false };

                labelBoardname.SetBinding(Label.TextProperty, new Binding("Boardname"));
                labelIP.SetBinding(Label.TextProperty, new Binding("IP", stringFormat: "IP: {0}"));
                labelMac.SetBinding(Label.TextProperty, new Binding("Mac", stringFormat: "MAC: {0}"));
                labelProduct.SetBinding(Label.TextProperty, new Binding("Product", stringFormat: "Product: {0}"));
                sw.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));

                Label lblId = new Label() { IsVisible = false };
                lblId.SetBinding(Label.TextProperty, new Binding("Id"));
                layout.Children.Add(lblId);

                layout.Children.Add(labelBoardname);
                layout.Children.Add(labelIP);
                layout.Children.Add(labelMac);
                layout.Children.Add(labelProduct);
                layout.Children.Add(sw);

                Color temp = slMain.BackgroundColor;
                sw.Toggled += new EventHandler<ToggledEventArgs>((s, e) =>
                {
                    slMain.BackgroundColor = sw.IsToggled ? DT.COLOR_SELECTED : temp;
                });

                slMain.Children.Add(layout);

                // Already Added
                Image img = new Image() { Source = ImageSource.FromFile("btn_ok.png"), HeightRequest = 20, WidthRequest = 20, Aspect = Aspect.AspectFit };
                img.SetBinding(Image.IsVisibleProperty, new Binding("AlreadyAdded"));
                slMain.Children.Add(img);

                View = slMain;
            }
        }


        public DialogBroadcast()
        {
            FormInit();

            Title = Resx.AppResources.BC_Header;
            tcs = new TaskCompletionSource<List<ETHModule.Module>>();

            this.Disappearing += (s, e) =>
            {
                tcs.SetResult(taskResult);
            };

            Scan();
        }

        public void Scan()
        {
            slHeader.IsVisible = false;
            //slScan.IsVisible = true;
            slListView.IsVisible = false;
            slFooter.IsVisible = false;

            Task<bool> t = new Task<bool>(() =>
            {
                uint cnt = DT.Bc.GetEthernetDevicesByBC(DT.eth_devs);
                Device.BeginInvokeOnMainThread(() =>
                {
                    items = new ObservableCollection<BCTemplateItem>();
                    lblCountModules.Text = String.Format(Resx.AppResources.BC_ListViewHeader, cnt);

                    foreach (var module in DT.eth_devs)
                    {
                        items.Add(new BCTemplateItem
                        {
                            Boardname = module.BoardName.boardname,
                            IP = String.Format("{0}:{1}", module.Network.ip, module.Network.port),
                            Mac = module.Network.mac_formatted,
                            Product = IntCommands.DapiInternGetModuleName(module.BLFWInfo.delib_module_id),
                            AlreadyAdded = (Sess.Xml.Modules.Find(x => x.mac == module.Network.mac_formatted) != null)
                        });
                    }
                    listView.ItemsSource = items;

                    slHeader.IsVisible = true;
                    slListView.IsVisible = true;
                    slFooter.IsVisible = true;

                }); // Device.BeginInvokeOnMainThread

                return true;
            });

#pragma warning disable CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
            GUIAnimation.ShowLoading(t);
#pragma warning restore CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
        }

        public void FormInit()
        {
            tiHelp = new ToolbarItem() { Text = "Help", Icon = "btn_help.png"};
            ToolbarItems.Add(tiHelp);

            slMain = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand };
            slMain.Padding = new Thickness(5, 5, 5, 5);

            // ------------------------------------
            // header
            slHeader = new StackLayout() { Orientation = StackOrientation.Horizontal, VerticalOptions = LayoutOptions.Start, IsVisible = false };
            var slInnerHeaderLabel = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.StartAndExpand };
            lblCountModules = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), VerticalOptions = LayoutOptions.Center };
            slInnerHeaderLabel.Children.Add(lblCountModules);
            slHeader.Children.Add(slInnerHeaderLabel);
            slInnerHeader = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };
            btnSelectAll = new Button() { Text = Resx.AppResources.BC_SelectAll };
            btnSelectAll.Clicked += (s, e) =>
            {
                foreach (var item in items)
                {
                    if (!item.AlreadyAdded)
                        item.IsSelected = true;
                }
            };
            btnSelectNone = new Button() { Text = Resx.AppResources.BC_SelectNone };
            btnSelectNone.Clicked += (s, e) =>
            {
                foreach (var item in items)
                {
                    item.IsSelected = false;
                }
            };
            slInnerHeader.Children.Add(btnSelectAll);
            slInnerHeader.Children.Add(btnSelectNone);
            slHeader.Children.Add(slInnerHeader);
            slMain.Children.Add(slHeader);

            /*
            // ------------------------------------
            // scan
            slScan = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand };
            aiScan = new ActivityIndicator() { Color = Color.Red, IsRunning = true };
            lblScan = new Label() { Text = Resx.AppResources.BC_Scan, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            slScan.Children.Add(aiScan);
            slScan.Children.Add(lblScan);
            slMain.Children.Add(slScan);
            */

            // ------------------------------------
            // listView
            slListView = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, IsVisible = false };
            listView = new ListView();
            listView.HasUnevenRows = true;
            listView.RowHeight = -1;
            listView.ItemTemplate = new DataTemplate(typeof(BCTemplateViewCell));
            listView.ItemTapped += new EventHandler<ItemTappedEventArgs>((s, e) =>
            {
                var item = e.Item as BCTemplateItem;
                if (item != null)
                {
                    if (!item.AlreadyAdded)
                    { 
                        item.IsSelected = !item.IsSelected;
                    }
                    else
                    {
                        DTControl.ShowToast(Resx.AppResources.BC_ModuleAlreadyExist);
                    }
                    ((ListView)s).SelectedItem = null;
                }
            });
            slListView.Children.Add(listView);
            slMain.Children.Add(slListView);

            // ------------------------------------
            // footer
            slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End, IsVisible = false, Padding = VCModels.PAD_FOOTER };
            grid = new Grid();

            imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            var imgCancelTapped = new TapGestureRecognizer();
            imgCancelTapped.Tapped += (s, e) => Navigation.PopAsync();
            imgCancel.GestureRecognizers.Add(imgCancelTapped);
            grid.Children.Add(imgCancel, 0, 0);

            imgRefresh = new Image() { Source = ImageSource.FromFile("btn_refresh.png") };
            var imgRefreshTapped = new TapGestureRecognizer();
            imgRefreshTapped.Tapped += (s, e) => Scan();
            imgRefresh.GestureRecognizers.Add(imgRefreshTapped);
            grid.Children.Add(imgRefresh, 1, 0);

            imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            var imgOKTapped = new TapGestureRecognizer();
            imgOKTapped.Tapped += (s, e) => AddModules();
            imgOK.GestureRecognizers.Add(imgOKTapped);
            grid.Children.Add(imgOK, 2, 0);

            slFooter.Children.Add(grid);
            slMain.Children.Add(slFooter);

            Content = slMain;
        }

        void AddModules()
        {
            taskResult = new List<ETHModule.Module>();

            foreach (var selectedItem in items.Where(x => x.IsSelected))
            {
                var dev = DT.eth_devs.Find(x => x.Network.mac_formatted == selectedItem.Mac);
                if (dev != null)
                {
                    taskResult.Add(new ETHModule.Module(
                        dev.BoardName.boardname,
                        dev.Network.ip,
                        (int)dev.Network.port,
                        5000,
                        dev.Network.mac_formatted
                    ));
                }
            }

            Navigation.PopAsync();
        }
    }
}