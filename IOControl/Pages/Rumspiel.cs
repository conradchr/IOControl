using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections.Generic;
using System.ComponentModel;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
//using XLabs.Forms.Controls;

namespace IOControl
{
    
    // Inherit from this page to get a multi-select option in XF
    public class SelectMultipleBasePage<T> : ContentPage
    {
        public class WrappedSelection<T> : INotifyPropertyChanged
        {
            public T Item { get; set; }

            bool isSelected = false;
            public bool IsSelected
            {
                get
                {
                    return isSelected;
                }
                set
                {
                    if (isSelected != value)
                    {
                        isSelected = value;
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                    }
                }
            }
            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }

        public class WrappedItemSelectionTemplate : ViewCell
        {
            public WrappedItemSelectionTemplate() : base()
            {
                Label name = new Label();
                name.SetBinding(Label.TextProperty, new Binding("Item.Name"));

                Switch mainSwitch = new Switch();
                mainSwitch.SetBinding(Switch.IsToggledProperty, new Binding("IsSelected"));

                RelativeLayout layout = new RelativeLayout();
                layout.Children.Add(name,
                    Constraint.Constant(5),
                    Constraint.Constant(5),
                    Constraint.RelativeToParent(p => p.Width - 60),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );

                layout.Children.Add(mainSwitch,
                    Constraint.RelativeToParent(p => p.Width - 55),
                    Constraint.Constant(5),
                    Constraint.Constant(50),
                    Constraint.RelativeToParent(p => p.Height - 10)
                );

                View = layout;
            }
        }

        public List<WrappedSelection<T>> WrappedItems = new List<WrappedSelection<T>>();

        public SelectMultipleBasePage(List<T> items)
        {
            WrappedItems = items.Select(item => new WrappedSelection<T>() { Item = item, IsSelected = false }).ToList();

            ListView mainList = new ListView()
            {
                ItemsSource = WrappedItems,
                ItemTemplate = new DataTemplate(typeof(WrappedItemSelectionTemplate))
            };

            Content = mainList;

            ToolbarItems.Add(new ToolbarItem("All", null, SelectAll, ToolbarItemOrder.Primary));
            ToolbarItems.Add(new ToolbarItem("None", null, SelectNone, ToolbarItemOrder.Primary));
        }

        void SelectAll()
        {
            foreach (var wi in WrappedItems)
                wi.IsSelected = true;
        }

        void SelectNone()
        {
            foreach (var wi in WrappedItems)
                wi.IsSelected = false;
        }

        public List<T> GetSelection()
        {
            return WrappedItems.Where(item => item.IsSelected).Select(wrappedItem => wrappedItem.Item).ToList();
        }
    }
    

    /*
class Rumspiel : ContentPage
{
    // Dictionary to get Color from color name.
    Dictionary<string, Color> nameToColor = new Dictionary<string, Color>
    {
        { "Aqua", Color.Aqua }, { "Black", Color.Black },
        { "Blue", Color.Blue }, { "Fuschia", Color.Fuschia },
        { "Gray", Color.Gray }, { "Green", Color.Green },
        { "Lime", Color.Lime }, { "Maroon", Color.Maroon },
        { "Navy", Color.Navy }, { "Olive", Color.Olive },
        { "Purple", Color.Purple }, { "Red", Color.Red },
        { "Silver", Color.Silver }, { "Teal", Color.Teal },
        { "White", Color.White }, { "Yellow", Color.Yellow }
    };

    public Rumspiel()
    {
        Label header = new Label
        {
            Text = "Picker",
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            HorizontalOptions = LayoutOptions.Center
        };

        Picker picker = new Picker
        {
            Title = "Color",
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        Picker picker2 = new Picker
        {
            Title = "Color",
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        foreach (string colorName in nameToColor.Keys)
        {
            picker.Items.Add(colorName);
            picker2.Items.Add(colorName);
        }

        // Create BoxView for displaying picked Color
        BoxView boxView = new BoxView
        {
            WidthRequest = 150,
            HeightRequest = 150,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        picker.SelectedIndexChanged += (sender, args) =>
        {
            if (picker.SelectedIndex == -1)
            {
                boxView.Color = Color.Default;
            }
            else
            {
                string colorName = picker.Items[picker.SelectedIndex];
                boxView.Color = nameToColor[colorName];
            }
        };

        // Accomodate iPhone status bar.
        this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

        // Build the page.
        this.Content = new StackLayout
        {
            Children =
            {
                header,
                picker,
                picker2,
                boxView
            }
        };

    }
}
*/
/*
    public partial class ExtendedPickerPage : ContentPage
    {
        public class DataClass
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public ExtendedPickerPage()
        {
            InitializeComponent();
            BindingContext = this;
            myPicker = new ExtendedPicker() { DisplayProperty = "FirstName" };
            myPicker.SetBinding(ExtendedPicker.ItemsSourceProperty, new Binding("MyDataList", BindingMode.TwoWay));
            myPicker.SetBinding(ExtendedPicker.SelectedItemProperty, new Binding("TheChosenOne", BindingMode.TwoWay));
            myStackLayout.Children.Add(new Label { Text = "Code Created:" });
            myStackLayout.Children.Add(myPicker);


            TheChosenOne = new DataClass() { FirstName = "Jet", LastName = "Li" };
            MyDataList = new ObservableCollection<object>() {
                new DataClass(){FirstName="Jack",LastName="Doe"},
                TheChosenOne,
                new DataClass(){FirstName="Matt",LastName="Bar"},
                new DataClass(){FirstName="Mic",LastName="Jaggery"},
                new DataClass(){FirstName="Michael",LastName="Jackon"}
            };

        }
        public ICommand DoIt { get; set; }

        private object chosenOne;
        public object TheChosenOne
        {
            get
            {
                return chosenOne;
            }
            set
            {
                chosenOne = value;
                OnPropertyChanged("TheChosenOne");
            }
        }

        ObservableCollection<object> dataList;
        public ObservableCollection<object> MyDataList
        {
            get { return dataList; }
            set
            {
                dataList = value;
                OnPropertyChanged("MyDataList");
            }
        }

    }
    */
}