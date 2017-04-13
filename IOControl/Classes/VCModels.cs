using Xamarin.Forms;

namespace IOControl
{
    public class VCModels
    {
        public static Thickness PAD_HEADER = new Thickness(10, 5, 10, 5);
        public static Thickness PAD_ICON = new Thickness(10, 10, 10, 10);
        public static Thickness PAD_FOOTER = new Thickness(10, 10, 10, 10);
        public static Thickness PAD_MAIN = new Thickness(0, 5, 0, 0);

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public static Label ViewCellLabel(string binding)
        {
            Label label = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center }; ;
            label.SetBinding(Label.TextProperty, new Binding(binding));
            return label;
        }

        public static ListView ListViewInit()
        {
            ListView listView = new ListView();
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("LongName");
            listView.GroupShortNameBinding = new Binding("ShortName");
            /*
            listView.SeparatorVisibility = SeparatorVisibility.None;
            listView.SeparatorColor = Color.Transparent; //"Transparent";
            */
            listView.SeparatorColor = DT.COLOR;

            listView.HasUnevenRows = true;
            return listView;
        }

        public static Layout ListViewSeparator()
        {
            StackLayout layout = new StackLayout();
            Label label = new Label() { BackgroundColor = Color.White, HeightRequest = 0.2F };
            layout.Children.Add(label);
            return layout;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class NaviHeader : ViewCell
        {
            public NaviHeader()
            {
                StackLayout layout = new StackLayout() { Padding = PAD_ICON, BackgroundColor = DT.COLOR };

                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("LongName"));
                layout.Children.Add(labelName);
                //layout.Children.Add(ListViewSeparator());

                View = layout;
            }
        }

        public class NaviIcon : ViewCell
        {
            public NaviIcon()
            {
                StackLayout layout = new StackLayout() { Padding = PAD_ICON, Orientation = StackOrientation.Vertical };

                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));
                layout.Children.Add(labelName);
                //layout.Children.Add(ListViewSeparator());

                View = layout;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class CfgHeader : ViewCell
        {
            public CfgHeader(ContentPage context)
            {
                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 5, 10, 5),
                    BackgroundColor = DT.COLOR
                };

                Label lbl = ViewCellLabel("LongName");
                lbl.FontAttributes = FontAttributes.Bold;
                layout.Children.Add(lbl);

                // Add Button
                Image imgAdd = new Image() { Source = ImageSource.FromFile("btn_add.png") };
                var imgAddTapped = new TapGestureRecognizer();
                imgAddTapped.Tapped += async (s, e) =>
                {
                    if (((AppearancePage.HeaderModel)BindingContext).Type == typeof(Module))
                    {
                        await ((AppearancePage)context).AddModule();
                    }
                    else if (((AppearancePage.HeaderModel)BindingContext).Type == typeof(ContentLocation))
                    {
                        await ((AppearancePage)context).AddLocation();
                    }
                    else if (((AppearancePage.HeaderModel)BindingContext).Type == typeof(ContentGroup))
                    {
                        await ((AppearancePage)context).AddLocation();
                    }
                    else if (((AppearancePage.HeaderModel)BindingContext).Type == typeof(ContentIO))
                    {
                        ((AppearancePage)context).taskComplete = false;
                        DT.Log("await AddIO");
                        await ((AppearancePage)context).AddIO();
                        DT.Log("AddIO fertig");
                        ((AppearancePage)context).taskComplete = true;
                    }
                };
                imgAdd.GestureRecognizers.Add(imgAddTapped);
                layout.Children.Add(imgAdd);
                View = layout;
            }
        }
    }
}