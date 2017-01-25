using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System;

namespace IOControl
{
    public class MenuNaviModel
    {
        public string Name { get; set; }
        public Type GroupType { get; set; }
        public Object Object { get; set; }
    }

    public class GroupedMenuNaviModel : ObservableCollection<MenuNaviModel>
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
    }

    public class MenuNavigation : ContentPage
	{
        public ListView ListView { get { return listView; } }

		ListView listView;
        private ObservableCollection<GroupedMenuNaviModel> items { get; set; }

        public MenuNavigation(/*Constructor ctor*/)
        {
            items = new ObservableCollection<GroupedMenuNaviModel>();

            // --------------------
            // Locations

            var groupLocation = new GroupedMenuNaviModel()
            {
                LongName = Resx.AppResources.Location.ToUpper(),
                ShortName = Resx.AppResources.Location.ToUpper().Substring(0, 1)
            };

            foreach (ContentLocation location in DT.Session.xmlContent.loc)
            {
                groupLocation.Add(new MenuNaviModel()
                {
                    Name = location.name,
                    GroupType = location.GetType(),
                    Object = location
                });
            }

            if (DT.Session.xmlContent.loc.Count > 0)
            { 
                items.Add(groupLocation);
            }

            // --------------------
            // Module

            var groupModule = new GroupedMenuNaviModel()
            {
                LongName = Resx.AppResources.NAV_Modules.ToUpper(),
                ShortName = Resx.AppResources.NAV_Modules.ToUpper().Substring(0, 1)
            };

            foreach(Module module in DT.Session.xmlContent.modules)
            {
                groupModule.Add(new MenuNaviModel()
                {
                    Name = module.boardname,
                    GroupType = module.GetType(),
                    Object = module
                });
            }

            if (DT.Session.xmlContent.modules.Count > 0)
            { 
                items.Add(groupModule);
            }

            // --------------------
            // Demo            
            
            var groupDemo = new GroupedMenuNaviModel()
            {
                LongName = "DEMO",
                ShortName = "D"
            };

            Module demo = new Module(
                "DEDITEC RO-ETH Webcam",
                "mx0.usb-la.de",
                8890,
                5000,
                "00:C0:D5:01:10:8F"
            );

            groupDemo.Add(new MenuNaviModel()
            {
                Name = demo.boardname,
                GroupType = demo.GetType(),
                Object = demo,
            });

            if (DT.Session.xmlContent.config.ShowDemoModule)
            { 
                items.Add(groupDemo);
            }

            // --------------------
            // Settings
            
            var groupSettings = new GroupedMenuNaviModel()
            {
                LongName = Resx.AppResources.NAV_Settings.ToUpper(),
                ShortName = Resx.AppResources.NAV_Settings.ToUpper().Substring(0, 1)
            };

            groupSettings.Add(new MenuNaviModel()
            {
                Name = Resx.AppResources.NAV_Configuration,
                GroupType = typeof(AppearancePage)
            });

            if (DT.Session.xmlContent.config.ShowSetting)
            { 
                items.Add(groupSettings);
            }

            // --------------------
            // --------------------
            // ListView

            listView = new ListView();
            listView.ItemsSource = items;
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("LongName");
            listView.GroupShortNameBinding = new Binding("ShortName");



            // problem mit blauen separator...
            // listView.SeparatorVisibility = SeparatorVisibility.None;
            // dann im group und item template denn strich selbst mit ins layout machen

            

            





            listView.GroupHeaderTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.TextColor = Color.White;
                textCell.SetBinding(TextCell.TextProperty, "LongName");

                //var label = new Label() { FontAttributes = FontAttributes.Bold };
                //label.TextColor = Color.White;
                //label.SetBinding(Label.TextProperty, "LongName");
                //layout.Children.Add(label);

                return textCell;
            });

            listView.ItemTemplate = new DataTemplate(typeof(TextCell));
            listView.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");

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
