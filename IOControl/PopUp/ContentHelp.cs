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
        public static View ContentHelpLocation()
        {
            return DTControl.GetLabel(Resx.AppResources.HELP_HelpLocation);
        }

        public static View NetworkConfig()
        {
            return DTControl.GetLabel(Resx.AppResources.NC_HelpText);
        }

        public static View ContentHelpLocationButtons()
        {
            StackLayout sl = new StackLayout();

            sl.Children.Add(DTControl.GetImageCell("btn_delete.png", "Eintrag löschen"));
            sl.Children.Add(DTControl.GetImageCell("btn_edit.png", "Eintrag bearbeiten"));
            sl.Children.Add(DTControl.GetImageCell("btn_up.png", "Eintrag nach oben verschieben"));
            sl.Children.Add(DTControl.GetImageCell("btn_down.png", "Eintrag nach unten verschieben"));
            sl.Children.Add(DTControl.GetImageCell("btn_continue.png", "Nächste Ansicht"));

            return sl;
        }
    }
}
