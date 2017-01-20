using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using Acr.UserDialogs;

using Xamarin.Forms;

namespace IOControl
{
    public partial class ConfigurationPage : ContentPage
    {
        ListView listView;
        public static ObservableCollection<HeaderModel> items;

        public class ItemModel
        {
            public string Name { get; set; }
            public Type GroupType { get; set; }
            public Object Object { get; set; }
            public int Id { get; set; }
        }

        public class HeaderModel : ObservableCollection<ItemModel>
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public int Id { get; set; }
            public ContentPage Context { get; set; }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class ItemViewCell : ViewCell
        {
            enum ViewMode
            {
                VIEW,
                EDIT
            }

            // --------------------
            // --------------------
            // --------------------

            ItemModel FindItem(int id)
            {
                DT.Log("item suche nach " + id.ToString());
                foreach (var group in items)
                {
                    foreach (var item in group)
                    {
                        if (item.Id == id)
                        {
                            return item;
                        }
                    }
                }
                return null;
            }

            // --------------------
            // --------------------
            // --------------------

            public ItemViewCell()
            {
                Entry entryName = null;
                Label labelName = null;
                Image imgEdit = null;
                Image imgDelete = null;
                Image imgSave = null;
                Image imgCancel = null;

                Func<ViewMode, int> SwitchViewMode = null;
                SwitchViewMode = mode =>
                {
                    labelName.IsVisible = (mode == ViewMode.VIEW ? true : false);
                    imgEdit.IsVisible = (mode == ViewMode.VIEW ? true : false);
                    imgDelete.IsVisible = (mode == ViewMode.VIEW ? true : false);

                    entryName.IsVisible = (mode == ViewMode.EDIT ? true : false);
                    imgSave.IsVisible = (mode == ViewMode.EDIT ? true : false);
                    imgCancel.IsVisible = (mode == ViewMode.EDIT ? true : false);

                    return 0;
                };

                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 5, 10, 5)
                };

                // Label Name
                labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("Name"));
                layout.Children.Add(labelName);

