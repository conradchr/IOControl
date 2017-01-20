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
        public static ContentPage context;

        public Task<bool> PageCloseTask { get { return tcs.Task; } }
        TaskCompletionSource<bool> tcs;
        bool taskComplete = false;
        bool taskResult = false;

        public class BCTemplateItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public string Boardname { get; set; }
            public string IP { get; set; }
            public string Mac { get; set; }
            public string Product { get; set; }

            public int Id { get; set; }


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
                //layout.Padding = new Thickness(0, 10, 0, 10);

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

                Color temp = layout.BackgroundColor;

                sw.Toggled += new EventHandler<ToggledEventArgs>((s, e) =>
                {
                    layout.BackgroundColor = sw.IsToggled ? DT.COLOR : temp;
                });

                slMain.Children.Add(layout);

                StackLayout slEdit = new StackLayout() { HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                Image imgEdit = new Image() { Source = ImageSource.FromFile("btn_edit.png") };
                var imgEditTapped = new TapGestureRecognizer();
                imgEditTapped.Tapped += async (s, e) =>
                {
                    DT.Log(String.Format("machwas id={0}!!!", lblId.Text));
                    var id = Convert.ToInt32(lblId.Text);
                    var dev = DT.eth_devs.Find(x => x.Network.mac_formatted == items[id].Mac);
                    Module module = null;
                    if (dev != null)
                    {
                        module = new Module(
                            dev.BoardName.boardname,
                            dev.Network.ip,
                            (int)dev.Network.port,
                            5000,
                            dev.Network.mac
                        );
                    }

                    DialogNetworkConfig dnc = new DialogNetworkConfig(new DialogNetworkConfig.Constructor()
                    {
                        ViewType = DialogNetworkConfig.ViewType.EDIT,
                        Module = module
                    });
                    DT.Log("starte EDIT");
                    await context.Navigation.PushAsync(dnc);
                    DT.Log("warte auf EDIT ende");
                    await dnc.PageCloseTask;
                    DT.Log("ende EDIT");

                };
                imgEdit.GestureRecognizers.Add(imgEditTapped);
                slEdit.Children.Add(imgEdit);
                slMain.Children.Add(slEdit);

                View = slMain;

                //View = layout;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            taskComplete = true;
            taskResult = false;

            return base.OnBackButtonPressed();
        }


        public DialogBroadcast()
        {
            FormInit();

            Title = Resx.AppResources.BC_Header;
            tcs = new TaskCompletionSource<bool>();

            this.Disappearing += (s, e) =>
            {
                if (taskComplete)
                { 
                    tcs.SetResult(taskResult);
                    DT.Log("DialogBroadcast weg");
                }
            };

            context = this;

            Scan();
        }

        public void Scan()
        {
            slHeader.IsVisible = false;
            slScan.IsVisible = true;
            slListView.IsVisible = false;
            slFooter.IsVisible = false;

            int deviceId = 0;

            Task<int>.Run(() =>
            {
                uint cnt = DT.Bc.GetEthernetDevicesByBC(DT.eth_devs);
                Device.BeginInvokeOnMainThread(() =>
                {
                    items = new ObservableCollection<BCTemplateItem>();
                    lblCountModules.Text = String.Format(Resx.AppResources.BC_ListViewHeader, cnt);

                    foreach (var x in DT.eth_devs)
                    {
                        items.Add(new BCTemplateItem
                        {
                            Boardname = x.BoardName.boardname,
                            IP = String.Format("{0}:{1}", x.Network.ip, x.Network.port),
                            Mac = x.Network.mac_formatted,
                            Product = IntCommands.DapiInternGetModuleName(x.BLFWInfo.delib_module_id),
                            Id = deviceId++
                        });
                    }
                    listView.ItemsSource = items;

                    slHeader.IsVisible = true;
                    slScan.IsVisible = false;
                    slListView.IsVisible = true;
                    slFooter.IsVisible = true;

                }); // Device.BeginInvokeOnMainThread
            }); // Task<int>.Run
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

            // ------------------------------------
            // scan
            slScan = new StackLayout() { VerticalOptions = LayoutOptions.CenterAndExpand };
            aiScan = new ActivityIndicator() { Color = Color.Red, IsRunning = true };
            lblScan = new Label() { Text = Resx.AppResources.BC_Scan, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, TextColor = Color.White, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            slScan.Children.Add(aiScan);
            slScan.Children.Add(lblScan);
            slMain.Children.Add(slScan);

            // ------------------------------------
            // listView
            slListView = new StackLayout() { VerticalOptions = LayoutOptions.FillAndExpand, IsVisible = false };
            listView = new ListView();
            listView.HasUnevenRows = true;
            listView.RowHeight = -1;
            //listView.RowHeight = 140;
            listView.ItemTemplate = new DataTemplate(typeof(BCTemplateViewCell));
            listView.ItemTapped += new EventHandler<ItemTappedEventArgs>((s, e) =>
            {
                var item = e.Item as BCTemplateItem;
                if (item != null)
                {
                    item.IsSelected = !item.IsSelected;
                    ((ListView)s).SelectedItem = null;
                }
            });
            slListView.Children.Add(listView);
            slMain.Children.Add(slListView);

            // ------------------------------------
            // footer
            slFooter = new StackLayout() { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.End, IsVisible = false };
            grid = new Grid();

            imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png") };
            var imgCancelTapped = new TapGestureRecognizer();
            imgCancelTapped.Tapped += (s, e) =>
            {
                ClosePage(false);
            };
            imgCancel.GestureRecognizers.Add(imgCancelTapped);
            grid.Children.Add(imgCancel, 0, 0);

            imgRefresh = new Image() { Source = ImageSource.FromFile("btn_refresh.png") };
            var imgRefreshTapped = new TapGestureRecognizer();
            imgRefreshTapped.Tapped += (s, e) =>
            {
                Scan();
            };
            imgRefresh.GestureRecognizers.Add(imgRefreshTapped);
            grid.Children.Add(imgRefresh, 1, 0);

            imgOK = new Image() { Source = ImageSource.FromFile("btn_ok.png") };
            var imgOKTapped = new TapGestureRecognizer();
            imgOKTapped.Tapped += (s, e) =>
            {
                AddModules();
            };
            imgOK.GestureRecognizers.Add(imgOKTapped);
            grid.Children.Add(imgOK, 2, 0);

            slFooter.Children.Add(grid);
            slMain.Children.Add(slFooter);

            Content = slMain;
        }

        void AddModules()
        {
            List<BCTemplateItem> list = items.Where(item => item.IsSelected).ToList();
            List<String> ret = new List<string>();

            foreach (var item in list)
            {
                var dev = DT.eth_devs.Find(x => x.Network.mac_formatted == item.Mac);
                if (dev != null)
                {
                    DT.Session.xmlContent.modules.Add(new Module(
                        dev.BoardName.boardname,
                        dev.Network.ip,
                        (int) dev.Network.port,
                        5000,
                        //DT.Session.xmlContent.moduleID++
                        dev.Network.mac_formatted
                    ));

                    DT.Session.xmlContent.Save();
                }
            }

            ClosePage(true);
        }

        public void ClosePage(bool result)
        {
            taskComplete = true;
            taskResult = result;
            Navigation.PopAsync();
        }
    }
}