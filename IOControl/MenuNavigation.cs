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
        /*
        public class EditorViewCell : ViewCell
        {
            public EditorViewCell()
            {
                //StackLayout outerLayout = new StackLayout();

                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 10, 10, 10)
                };
                
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));

                Image imgEdit = new Image() { Source = ImageSource.FromFile("btn_edit.png") };
                var imgEditTapped = new TapGestureRecognizer();
                imgEditTapped.Tapped += (s, e) => {
                    DT.Log("edit");
                };
                imgEdit.GestureRecognizers.Add(imgEditTapped);

                Image imgDelete = new Image() { Source = ImageSource.FromFile("btn_delete.png") };
                var imgDeleteTapped = new TapGestureRecognizer();
                imgDeleteTapped.Tapped += (s, e) => {
                    DT.Log("delete");
                };
                imgDelete.GestureRecognizers.Add(imgDeleteTapped);

                layout.Children.Add(labelName);
                layout.Children.Add(imgEdit);
                layout.Children.Add(imgDelete);

                //outerLayout.Children.Add(layout);
                //outerLayout.Children.Add(DTControl.Separator());
                //View = outerLayout;

                View = layout;
            }
        }

        public class EditorHeaderCell : ViewCell
        {
            public EditorHeaderCell()
            {
                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 10, 10, 10),
                    BackgroundColor = DT.COLOR
                };

                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand };
                labelName.SetBinding(Label.TextProperty, new Binding("LongName"));

                Image imgAdd = new Image() { Source = ImageSource.FromFile("btn_add.png") };
                var imgAddTapped = new TapGestureRecognizer();
                imgAddTapped.Tapped += (s, e) => {
                    DT.Log("add");
                };
                imgAdd.GestureRecognizers.Add(imgAddTapped);

                layout.Children.Add(labelName);
                layout.Children.Add(imgAdd);
                //layout.Children.Add(DTControl.Separator());

                View = layout;
            }
        }*/

        /*
        public class HeaderTemplateCell : ViewCell
        {
            public HeaderTemplateCell()
            {
                Label lblHeaderName = new Label
                {
                    TextColor = Color.White,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.End,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                };
                lblHeaderName.SetBinding(Label.TextProperty, "LongName");

                StackLayout slContent = new StackLayout
                {
                    BackgroundColor = Color.White,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children ={
                    new ContentView{
                        VerticalOptions=LayoutOptions.CenterAndExpand,
                        Padding=new Thickness(0,0,5,0),
                        Content=lblHeaderName,
                    }
                }
                };

                View = slContent;
            }
        }
        */
            

            /*
        public class Constructor
        {
            public ViewType ViewType { get; set; }
        }

        public Constructor Ctor { get; set; }
        */

        public ListView ListView { get { return listView; } }

		ListView listView;
        private ObservableCollection<GroupedMenuNaviModel> items { get; set; }

        public MenuNavigation(/*Constructor ctor*/)
        {
            //Ctor = ctor;

            /*

            listView = new ListView {
				ItemsSource = masterPageItems,
				ItemTemplate = new DataTemplate (() => {
					var imageCell = new ImageCell ();
					imageCell.SetBinding (TextCell.TextProperty, "Title");
					imageCell.SetBinding (ImageCell.ImageSourceProperty, "IconSource");
					return imageCell;
				}),
				VerticalOptions = LayoutOptions.FillAndExpand,
				SeparatorVisibility = SeparatorVisibility.None
			};

			Padding = new Thickness (0, 40, 0, 0);
			Icon = "hamburger.png";
			Title = "Personal Organiser";
			Content = new StackLayout {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = {
					listView
				}	
			};
            */

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
                LongName = "MODULES",
                ShortName = "M"
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
                LongName = "SETTINGS",
                ShortName = "S"
            };

            groupSettings.Add(new MenuNaviModel()
            {
                Name = "Appearance",
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
                    




            /*

            */



            //lstView.ItemTemplate.SetBinding (TextCell.DetailProperty, "Comment");
            //Padding = new Thickness(0, 40, 0, 0);

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