                // Entry Name
                entryName = new Entry() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry)), HorizontalOptions = LayoutOptions.StartAndExpand, IsVisible = false };
                entryName.SetBinding(Entry.TextProperty, new Binding("Name"));
                layout.Children.Add(entryName);

                // Label ID (hidden)
                Label labelId = new Label() { IsVisible = false };
                labelId.SetBinding(Label.TextProperty, new Binding("Id"));
                layout.Children.Add(labelId);

                // Edit Button
                imgEdit = new Image() { Source = ImageSource.FromFile("btn_edit.png") };
                var imgEditTapped = new TapGestureRecognizer();
                imgEditTapped.Tapped += (s, e) =>
                {
                    SwitchViewMode(ViewMode.EDIT);
                };
                imgEdit.GestureRecognizers.Add(imgEditTapped);
                layout.Children.Add(imgEdit);

                // Save Button
                imgSave = new Image() { Source = ImageSource.FromFile("btn_ok.png"), IsVisible = false };
                var imgSaveTapped = new TapGestureRecognizer();
                imgSaveTapped.Tapped += (s, e) =>
                {
                    SwitchViewMode(ViewMode.VIEW);
                };
                imgSave.GestureRecognizers.Add(imgSaveTapped);
                layout.Children.Add(imgSave);

                // Delete Button
                imgDelete = new Image() { Source = ImageSource.FromFile("btn_delete.png") };
                var imgDeleteTapped = new TapGestureRecognizer();
                imgDeleteTapped.Tapped += (s, e) =>
                {
                    ItemModel item = FindItem(Convert.ToInt32(labelId.Text));
                };
                imgDelete.GestureRecognizers.Add(imgDeleteTapped);
                layout.Children.Add(imgDelete);

                // Cancel Button
                imgCancel = new Image() { Source = ImageSource.FromFile("btn_cancel.png"), IsVisible = false };
                var imgCancelTapped = new TapGestureRecognizer();
                imgCancelTapped.Tapped += (s, e) =>
                {
                    SwitchViewMode(ViewMode.VIEW);
                };
                imgCancel.GestureRecognizers.Add(imgCancelTapped);
                layout.Children.Add(imgCancel);

                /*
                StackLayout outerLayout = new StackLayout();
                outerLayout.Children.Add(layout);
                outerLayout.Children.Add(DTControl.Separator());
                View = outerLayout;
                */

                View = layout;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public class HeaderViewCell : ViewCell
        {
            public HeaderViewCell()
            {
                Func<int, HeaderModel> FindItem = null;
                FindItem = id =>
                {
                    DT.Log("header suche nach " + id.ToString());
                    foreach (var item in items)
                    {
                        if (item.Id == id)
                        {
                            return item;
                        }
                    }
                    return null;
                };

                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 5, 10, 5),
                    BackgroundColor = DT.COLOR
                };

                // Label Name
                Label labelName = new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.Center };
                labelName.SetBinding(Label.TextProperty, new Binding("LongName"));
                layout.Children.Add(labelName);

                // Label ID (hidden)
                Label labelId = new Label() { IsVisible = false };
                labelId.SetBinding(Label.TextProperty, new Binding("Id"));
                layout.Children.Add(labelId);

                // Add Button
                Image imgAdd = new Image() { Source = ImageSource.FromFile("btn_add.png") };
                var imgAddTapped = new TapGestureRecognizer();
                imgAddTapped.Tapped += async (s, e) =>
                {
                    HeaderModel item = FindItem(Convert.ToInt32(labelId.Text));
                    if (item.ShortName == "M")
                    {
                        // Modul
                        await ((ConfigurationPage)item.Context).Machwas();
                    }
                    else
                    {
                        // Location
                        await ((ConfigurationPage)item.Context).AddLocation();
                    }
                };
                imgAdd.GestureRecognizers.Add(imgAddTapped);
                layout.Children.Add(imgAdd);

                /*
                StackLayout outerLayout = new StackLayout();
                outerLayout.Children.Add(layout);
                outerLayout.Children.Add(DTControl.Separator());
                View = outerLayout;
                */

                View = layout;
            }
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public ConfigurationPage()
        {
            int idHeader = 0;
            int idItem = 0;

            Icon = "hamburger.png";
            Title = "Configuration";

            items = new ObservableCollection<HeaderModel>();

            // --------------------
            // Locations

            var groupLocation = new HeaderModel()
            {
                LongName = Resx.AppResources.Location.ToUpper(),
                ShortName = Resx.AppResources.Location.ToUpper().Substring(0, 1),
                Id = idHeader++,
                Context = this
            };

            foreach (ContentLocation loc in DT.Session.xmlContent.loc)
            {
                groupLocation.Add(new ItemModel()
                {
                    Name = loc.name,
                    GroupType = loc.GetType(),
                    Object = loc,
                    Id = idItem++
                });
            }

            items.Add(groupLocation);

            // --------------------
            // Module

            var groupModule = new HeaderModel()
            {
                LongName = Resx.AppResources.Modules.ToUpper(),
                ShortName = Resx.AppResources.Modules.ToUpper().Substring(0, 1),
                Id = idHeader++,
                Context = this
            };

            foreach (Module module in DT.Session.xmlContent.modules)
            {
                groupModule.Add(new ItemModel()
                {
                    Name = module.boardname,
                    GroupType = module.GetType(),
                    Object = module,
                    Id = idItem++,
                });
            }

            items.Add(groupModule);

            // --------------------
            // ListView

            listView = new ListView();
            listView.ItemsSource = items;
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("LongName");
            listView.GroupShortNameBinding = new Binding("ShortName");
            //listView.SeparatorVisibility = SeparatorVisibility.None;
            listView.GroupHeaderTemplate = new DataTemplate(typeof(HeaderViewCell));
            listView.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            listView.HasUnevenRows = true;
            //listView.RowHeight = -1;

            listView.ItemTapped += (s, e) =>
            {
                ((ListView)s).SelectedItem = null;
            };

            Content = new StackLayout
            {
                Padding = new Thickness(0, 5, 0, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { listView }
            };
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<int> Machwas()
        {
            DT.Log("zeige dialog");
            var answer = await DisplayAlert(Resx.AppResources.AddNewModuleHeader, Resx.AppResources.AddNewModuleText, Resx.AppResources.AddNewModuleSearch, Resx.AppResources.AddNewModuleAdd);
            DT.Log("ende dialog");

            if (answer)
            {
                // scan
                DT.Log("zeige BC");
                DialogBroadcast db = new DialogBroadcast();
                await Navigation.PushAsync(db);
                DT.Log("warte auf BC ende");
                await db.PageCloseTask;
                DT.Log("ende BC");
            }
            else
            {
                // add
                DT.Log("zeige ADD");
                DialogNetworkConfig dnc = new DialogNetworkConfig(new DialogNetworkConfig.Constructor()
                {
                    ViewType = DialogNetworkConfig.ViewType.ADD
                });
                await Navigation.PushAsync(dnc);
                DT.Log("warte auf ADD ende");
                await dnc.PageCloseTask;
                DT.Log("ende ADD");
            }

            return 1;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------

        public async Task<bool> AddLocation()
        {
            PromptResult pm = await UserDialogs.Instance.PromptAsync(new PromptConfig()
            {
                Title = Resx.AppResources.CFG_AddNewLocationHeader,
                Placeholder = Resx.AppResources.CFG_AddNewLocationPlaceholder,
                Message = Resx.AppResources.CFG_AddNewLocationText,
                OkText = Resx.AppResources.MSG_OK,
                CancelText = Resx.AppResources.MSG_Cancel
            });

            if (pm.Ok)
            {
                DT.Log(String.Format("text = {0}", pm.Text));
            }

            return true;
        }

        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
        // ----------------------------------------------------------------------------
    }
}
