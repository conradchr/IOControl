using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace IOControl
{
    public class ContentHelp
    {
        static View GetLabel(String text)
        {
            return new Label()
            {
                Text = text,
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            };
        }

        static View GetImageCell(string imgSource, string text)
        {
            Image img = new Image() { Source = ImageSource.FromFile(imgSource), HorizontalOptions = LayoutOptions.Start };
            StackLayout layout = new StackLayout() { Orientation = StackOrientation.Horizontal };
            layout.Children.Add(img);
            layout.Children.Add(GetLabel(text));
            return layout;
        }

        public static View ContentHelpLocation()
        {
            return GetLabel(Resx.AppResources.HELP_HelpLocation);
        }

        public static View ContentHelpLocationButtons()
        {
            StackLayout sl = new StackLayout();

            sl.Children.Add(GetImageCell("btn_delete.png", "Eintrag löschen"));
            sl.Children.Add(GetImageCell("btn_edit.png", "Eintrag bearbeiten"));
            sl.Children.Add(GetImageCell("btn_up.png", "Eintrag nach oben verschieben"));
            sl.Children.Add(GetImageCell("btn_down.png", "Eintrag nach unten verschieben"));
            sl.Children.Add(GetImageCell("btn_continue.png", "Nächste Ansicht"));

            return sl;
        }
    }
}
