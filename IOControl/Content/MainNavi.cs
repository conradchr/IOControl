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
        ListViewGroups<MainNaviItem> Items { get; set; } = new ListViewGroups<MainNaviItem>();



        public MenuNavigation()
        {

            // --------------------
            // Views

            var views = new ListViewGroupHeader<MainNaviItem>(Resx.AppResources.Location);
            Sess.Xml.Views.ForEach(view => views.Add(new MainNaviItem(view.Name, view.GetType(), view)));
            if (Sess.Xml.Views.Count > 0)
            {
                Items.Add(views);
            }

            // --------------------
            // Module

            var groups = new ListViewGroupHeader<MainNaviItem>(Resx.AppResources.Location);
            {
                LongName = Resx.AppResources.NAV_Modules.ToUpper(),
                ShortName = Resx.AppResources.NAV_Modules.ToUpper().Substring(0, 1)
            };

            Sess.Xml.Modules.ForEach(module => groups.Add(new MainNaviItem() { Name = module.boardname, GroupType = module.GetType(), Object = module }));
            if (Sess.Xml.Modules.Count > 0)
            {
                Items.Add(groups);
            }

            // --------------------
            // Demo            
            
            var groupDemo = new GroupedMenuNaviModel()
            {
                LongName = "DEMO",
                ShortName = "D"
            };

            ETHModule.Module demo = new ETHModule.Module(
                "DEDITEC RO-ETH Webcam",
                "mx0.usb-la.de",
                8890,
                5000,
                "00:C0:D5:01:10:8F"
            );

            groupDemo.Add(new MainNaviItem()
            {
                Name = demo.boardname,
                GroupType = demo.GetType(),
                Object = demo,
            });

            if (Sess.Xml.Config.ShowDemoModule)
            {
                Items.Add(groupDemo);
            }

            // --------------------
            // Settings
            
            var groupSettings = new GroupedMenuNaviModel()
            {
                LongName = Resx.AppResources.NAV_Settings.ToUpper(),
                ShortName = Resx.AppResources.NAV_Settings.ToUpper().Substring(0, 1)
            };

            groupSettings.Add(new MainNaviItem()
            {
                Name = Resx.AppResources.NAV_Configuration,
                GroupType = typeof(AppearancePage)
            });

            if (Sess.Xml.Config.ShowSetting)
            {
                Items.Add(groupSettings);
            }

            // --------------------
            // --------------------
            // ListView

            listView = VCModels.ListViewInit();
            listView.GroupHeaderTemplate = new DataTemplate(typeof(VCModels.NaviHeader));
            listView.ItemTemplate = new DataTemplate(typeof(VCModels.NaviIcon));
            listView.ItemsSource = Items;

            Icon = "hamburger.png";
            Title = "Configuration";

            Content = new StackLayout
            {
                Padding = new Thickness(0, 5, 0, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    listView
                }
            };
        }
	}
}
