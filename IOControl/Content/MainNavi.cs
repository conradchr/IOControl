using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System;

namespace IOControl
{
    public class MainNaviItem
    {
        public string Name { get; set; }
        public Type GroupType { get; set; }
        public Object Object { get; set; }

        public MainNaviItem(String name, Type groupType, Object obj)
        {
            Name = name;
            GroupType = groupType;
            Object = obj;
        }
    }

    public class MenuNavigation : ContentPage
    {
        public ListView ListView { get; set; }


        public MenuNavigation()
        {
            ListViewGroup<MainNaviItem> grp;
            Sess.GUI.ListViewItems = new ListViewItems<MainNaviItem>();
            
            // Views
            grp = new ListViewGroup<MainNaviItem>(Resx.AppResources.Location);
            Sess.Xml.Views.ForEach(view => grp.Add(new MainNaviItem(view.Name, view.GetType(), view)));
            Sess.GUI.ListViewItems.AddGroup(grp);
            
            // Module
            grp = new ListViewGroup<MainNaviItem>(Resx.AppResources.NAV_Modules);
            Sess.Xml.Modules.ForEach(module => grp.Add(new MainNaviItem(module.boardname, module.GetType(), module)));
            Sess.GUI.ListViewItems.AddGroup(grp);

            // Demo 
            ETHModule.Module demo = new ETHModule.Module(
                "DEDITEC RO-ETH Webcam",
                "mx0.usb-la.de",
                8890,
                5000,
                "00:C0:D5:01:10:8F"
            );

            grp = new ListViewGroup<MainNaviItem>("DEMO");
            grp.Add(new MainNaviItem(demo.boardname, demo.GetType(), demo));
            Sess.GUI.ListViewItems.AddGroup(grp);
            
            // Settings
            grp = new ListViewGroup<MainNaviItem>(Resx.AppResources.NAV_Settings);
            grp.Add(new MainNaviItem(Resx.AppResources.NAV_Configuration, typeof(AppearancePage), null));
            Sess.GUI.ListViewItems.AddGroup(grp);

            // --------------------
            // --------------------
            // ListView

            ListView = VCModels.ListViewInit();
            ListView.GroupHeaderTemplate = new DataTemplate(typeof(VCModels.NaviHeader));
            ListView.ItemTemplate = new DataTemplate(typeof(VCModels.NaviIcon));
            ListView.ItemsSource = Sess.GUI.ListViewItems;

            Icon = "hamburger.png";
            Title = "Configuration";

            Content = new StackLayout
            {
                Padding = new Thickness(0, 5, 0, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    ListView
                }
            };
        }
	}
}
